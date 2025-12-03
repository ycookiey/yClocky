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
        UpdateClock();
        UpdateGhostMode();
    }

    private void UpdateClock()
    {
        var now = DateTime.Now;
        TimeText.Text = now.ToString("HH:mm:ss");
        DateText.Text = now.ToString("yyyy/MM/dd (ddd)");
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
            // Simple distance check from center
            var centerX = this.Left + this.Width / 2;
            var centerY = this.Top + this.Height / 2;
            
            var dx = p.X - centerX;
            var dy = p.Y - centerY;
            var dist = Math.Sqrt(dx*dx + dy*dy);
            
            // Thresholds
            double hideDist = 150; // Start fading
            double fullHideDist = 50; // Fully hidden
            
            if (dist < fullHideDist)
            {
                this.Opacity = 0;
            }
            else if (dist < hideDist)
            {
                // Linear fade
                var factor = (dist - fullHideDist) / (hideDist - fullHideDist);
                this.Opacity = SettingsManager.Current.Opacity * factor;
            }
            else
            {
                this.Opacity = SettingsManager.Current.Opacity;
            }
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

    private void SetWindowStyle()
    {
        var helper = new WindowInteropHelper(this);
        int exStyle = (int)GetWindowLong(helper.Handle, GWL_EXSTYLE);
        SetWindowLong(helper.Handle, GWL_EXSTYLE, exStyle | WS_EX_TOOLWINDOW);
    }

    private const int GWL_EXSTYLE = -20;
    private const int WS_EX_TOOLWINDOW = 0x00000080;
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