using Cosmos.Core;
using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Network.Config;
using CosmosHttp.Client;
using System;
using System.Collections.Generic;
using System.Drawing;
using Yek.Render;
using Yek.Resources;

namespace Yek.Pages
{
    public static class Terminal
    {
        public static bool Active = false;

        static string Content = "";
        static int LineY = 0;
        static int Dip = 65;
        static int LineX = 5;
        static bool Fm = false;

        static List<string> Looped = new();
        static Dictionary<int, string> Macros = new();

        static bool PersistLoops = false;
        static bool PersistMacros = true;

        static List<string> PreviousContent = new();
        static int PreviousContentCycles = 0;

        public static void Check()
        {
            if (ModernInterface.Active) return;
            KeyboardManager.TryReadKey(out KeyEvent k);

            if ((int)k.Key != -2071400448) Graphics.Canvas.DrawString($"you pressed {(int)k.Key}", Kernel.DefaultFont, Color.Cyan, 0, Kernel.ScreenHeight - 40);

            if (Macros.ContainsKey((int)k.Key))
            {
                try
                {
                    ProcessCommand(Macros[(int)k.Key]);
                }
                catch
                {
                    Graphics.Canvas.DrawString($"MACRO ERROR FOR KEY {(int)k.Key}", Kernel.DefaultFont, Color.Red, 0, 0);
                }
            }

            switch (k.Key)
            {
                default:
                    if (Active && char.IsAscii(k.KeyChar)) Content += k.KeyChar;
                    break;
                case ConsoleKeyEx.F1:
                    Content += "⠀";
                    break;
                case ConsoleKeyEx.Escape:
                    if (HurtEyes.Active || AncientInterface.Active) break;
                    Active = !Active;
                    if (Active)
                    {
                        Kernel.ClearColor = Color.Beige;
                    }
                    else
                    {
                        Kernel.ClearColor = Color.Black;
                    }
                    Content = "";
                    LineY = 0;
                    Dip = 65;
                    LineX = 5;
                    Fm = false;
                    if (!PersistLoops) Looped.Clear();
                    if (!PersistMacros) Macros.Clear();
                    if (Active) FirstDraw();
                    break;
                case ConsoleKeyEx.Enter:
                    if (Active && Content != "")
                    {
                        LineY++;
                        if (LineY > (Kernel.ScreenHeight / 20) - 4)
                        {
                            LineY = 0;
                            if (Dip == 65) Dip = 5;
                        }
                        ProcessCommand(Content);
                        PreviousContent.Add(Content);
                        PreviousContentCycles = 0;
                        Content = "";
                    }
                    break;
                case ConsoleKeyEx.Backspace:
                    if (Content.Length > 0) Content = Content.Remove(Content.Length - 1);
                    break;
                case ConsoleKeyEx.Tab:
                    Content += "    ";
                    break;
                case ConsoleKeyEx.UpArrow:
                    if (PreviousContent.Count > PreviousContentCycles)
                    {
                        PreviousContentCycles++;
                        Content = PreviousContent[PreviousContent.Count - PreviousContentCycles];
                    }
                    break;
                case ConsoleKeyEx.DownArrow:
                    if (PreviousContentCycles > 1)
                    {
                        PreviousContentCycles--;
                        Content = PreviousContent[PreviousContent.Count - PreviousContentCycles];
                    }
                    break;
                case ConsoleKeyEx.NoName:
                    HurtEyes.Active = !HurtEyes.Active;
                    Active = !Active;
                    Content = "";
                    LineY = 0;
                    Dip = 65;
                    LineX = 5;
                    Fm = false;
                    if (!PersistLoops) Looped.Clear();
                    if (!PersistMacros) Macros.Clear();
                    break;
            }
        }

        static void FirstDraw()
        {
            Graphics.Canvas.Clear(Color.Beige);
            Graphics.Canvas.DrawRectangle(Color.Brown, 2, 2, 642, 41);
            Graphics.Canvas.DrawString("THIS IS THE BEST TERMINAL EV3R!!!!!!!!", Kernel.DefaultFont, Color.Brown, 5, 5);
            Graphics.Canvas.DrawString("Say like @help or something if you dunno what ta do L0LZ!!", Kernel.DefaultFont, Color.Brown, 5, 25);
        }

        public static void Draw()
        {
            if (ModernInterface.Active) return;

            foreach (string Command in Looped)
            {
                try
                {
                    ProcessCommand(Command);
                }
                catch
                {
                    Graphics.Canvas.DrawString("LOOP ERROR", Kernel.DefaultFont, Color.Red, 0, 0);
                }
            }

            if (Active)
            {
                if (Fm)
                {
                    Dip = (int)MouseManager.Y;
                    LineX = (int)MouseManager.X;
                }

                if (Content.Length < 25)
                {
                    Graphics.Canvas.DrawString($"{Content}", Kernel.DefaultFont, Color.Brown, LineX, Dip + (LineY * 20));
                }
                else
                {
                    Graphics.Canvas.DrawString($"{Content.Substring(Content.Length - 25)}", Kernel.DefaultFont, Color.Brown, LineX, Dip + (LineY * 20));
                }
            }
        }

        static void ProcessCommand(string Command)
        {
            if (Command.Contains('⠀'))
            {
                foreach (string Cmd in Command.Split('⠀'))
                {
                    ProcessCommand(Cmd);
                }
                return;
            }

            if (Command.Contains("string") || Command.Contains("text")) Command = Command.Replace("$mx", "$TEMP$!!<%>mx").Replace("$my", "$TEMP!!<%>my").Replace("$b", "$TEMP!!<%>b");
            Command = Command.Replace("$mx", MouseManager.X.ToString()).Replace("$my", MouseManager.Y.ToString()).Replace("$b", Home.Score.ToString());

            if (Command.StartsWith('@') || Command.StartsWith('&'))
            {
                if (Command.StartsWith('&')) Looped.Add(Command.TrimStart('&'));
                string[] Split = Command.Split(' ');
                switch (Split[0].ToLower().TrimStart('&'))
                {
                    default:
                        Graphics.Canvas.DrawString("??? dafuq", Kernel.DefaultFont, Color.Red, LineX, Dip + (LineY * 20));
                        LineY++;
                        break;
                    #region If Statements
                    case "@?":
                        if (Split.Length > 6)
                        {
                            bool Run = false;
                            switch (Split[2])
                            {
                                default:
                                    Graphics.Canvas.DrawString("=, <, >, !", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                                    LineY++;
                                    break;
                                case "=":
                                    Run = Split[1] == Split[3];
                                    break;
                                case "<":
                                    Run = Convert.ToInt32(Split[1]) < Convert.ToInt32(Split[3]);
                                    break;
                                case ">":
                                    Run = Convert.ToInt32(Split[1]) > Convert.ToInt32(Split[3]);
                                    break;
                                case "!":
                                    Run = Split[1] != Split[3];
                                    break;
                            }

                            if (Run)
                            {
                                string NewCommand = "";
                                for (int i = 4; i < Split.Length; i++)
                                {
                                    NewCommand += Split[i] + " ";
                                }
                                ProcessCommand(NewCommand);
                            }
                        }
                        else
                        {
                            Graphics.Canvas.DrawString("@? $ = $ [command], @? $ < $ [command], @? $ > $ [command], @? $ ! $ [command]", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                            LineY++;
                        }
                        break;
                    #endregion
                    #region Help
                    case "@what":
                    case "@help":
                        if (Split.Length > 1)
                        {
                            try
                            {
                                switch (Convert.ToInt32(Split[1]))
                                {
                                    default:
                                        Graphics.Canvas.DrawString("HELP PAGE 1:", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                                        LineY++;
                                        Graphics.Canvas.DrawString("press f1 to chain commands together", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                                        LineY++;
                                        Graphics.Canvas.DrawString("&@[command] - loop", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                                        LineY++;
                                        Graphics.Canvas.DrawString("@? - if statement", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                                        LineY++;
                                        Graphics.Canvas.DrawString("@exit - leave terminal", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                                        LineY++;
                                        Graphics.Canvas.DrawString("@sysinfo - get information about your computer", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                                        LineY++;
                                        break;
                                    case 2:
                                        Graphics.Canvas.DrawString("HELP PAGE 2:", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                                        LineY++;
                                        Graphics.Canvas.DrawString("@followmouse - make the terminal follow the mouse", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                                        LineY++;
                                        Graphics.Canvas.DrawString("@doarc - toggle mouse arcs", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                                        LineY++;
                                        Graphics.Canvas.DrawString("@c [line] - move cursor to line", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                                        LineY++;
                                        Graphics.Canvas.DrawString("@set [variable name] [value] - set a variable to a value", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                                        LineY++;
                                        Graphics.Canvas.DrawString("@halt - halt the cpu", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                                        LineY++;
                                        break;
                                    case 3:
                                        Graphics.Canvas.DrawString("HELP PAGE 3:", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                                        LineY++;
                                        Graphics.Canvas.DrawString("@shutdown - shut down", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                                        LineY++;
                                        Graphics.Canvas.DrawString("@reboot - reboot the cpu", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                                        LineY++;
                                        Graphics.Canvas.DrawString("@clear [color] - clear the screen", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                                        LineY++;
                                        Graphics.Canvas.DrawString("@forceclear - toggles force clear", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                                        LineY++;
                                        Graphics.Canvas.DrawString("@draw - draw stuff on screen", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                                        LineY++;
                                        break;
                                    case 4:
                                        Graphics.Canvas.DrawString("HELP PAGE 4:", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                                        LineY++;
                                        Graphics.Canvas.DrawString("@font - set the system font", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                                        LineY++;
                                        Graphics.Canvas.DrawString("@http [post/get/?] [ip] [domain] [path] - send http requests or get network info", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                                        LineY++;
                                        Graphics.Canvas.DrawString("@memory - do stuff with memory", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                                        LineY++;
                                        Graphics.Canvas.DrawString("@persist [loops/macros] - toggle macros/loops persisting when terminal is closed", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                                        LineY++;
                                        Graphics.Canvas.DrawString("@beep [hertz] [ms] - beep", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                                        LineY++;
                                        break;
                                    case 5:
                                        Graphics.Canvas.DrawString("HELP PAGE 5:", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                                        LineY++;
                                        Graphics.Canvas.DrawString("@macro [key] command - assign a command to a key", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                                        LineY++;
                                        Graphics.Canvas.DrawString("@time - get the current time", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                                        LineY++;
                                        Graphics.Canvas.DrawString("@hampster - wawaweewa", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                                        LineY++;
                                        Graphics.Canvas.DrawString("@ascend - top secret", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                                        LineY++;
                                        Graphics.Canvas.DrawString("@descend - top secret", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                                        LineY++;
                                        break;
                                    case 6:
                                        Graphics.Canvas.DrawString("HELP PAGE 6:", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                                        LineY++;
                                        Graphics.Canvas.DrawString("$mx - mouse x position", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                                        LineY++;
                                        Graphics.Canvas.DrawString("$my - mouse y position", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                                        LineY++;
                                        Graphics.Canvas.DrawString("$b - buttons pressed", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                                        LineY++;
                                        break;
                                }
                            }
                            catch
                            {
                                Graphics.Canvas.DrawString("@help [page]", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                                LineY++;
                            }
                        }
                        else
                        {
                            Graphics.Canvas.DrawString("@help [page]", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                            LineY++;
                        }
                        break;
                    #endregion
                    #region Basic Commands
                    case "@sysinfo":
                    case "@info":
                    case "@i":
                    case "@neofetch":
                        Graphics.Canvas.DrawString("here is everything you will ever need to know:", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                        LineY++;
                        Graphics.Canvas.DrawString($"TOTAL RAM: {CPU.GetAmountOfRAM() + 1} MB, USED: {(CPU.GetEndOfKernel() + 1024) / 1048576} MB, FREE: {CPU.GetAmountOfRAM() + 1 - ((CPU.GetEndOfKernel() + 1024) / 1048576)} MB", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                        LineY++;
                        Graphics.Canvas.DrawString($"CPU: {CPU.GetCPUBrandString()}", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                        LineY++;
                        break;
                    case "@fm":
                    case "@followmouse":
                    case "@mouse":
                    case "@m":
                        Fm = !Fm;
                        break;
                    case "@doarc":
                    case "@doarcs":
                    case "@arc":
                    case "@arcs":
                    case "@da":
                    case "@mousearc":
                    case "@mousearcs":
                        Mouse.DoArc = !Mouse.DoArc;
                        break;
                    case "@c":
                    case "@cursor":
                        if (Split.Length > 1)
                        {
                            LineY = Convert.ToInt32(Split[1]);
                        }
                        else
                        {
                            Graphics.Canvas.DrawString("@c [line]", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                            LineY++;
                        }
                        break;
                    case "@set":
                    case "@s":
                    case "@v":
                        if (Split.Length > 2)
                        {
                            try
                            {
                                switch (Split[1].ToLower())
                                {
                                    default:
                                        Graphics.Canvas.DrawString("??? its just mx or my or b", Kernel.DefaultFont, Color.Red, LineX, Dip + (LineY * 20));
                                        LineY++;
                                        break;
                                    case "mx":
                                        MouseManager.X = Convert.ToUInt32(Split[2]);
                                        break;
                                    case "my":
                                        MouseManager.Y = Convert.ToUInt32(Split[2]);
                                        break;
                                    case "b":
                                        Home.Score = Convert.ToInt32(Split[2]);
                                        break;
                                }
                            }
                            catch
                            {
                                if (Command.StartsWith('&')) Looped.RemoveAt(Looped.IndexOf(Command));
                                Graphics.Canvas.DrawString("??? you did something wrong i think", Kernel.DefaultFont, Color.Red, LineX, Dip + (LineY * 20));
                                LineY++;
                            }
                        }
                        else
                        {
                            Graphics.Canvas.DrawString("@set [variable name] [value]", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                            LineY++;
                        }
                        break;
                    case "@escape":
                    case "@out":
                    case "@getouttahere":
                    case "@getouddahere":
                    case "@leave":
                    case "@button":
                    case "@buttons":
                    case "@orangecircle":
                    case "@exit":
                    case "@quit":
                    case "@q":
                        Active = !Active;
                        Content = "";
                        LineY = 0;
                        break;
                    case "@shutdown":
                    case "@sd":
                        Power.Shutdown();
                        break;
                    case "@halt":
                    case "@die":
                    case "@h":
                        CPU.Halt();
                        break;
                    case "@reboot":
                    case "@restart":
                    case "@r":
                        CPU.Reboot();
                        break;
                    case "@memory":
                    case "@mem":

                        break;
                    case "@persist":
                    case "@ps":
                    case "@p":
                        if (Split.Length > 1)
                        {
                            switch (Split[1])
                            {
                                default:
                                    Graphics.Canvas.DrawString("@persist [loops/macros]", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                                    LineY++;
                                    break;
                                case "macros":
                                case "macro":
                                case "m":
                                    PersistMacros = !PersistMacros;
                                    if (PersistMacros)
                                    {
                                        Graphics.Canvas.DrawString("macros will now persist", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                                        LineY++;
                                    }
                                    else
                                    {
                                        Graphics.Canvas.DrawString("macros will no longer persist", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                                        LineY++;
                                    }
                                    break;
                                case "loops":
                                case "loop":
                                case "l":
                                    PersistLoops = !PersistLoops;
                                    if (PersistLoops)
                                    {
                                        Graphics.Canvas.DrawString("loops will now persist", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                                        LineY++;
                                    }
                                    else
                                    {
                                        Graphics.Canvas.DrawString("loops will no longer persist", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                                        LineY++;
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            Graphics.Canvas.DrawString("@persist [loops/macros]", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                            LineY++;
                        }
                        break;
                    case "@beep":
                        if (Split.Length > 2)
                        {
                            try
                            {
                                PCSpeaker.Beep(Convert.ToUInt32(Split[1]), Convert.ToUInt32(Split[2]));
                            }
                            catch
                            {
                                Graphics.Canvas.DrawString("@beep [hertz] [ms]", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                                LineY++;
                            }
                        }
                        else
                        {
                            Graphics.Canvas.DrawString("@beep [hertz] [ms]", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                            LineY++;
                        }
                        break;
                    case "@macro":
                    case "@mc":
                    case "@assign":
                        if (Split.Length > 2)
                        {
                            string Cmd = "";
                            for (int i = 2; i < Split.Length; i++)
                            {
                                Cmd += Split[i] + " ";
                            }
                            Macros.Add(Convert.ToInt32(Split[1]), Cmd);
                            Graphics.Canvas.DrawString($"added new macro for key {Split[1]}: {Cmd}", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                            LineY++;
                            Graphics.Canvas.DrawString("not undoable (unless you reboot)", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                            LineY++;
                        }
                        else
                        {
                            Graphics.Canvas.DrawString("@macro [key] command", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                            LineY++;
                        }
                        break;
                    case "@time":
                    case "@t":
                    case "@now":
                    case "@date":
                        Graphics.Canvas.DrawString($"it is {Cosmos.HAL.RTC.Month}/{Cosmos.HAL.RTC.DayOfTheMonth}/{Cosmos.HAL.RTC.Year} and the time is {Cosmos.HAL.RTC.Hour}:{Cosmos.HAL.RTC.Minute}:{Cosmos.HAL.RTC.Second}", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                        LineY++;
                        break;
                    case "@hampster":
                    case "@hamster":
                    case "@hampter":
                    case "@hm":
                    case "@hamter":
                    case "@guineapig":
                    case "@guineaupig":
                    case "@ginneepig":
                    case "@gineepig":
                    case "@ilovethisos":
                    case "@wawaweewa":
                        Graphics.Canvas.DrawImageAlpha(ResourceLoader.Hampter1, LineX, Dip + LineY);
                        break;
                    case "@ascend":
                        if (Home.Score == 23)
                        {
                            ModernInterface.Active = true;
                            Active = false;
                            Content = "";
                            LineY = 0;
                            Dip = 65;
                            LineX = 5;
                            Fm = false;
                            Looped.Clear();
                        }
                        else
                        {
                            if (Home.Score > 23)
                            {
                                Graphics.Canvas.DrawString("too late", Kernel.DefaultFont, Color.DarkKhaki, LineX, Dip + (LineY * 20));
                                LineY++;
                            }
                            else
                            {
                                Graphics.Canvas.DrawString("click some more buttons", Kernel.DefaultFont, Color.DarkKhaki, LineX, Dip + (LineY * 20));
                                LineY++;
                            }
                        }
                        break;
                    case "@descend":
                        HurtEyes.Active = true;
                        Active = false;
                        break;
                    #endregion
                    #region Draw Commands
                    case "@clear":
                    case "@clr":
                        if (Split.Length > 1)
                        {
                            try
                            {
                                Graphics.Canvas.Clear(Color.FromName($"{Split[1][0].ToString().ToUpper()}{Split[1].ToString().Substring(1).ToLower()}"));
                            }
                            catch
                            {
                                if (Command.StartsWith('&')) Looped.RemoveAt(Looped.IndexOf(Command));
                                Graphics.Canvas.DrawString("??? that is not a COLOLR!!", Kernel.DefaultFont, Color.Red, LineX, Dip + (LineY * 20));
                                LineY++;
                                Graphics.Canvas.DrawString("roygbiv (but not blue or indigo or violet i must save your eye from blue light)", Kernel.DefaultFont, Color.Red, LineX, Dip + (LineY * 20));
                                LineY++;
                            }
                        }
                        else
                        {
                            Graphics.Canvas.Clear();
                        }
                        break;
                    case "@forceclear":
                    case "@fc":
                        Kernel.ForceClear = !Kernel.ForceClear;
                        if (!Kernel.ForceClear)
                        {
                            Graphics.Canvas.DrawString("OK ITS OFF!!!!!!", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                            LineY++;
                        }
                        break;
                    case "@draw":
                    case "@d":
                        if (Split.Length > 1)
                        {
                            switch (Split[1].ToLower())
                            {
                                default:
                                    Graphics.Canvas.DrawString("@draw [shape]", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                                    LineY++;
                                    break;
                                case "point":
                                    if (Split.Length > 3)
                                    {
                                        try
                                        {
                                            Graphics.Canvas.DrawPoint(Color.FromName($"{Split[4][0].ToString().ToUpper()}{Split[4].ToString().Substring(1).ToLower()}"), Convert.ToInt32(Split[2]), Convert.ToInt32(Split[3]));
                                        }
                                        catch
                                        {
                                            if (Command.StartsWith('&')) Looped.RemoveAt(Looped.IndexOf(Command));
                                            Graphics.Canvas.DrawString("@draw point [x] [y] [color]", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                                            LineY++;
                                        }
                                    }
                                    else
                                    {
                                        Graphics.Canvas.DrawString("@draw point [x] [y] [color]", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                                        LineY++;
                                    }
                                    break;
                                case "text":
                                case "string":
                                    if (Split.Length > 4)
                                    {
                                        try
                                        {
                                            string Text = "";
                                            for (int i = 5; i < Split.Length; i++)
                                            {
                                                Text += Split[i] + " ";
                                            }
                                            Text = Text.Replace("$TEMP!!<%>mx", MouseManager.X.ToString()).Replace("$TEMP!!<%>my", MouseManager.Y.ToString()).Replace("$TEMP!!<%>b", Home.Score.ToString());
                                            Graphics.Canvas.DrawString($"{Text}", Kernel.DefaultFont, Color.FromName($"{Split[4][0].ToString().ToUpper()}{Split[4].ToString().Substring(1).ToLower()}"), Convert.ToInt32(Split[2]), Convert.ToInt32(Split[3]));
                                        }
                                        catch
                                        {
                                            if (Command.StartsWith('&')) Looped.RemoveAt(Looped.IndexOf(Command));
                                            Graphics.Canvas.DrawString("@draw string [x] [y] [color] text", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                                            LineY++;
                                        }
                                    }
                                    else
                                    {
                                        Graphics.Canvas.DrawString("@draw string [x] [y] [color] text", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                                        LineY++;
                                    }
                                    break;
                                case "square":
                                case "rectangle":
                                    if (Split.Length > 6)
                                    {
                                        try
                                        {
                                            switch (Split[7])
                                            {
                                                default:
                                                case "false":
                                                case "no":
                                                    Graphics.Canvas.DrawRectangle(Color.FromName($"{Split[6][0].ToString().ToUpper()}{Split[6].ToString().Substring(1).ToLower()}"), Convert.ToInt32(Split[2]), Convert.ToInt32(Split[3]), Convert.ToInt32(Split[4]), Convert.ToInt32(Split[5]));
                                                    break;
                                                case "true":
                                                case "yes":
                                                case "fill":
                                                    Graphics.Canvas.DrawFilledRectangle(Color.FromName($"{Split[6][0].ToString().ToUpper()}{Split[6].ToString().Substring(1).ToLower()}"), Convert.ToInt32(Split[2]), Convert.ToInt32(Split[3]), Convert.ToInt32(Split[4]), Convert.ToInt32(Split[5]));
                                                    break;
                                            }
                                        }
                                        catch
                                        {
                                            if (Command.StartsWith('&')) Looped.RemoveAt(Looped.IndexOf(Command));
                                            Graphics.Canvas.DrawString("@draw rectangle [x] [y] [width] [height] [color] [fill]", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                                            LineY++;
                                        }
                                    }
                                    else
                                    {
                                        Graphics.Canvas.DrawString("@draw rectangle [x] [y] [width] [height] [color] [fill]", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                                        LineY++;
                                    }
                                    break;
                                case "oval":
                                case "circle":
                                case "ellipse":
                                    if (Split.Length > 6)
                                    {
                                        try
                                        {
                                            switch (Split[7])
                                            {
                                                default:
                                                case "false":
                                                case "no":
                                                    Graphics.Canvas.DrawEllipse(Color.FromName($"{Split[6][0].ToString().ToUpper()}{Split[6].ToString().Substring(1).ToLower()}"), Convert.ToInt32(Split[2]), Convert.ToInt32(Split[3]), Convert.ToInt32(Split[4]), Convert.ToInt32(Split[5]));
                                                    break;
                                                case "true":
                                                case "yes":
                                                case "fill":
                                                    Graphics.Canvas.DrawFilledEllipse(Color.FromName($"{Split[6][0].ToString().ToUpper()}{Split[6].ToString().Substring(1).ToLower()}"), Convert.ToInt32(Split[2]), Convert.ToInt32(Split[3]), Convert.ToInt32(Split[4]), Convert.ToInt32(Split[5]));
                                                    break;
                                            }
                                        }
                                        catch
                                        {
                                            if (Command.StartsWith('&')) Looped.RemoveAt(Looped.IndexOf(Command));
                                            Graphics.Canvas.DrawString("@draw ellipse [x] [y] [width] [height] [color] [fill]", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                                            LineY++;
                                        }
                                    }
                                    else
                                    {
                                        Graphics.Canvas.DrawString("@draw ellipse [x] [y] [width] [height] [color] [fill]", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                                        LineY++;
                                    }
                                    break;
                                case "arch":
                                case "arc":
                                    if (Split.Length > 8)
                                    {
                                        try
                                        {
                                            Graphics.Canvas.DrawArc(Convert.ToInt32(Split[2]), Convert.ToInt32(Split[3]), Convert.ToInt32(Split[4]), Convert.ToInt32(Split[5]), Color.FromName($"{Split[8][0].ToString().ToUpper()}{Split[8].ToString().Substring(1).ToLower()}"), Convert.ToInt32(Split[6]), Convert.ToInt32(Split[7]));
                                        }
                                        catch
                                        {
                                            if (Command.StartsWith('&')) Looped.RemoveAt(Looped.IndexOf(Command));
                                            Graphics.Canvas.DrawString("@draw arc [x] [y] [width] [height] [start angle] [end angle] [color]", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                                            LineY++;
                                        }
                                    }
                                    else
                                    {
                                        Graphics.Canvas.DrawString("@draw arc [x] [y] [width] [height] [start angle] [end angle] [color]", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                                        LineY++;
                                    }
                                    break;
                                case "image":
                                case "picture":
                                    Graphics.Canvas.DrawString("lol didnt implement this yet", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                                    LineY++;
                                    break;
                            }
                        }
                        else
                        {
                            Graphics.Canvas.DrawString("@draw [shape]", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                            LineY++;
                        }
                        break;
                    case "@font":
                    case "@fnt":
                    case "@typeface":
                    case "@type":
                    case "@glyph":
                        if (Split.Length > 1)
                        {
                            switch (Split[1])
                            {
                                default:
                                    Graphics.Canvas.DrawString("@font [default/cosmos/thin/sun/aisarn/consl/kmfont/light/ramafo/small]", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                                    LineY++;
                                    break;
                                case "default":
                                case "d":
                                case "cosmos":
                                case "os":
                                case "system":
                                case "sys":
                                    Kernel.DefaultFont = Cosmos.System.Graphics.Fonts.PCScreenFont.Default;
                                    break;
                                case "thin":
                                    Kernel.DefaultFont = ResourceLoader.FontThin;
                                    break;
                                case "sun":
                                    Kernel.DefaultFont = ResourceLoader.FontSun;
                                    break;
                                case "aisarn":
                                    Kernel.DefaultFont = ResourceLoader.FontTisAisarn;
                                    break;
                                case "consl":
                                case "console":
                                case "c":
                                    Kernel.DefaultFont = ResourceLoader.FontTisConsl;
                                    break;
                                case "cufont":
                                case "cu":
                                    Kernel.DefaultFont = ResourceLoader.FontTisCufont;
                                    break;
                                case "kmfont":
                                case "km":
                                    Kernel.DefaultFont = ResourceLoader.FontTisKmfont;
                                    break;
                                case "light":
                                    Kernel.DefaultFont = ResourceLoader.FontTisLight;
                                    break;
                                case "ramafo":
                                    Kernel.DefaultFont = ResourceLoader.FontTisRamafo;
                                    break;
                                case "small":
                                    Kernel.DefaultFont = ResourceLoader.FontTisSmall;
                                    break;
                            }
                        }
                        else
                        {
                            Graphics.Canvas.DrawString("@font [default/cosmos/thin/sun/aisarn/cufont/kmfont/light/ramafo/small]", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                            LineY++;
                        }
                        break;
                    #endregion
                    #region Networking
                    case "@http":
                    case "@https":
                        if (Split.Length > 1)
                        {
                            try
                            {
                                HttpRequest Request = new();

                                switch (Split[1].ToLower())
                                {
                                    default:
                                        Graphics.Canvas.DrawString("@http [post/get/?] [ip] [domain] [path]", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                                        LineY++;
                                        break;
                                    case "post":
                                    case "p":
                                        Request.Method = "POST";
                                        break;
                                    case "get":
                                    case "g":
                                        Request.Method = "GET";
                                        break;
                                    case "?":
                                    case "info":
                                        if (NetworkConfiguration.Count < 1)
                                        {
                                            Graphics.Canvas.DrawString("no network configurations available", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                                            LineY++;
                                        }
                                        else
                                        {
                                            Graphics.Canvas.DrawString($"NETWORK INFO:", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                                            LineY++;
                                            Graphics.Canvas.DrawString($"ip: {NetworkConfiguration.CurrentAddress.ToString()}", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                                            LineY++;
                                        }
                                        return;
                                }

                                Request.IP = Split[3].ToLower().Replace("default", NetworkConfiguration.CurrentAddress.ToString());
                                Request.Domain = Split[4];
                                Request.Path = Split[5];

                                Graphics.Canvas.DrawString($"sent request ({Split[3].ToLower().Replace("default", NetworkConfiguration.CurrentAddress.ToString())}) to {Split[4]}{Split[5]}", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                                LineY++;

                                Request.Send();

                                Graphics.Canvas.DrawString($"got response: {Request.Response.Content}", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                                LineY++;
                            }
                            catch
                            {
                                Graphics.Canvas.DrawString("something went wrong", Kernel.DefaultFont, Color.Red, LineX, Dip + (LineY * 20));
                                LineY++;
                            }
                        }
                        else
                        {
                            Graphics.Canvas.DrawString("@http [post/get/?] [ip] [domain] [path]", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                            LineY++;
                        }
                        break;
                        #endregion
                }
            }
            else
            {
                Graphics.Canvas.DrawString($"{Command}", Kernel.DefaultFont, Color.Green, LineX, Dip + (LineY * 20));
                LineY++;
            }
        }
    }
}
