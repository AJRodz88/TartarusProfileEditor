using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RazerEditor;

public partial class MainWindow : Window
{
    // Physical Tartarus button definitions: (HID, display label, section, grid row, grid col)
    // Section: 0=Main, 1=Thumb, 2=DPad
    private static readonly (string HID, string Label, int Section, int Row, int Col)[] PhysicalKeys =
    [
        // Main 5×3 grid
        ("07002B", "Tab",   0, 0, 0),
        ("070014", "Q",     0, 0, 1),
        ("07001A", "W",     0, 0, 2),
        ("070008", "E",     0, 0, 3),
        ("070015", "R",     0, 0, 4),

        ("070039", "Caps",  0, 1, 0),
        ("070004", "A",     0, 1, 1),
        ("070016", "S",     0, 1, 2),
        ("070007", "D",     0, 1, 3),
        ("070009", "F",     0, 1, 4),

        ("0700E1", "Shift", 0, 2, 0),
        ("07001D", "Z",     0, 2, 1),
        ("07001B", "X",     0, 2, 2),
        ("070006", "C",     0, 2, 3),
        ("070019", "V",     0, 2, 4),

        // Thumb
        ("0700E2", "Alt",   1, 0, 0),
        ("07002C", "Space", 1, 0, 1),

        // D-pad  (Row/Col used as canvas position index)
        ("070052", "Up",    2, 0, 1),
        ("070050", "Left",  2, 1, 0),
        ("070051", "Down",  2, 1, 1),
        ("07004F", "Right", 2, 1, 2),
        ("07004A", "Home",  2, 2, 0),
        ("07004B", "PgUp",  2, 2, 1),
        ("07004D", "End",   2, 3, 0),
        ("07004E", "PgDn",  2, 3, 1),
    ];

    private List<TartarusProfile> _profiles = [];
    private TartarusProfile? _current;
    private KeymapLayer? _currentLayer;
    private bool _suppressEvents;

    public MainWindow()
    {
        InitializeComponent();
        Loaded += (_, _) => LoadProfiles();
    }

    // ─── Loading ──────────────────────────────────────────────────────────────

    private void LoadProfiles()
    {
        _profiles = ProfileService.LoadAll();

        _suppressEvents = true;
        ProfileCombo.ItemsSource = _profiles;
        ProfileCombo.DisplayMemberPath = null;
        _suppressEvents = false;

        if (_profiles.Count == 0)
        {
            StatusLabel.Text = "No Tartarus profiles found.";
            return;
        }

        // Select the active profile first, otherwise first in list
        var active = _profiles.FirstOrDefault(p => p.Active) ?? _profiles[0];
        ProfileCombo.SelectedItem = active;
    }

    private void SelectProfile(TartarusProfile profile)
    {
        _current = profile;
        ActiveLabel.Text = profile.Active ? "● ACTIVE" : "";

        _suppressEvents = true;
        LayerCombo.ItemsSource = profile.Layers.Select(l => $"Layer {l.Index}").ToList();
        LayerCombo.SelectedIndex = profile.Layers.FindIndex(l => l.Index == profile.ActiveKeymap)
                                   is int i && i >= 0 ? i : 0;
        _suppressEvents = false;

        SelectLayer();
    }

    private void SelectLayer()
    {
        if (_current == null || LayerCombo.SelectedIndex < 0) return;
        _currentLayer = _current.Layers.ElementAtOrDefault(LayerCombo.SelectedIndex);
        BuildKeyButtons();
    }

    // ─── UI Building ─────────────────────────────────────────────────────────

    private void BuildKeyButtons()
    {
        MainGrid.Children.Clear();
        ThumbPanel.Children.Clear();
        DpadGrid.Children.Clear();

        foreach (var (hid, label, section, row, col) in PhysicalKeys)
        {
            var assignment = _currentLayer?.Assignments
                .FirstOrDefault(a => string.Equals(a.PhysicalKeyId, hid,
                                                   StringComparison.OrdinalIgnoreCase));

            var btn = MakeKeyButton(hid, label, assignment);

            switch (section)
            {
                case 0: // Main grid
                    Grid.SetRow(btn, row);
                    Grid.SetColumn(btn, col);
                    MainGrid.Children.Add(btn);
                    break;

                case 1: // Thumb row
                    ThumbPanel.Children.Add(btn);
                    break;

                case 2: // D-pad — absolute position in a 3-col × 4-row canvas-like grid
                    btn.Width = 70;
                    btn.Height = 60;
                    Canvas.SetLeft(btn, col * 74);
                    Canvas.SetTop(btn,  row * 64);
                    DpadGrid.Children.Add(btn);
                    break;
            }
        }
    }

    private Button MakeKeyButton(string physicalHid, string physicalLabel, KeyAssignment? assignment)
    {
        string mappedName;
        string styleName;

        if (assignment == null)
        {
            // No assignment in this layer — show physical default
            mappedName = physicalLabel;
            styleName = "KeyBtnDefault";
        }
        else if (assignment.AssignmentType != "SINGLEKEY")
        {
            mappedName = $"[{assignment.AssignmentType}]";
            styleName = "KeyBtnUnsupported";
        }
        else
        {
            mappedName = HidKeys.GetName(assignment.TargetHID);
            bool isRemapped = !string.Equals(assignment.TargetHID, physicalHid,
                                             StringComparison.OrdinalIgnoreCase);
            styleName = isRemapped ? "KeyBtnRemapped" : "KeyBtnDefault";
        }

        var content = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
        content.Children.Add(new TextBlock
        {
            Text = physicalLabel,
            FontSize = 9,
            Foreground = new SolidColorBrush(Color.FromRgb(0x66, 0x66, 0x66)),
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, 0, 0, 2)
        });
        content.Children.Add(new TextBlock
        {
            Text = mappedName,
            FontSize = 13,
            FontWeight = FontWeights.SemiBold,
            Foreground = Brushes.White,
            HorizontalAlignment = HorizontalAlignment.Center,
        });

        var btn = new Button
        {
            Content = content,
            Style = (Style)Resources[styleName],
            Tag = (physicalHid, assignment)
        };

        btn.Click += KeyButton_Click;
        return btn;
    }

    // ─── Key Remapping ────────────────────────────────────────────────────────

    private void KeyButton_Click(object sender, RoutedEventArgs e)
    {
        if (_currentLayer == null || sender is not Button btn) return;
        if (btn.Tag is not ValueTuple<string, KeyAssignment?> tag) return;
        var physicalHid = tag.Item1;
        var assignment = tag.Item2;
        if (assignment?.AssignmentType is string t && t != "SINGLEKEY") return;

        var picker = new KeyPickerDialog { Owner = this };
        if (picker.ShowDialog() != true || picker.SelectedKey == null) return;

        var chosen = picker.SelectedKey;

        if (assignment == null)
        {
            // Create a new assignment
            assignment = new KeyAssignment { PhysicalKeyId = physicalHid.ToUpperInvariant() };
            _currentLayer.Assignments.Add(assignment);
        }

        assignment.TargetHID = chosen.HID;
        assignment.Mod = chosen.ModString;

        StatusLabel.Text = $"Remapped {GetPhysicalLabel(physicalHid)} → {chosen.Name}  (unsaved)";
        BuildKeyButtons();
    }

    private static string GetPhysicalLabel(string hid) =>
        PhysicalKeys.FirstOrDefault(k => string.Equals(k.HID, hid,
                                                        StringComparison.OrdinalIgnoreCase)).Label
        ?? hid;

    // ─── Save ─────────────────────────────────────────────────────────────────

    private void SaveBtn_Click(object sender, RoutedEventArgs e)
    {
        if (_current == null) return;

        try
        {
            ProfileService.Save(_current);

            var dir = Path.GetDirectoryName(_current.FilePath);
            foreach (var p in _profiles.Where(p => p != _current &&
                         string.Equals(Path.GetDirectoryName(p.FilePath), dir,
                                       StringComparison.OrdinalIgnoreCase)))
                p.Active = false;
            ActiveLabel.Text = "● ACTIVE";

            StatusLabel.Text = $"Saved and set as active profile — {DateTime.Now:HH:mm:ss}";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Save failed:\n{ex.Message}", "Error",
                            MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    // ─── Event Handlers ───────────────────────────────────────────────────────

    private void ProfileCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_suppressEvents) return;
        if (ProfileCombo.SelectedItem is TartarusProfile p)
        {
            StatusLabel.Text = "";
            SelectProfile(p);
        }
    }

    private void LayerCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_suppressEvents) return;
        StatusLabel.Text = "";
        SelectLayer();
    }
}
