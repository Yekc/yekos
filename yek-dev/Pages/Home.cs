using Cosmos.System;
using System;
using System.Collections.Generic;
using System.Drawing;
using Yek.Render;
using Yek.Resources;

namespace Yek.Pages
{
    public static class Home
    {
        public static int Score = 0;

        static bool Orange = false;
        public static bool CanOrange = true;
        public static bool CanAncient = true;

        static int X = 10;
        static int Y = 10;

        static int ArcX = 0;
        static int ArcY = 0;

        static int BounceX = 0;
        static int BounceY = 0;
        static int BounceX2 = 50;
        static int BounceY2 = 50;

        static int BounceXDir = 1;
        static int BounceYDir = 1;
        static int BounceXDir2 = -1;
        static int BounceYDir2 = -1;

        static int HampterFrame = 0;
        static int HampterX = 10;
        static int HampterY = 10;

        static int TextX = 10;
        static int TextY = 10;
        static int TextXDir = 5;
        static int TextYDir = -5;

        static List<Point> Past = new List<Point>() { new Point(10, 10) };

        public static void Draw()
        {
            Random r = new Random();

            //200
            #region Ancient Interface

            if (Score >= 200 && CanAncient)
            {
                AncientInterface.Active = true;
            }

            #endregion

            //100
            #region Orange Circle

            if (Score >= 100 && CanOrange)
            {
                Orange = true;
            }

            if (Orange)
            {
                Graphics.Canvas.Clear(Color.Red);
                PCSpeaker.Beep(400, 10);
                PCSpeaker.Beep(100, 10);
                PCSpeaker.Beep(400, 10);
                PCSpeaker.Beep(100, 10);
                PCSpeaker.Beep(400, 10);
                PCSpeaker.Beep(100, 10);
                PCSpeaker.Beep(400, 10);
                PCSpeaker.Beep(100, 10);
                Graphics.Canvas.DrawFilledCircle(Color.Orange, (int)(Kernel.ScreenWidth / 2), (int)(Kernel.ScreenHeight / 2), 50);
                PCSpeaker.Beep(400, 10);
                PCSpeaker.Beep(100, 10);
                PCSpeaker.Beep(400, 10);
                PCSpeaker.Beep(100, 10);
                PCSpeaker.Beep(400, 10);
                PCSpeaker.Beep(100, 10);
                PCSpeaker.Beep(400, 10);
                PCSpeaker.Beep(100, 10);
                return;
            }

            #endregion

            if (!Terminal.Active && !HurtEyes.Active && !ModernInterface.Active && !PresentInterface.Active && !AncientInterface.Active && !Infinity.Active)
            {
                //1000
                #region Chaos

                if (Score >= 1000)
                {
                    Graphics.Canvas.DrawString($"{r.Next(int.MaxValue / 2, int.MaxValue)}", Kernel.DefaultFont, Color.FromArgb(r.Next(256), r.Next(256), r.Next(256)), r.Next(Kernel.ScreenWidth), r.Next(Kernel.ScreenHeight));
                }

                #endregion

                //500
                #region Random Circles

                if (Score >= 500)
                {
                    Graphics.Canvas.DrawFilledCircle(Color.FromArgb(r.Next(256), r.Next(256), r.Next(256)), r.Next(Kernel.ScreenWidth), r.Next(Kernel.ScreenHeight), r.Next(100));
                }

                #endregion

                //250
                #region Random Squares

                if (Score >= 250)
                {
                    Graphics.Canvas.DrawFilledRectangle(Color.FromArgb(r.Next(256), r.Next(256), r.Next(256)), r.Next(Kernel.ScreenWidth), r.Next(Kernel.ScreenHeight), r.Next(Kernel.ScreenWidth / 2), r.Next(Kernel.ScreenHeight / 2));
                }

                #endregion

                //150
                #region Rat

                if (Score >= 150)
                {
                    Graphics.DrawImageAlpha(ResourceLoader.Rat, (int)MouseManager.X, (int)MouseManager.Y);
                }

                #endregion

                //125
                #region Arrow

                if (Score >= 125)
                {
                    Graphics.DrawImageAlpha(ResourceLoader.Arrow, X - 50, Y);
                }

                #endregion

                //80
                #region Orbs

                if (Score >= 80)
                {
                    int GR = r.Next(5, 25);
                    int BR = r.Next(5, 25);

                    Graphics.Canvas.DrawCircle(Color.LawnGreen, Math.Clamp((int)((BounceX + ArcX) / 2), 30, Kernel.ScreenWidth - 30), Math.Clamp((int)((BounceY2 + MouseManager.Y) / 2), 30, Kernel.ScreenHeight - 30), GR);
                    Graphics.Canvas.DrawCircle(Color.RoyalBlue, Math.Clamp((int)((BounceX2 + MouseManager.X) / 2), 30, Kernel.ScreenWidth - 30), Math.Clamp((int)((BounceY + ArcY) / 2), 30, Kernel.ScreenHeight - 30), BR);
                }

                #endregion

                //65
                #region Brown Rectangle

                if (Score >= 65)
                {
                    int Width = r.Next(10, 50);
                    int Height = r.Next(10, 50);
                    Graphics.Canvas.DrawRectangle(Color.Brown, (int)((HampterX + TextX) / 2), (int)((HampterY + TextY) / 2), Width, Height);
                }

                #endregion

                //50
                #region Hampter

                if (Score >= 50)
                {
                    if (Past.Count > 1)
                    {
                        if (HampterX != (int)((MouseManager.X + BounceX) / 2))
                        {
                            if (HampterX > (int)((MouseManager.X + BounceX) / 2))
                            {
                                HampterX--;
                            }
                            else
                            {
                                HampterX++;
                            }
                        }

                        if (HampterY != (int)((MouseManager.Y + BounceY) / 2))
                        {
                            if (HampterY > (int)((MouseManager.Y + BounceY) / 2))
                            {
                                HampterY--;
                            }
                            else
                            {
                                HampterY++;
                            }
                        }

                        switch (HampterFrame)
                        {
                            default:
                                HampterFrame = 0;
                                break;
                            case 0:
                                Graphics.Canvas.DrawImageAlpha(ResourceLoader.Hampter1, HampterX, HampterY);
                                HampterFrame++;
                                break;
                            case 1:
                                Graphics.Canvas.DrawImageAlpha(ResourceLoader.Hampter2, HampterX, HampterY);
                                HampterFrame++;
                                break;
                            case 2:
                                Graphics.Canvas.DrawImageAlpha(ResourceLoader.Hampter3, HampterX, HampterY);
                                HampterFrame++;
                                break;
                            case 3:
                                Graphics.Canvas.DrawImageAlpha(ResourceLoader.Hampter4, HampterX, HampterY);
                                HampterFrame++;
                                break;
                            case 4:
                                Graphics.Canvas.DrawImageAlpha(ResourceLoader.Hampter5, HampterX, HampterY);
                                HampterFrame++;
                                break;
                            case 5:
                                Graphics.Canvas.DrawImageAlpha(ResourceLoader.Hampter6, HampterX, HampterY);
                                HampterFrame = 0;
                                break;
                        }
                    }
                }

                #endregion

                //50
                #region Red Triangle

                if (Score >= 50) Graphics.Canvas.DrawTriangle(Color.Red, (int)MouseManager.X, (int)MouseManager.Y, BounceX, BounceY, ArcX, ArcY);

                #endregion

                //35
                #region Poland Text

                if (Score >= 35)
                {
                    if (TextY < (int)(Kernel.ScreenHeight / 2))
                    {
                        Graphics.Canvas.DrawString("I LOVE POLAND", Kernel.DefaultFont, Color.White, TextX, TextY);
                    }
                    else
                    {
                        Graphics.Canvas.DrawString("I LOVE POLAND", Kernel.DefaultFont, Color.Red, TextX, TextY);
                    }

                    TextX += TextXDir;
                    TextY += TextYDir;

                    if (TextX >= Kernel.ScreenWidth - 5) TextXDir = -5;
                    if (TextX <= 5) TextXDir = 5;
                    if (TextY >= Kernel.ScreenHeight - 5) TextYDir = -5;
                    if (TextY <= 5) TextYDir = 5;
                }

                #endregion

                //25
                #region Green Triangle

                if (MouseManager.MouseState == MouseState.Left && Score >= 25) Graphics.Canvas.DrawTriangle(Color.Lime, (int)MouseManager.X, (int)MouseManager.Y, X, Y, ArcX, ArcY);

                #endregion

                //10
                #region Blue Bounce

                if (Score >= 10)
                {
                    Graphics.Canvas.DrawLine(Color.CornflowerBlue, BounceX, BounceY, BounceX2, BounceY2);

                    BounceX += BounceXDir;
                    BounceY += BounceYDir;

                    BounceX2 += BounceXDir2;
                    BounceY2 += BounceYDir2;

                    if (BounceX >= Kernel.ScreenWidth - 1) BounceXDir = -1;
                    if (BounceX <= 1) BounceXDir = 1;
                    if (BounceY >= Kernel.ScreenHeight - 1) BounceYDir = -1;
                    if (BounceY <= 1) BounceYDir = 1;

                    if (BounceX2 >= Kernel.ScreenWidth - 1) BounceXDir2 = -1;
                    if (BounceX2 <= 1) BounceXDir2 = 1;
                    if (BounceY2 >= Kernel.ScreenHeight - 1) BounceYDir2 = -1;
                    if (BounceY2 <= 1) BounceYDir2 = 1;
                }

                #endregion

                //5
                #region Yellow Arc

                if (Score >= 5)
                {
                    Graphics.Canvas.DrawArc(ArcX, ArcY, Math.Abs(ArcX - (int)(Kernel.ScreenWidth / 2)), Math.Abs(ArcY - (int)(Kernel.ScreenHeight / 2)), Color.Yellow);
                    Graphics.Canvas.DrawFilledRectangle(Color.Green, ArcX - 2, ArcY - 2, 4, 4);
                    if (ArcX != MouseManager.X)
                    {
                        if (ArcX > MouseManager.X)
                        {
                            ArcX--;
                        }
                        else
                        {
                            ArcX++;
                        }
                    }
                    if (ArcY != MouseManager.Y)
                    {
                        if (ArcY > MouseManager.Y)
                        {
                            ArcY--;
                        }
                        else
                        {
                            ArcY++;
                        }
                    }
                }

                #endregion

                //3
                #region Pink Triangle

                if (Past.Count > 2)
                {
                    Graphics.Canvas.DrawTriangle(Color.Magenta, Past[Past.Count - 1].X, Past[Past.Count - 1].Y, Past[Past.Count - 2].X, Past[Past.Count - 2].Y, Past[Past.Count - 3].X, Past[Past.Count - 3].Y);
                }

                #endregion

                #region Score Text

                Graphics.Canvas.DrawString($"YOU HAVE CLICKED {Score} BUTONS!", Kernel.DefaultFont, Color.Green, (int)MouseManager.X, (int)MouseManager.Y);

                #endregion

                #region Button

                if (MouseManager.X > X && MouseManager.X < X + 50 && MouseManager.Y > Y && MouseManager.Y < Y + 20)
                {
                    Graphics.Canvas.DrawFilledRectangle(Color.LightBlue, X, Y, 50, 20);
                    if (MouseManager.MouseState == MouseState.Left)
                    {
                        PCSpeaker.Beep(MouseManager.X + 100, 100);

                        Score++;

                        if (Past.Count > 1)
                        {
                            HampterX = Past[Past.Count - 2].X;
                            HampterY = Past[Past.Count - 2].Y;
                        }

                        X = r.Next(10, Kernel.ScreenWidth - 60);
                        Y = r.Next(10, Kernel.ScreenHeight - 30);
                        Past.Add(new Point(X, Y));
                    }
                }
                else
                {
                    Graphics.Canvas.DrawFilledRectangle(Color.LightSkyBlue, X, Y, 50, 20);
                }

                #endregion

                #region Top Info

                Graphics.Canvas.DrawString($"M:{MouseManager.X},{MouseManager.Y}    HEAP:{Kernel.PreviousHeap}", Kernel.DefaultFont, Color.Blue, Kernel.ScreenWidth / 3, 0);

                #endregion
            }
        }
    }
}
