using Cosmos.System;
using System;
using System.Drawing;

namespace Yek.Render
{
    public static class Mouse
    {
        public static bool DoArc = true;

        public static void Initialize()
        {
            MouseManager.ScreenWidth = (uint)Kernel.ScreenWidth;
            MouseManager.ScreenHeight = (uint)Kernel.ScreenHeight;
        }

        public static void Draw()
        {
            if (MouseManager.MouseState == MouseState.Right)
            {
                Kernel.DoClear = true;
                if (DoArc) Graphics.Canvas.DrawArc(0, 0, Convert.ToInt32(MouseManager.X), Convert.ToInt32(MouseManager.Y), Color.Red);
                Graphics.Canvas.DrawFilledCircle(Color.Blue, Convert.ToInt32(MouseManager.X), Convert.ToInt32(MouseManager.Y), 2);
            }
            else
            {
                Kernel.DoClear = false;
                if (DoArc) Graphics.Canvas.DrawArc(0, 0, Convert.ToInt32(MouseManager.X), Convert.ToInt32(MouseManager.Y), Color.Blue);
                Graphics.Canvas.DrawFilledCircle(Color.Red, Convert.ToInt32(MouseManager.X), Convert.ToInt32(MouseManager.Y), 2);
            }
        }
    }
}
