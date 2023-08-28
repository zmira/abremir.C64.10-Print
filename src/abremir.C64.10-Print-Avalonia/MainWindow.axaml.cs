// Ignore Spelling: Avalonia

using System;
using System.Linq;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Threading;

namespace C64_10_Print_Avalonia
{
    public partial class MainWindow : Window
    {
        private readonly string _windowTitle = "C=64 10 PRINT [ESC to close] [SPACE to start/stop] [delay {0}ms (+/- to adjust)]";
        private readonly Random _random = new();
        private bool _running;
        private int _delay = 10;

        public MainWindow()
        {
            DispatcherTimer timer = new() { Interval = new TimeSpan(0, 0, 0, 0, 1) };
            timer.Tick += delegate { DoTick(); };
            const uint multiplier = 2;
            const uint width = 400 * multiplier;
            const uint height = 280 * multiplier;
            const uint borderWidth = 40 * multiplier; // 8px * 5 characters
            const string windowText = @"
    **** COMMODORE 64 BASIC V2 ****

 64K RAM SYSTEM  38911 BASIC BYTES FREE

READY.
";
            InitializeComponent();

            c64Window.Width = width;
            c64Window.Height = height;
            c64TextContainer.Width = width;
            c64TextContainer.Height = height;
            c64Window.Title = string.Format(_windowTitle, _delay);
            c64TextBlock.Width = width - (borderWidth * 2); // 40 columns (320px)
            c64TextBlock.Height = height - (borderWidth * 2); // 25 lines (200px)
            c64TextBlock.Text = windowText;

            bool tenPrintDisplayed = false;
            const string tenPrint = @"10 PRINT CHR$(205.5+RND(1));:GOTO 10
RUN
";

            KeyUp += (object? sender, Avalonia.Input.KeyEventArgs e) =>
            {
                switch (e.Key)
                {
                    case Avalonia.Input.Key.Escape:
                        e.Handled = true;
                        Close();
                        break;
                    case Avalonia.Input.Key.Space:
                        e.Handled = true;
                        _running = !_running;
                        if (!tenPrintDisplayed)
                        {
                            tenPrintDisplayed = true;
                            c64TextBlock.Text += tenPrint;
                        }
                        break;
                    case Avalonia.Input.Key.Add:
                    case Avalonia.Input.Key.OemPlus:
                        e.Handled = true;
                        _delay += _delay < 100 ? 10 : 100;
                        SetWindowTitle(_delay);
                        break;
                    case Avalonia.Input.Key.OemMinus:
                    case Avalonia.Input.Key.Subtract:
                        e.Handled = true;
                        if (_delay is not 0)
                        {
                            _delay -= _delay > 100 ? 100 : 10;
                            SetWindowTitle(_delay);
                        }
                        break;
                }
            };

            timer.Start();
        }

        private void DoTick()
        {
            if (_running)
            {
                var lines = c64TextBlock.Text!.Split(Environment.NewLine);
                if (lines.Length > 25)
                {
                    c64TextBlock.Text = string.Join(Environment.NewLine, lines.Skip(1));
                }
                Thread.Sleep(_delay);
                c64TextBlock.Text += (lines[^1].Length == 40 ? Environment.NewLine : string.Empty) + ((char)(_random.Next(2) == 0 ? 47 : 92)).ToString();
            }
        }

        private void SetWindowTitle(int delay)
        {
            c64Window.Title = string.Format(_windowTitle, delay);
        }
    }
}
