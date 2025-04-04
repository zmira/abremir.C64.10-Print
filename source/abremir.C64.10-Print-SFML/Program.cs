using System.Reflection;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using TGUI;

var random = new Random();

var assembly = Assembly.GetExecutingAssembly();
using var fontStream = assembly.GetManifestResourceStream(
    assembly.GetManifestResourceNames().First(
        name => name.EndsWith("C64_Pro_Mono-STYLE.ttf")
    )
);

Font petsciiFont = new(fontStream);

var delay = 10;
uint multiplier = 2;
var windowTitle = "C=64 10 PRINT [ESC to close] [SPACE to start/stop] [delay {0}ms (+/- to adjust)]";
uint width = 400 * multiplier;
uint height = 280 * multiplier;
RenderWindow window = new(new VideoMode(width, height), string.Format(windowTitle, delay));
window.SetKeyRepeatEnabled(false);
Gui gui = new(window);

Color backgroundColour = new(0x00, 0x00, 0xAA);
Color frameColour = new(0x00, 0x88, 0xFF);
Color textColour = new(0x00, 0x88, 0xFF);
uint borderWidth = 40 * multiplier; // 8px * 5 characters

TextBoxRenderer textBoxRenderer = new()
{
    BackgroundColor = backgroundColour,
    Font = petsciiFont,
    TextColor = textColour,
    CaretColor = textColour
};

TextBox textBox = new()
{
    Position = new Vector2f(borderWidth, borderWidth),
    Size = new Vector2f(width - (borderWidth * 2) + 20, height - (borderWidth * 2) + 4), // 40 columns (320px) * 25 lines (200px)
    VerticalScrollbarPolicy = Scrollbar.Policy.Never,
    ReadOnly = true,
    Renderer = textBoxRenderer,
    TextSize = 8 * multiplier,
    Text = @"
    **** COMMODORE 64 BASIC V2 ****

 64K RAM SYSTEM  38911 BASIC BYTES FREE

READY.
"
};
gui.Add(textBox);

window.SetActive();
window.Closed += (object? sender, EventArgs e) =>
{
    if (sender is not null)
    {
        RenderWindow windowInner = (RenderWindow)sender;
        windowInner?.Close();
    }
};

bool running = false;
bool tenPrintDisplayed = false;
var tenPrint = @"10 PRINT CHR$(205.5+RND(1));:GOTO 10
RUN
";

void SetWindowTitle(int delay)
{
    window.SetTitle(string.Format(windowTitle!, delay));
}

window.KeyPressed += (object? sender, KeyEventArgs e) =>
{
    switch (e.Code)
    {
        case Keyboard.Key.Escape:
            window.Close();
            break;
        case Keyboard.Key.Space:
            running = !running;
            if (!tenPrintDisplayed)
            {
                tenPrintDisplayed = true;
                textBox.AddText(tenPrint);
            }
            break;
        case Keyboard.Key.Add:
            delay += delay < 100 ? 10 : 100;
            SetWindowTitle(delay);
            break;
        case Keyboard.Key.Hyphen:
        case Keyboard.Key.Subtract:
            if (delay is not 0)
            {
                delay -= delay > 100 ? 100 : 10;
                SetWindowTitle(delay);
            }
            break;
    }
};

while (window.IsOpen)
{
    if (running)
    {
        var lines = textBox.Text.Split(Environment.NewLine);
        if (textBox.LinesCount > 25)
        {
            textBox.Text = string.Join(Environment.NewLine, lines.Skip(1));
        }
        Thread.Sleep(delay);
        textBox.AddText((lines[^1].Length == 40 ? Environment.NewLine : string.Empty) + ((char)(random.Next(2) == 0 ? 47 : 92)).ToString());
    }

    window.DispatchEvents();
    window.Clear(frameColour);
    gui.Draw();
    window.Display();
}
