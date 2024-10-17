using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;

class Program
{
    static void Main(string[] args)
    {
        GameWindowSettings gameWindowSettings = GameWindowSettings.Default;
        NativeWindowSettings nativeWindowSettings = new NativeWindowSettings()
        {
            ClientSize = new Vector2i(800, 800),
            Title = "test"
        };

        using (Game game = new Game(gameWindowSettings, nativeWindowSettings))
        {
            game.Run();
        }
    }
}