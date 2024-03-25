using Cosmos.Core;
using Cosmos.System;
using System;
using System.Drawing;
using Yek.Render;
using Yek.Resources;

namespace Yek.Pages
{
    public static class PresentInterface
    {
        public static bool Active = false;
        static bool Start = false;

        static int Map = 0;

        static int Attempts = 0;

        static int HampterX = Kernel.ScreenWidth;
        static int HampterY = Kernel.ScreenHeight;
        static int HampterFrameCounter = 0;
        static int HampterFrame = 0;

        static int ButtonX = Kernel.ScreenWidth / 2;
        static int ButtonY = Kernel.ScreenHeight / 2;
        static int Pressed = 0;

        static int Lives = 4;

        public static void Draw()
        {
            if (Active && !HurtEyes.Active)
            {
                if (!Start)
                {
                    if (Math.Sqrt(Math.Pow(MouseManager.X - (Kernel.ScreenWidth / 2), 2) + Math.Pow(MouseManager.Y - (Kernel.ScreenHeight / 2), 2)) <= 100)
                    {
                        if (MouseManager.MouseState == MouseState.Left)
                        {
                            Start = true;
                            Graphics.Canvas.Clear(Color.DarkViolet);
                            Kernel.ClearColor = Color.DarkOrchid;
                            PCSpeaker.Beep(100, 1000);
                        }
                        else
                        {
                            Graphics.Canvas.DrawFilledCircle(Color.Coral, Kernel.ScreenWidth / 2, Kernel.ScreenHeight / 2, 100);
                        }
                    }
                    else
                    {
                        Graphics.Canvas.DrawString("click me NOW!", Kernel.DefaultFont, Color.Blue, (Kernel.ScreenWidth / 2) - 50, (Kernel.ScreenHeight / 2) - 10);
                        Graphics.Canvas.DrawFilledCircle(Color.Chocolate, Kernel.ScreenWidth / 2, Kernel.ScreenHeight / 2, 100);
                    }
                }
                else
                {
                    Random r = new Random();

                    if (Attempts > 2)
                    {
                        Graphics.Canvas.Clear(Color.FloralWhite);

                        Graphics.Canvas.DrawString($"LIVES LEFT: {Lives}", Kernel.DefaultFont, Color.Red, 0, 0);
                        Graphics.Canvas.DrawString($"BUTTONS PRESSED: {Pressed}", Kernel.DefaultFont, Color.ForestGreen, 0, 20);

                        if (Lives < 1)
                        {
                            Graphics.Canvas.Clear(Color.Red);
                            Graphics.Canvas.DrawString("YOUR CPU HAS BEEN HALTED FOO", Kernel.DefaultFont, Color.White, Kernel.ScreenWidth / 2, Kernel.ScreenHeight / 2);
                        }

                        #region Hampter

                        if (HampterY == MouseManager.Y)
                        {
                            if (Math.Sqrt(Math.Pow(MouseManager.X - HampterX, 2) + Math.Pow(MouseManager.Y - HampterY, 2)) <= 10)
                            {
                                Lives--;
                                HampterX = Kernel.ScreenWidth;
                                HampterY = Kernel.ScreenHeight;
                                Graphics.Canvas.Clear(Color.Red);
                            }
                        }

                        if (Lives < 1) CPU.Halt();

                        if (HampterX < MouseManager.X)
                        {
                            HampterX += Kernel.ScreenWidth / 640 * 2;
                        }
                        else if (HampterX > MouseManager.X)
                        {
                            HampterX -= Kernel.ScreenWidth / 640 * 2;
                        }

                        if (HampterY < MouseManager.Y)
                        {
                            HampterY += Kernel.ScreenWidth / 640 * 2;
                        }
                        else if (HampterY > MouseManager.Y)
                        {
                            HampterY -= Kernel.ScreenWidth / 640 * 2;
                        }

                        HampterFrameCounter++;
                        if (HampterFrameCounter > 10)
                        {
                            HampterFrameCounter = 0;
                            HampterFrame++;
                            if (HampterFrame > 5) HampterFrame = 0;
                        }

                        switch (HampterFrame)
                        {
                            default:
                                HampterFrame = 0;
                                break;
                            case 0:
                                Graphics.Canvas.DrawImageAlpha(ResourceLoader.Hampter1, HampterX, HampterY);
                                break;
                            case 1:
                                Graphics.Canvas.DrawImageAlpha(ResourceLoader.Hampter2, HampterX, HampterY);
                                break;
                            case 2:
                                Graphics.Canvas.DrawImageAlpha(ResourceLoader.Hampter3, HampterX, HampterY);
                                break;
                            case 3:
                                Graphics.Canvas.DrawImageAlpha(ResourceLoader.Hampter4, HampterX, HampterY);
                                break;
                            case 4:
                                Graphics.Canvas.DrawImageAlpha(ResourceLoader.Hampter5, HampterX, HampterY);
                                break;
                            case 5:
                                Graphics.Canvas.DrawImageAlpha(ResourceLoader.Hampter6, HampterX, HampterY);
                                break;
                        }

                        #endregion

                        #region Button

                        if (Map == 0)
                        {
                            Map++;
                        }
                        else
                        {
                            Map--;
                        }

                        if (Map == 0)
                        {
                            if (ButtonX > MouseManager.X)
                            {
                                ButtonX++;
                                if (ButtonX >= Kernel.ScreenWidth) ButtonX = r.Next(50, Kernel.ScreenWidth / 2);
                            }
                            else
                            {
                                ButtonX--;
                                if (ButtonX <= 0) ButtonX = r.Next(Kernel.ScreenWidth / 2, Kernel.ScreenWidth - 50);
                            }

                            if (ButtonY > MouseManager.Y)
                            {
                                ButtonY++;
                                if (ButtonY >= Kernel.ScreenHeight) ButtonY = r.Next(50, Kernel.ScreenHeight / 2);
                            }
                            else
                            {
                                ButtonY--;
                                if (ButtonY <= 0) ButtonY = r.Next(Kernel.ScreenHeight / 2, Kernel.ScreenHeight - 50);
                            }
                        }

                        if (Math.Sqrt(Math.Pow(MouseManager.X - ButtonX, 2) + Math.Pow(MouseManager.Y - ButtonY, 2)) <= 25)
                        {
                            if (MouseManager.MouseState == MouseState.Left)
                            {
                                Pressed++;
                                PCSpeaker.Beep(1000, 10);
                                ButtonX = r.Next((Kernel.ScreenWidth / 2) - 200, (Kernel.ScreenWidth / 2) + 200);
                                ButtonY = r.Next((Kernel.ScreenHeight / 2) - 200, (Kernel.ScreenHeight / 2) + 200);
                            }
                            else
                            {
                                Graphics.Canvas.DrawFilledCircle(Color.Lime, ButtonX, ButtonY, 25);
                            }
                        }
                        else
                        {
                            Graphics.Canvas.DrawFilledCircle(Color.ForestGreen, ButtonX, ButtonY, 25);
                        }

                        if (Pressed >= 5)
                        {
                            Active = false;
                            ModernInterface.Active = false;
                            Terminal.Active = false;
                            HurtEyes.Active = false;
                            Home.CanOrange = false;

                            Kernel.ClearColor = Color.Black;
                            Graphics.Canvas.Clear(Color.HotPink);
                            Graphics.Canvas.DrawFilledCircle(Color.Brown, Kernel.ScreenWidth / 2, Kernel.ScreenHeight / 2, 400);
                            Graphics.Canvas.DrawString("DISABLED ORANGE CIRCLE SUCCESS FULL", Kernel.DefaultFont, Color.Blue, (Kernel.ScreenWidth / 2) - 200, (Kernel.ScreenHeight / 2) - 10);

                            PCSpeaker.Beep(5000, 2500);
                        }

                        #endregion
                    }
                    else
                    {
                        #region Map

                        if (Map == 0)
                        {
                            Graphics.Canvas.DrawImage(ResourceLoader.Map1, (Kernel.ScreenWidth / 2) - 320, (Kernel.ScreenHeight / 2) - 280);
                            Map++;
                        }
                        else
                        {
                            Graphics.Canvas.DrawImage(ResourceLoader.Map2, (Kernel.ScreenWidth / 2) - 320, (Kernel.ScreenHeight / 2) - 280);
                            Map--;
                        }

                        if (Math.Sqrt(Math.Pow(MouseManager.X - (Kernel.ScreenWidth / 2), 2) + Math.Pow(MouseManager.Y - (Kernel.ScreenHeight / 2), 2)) <= 200)
                        {
                            MouseManager.X = (uint)r.Next(400);
                            MouseManager.Y = (uint)r.Next(480);
                            Attempts++;
                        }

                        #endregion
                    }
                }
            }
        }
    }
}
