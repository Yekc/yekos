using System;
using System.Drawing;
using Yek.Render;

namespace Yek.Pages
{
    public static class HurtEyes
    {
        public static bool Active = false;

        public static void Draw()
        {
            if (Active)
            {
                Random r = new Random();

                Graphics.Canvas.Clear(Color.Blue);

                Graphics.Canvas.DrawArc(r.Next(Kernel.ScreenWidth / 2), r.Next(Kernel.ScreenHeight / 2), r.Next(10, (Kernel.ScreenWidth / 2) - 10), r.Next(10, (Kernel.ScreenHeight / 2) - 10), Color.DeepSkyBlue, r.Next(179), r.Next(181, 360));
                Graphics.Canvas.DrawArc(r.Next(Kernel.ScreenWidth / 2), r.Next(Kernel.ScreenHeight / 2), r.Next(10, (Kernel.ScreenWidth / 2) - 10), r.Next(10, (Kernel.ScreenHeight / 2) - 10), Color.DeepSkyBlue, r.Next(179), r.Next(181, 360));
                Graphics.Canvas.DrawArc(r.Next(Kernel.ScreenWidth / 2), r.Next(Kernel.ScreenHeight / 2), r.Next(10, (Kernel.ScreenWidth / 2) - 10), r.Next(10, (Kernel.ScreenHeight / 2) - 10), Color.DeepSkyBlue, r.Next(179), r.Next(181, 360));
                Graphics.Canvas.DrawArc(r.Next(Kernel.ScreenWidth / 2), r.Next(Kernel.ScreenHeight / 2), r.Next(10, (Kernel.ScreenWidth / 2) - 10), r.Next(10, (Kernel.ScreenHeight / 2) - 10), Color.DeepSkyBlue, r.Next(179), r.Next(181, 360));

                Graphics.Canvas.DrawArc(r.Next(Kernel.ScreenWidth / 2), r.Next(Kernel.ScreenHeight / 2), r.Next(10, (Kernel.ScreenWidth / 2) - 10), r.Next(10, (Kernel.ScreenHeight / 2) - 10), Color.BlueViolet, r.Next(179), r.Next(181, 360));
                Graphics.Canvas.DrawArc(r.Next(Kernel.ScreenWidth / 2), r.Next(Kernel.ScreenHeight / 2), r.Next(10, (Kernel.ScreenWidth / 2) - 10), r.Next(10, (Kernel.ScreenHeight / 2) - 10), Color.BlueViolet, r.Next(179), r.Next(181, 360));
                Graphics.Canvas.DrawArc(r.Next(Kernel.ScreenWidth / 2), r.Next(Kernel.ScreenHeight / 2), r.Next(10, (Kernel.ScreenWidth / 2) - 10), r.Next(10, (Kernel.ScreenHeight / 2) - 10), Color.BlueViolet, r.Next(179), r.Next(181, 360));
                Graphics.Canvas.DrawArc(r.Next(Kernel.ScreenWidth / 2), r.Next(Kernel.ScreenHeight / 2), r.Next(10, (Kernel.ScreenWidth / 2) - 10), r.Next(10, (Kernel.ScreenHeight / 2) - 10), Color.BlueViolet, r.Next(179), r.Next(181, 360));

                Graphics.Canvas.DrawString("BLUE LIGHT RADIATION SYNDROME DISEASE SPREADING DEVICE", Kernel.DefaultFont, Color.Violet, r.Next((Kernel.ScreenWidth / 2) - 220, (Kernel.ScreenWidth / 2) - 200), r.Next((Kernel.ScreenHeight / 2) - 10, (Kernel.ScreenHeight / 2) + 10));
            }
        }
    }
}
