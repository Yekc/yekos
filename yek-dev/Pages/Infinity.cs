using Cosmos.System;
using System;
using System.Drawing;
using Yek.Render;
using Yek.Resources;

namespace Yek.Pages
{
    public static class Infinity
    {
        public static bool Active = false;
        public static bool First = true;

        public static void Draw()
        {
            if (Active)
            {
                Random r = new();

                Color Col = Color.FromArgb(r.Next(256), r.Next(256), r.Next(256));
                int X = r.Next(Kernel.ScreenWidth);
                int Y = r.Next(Kernel.ScreenHeight);

                if (First)
                {
                    Graphics.Canvas.Clear(Col);
                    First = false;
                }

                Graphics.Canvas.DrawPoint(Col, X, Y);
            }
        }
    }
}
