using SFML.Graphics;
using SFML.Window;

namespace Titan
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            RenderWindow window = new RenderWindow(new VideoMode(1280, 720), "Titan");
            GameWorld game = new GameWorld();

            game.createWorld();
            window.Closed += (sender, eventArgs) => window.Close();

            while (window.IsOpen())
            {
                window.DispatchEvents();
                game.update(window);
                game.render(window);
                window.Display();
            }
        }
    }
}