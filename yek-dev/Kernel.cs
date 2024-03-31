using Cosmos.Core.Memory;
using Cosmos.HAL;
using Cosmos.System.Graphics.Fonts;
using System;
using System.Drawing;
using Yek.Pages;
using Yek.Render;
using Yek.Resources;

namespace Yek
{
    public class Kernel : Cosmos.System.Kernel
    {
        public static int ScreenWidth = 1024;
        public static int ScreenHeight = 768;

        public static PCScreenFont DefaultFont = PCScreenFont.Default;

        public static bool DoClear = false;
        public static bool ForceClear = false;
        public static Color ClearColor = Color.Black;

        public static int PreviousHeap = 0;

        protected override void OnBoot()
        {
            //Disable unnecessary drivers (Mousewheel, IDE Controller)
            Cosmos.System.Global.Init(GetTextScreen(), false, true, true, false);
        }

        protected override void BeforeRun()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("lpoadoimg...");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("if you see this for like more than 10 seconds then something is wrong lol whoopsie");
            Console.Beep(20000, 200);

            ResourceLoader.Load();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("loadinged resorucesf");
            Mouse.Initialize();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("did mouse thing");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("sendineg discover packet");

            try
            {
                Network.DHCP.Ask();
                PCSpeaker.Beep(37, 500);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("network failed to initialize (eth0)");
                Console.WriteLine($"{ex.Message}");
                PCSpeaker.Beep(37, 500);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("now graphic getting redy");
            Graphics.Test();
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("done now time to switch to graphic mode goodbyue");
            Graphics.Canvas.Display();
        }

        protected override void Run()
        {
            try
            {
                if (DoClear || ForceClear) Graphics.Clear(ClearColor);

                #region Pages

                Home.Draw();
                Terminal.Check();
                Terminal.Draw();
                HurtEyes.Draw();
                ModernInterface.Draw();
                PresentInterface.Draw();
                AncientInterface.Draw();
                Infinity.Draw();

                #endregion

                if (!Infinity.Active)
                {
                    Graphics.Canvas.DrawString($"M:{Terminal.Macros.Count},L:{Terminal.Looped.Count},SL:{Terminal.SetLooped.Count}", DefaultFont, Color.DarkCyan, 0, ScreenHeight - 20);
                    Mouse.Draw();
                }
                Graphics.Canvas.Display();

                PreviousHeap = Heap.Collect();
            }
            catch (Exception ex)
            {
                #region Exception

                Graphics.Canvas.Clear(Color.Red);
                Graphics.Canvas.DrawString("YOU DID IT WRONG! ITS YOUR FAULT! WHY DID YOU DO THIS ERROR EXCEPTION PROBLEM!", DefaultFont, Color.White, 0, 0);
                Graphics.Canvas.DrawString($"EX:{ex}", DefaultFont, Color.DarkRed, 0, 20);
                Graphics.Canvas.DrawString($"MSG:{ex.Message}", DefaultFont, Color.DarkRed, 0, 40);
                Graphics.Canvas.DrawString($"INNEREX:{ex.InnerException}", DefaultFont, Color.DarkRed, 0, 60);
                Graphics.Canvas.DrawString($"HRES:{ex.HResult}", DefaultFont, Color.DarkRed, 0, 80);
                Graphics.Canvas.DrawString($"DATA:{ex.Data}", DefaultFont, Color.DarkRed, 0, 100);

                #endregion
            }
        }
    }
}
