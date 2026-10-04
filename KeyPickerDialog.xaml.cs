using System.Windows;
using System.Windows.Controls;

namespace RazerEditor;

public partial class KeyPickerDialog : Window
{
    public HidKey? SelectedKey { get; private set; }

    public KeyPickerDialog()
    {
        InitializeComponent();
        BuildGroups(HidKeys.All);
        SearchBox.Focus();
    }

    private void BuildGroups(IEnumerable<HidKey> keys)
    {
        GroupsPanel.Children.Clear();

        var grouped = keys.GroupBy(k => k.Group).OrderBy(g => GroupOrder(g.Key));

        foreach (var group in grouped)
        {
            var header = new TextBlock
            {
                Text = group.Key.ToUpperInvariant(),
                Style = (Style)Resources["GroupHeader"]
            };
            GroupsPanel.Children.Add(header);

            var wrap = new WrapPanel();

            foreach (var key in group)
            {
                var btn = new Button
                {
                    Content = key.Name,
                    Style = (Style)Resources["KeyBtn"],
                    Tag = key
                };
                btn.Click += KeyBtn_Click;
                wrap.Children.Add(btn);
            }

            GroupsPanel.Children.Add(wrap);
        }
    }

    private void KeyBtn_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is HidKey key)
        {
            SelectedKey = key;
            DialogResult = true;
        }
    }

    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        var query = SearchBox.Text.Trim().ToLowerInvariant();
        var filtered = string.IsNullOrEmpty(query)
            ? HidKeys.All
            : HidKeys.All.Where(k => k.Name.ToLowerInvariant().Contains(query));
        BuildGroups(filtered);
    }

    private void Cancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;

    private static int GroupOrder(string group) => group switch
    {
        "Letters"    => 0,
        "Numbers"    => 1,
        "Function"   => 2,
        "Special"    => 3,
        "Navigation" => 4,
        "Numpad"     => 5,
        "Modifiers"  => 6,
        _            => 99
    };
}
