using System.Drawing;
using PastelExtended;

Console.CursorVisible = false;
PastelEx.Background = Color.FromArgb(0, 0, 0xAA);
PastelEx.Foreground = Color.FromArgb(0, 0x88, 0xFF);

PastelEx.ClearConsole();

var random = new Random();

var delay = 10;
var windowTitle = "C=64 10 PRINT [ESC to close] [SPACE to start/stop] [delay {0}ms (+/- to adjust)]";
bool running = false;
bool tenPrintDisplayed = false;
var tenPrint = @"10 PRINT CHR$(205.5+RND(1));:GOTO 10
RUN
";

Console.Title = string.Format(windowTitle, delay);

Console.Write(@"
    **** COMMODORE 64 BASIC V2 ****

 64K RAM SYSTEM  38911 BASIC BYTES FREE

READY.
".Bg(PastelEx.Background).Fg(PastelEx.Foreground));

void SetWindowTitle(int delay)
{
    Console.Title = string.Format(windowTitle!, delay);
}

var pos = 0;
while (true)
{
    if (Console.KeyAvailable)
    {
        switch (Console.ReadKey(true).Key)
        {
            case ConsoleKey.Escape:
                Environment.Exit(0);
                break;
            case ConsoleKey.Spacebar:
                running = !running;
                if (!tenPrintDisplayed)
                {
                    tenPrintDisplayed = true;
                    Console.Write(
                        tenPrint.Bg(PastelEx.Background).Fg(PastelEx.Foreground));
                }
                break;
            case ConsoleKey.Add:
                delay += delay < 100 ? 10 : 100;
                SetWindowTitle(delay);
                break;
            case ConsoleKey.Subtract:
                if (delay is not 0)
                {
                    delay -= delay > 100 ? 100 : 10;
                    SetWindowTitle(delay);
                }
                break;
        }
    }

    if (running)
    {
        if (pos > 39)
        {
            pos = 0;
        }
        Thread.Sleep(delay);
        Console.Write(
            ((char)(random.Next(2) == 0 ? 47 : 92) + (pos == 39 ? Environment.NewLine : string.Empty))
            .Bg(PastelEx.Background).Fg(PastelEx.Foreground));
        pos++;
    }
}
