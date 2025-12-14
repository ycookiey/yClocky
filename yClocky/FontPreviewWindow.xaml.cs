using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace yClocky;

public partial class FontPreviewWindow : Window
{
    private DispatcherTimer _timer;
    private FontFamily _currentFont;

    public FontPreviewWindow(FontFamily currentFont)
    {
        InitializeComponent();
        _currentFont = currentFont;

        // Apply current settings
        ApplySettings();

        // Start timer for real-time clock
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        _timer.Tick += Timer_Tick;
        _timer.Start();

        // Update immediately
        UpdateTime();
    }

    private void ApplySettings()
    {
        var settings = SettingsManager.Current;

        // Apply current font to current preview
        CurrentTimeText.FontFamily = _currentFont;
        CurrentDateText.FontFamily = _currentFont;
        CurrentFontNameRun.Text = _currentFont.Source;

        // Initially same font for preview
        PreviewTimeText.FontFamily = _currentFont;
        PreviewDateText.FontFamily = _currentFont;
        PreviewFontNameRun.Text = _currentFont.Source;

        // Apply colors and opacity to both sections
        try
        {
            var textBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(settings.TextColor);
            var bgBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(settings.BackgroundColor);

            // Apply opacity to background color alpha channel instead of window opacity
            var bgColor = bgBrush.Color;
            bgColor.A = (byte)(bgColor.A * settings.Opacity);
            var opacityAdjustedBgBrush = new SolidColorBrush(bgColor);

            CurrentTimeText.Foreground = textBrush;
            CurrentDateText.Foreground = textBrush;
            PreviewTimeText.Foreground = textBrush;
            PreviewDateText.Foreground = textBrush;

            if (CurrentPreview.Parent is Border currentBorder)
            {
                currentBorder.Background = opacityAdjustedBgBrush;
            }
            if (PreviewPanel.Parent is Border previewBorder)
            {
                previewBorder.Background = opacityAdjustedBgBrush;
            }
        }
        catch
        {
            // Use defaults if color parsing fails
        }
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        UpdateTime();
    }

    private void UpdateTime()
    {
        var now = DateTime.Now;
        var timeString = now.ToString("HH:mm:ss");
        var dateString = now.ToString("yyyy-MM-dd");

        CurrentTimeText.Text = timeString;
        CurrentDateText.Text = dateString;
        PreviewTimeText.Text = timeString;
        PreviewDateText.Text = dateString;

        var settings = SettingsManager.Current;
        if (!settings.ShowDate)
        {
            CurrentDateText.Visibility = Visibility.Collapsed;
            PreviewDateText.Visibility = Visibility.Collapsed;
        }
    }

    public void UpdatePreviewFont(FontFamily font)
    {
        PreviewTimeText.FontFamily = font;
        PreviewDateText.FontFamily = font;
        PreviewFontNameRun.Text = font.Source;
    }

    protected override void OnClosed(EventArgs e)
    {
        _timer?.Stop();
        base.OnClosed(e);
    }
}
