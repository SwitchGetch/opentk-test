using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

class Program
{
    static void Main(string[] args)
    {
        GameWindowSettings gameWindowSettings = GameWindowSettings.Default;

        NativeWindowSettings nativeWindowSettings = new NativeWindowSettings()
        {
            //ClientSize = new Vector2i(1600, 900),
            WindowState = WindowState.Fullscreen,
            Title = "test"
        };

        using (Game game = new Game(gameWindowSettings, nativeWindowSettings))
        {
            game.Run();
        }
    }
}