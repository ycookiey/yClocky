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

    public MainWindow()
    {
        InitializeComponent();
        InitializeClock();
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
    }

    private void UpdateClock()
    {
        var now = DateTime.Now;
        TimeText.Text = now.ToString("HH:mm:ss");
        DateText.Text = now.ToString("yyyy/MM/dd (ddd)");
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
                }
                catch { } // Ignore errors during drag

                var endLeft = this.Left;
                var endTop = this.Top;

                // If moved less than 2 pixels, treat as click
                if (Math.Abs(startLeft - endLeft) < 2 && Math.Abs(startTop - endTop) < 2)
                {
                    OpenSettings();
                }
            }
        }
    }

    private void OpenSettings()
    {
        // Placeholder for Settings Window
        MessageBox.Show("Settings Window will open here.", "yClocky Settings");
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        SetWindowStyle();
    }

    private void SetWindowStyle()
    {
        var helper = new WindowInteropHelper(this);
        int exStyle = (int)GetWindowLong(helper.Handle, GWL_EXSTYLE);
        SetWindowLong(helper.Handle, GWL_EXSTYLE, exStyle | WS_EX_TOOLWINDOW);
    }

    private const int GWL_EXSTYLE = -20;
    private const int WS_EX_TOOLWINDOW = 0x00000080;

    [DllImport("user32.dll")]
    private static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
}