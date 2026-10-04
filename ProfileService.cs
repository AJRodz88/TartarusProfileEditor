using System.IO;
using System.Xml.Linq;

namespace RazerEditor;

public class TartarusProfile
{
    public string FilePath { get; set; } = "";
    public string UUID { get; set; } = "";
    public string ProfileName { get; set; } = "";
    public bool Active { get; set; }
    public int ActiveKeymap { get; set; } = 1;
    public List<KeymapLayer> Layers { get; set; } = [];

    public override string ToString() => string.IsNullOrWhiteSpace(ProfileName) ? UUID : ProfileName;
}

public class KeymapLayer
{
    public int Index { get; set; }
    public List<KeyAssignment> Assignments { get; set; } = [];
}

public class KeyAssignment
{
    public string PhysicalKeyId { get; set; } = "";
    public string AssignmentType { get; set; } = "SINGLEKEY";
    public string TargetHID { get; set; } = "";
    public string Mod { get; set; } = "";
}

public static class ProfileService
{
    private static readonly string BasePath =
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "Razer", "Synapse", "Accounts");

    public static List<TartarusProfile> LoadAll()
    {
        var profiles = new List<TartarusProfile>();

        if (!Directory.Exists(BasePath))
            return profiles;

        foreach (var accountDir in Directory.GetDirectories(BasePath))
        {
            var profilesDir = Path.Combine(accountDir, "Devices", "Tartarus", "Profiles");
            if (!Directory.Exists(profilesDir)) continue;

            foreach (var xmlFile in Directory.GetFiles(profilesDir, "*.xml"))
            {
                try
                {
                    var p = Parse(xmlFile);
                    if (p != null) profiles.Add(p);
                }
                catch { /* skip malformed files */ }
            }
        }

        return [.. profiles.OrderBy(p => p.ProfileName)];
    }

    private static TartarusProfile? Parse(string filePath)
    {
        var doc = XDocument.Load(filePath);
        var root = doc.Root;
        if (root == null) return null;

        var profile = new TartarusProfile
        {
            FilePath = filePath,
            UUID = root.Element("UUID")?.Value ?? "",
            ProfileName = root.Element("ProfileName")?.Value
                          ?? Path.GetFileNameWithoutExtension(filePath),
            Active = root.Element("Active")?.Value == "1",
        };

        var keymapEl = root.Element("Keymap");
        if (keymapEl == null) return profile;

        // ActiveKeymap can sit directly in <Keymap> or in the first <KeymapItem>
        var activeKmStr = keymapEl.Element("ActiveKeymap")?.Value
                       ?? keymapEl.Elements("KeymapItem")
                                  .Select(i => i.Element("ActiveKeymap")?.Value)
                                  .FirstOrDefault(v => v != null);
        profile.ActiveKeymap = int.TryParse(activeKmStr, out var ak) ? ak : 1;

        foreach (var item in keymapEl.Elements("KeymapItem"))
        {
            if (!int.TryParse(item.Element("KeymapIndex")?.Value, out var idx)) continue;

            var layer = new KeymapLayer { Index = idx };

            foreach (var keyEl in item.Element("KeyList")?.Elements("Key")
                                  ?? Enumerable.Empty<XElement>())
            {
                var physId = keyEl.Attribute("id")?.Value ?? "";
                var assignEl = keyEl.Element("Assignment");
                if (assignEl == null) continue;

                layer.Assignments.Add(new KeyAssignment
                {
                    PhysicalKeyId = physId.ToUpperInvariant(),
                    AssignmentType = assignEl.Attribute("id")?.Value ?? "SINGLEKEY",
                    TargetHID = assignEl.Element("HID")?.Value ?? "",
                    Mod = assignEl.Element("Mod")?.Value ?? "",
                });
            }

            profile.Layers.Add(layer);
        }

        return profile;
    }

    public static void Save(TartarusProfile profile)
    {
        var doc = XDocument.Load(profile.FilePath);
        var root = doc.Root!;
        var keymapEl = root.Element("Keymap")!;

        foreach (var layer in profile.Layers)
        {
            var keymapItem = keymapEl.Elements("KeymapItem")
                .FirstOrDefault(i => i.Element("KeymapIndex")?.Value == layer.Index.ToString());
            if (keymapItem == null) continue;

            var keyList = keymapItem.Element("KeyList");
            if (keyList == null) continue;

            foreach (var assignment in layer.Assignments)
            {
                if (assignment.AssignmentType != "SINGLEKEY") continue;

                var keyEl = keyList.Elements("Key")
                    .FirstOrDefault(k =>
                        string.Equals(k.Attribute("id")?.Value, assignment.PhysicalKeyId,
                                      StringComparison.OrdinalIgnoreCase));
                if (keyEl == null) continue;

                var assignEl = keyEl.Element("Assignment");
                if (assignEl == null) continue;

                var hidEl = assignEl.Element("HID");
                var modEl = assignEl.Element("Mod");
                if (hidEl != null) hidEl.Value = assignment.TargetHID;
                if (modEl != null) modEl.Value = assignment.Mod;
            }
        }

        // Mark this profile as the one Synapse selects
        var activeEl = root.Element("Active");
        if (activeEl != null) activeEl.Value = "1";
        else root.Element("ProfileName")?.AddAfterSelf(new XElement("Active", "1"));

        doc.Save(profile.FilePath);
        profile.Active = true;

        DeactivateOthers(profile.FilePath);
    }

    // Only one profile per device folder may be active; clear the flag on the rest
    private static void DeactivateOthers(string activeFilePath)
    {
        var profilesDir = Path.GetDirectoryName(activeFilePath)!;

        foreach (var xmlFile in Directory.GetFiles(profilesDir, "*.xml"))
        {
            if (string.Equals(Path.GetFullPath(xmlFile), Path.GetFullPath(activeFilePath),
                              StringComparison.OrdinalIgnoreCase)) continue;

            try
            {
                var doc = XDocument.Load(xmlFile);
                var activeEl = doc.Root?.Element("Active");
                if (activeEl?.Value != "1") continue;

                activeEl.Value = "0";
                doc.Save(xmlFile);
            }
            catch { /* skip malformed files */ }
        }
    }
}
