using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace yClocky;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private DispatcherTimer _timer;
    private SettingsWindow? _settingsWindow;

    public MainWindow()
    {
        InitializeComponent();
        
        SettingsManager.Load();
        SettingsManager.SettingsChanged += ApplySettings;
        
        InitializeClock();
        ApplySettings();
        
        // Restore position
        this.Left = SettingsManager.Current.Left;
        this.Top = SettingsManager.Current.Top;
    }

    private void InitializeClock()
    {
        _timer = new DispatcherTimer();
        _timer.Interval = TimeSpan.FromSeconds(0.1);
        _timer.Tick += Timer_Tick;
        _timer.Start();
        UpdateClock();
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        try
        {
            UpdateClock();
            UpdateClickThrough();
            UpdateGhostMode();
            EnsureTopmost();
        }
        catch
        {
            // Prevent crash from timer tick
        }
    }

    private void UpdateClock()
    {
        var now = DateTime.Now;
        TimeText.Text = now.ToString("HH:mm:ss");
        DateText.Text = now.ToString("yyyy/MM/dd (ddd)");
    }

    private void UpdateClickThrough()
    {
        var helper = new WindowInteropHelper(this);
        int exStyle = (int)GetWindowLong(helper.Handle, GWL_EXSTYLE);

        if (Keyboard.Modifiers == ModifierKeys.Control)
        {
            // Ctrl押下中はクリックを受け付ける
            SetWindowLong(helper.Handle, GWL_EXSTYLE, exStyle & ~WS_EX_TRANSPARENT);
        }
        else
        {
            // 通常時はクリックを貫通
            SetWindowLong(helper.Handle, GWL_EXSTYLE, exStyle | WS_EX_TRANSPARENT);
        }
    }

    private void UpdateGhostMode()
    {
        if (!SettingsManager.Current.GhostMode) return;

        // If Ctrl is pressed, don't hide (allow interaction)
        if (Keyboard.Modifiers == ModifierKeys.Control)
        {
            this.Opacity = SettingsManager.Current.Opacity;
            return;
        }

        POINT p;
        if (GetCursorPos(out p))
        {
            // Check if cursor is inside window bounds
            bool isInside = p.X >= this.Left &&
                           p.X <= this.Left + this.Width &&
                           p.Y >= this.Top &&
                           p.Y <= this.Top + this.Height;

            if (isInside)
            {
                // Inside window: fully transparent
                this.Opacity = 0;
            }
            else
            {
                // Outside window: calculate distance from window edge
                double dx = 0;
                double dy = 0;

                if (p.X < this.Left)
                    dx = this.Left - p.X;
                else if (p.X > this.Left + this.Width)
                    dx = p.X - (this.Left + this.Width);

                if (p.Y < this.Top)
                    dy = this.Top - p.Y;
                else if (p.Y > this.Top + this.Height)
                    dy = p.Y - (this.Top + this.Height);

                double distance = Math.Sqrt(dx * dx + dy * dy);

                // Fade distance threshold
                double fadeDistance = 100;

                if (distance < fadeDistance)
                {
                    // Gradual fade based on distance from window edge
                    double factor = distance / fadeDistance;
                    this.Opacity = SettingsManager.Current.Opacity * factor;
                }
                else
                {
                    this.Opacity = SettingsManager.Current.Opacity;
                }
            }
        }
    }

    private void EnsureTopmost()
    {
        // If Topmost is enabled in settings, force it to stay on top
        if (SettingsManager.Current.Topmost && !this.Topmost)
        {
            this.Topmost = true;
        }
    }

    private void ApplySettings()
    {
        var settings = SettingsManager.Current;

        try
        {
            // Appearance
            var converter = new BrushConverter();
            var textBrush = (Brush?)converter.ConvertFromString(settings.TextColor) ?? Brushes.White;
            var bgBrush = (Brush?)converter.ConvertFromString(settings.BackgroundColor) ?? Brushes.Transparent;

            TimeText.Foreground = textBrush;
            DateText.Foreground = textBrush;
            this.Background = bgBrush;
            
            // Only set opacity if NOT in ghost mode (Ghost mode handles it dynamically)
            if (!settings.GhostMode)
            {
                this.Opacity = settings.Opacity;
            }
            
            this.Topmost = settings.Topmost;
            
            if (!string.IsNullOrEmpty(settings.FontFamily))
            {
                var font = new FontFamily(settings.FontFamily);
                TimeText.FontFamily = font;
                DateText.FontFamily = font;
            }

            // Behavior
            DateText.Visibility = settings.ShowDate ? Visibility.Visible : Visibility.Collapsed;
            
            // Capture Protection
            var helper = new WindowInteropHelper(this);
            uint affinity = settings.ExcludeFromCapture ? WDA_EXCLUDEFROMCAPTURE : WDA_NONE;
            SetWindowDisplayAffinity(helper.Handle, affinity);
        }
        catch
        {
            // Ignore invalid settings
        }
    }

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ButtonState == MouseButtonState.Pressed)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                var startLeft = this.Left;
                var startTop = this.Top;

                try
                {
                    this.DragMove();
                    
                    // Save position after move
                    SettingsManager.Current.Left = this.Left;
                    SettingsManager.Current.Top = this.Top;
                    SettingsManager.Save();
                }
                catch { }

                var endLeft = this.Left;
                var endTop = this.Top;

                if (Math.Abs(startLeft - endLeft) < 2 && Math.Abs(startTop - endTop) < 2)
                {
                    OpenSettings();
                }
            }
        }
    }

    private void OpenSettings()
    {
        if (_settingsWindow == null || !_settingsWindow.IsLoaded)
        {
            _settingsWindow = new SettingsWindow();
            _settingsWindow.Closed += (s, e) => _settingsWindow = null;
            _settingsWindow.Show();
        }
        else
        {
            _settingsWindow.Activate();
        }
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        SetWindowStyle();
        ApplySettings(); // Apply initial settings (including affinity)
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        
        if (_timer != null)
        {
            _timer.Stop();
            _timer.Tick -= Timer_Tick;
        }
        
        SettingsManager.SettingsChanged -= ApplySettings;
        Application.Current.Shutdown();
    }

    private void SetWindowStyle()
    {
        var helper = new WindowInteropHelper(this);
        int exStyle = (int)GetWindowLong(helper.Handle, GWL_EXSTYLE);
        SetWindowLong(helper.Handle, GWL_EXSTYLE, exStyle | WS_EX_TOOLWINDOW);
    }

    private const int GWL_EXSTYLE = -20;
    private const int WS_EX_TOOLWINDOW = 0x00000080;
    private const int WS_EX_TRANSPARENT = 0x00000020;
    private const uint WDA_NONE = 0x00000000;
    private const uint WDA_EXCLUDEFROMCAPTURE = 0x00000011;

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;
    }

    [DllImport("user32.dll")]
    private static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(out POINT lpPoint);

    [DllImport("user32.dll")]
    private static extern bool SetWindowDisplayAffinity(IntPtr hWnd, uint dwAffinity);
}