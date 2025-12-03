using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace yClocky;

public partial class SettingsWindow : Window
{
    private bool _isInitialized = false;

    public SettingsWindow()
    {
        InitializeComponent();
        LoadSettings();
        _isInitialized = true;
    }

    private void LoadSettings()
    {
        var settings = SettingsManager.Current;

        // Load Fonts
        foreach (var font in Fonts.SystemFontFamilies)
        {
            FontCombo.Items.Add(font.Source);
        }
        FontCombo.SelectedItem = settings.FontFamily;

        TextColorBox.Text = settings.TextColor;
        BgColorBox.Text = settings.BackgroundColor;
        OpacitySlider.Value = settings.Opacity;
        ShowDateCheck.IsChecked = settings.ShowDate;
        TopmostCheck.IsChecked = settings.Topmost;
        GhostModeCheck.IsChecked = settings.GhostMode;
        CaptureCheck.IsChecked = settings.ExcludeFromCapture;
        MultiInstanceCheck.IsChecked = settings.AllowMultipleInstances;
    }

    private void SaveSettings()
    {
        if (!_isInitialized) return;

        var settings = SettingsManager.Current;
        
        if (FontCombo.SelectedItem != null)
            settings.FontFamily = FontCombo.SelectedItem.ToString();
        
        settings.TextColor = TextColorBox.Text;
        settings.BackgroundColor = BgColorBox.Text;
        settings.Opacity = OpacitySlider.Value;
        settings.ShowDate = ShowDateCheck.IsChecked ?? true;
        settings.Topmost = TopmostCheck.IsChecked ?? true;
        settings.GhostMode = GhostModeCheck.IsChecked ?? false;
        settings.ExcludeFromCapture = CaptureCheck.IsChecked ?? false;
        settings.AllowMultipleInstances = MultiInstanceCheck.IsChecked ?? false;

        SettingsManager.Save();
        
        // Notify MainWindow to update (simple static event or direct reference could work, 
        // but for now let's assume MainWindow polls or we add an event to SettingsManager)
        // For simplicity, let's add an event to SettingsManager.
    }

    private void FontCombo_SelectionChanged(object sender, SelectionChangedEventArgs e) => SaveSettings();
    private void TextColorBox_TextChanged(object sender, TextChangedEventArgs e) => SaveSettings();
    private void BgColorBox_TextChanged(object sender, TextChangedEventArgs e) => SaveSettings();
    private void OpacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) => SaveSettings();
    private void Setting_Changed(object sender, RoutedEventArgs e) => SaveSettings();

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void ExitButton_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }
}
