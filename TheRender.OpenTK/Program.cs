using System;

namespace TheRender.OpenTK
{
    class Program
    {
        static void Main(string[] args)
        {
            var window = new Window();
            window.MouseMove += (sender, eventArgs) => window.Title = window.GetPixelColor(eventArgs.Position.X, eventArgs.Position.Y) ?? "TheRender";
            window.Closing += (sender, e) => Environment.Exit(0);
            
            window.Start();
        }
    }
}
