using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Yek.Resources;

namespace Yek.Render
{
    public static class Graphics
    {
        //public static Canvas Canvas = FullScreenCanvas.GetFullScreenCanvas(new Mode((uint)Kernel.ScreenWidth, (uint)Kernel.ScreenHeight, ColorDepth.ColorDepth32));
        public static Canvas Canvas = FullScreenCanvas.GetFullScreenCanvas();

        public static void Test()
        {
            Random r = new Random();
            Canvas.DrawImage(ResourceLoader.Test, 10, 10);
            for (int i = 0; i < 100; i++)
            {
                Canvas.DrawFilledRectangle(Color.FromArgb(r.Next(256), r.Next(256), r.Next(256)), r.Next(Kernel.ScreenWidth), r.Next(Kernel.ScreenHeight), r.Next(Kernel.ScreenWidth / 2), r.Next(Kernel.ScreenHeight / 2));
            }
            Canvas.DrawFilledRectangle(Color.Black, 0, 0, 100, 200);
            Canvas.DrawString("KA:THIN", ResourceLoader.FontThin, Color.White, 0, 0);
            Canvas.DrawString("KA:TINY", ResourceLoader.FontTiny, Color.White, 0, 20);
            Canvas.DrawString("LAT:SUN", ResourceLoader.FontSun, Color.White, 0, 40);
            Canvas.DrawString("TIS:AISARN", ResourceLoader.FontTisAisarn, Color.White, 0, 60);
            Canvas.DrawString("TIS:CONSL", ResourceLoader.FontTisConsl, Color.White, 0, 80);
            Canvas.DrawString("TIS:CUFONT", ResourceLoader.FontTisCufont, Color.White, 0, 100);
            Canvas.DrawString("TIS:KMFONT", ResourceLoader.FontTisKmfont, Color.White, 0, 120);
            Canvas.DrawString("TIS:LIGHT", ResourceLoader.FontTisLight, Color.White, 0, 140);
            Canvas.DrawString("TIS:RAMAFO", ResourceLoader.FontTisRamafo, Color.White, 0, 160);
            Canvas.DrawString("TIS:SMALL", ResourceLoader.FontTisSmall, Color.White, 0, 180);
        }

        public static void Clear(Color color)
        {
            Random r = new Random();
            Canvas.DrawFilledRectangle(Color.FromArgb(Math.Clamp(r.Next(color.R - 5, color.R + 5), 0, 256), Math.Clamp(r.Next(color.G - 5, color.G + 5), 0, 256), Math.Clamp(r.Next(color.B - 5, color.B + 5), 0, 256)), r.Next(Kernel.ScreenWidth), r.Next(Kernel.ScreenHeight), r.Next(Kernel.ScreenWidth / 2), r.Next(Kernel.ScreenHeight / 2));
        }

        public static void DrawImageAlpha(Image image, int x, int y)
        {
            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    Color color = Color.FromArgb(image.RawData[i + j * image.Width]);
                    if (color.A > 0) Canvas.DrawPoint(color, x + i, y + j);
                }
            }
        }
        public static void DrawString(string str, Font font, Color color, int x, int y)
        {
            byte height = font.Height;
            byte width = font.Width;

            for (int i = 0; i < str.Length; i++)
            {
                char currentChar = str[i];

                if (currentChar == ' ')
                {
                    x += width;
                }
                else
                {
                    DrawChar(currentChar, font, color, x, y);
                    x += width;
                }
            }
        }

        public static void DrawChar(char c, Font font, Color color, int x, int y)
        {
            byte height = font.Height;
            byte width = font.Width;
            byte[] data = font.Data;
            int num = height * (byte)c;

            for (int i = 0; i < height; i++)
            {
                for (byte b = 0; b < width; b++)
                {
                    if (font.ConvertByteToBitAddress(data[num + i], b + 1))
                    {
                        Canvas.DrawPoint(color, (ushort)(x + b), (ushort)(y + i));
                    }
                }
            }
        }

        public static void DrawPoints(Color color, List<Point> points)
        {
            foreach (Point point in points)
            {
                Canvas.DrawPoint(color, (ushort)point.X, (ushort)point.Y);
            }
        }
    }
}
