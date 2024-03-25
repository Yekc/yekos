using Cosmos.System;
using System.Collections.Generic;
using System.Drawing;
using Yek.Render;
using Yek.Resources;

namespace Yek.Pages
{
    public static class ModernInterface
    {
        public static bool Active = false;
        static bool Start = false;

        static List<int> Sequence = new();

        static int FutureX = 0;
        static int FutureY = 0;
        static int FutureXDir = 1;
        static int FutureYDir = 1;

        public static void FirstDraw()
        {
            if (Active && !HurtEyes.Active)
            {
                Graphics.Canvas.Clear(Color.CornflowerBlue);
                Graphics.Canvas.DrawFilledRectangle(Color.DeepSkyBlue, 199, 199, 200, 100);
                Graphics.DrawString("welcome to the MODERN INTERFACE", Kernel.DefaultFont, Color.DodgerBlue, 200, 200);
                Graphics.DrawString("teeming with poss and bilities", Kernel.DefaultFont, Color.DodgerBlue, 210, 220);
                Graphics.DrawString("press F4 key to start...", Kernel.DefaultFont, Color.PowderBlue, 230, 260);
            }
        }

        public static void Draw()
        {
            if (Active && !HurtEyes.Active)
            {
                Graphics.Canvas.Clear(Color.CornflowerBlue);
                Graphics.Canvas.DrawImage(ResourceLoader.Modern, (Kernel.ScreenWidth / 2) - 320, (Kernel.ScreenHeight / 2) - 240);
                if (Start)
                {
                    Graphics.Canvas.DrawFilledRectangle(Color.DeepSkyBlue, 1, 1, 400, 19);
                    Graphics.Canvas.DrawString("input a sequence now and then press enter", Kernel.DefaultFont, Color.Peru, 0, 0);
                }

                #region Future

                Graphics.Canvas.DrawString("the future (it is always moving)", Kernel.DefaultFont, Color.Cyan, FutureX, FutureY);

                FutureX += FutureXDir;
                FutureY += FutureYDir;

                if (FutureX >= Kernel.ScreenWidth - 1) FutureXDir = -1;
                if (FutureX <= 1) FutureXDir = 1;
                if (FutureY >= Kernel.ScreenHeight - 1) FutureYDir = -1;
                if (FutureY <= 1) FutureYDir = 1;

                #endregion

                KeyboardManager.TryReadKey(out KeyEvent k);

                switch (k.Key)
                {
                    default:
                        if ((int)k.Key != -2071400448) Sequence.Add((int)k.Key);
                        break;
                    case ConsoleKeyEx.F4:
                        Start = true;
                        break;
                    case ConsoleKeyEx.Enter:
                        int Total = 0;
                        foreach (int Beep in Sequence) Total += Beep;
                        if (Total == 100)
                        {
                            PCSpeaker.Beep(10000, 1000);
                            PresentInterface.Active = true;
                            Active = false;
                            return;
                        }
                        if (Sequence.Count > 0) PlaySequence(Sequence);
                        Sequence.Clear();
                        break;
                }

                if (!Start)
                {
                    FirstDraw();
                    return;
                }
            }
        }

        public static void PlaySequence(List<int> Seq)
        {
            Graphics.Canvas.DrawImage(ResourceLoader.Modern, 0, 0);
            Graphics.Canvas.DrawFilledRectangle(Color.DeepSkyBlue, 1, 1, 400, 19);
            Graphics.Canvas.DrawString("sequencifying sequence...", Kernel.DefaultFont, Color.Ivory, 0, 0);

            foreach (int Beep in Seq) PCSpeaker.Beep((uint)Beep * 50, 50);
        }
    }
}
