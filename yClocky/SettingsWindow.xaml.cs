using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace yClocky;

public partial class SettingsWindow : Window
{
    private bool _isInitialized = false;
    private FontPreviewWindow? _previewWindow;
    private FontFamily? _originalFont;

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
        foreach (var font in Fonts.SystemFontFamilies.OrderBy(f => f.Source))
        {
            FontCombo.Items.Add(font);
        }

        // Select the current font
        var currentFont = Fonts.SystemFontFamilies.FirstOrDefault(f => f.Source == settings.FontFamily);
        if (currentFont != null)
        {
            FontCombo.SelectedItem = currentFont;
        }

        TextColorBox.Text = settings.TextColor;
        BgColorBox.Text = settings.BackgroundColor;
        OpacitySlider.Value = settings.Opacity;
        ShowDateCheck.IsChecked = settings.ShowDate;
        TopmostCheck.IsChecked = settings.Topmost;
        StartupCheck.IsChecked = settings.RunOnStartup;
        GhostModeCheck.IsChecked = settings.GhostMode;
        CaptureCheck.IsChecked = settings.ExcludeFromCapture;
        MultiInstanceCheck.IsChecked = settings.AllowMultipleInstances;
    }

    private void SaveSettings()
    {
        if (!_isInitialized) return;

        var settings = SettingsManager.Current;

        if (FontCombo.SelectedItem is FontFamily selectedFont)
            settings.FontFamily = selectedFont.Source;
        
        settings.TextColor = TextColorBox.Text;
        settings.BackgroundColor = BgColorBox.Text;
        settings.Opacity = OpacitySlider.Value;
        settings.ShowDate = ShowDateCheck.IsChecked ?? true;
        settings.Topmost = TopmostCheck.IsChecked ?? true;
        settings.RunOnStartup = StartupCheck.IsChecked ?? false;
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

    private void FontCombo_DropDownOpened(object sender, EventArgs e)
    {
        // Prevent multiple preview windows
        if (_previewWindow != null)
        {
            return;
        }

        // Save the current font
        _originalFont = FontCombo.SelectedItem as FontFamily;

        if (_originalFont != null)
        {
            // Create and show preview window
            _previewWindow = new FontPreviewWindow(_originalFont);

            // Set owner to maintain proper Z-order
            _previewWindow.Owner = this;

            // Position the preview window to the right of the settings window
            _previewWindow.Left = this.Left + this.Width + 10;
            _previewWindow.Top = this.Top;
            _previewWindow.Show();
        }
    }

    private void FontCombo_DropDownClosed(object sender, EventArgs e)
    {
        // Close preview window when dropdown closes
        if (_previewWindow != null)
        {
            _previewWindow.Close();
            _previewWindow = null;
        }
    }

    private void ComboBoxItem_MouseEnter(object sender, MouseEventArgs e)
    {
        if (sender is ComboBoxItem item && item.DataContext is FontFamily font)
        {
            _previewWindow?.UpdatePreviewFont(font);
        }
    }

    private FontFamily? _lastHoveredFont;

    private void ComboBoxItem_PreviewMouseMove(object sender, MouseEventArgs e)
    {
        if (sender is ComboBoxItem item && item.DataContext is FontFamily font)
        {
            // Only update if the font has changed to avoid excessive updates
            if (_lastHoveredFont != font && _previewWindow != null)
            {
                _lastHoveredFont = font;
                _previewWindow.UpdatePreviewFont(font);
            }
        }
    }

    protected override void OnClosed(EventArgs e)
    {
        // Close preview window when settings window closes
        if (_previewWindow != null)
        {
            _previewWindow.Close();
            _previewWindow = null;
        }
        base.OnClosed(e);
    }
}
