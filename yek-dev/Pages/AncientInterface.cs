using Cosmos.Core;
using Cosmos.System;
using System.Drawing;
using Yek.Render;

namespace Yek.Pages
{
    public static class AncientInterface
    {
        public static bool Active = false;

        static int PurpleSquare = 0;
        static int YellowCircle = 0;
        static int GreenSquare = 0;

        static bool Correct = false;

        public static void Draw()
        {
            if (Active && !HurtEyes.Active)
            {
                Graphics.Canvas.Clear(Color.FromArgb(210, 210, 210));

                Graphics.Canvas.DrawFilledRectangle(Color.Purple, (Kernel.ScreenWidth / 2) - 300, (Kernel.ScreenHeight / 2) - 200, 200, 200);
                Graphics.Canvas.DrawFilledCircle(Color.Goldenrod, (Kernel.ScreenWidth / 2) + 100, (Kernel.ScreenHeight / 2) - 100, 100);
                Graphics.Canvas.DrawFilledRectangle(Color.Lime, (Kernel.ScreenWidth / 2) + 300, (Kernel.ScreenHeight / 2) - 200, 200, 200);

                Graphics.Canvas.DrawFilledRectangle(Color.White, (Kernel.ScreenWidth / 2) - 250, (Kernel.ScreenHeight / 2) + 75, 100, 30);
                Graphics.Canvas.DrawFilledRectangle(Color.White, (Kernel.ScreenWidth / 2) + 50, (Kernel.ScreenHeight / 2) + 75, 100, 30);
                Graphics.Canvas.DrawFilledRectangle(Color.White, (Kernel.ScreenWidth / 2) + 350, (Kernel.ScreenHeight / 2) + 75, 100, 30);

                Graphics.Canvas.DrawString($"{PurpleSquare}", Kernel.DefaultFont, Color.Black, (Kernel.ScreenWidth / 2) - 240, (Kernel.ScreenHeight / 2) + 83);
                Graphics.Canvas.DrawString($"{YellowCircle}", Kernel.DefaultFont, Color.Black, (Kernel.ScreenWidth / 2) + 60, (Kernel.ScreenHeight / 2) + 83);
                Graphics.Canvas.DrawString($"{GreenSquare}", Kernel.DefaultFont, Color.Black, (Kernel.ScreenWidth / 2) + 360, (Kernel.ScreenHeight / 2) + 83);

                if (PurpleSquare == 1 && YellowCircle == 1 && GreenSquare == 8) Correct = true;

                if (PurpleSquare >= 1 && YellowCircle >= 1 && GreenSquare >= 8)
                {
                    if (MouseManager.X > (Kernel.ScreenWidth / 2) + 25 && MouseManager.X < (Kernel.ScreenWidth / 2) + 175 && MouseManager.Y > (Kernel.ScreenHeight / 2) + 225 && MouseManager.Y < (Kernel.ScreenHeight / 2) + 275)
                    {
                        Graphics.Canvas.DrawFilledRectangle(Color.FromArgb(180, 180, 180), (Kernel.ScreenWidth / 2) + 25, (Kernel.ScreenHeight / 2) + 225, 150, 50);
                        Graphics.Canvas.DrawString("ENTER THE CREVACE", Kernel.DefaultFont, Color.Black, (Kernel.ScreenWidth / 2) + 35, (Kernel.ScreenHeight / 2) + 243);
                        if (MouseManager.MouseState == MouseState.Left)
                        {
                            CPU.Reboot();
                        }
                    }
                    else
                    {
                        Graphics.Canvas.DrawFilledRectangle(Color.FromArgb(150, 150, 150), (Kernel.ScreenWidth / 2) + 25, (Kernel.ScreenHeight / 2) + 225, 150, 50);
                        Graphics.Canvas.DrawString("ENTER THE CREVACE", Kernel.DefaultFont, Color.Black, (Kernel.ScreenWidth / 2) + 35, (Kernel.ScreenHeight / 2) + 243);
                    }
                }

                if (Correct)
                {
                    if (MouseManager.X > (Kernel.ScreenWidth / 2) + 25 && MouseManager.X < (Kernel.ScreenWidth / 2) + 175 && MouseManager.Y > (Kernel.ScreenHeight / 2) + 175 && MouseManager.Y < (Kernel.ScreenHeight / 2) + 225)
                    {
                        Graphics.Canvas.DrawFilledRectangle(Color.FromArgb(180, 180, 180), (Kernel.ScreenWidth / 2) + 25, (Kernel.ScreenHeight / 2) + 175, 150, 50);
                        Graphics.Canvas.DrawString("OK", Kernel.DefaultFont, Color.Black, (Kernel.ScreenWidth / 2) + 90, (Kernel.ScreenHeight / 2) + 193);
                        if (MouseManager.MouseState == MouseState.Left)
                        {
                            Active = false;
                            Home.CanAncient = false;
                            Terminal.Active = false;
                        }
                    }
                    else
                    {
                        Graphics.Canvas.DrawFilledRectangle(Color.FromArgb(150, 150, 150), (Kernel.ScreenWidth / 2) + 25, (Kernel.ScreenHeight / 2) + 175, 150, 50);
                        Graphics.Canvas.DrawString("OK", Kernel.DefaultFont, Color.Black, (Kernel.ScreenWidth / 2) + 90, (Kernel.ScreenHeight / 2) + 193);
                    }
                }
                else if (MouseManager.MouseState == MouseState.Left)
                {
                    if (MouseManager.Y > (Kernel.ScreenHeight / 2) - 200 && MouseManager.Y < (Kernel.ScreenHeight / 2))
                    {
                        if (MouseManager.X > (Kernel.ScreenWidth / 2) - 300 && MouseManager.X < (Kernel.ScreenWidth / 2) - 100)
                        {
                            PurpleSquare++;
                            MouseManager.Y = 0;
                        }

                        if (MouseManager.X > (Kernel.ScreenWidth / 2) && MouseManager.X < (Kernel.ScreenWidth / 2) + 200)
                        {
                            YellowCircle++;
                            MouseManager.Y = 0;
                        }

                        if (MouseManager.X > (Kernel.ScreenWidth / 2) + 300 && MouseManager.X < (Kernel.ScreenWidth / 2) + 500)
                        {
                            GreenSquare++;
                            MouseManager.Y = 0;
                        }
                    }
                }
            }
        }
    }
}
