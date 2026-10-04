namespace RazerEditor;

public class HidKey
{
    public string HID { get; init; } = "";
    public string Name { get; init; } = "";
    public string Group { get; init; } = "";
    public byte Extended { get; init; }
    public byte ScanCode { get; init; }

    // Mod field format: [extended][00][scanCode][00][scanCode+0x80]
    public string ModString =>
        $"{Extended:X2}00{ScanCode:X2}00{(ScanCode + 0x80):X2}";
}

public static class HidKeys
{
    public static readonly List<HidKey> All =
    [
        // Letters
        new() { HID = "070004", Name = "A",          Group = "Letters",    Extended = 0, ScanCode = 0x1E },
        new() { HID = "070005", Name = "B",          Group = "Letters",    Extended = 0, ScanCode = 0x30 },
        new() { HID = "070006", Name = "C",          Group = "Letters",    Extended = 0, ScanCode = 0x2E },
        new() { HID = "070007", Name = "D",          Group = "Letters",    Extended = 0, ScanCode = 0x20 },
        new() { HID = "070008", Name = "E",          Group = "Letters",    Extended = 0, ScanCode = 0x12 },
        new() { HID = "070009", Name = "F",          Group = "Letters",    Extended = 0, ScanCode = 0x21 },
        new() { HID = "07000A", Name = "G",          Group = "Letters",    Extended = 0, ScanCode = 0x22 },
        new() { HID = "07000B", Name = "H",          Group = "Letters",    Extended = 0, ScanCode = 0x23 },
        new() { HID = "07000C", Name = "I",          Group = "Letters",    Extended = 0, ScanCode = 0x17 },
        new() { HID = "07000D", Name = "J",          Group = "Letters",    Extended = 0, ScanCode = 0x24 },
        new() { HID = "07000E", Name = "K",          Group = "Letters",    Extended = 0, ScanCode = 0x25 },
        new() { HID = "07000F", Name = "L",          Group = "Letters",    Extended = 0, ScanCode = 0x26 },
        new() { HID = "070010", Name = "M",          Group = "Letters",    Extended = 0, ScanCode = 0x32 },
        new() { HID = "070011", Name = "N",          Group = "Letters",    Extended = 0, ScanCode = 0x31 },
        new() { HID = "070012", Name = "O",          Group = "Letters",    Extended = 0, ScanCode = 0x18 },
        new() { HID = "070013", Name = "P",          Group = "Letters",    Extended = 0, ScanCode = 0x19 },
        new() { HID = "070014", Name = "Q",          Group = "Letters",    Extended = 0, ScanCode = 0x10 },
        new() { HID = "070015", Name = "R",          Group = "Letters",    Extended = 0, ScanCode = 0x13 },
        new() { HID = "070016", Name = "S",          Group = "Letters",    Extended = 0, ScanCode = 0x1F },
        new() { HID = "070017", Name = "T",          Group = "Letters",    Extended = 0, ScanCode = 0x14 },
        new() { HID = "070018", Name = "U",          Group = "Letters",    Extended = 0, ScanCode = 0x16 },
        new() { HID = "070019", Name = "V",          Group = "Letters",    Extended = 0, ScanCode = 0x2F },
        new() { HID = "07001A", Name = "W",          Group = "Letters",    Extended = 0, ScanCode = 0x11 },
        new() { HID = "07001B", Name = "X",          Group = "Letters",    Extended = 0, ScanCode = 0x2D },
        new() { HID = "07001C", Name = "Y",          Group = "Letters",    Extended = 0, ScanCode = 0x15 },
        new() { HID = "07001D", Name = "Z",          Group = "Letters",    Extended = 0, ScanCode = 0x2C },

        // Numbers
        new() { HID = "07001E", Name = "1",          Group = "Numbers",    Extended = 0, ScanCode = 0x02 },
        new() { HID = "07001F", Name = "2",          Group = "Numbers",    Extended = 0, ScanCode = 0x03 },
        new() { HID = "070020", Name = "3",          Group = "Numbers",    Extended = 0, ScanCode = 0x04 },
        new() { HID = "070021", Name = "4",          Group = "Numbers",    Extended = 0, ScanCode = 0x05 },
        new() { HID = "070022", Name = "5",          Group = "Numbers",    Extended = 0, ScanCode = 0x06 },
        new() { HID = "070023", Name = "6",          Group = "Numbers",    Extended = 0, ScanCode = 0x07 },
        new() { HID = "070024", Name = "7",          Group = "Numbers",    Extended = 0, ScanCode = 0x08 },
        new() { HID = "070025", Name = "8",          Group = "Numbers",    Extended = 0, ScanCode = 0x09 },
        new() { HID = "070026", Name = "9",          Group = "Numbers",    Extended = 0, ScanCode = 0x0A },
        new() { HID = "070027", Name = "0",          Group = "Numbers",    Extended = 0, ScanCode = 0x0B },

        // Function keys
        new() { HID = "07003A", Name = "F1",         Group = "Function",   Extended = 0, ScanCode = 0x3B },
        new() { HID = "07003B", Name = "F2",         Group = "Function",   Extended = 0, ScanCode = 0x3C },
        new() { HID = "07003C", Name = "F3",         Group = "Function",   Extended = 0, ScanCode = 0x3D },
        new() { HID = "07003D", Name = "F4",         Group = "Function",   Extended = 0, ScanCode = 0x3E },
        new() { HID = "07003E", Name = "F5",         Group = "Function",   Extended = 0, ScanCode = 0x3F },
        new() { HID = "07003F", Name = "F6",         Group = "Function",   Extended = 0, ScanCode = 0x40 },
        new() { HID = "070040", Name = "F7",         Group = "Function",   Extended = 0, ScanCode = 0x41 },
        new() { HID = "070041", Name = "F8",         Group = "Function",   Extended = 0, ScanCode = 0x42 },
        new() { HID = "070042", Name = "F9",         Group = "Function",   Extended = 0, ScanCode = 0x43 },
        new() { HID = "070043", Name = "F10",        Group = "Function",   Extended = 0, ScanCode = 0x44 },
        new() { HID = "070044", Name = "F11",        Group = "Function",   Extended = 0, ScanCode = 0x57 },
        new() { HID = "070045", Name = "F12",        Group = "Function",   Extended = 0, ScanCode = 0x58 },

        // Special / Punctuation
        new() { HID = "070028", Name = "Enter",      Group = "Special",    Extended = 0, ScanCode = 0x1C },
        new() { HID = "070029", Name = "Escape",     Group = "Special",    Extended = 0, ScanCode = 0x01 },
        new() { HID = "07002A", Name = "Backspace",  Group = "Special",    Extended = 0, ScanCode = 0x0E },
        new() { HID = "07002B", Name = "Tab",        Group = "Special",    Extended = 0, ScanCode = 0x0F },
        new() { HID = "07002C", Name = "Space",      Group = "Special",    Extended = 0, ScanCode = 0x39 },
        new() { HID = "07002D", Name = "-",          Group = "Special",    Extended = 0, ScanCode = 0x0C },
        new() { HID = "07002E", Name = "=",          Group = "Special",    Extended = 0, ScanCode = 0x0D },
        new() { HID = "07002F", Name = "[",          Group = "Special",    Extended = 0, ScanCode = 0x1A },
        new() { HID = "070030", Name = "]",          Group = "Special",    Extended = 0, ScanCode = 0x1B },
        new() { HID = "070031", Name = "\\",         Group = "Special",    Extended = 0, ScanCode = 0x2B },
        new() { HID = "070033", Name = ";",          Group = "Special",    Extended = 0, ScanCode = 0x27 },
        new() { HID = "070034", Name = "'",          Group = "Special",    Extended = 0, ScanCode = 0x28 },
        new() { HID = "070035", Name = "`",          Group = "Special",    Extended = 0, ScanCode = 0x29 },
        new() { HID = "070036", Name = ",",          Group = "Special",    Extended = 0, ScanCode = 0x33 },
        new() { HID = "070037", Name = ".",          Group = "Special",    Extended = 0, ScanCode = 0x34 },
        new() { HID = "070038", Name = "/",          Group = "Special",    Extended = 0, ScanCode = 0x35 },
        new() { HID = "070039", Name = "CapsLock",   Group = "Special",    Extended = 0, ScanCode = 0x3A },
        new() { HID = "070047", Name = "ScrollLock", Group = "Special",    Extended = 0, ScanCode = 0x46 },

        // Navigation
        new() { HID = "070049", Name = "Insert",     Group = "Navigation", Extended = 1, ScanCode = 0x52 },
        new() { HID = "07004A", Name = "Home",       Group = "Navigation", Extended = 1, ScanCode = 0x47 },
        new() { HID = "07004B", Name = "PageUp",     Group = "Navigation", Extended = 1, ScanCode = 0x49 },
        new() { HID = "07004C", Name = "Delete",     Group = "Navigation", Extended = 1, ScanCode = 0x53 },
        new() { HID = "07004D", Name = "End",        Group = "Navigation", Extended = 1, ScanCode = 0x4F },
        new() { HID = "07004E", Name = "PageDown",   Group = "Navigation", Extended = 1, ScanCode = 0x51 },
        new() { HID = "07004F", Name = "Right",      Group = "Navigation", Extended = 1, ScanCode = 0x4D },
        new() { HID = "070050", Name = "Left",       Group = "Navigation", Extended = 1, ScanCode = 0x4B },
        new() { HID = "070051", Name = "Down",       Group = "Navigation", Extended = 1, ScanCode = 0x50 },
        new() { HID = "070052", Name = "Up",         Group = "Navigation", Extended = 1, ScanCode = 0x48 },

        // Numpad
        new() { HID = "070053", Name = "NumLock",    Group = "Numpad",     Extended = 0, ScanCode = 0x45 },
        new() { HID = "070054", Name = "Num /",      Group = "Numpad",     Extended = 1, ScanCode = 0x35 },
        new() { HID = "070055", Name = "Num *",      Group = "Numpad",     Extended = 0, ScanCode = 0x37 },
        new() { HID = "070056", Name = "Num -",      Group = "Numpad",     Extended = 0, ScanCode = 0x4A },
        new() { HID = "070057", Name = "Num +",      Group = "Numpad",     Extended = 0, ScanCode = 0x4E },
        new() { HID = "070058", Name = "NumEnter",   Group = "Numpad",     Extended = 1, ScanCode = 0x1C },
        new() { HID = "070059", Name = "Num 1",      Group = "Numpad",     Extended = 0, ScanCode = 0x4F },
        new() { HID = "07005A", Name = "Num 2",      Group = "Numpad",     Extended = 0, ScanCode = 0x50 },
        new() { HID = "07005B", Name = "Num 3",      Group = "Numpad",     Extended = 0, ScanCode = 0x51 },
        new() { HID = "07005C", Name = "Num 4",      Group = "Numpad",     Extended = 0, ScanCode = 0x4B },
        new() { HID = "07005D", Name = "Num 5",      Group = "Numpad",     Extended = 0, ScanCode = 0x4C },
        new() { HID = "07005E", Name = "Num 6",      Group = "Numpad",     Extended = 0, ScanCode = 0x4D },
        new() { HID = "07005F", Name = "Num 7",      Group = "Numpad",     Extended = 0, ScanCode = 0x47 },
        new() { HID = "070060", Name = "Num 8",      Group = "Numpad",     Extended = 0, ScanCode = 0x48 },
        new() { HID = "070061", Name = "Num 9",      Group = "Numpad",     Extended = 0, ScanCode = 0x49 },
        new() { HID = "070062", Name = "Num 0",      Group = "Numpad",     Extended = 0, ScanCode = 0x52 },
        new() { HID = "070063", Name = "Num .",      Group = "Numpad",     Extended = 0, ScanCode = 0x53 },

        // Modifiers
        new() { HID = "0700E0", Name = "LCtrl",      Group = "Modifiers",  Extended = 0, ScanCode = 0x1D },
        new() { HID = "0700E1", Name = "LShift",     Group = "Modifiers",  Extended = 0, ScanCode = 0x2A },
        new() { HID = "0700E2", Name = "LAlt",       Group = "Modifiers",  Extended = 0, ScanCode = 0x38 },
        new() { HID = "0700E3", Name = "LWin",       Group = "Modifiers",  Extended = 1, ScanCode = 0x5B },
        new() { HID = "0700E4", Name = "RCtrl",      Group = "Modifiers",  Extended = 1, ScanCode = 0x1D },
        new() { HID = "0700E5", Name = "RShift",     Group = "Modifiers",  Extended = 0, ScanCode = 0x36 },
        new() { HID = "0700E6", Name = "RAlt",       Group = "Modifiers",  Extended = 1, ScanCode = 0x38 },
        new() { HID = "0700E7", Name = "RWin",       Group = "Modifiers",  Extended = 1, ScanCode = 0x5C },
    ];

    private static readonly Dictionary<string, HidKey> ByHID =
        All.ToDictionary(k => k.HID.ToUpperInvariant());

    public static HidKey? GetByHID(string hid) =>
        ByHID.TryGetValue(hid.ToUpperInvariant(), out var k) ? k : null;

    public static string GetName(string hid) =>
        GetByHID(hid)?.Name ?? hid;
}
