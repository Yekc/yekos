using Cosmos.Core;
using Cosmos.System;
using Cosmos.System.Network.Config;
using Cosmos.System.Network.IPv4;
using Cosmos.System.Network.IPv4.UDP.DNS;
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
        static int MaxLine = (Kernel.ScreenWidth / Kernel.DefaultFont.Width) - 1;
        static bool Fm = false;

        static bool DoOutput = true;

        static List<string> Looped = new();
        static Dictionary<int, string> Macros = new();

        static bool PersistLoops = false;
        static bool PersistMacros = true;

        static List<string> PreviousContent = new();
        static int PreviousContentCycles = 0;

        static Dictionary<string, int> Variables = new();

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
                    Content += "\n";
                    break;
                case ConsoleKeyEx.F2:
                    Content += "¢";
                    break;
                case ConsoleKeyEx.F3:
                    Content += "¡";
                    break;
                case ConsoleKeyEx.F4:
                    Content += "£";
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

                if (Content.Length < MaxLine)
                {
                    Graphics.Canvas.DrawString($"{Content}", Kernel.DefaultFont, Color.Brown, LineX, Dip + (LineY * 20));
                }
                else
                {
                    Graphics.Canvas.DrawString($"{Content.Substring(Content.Length - MaxLine)}", Kernel.DefaultFont, Color.Brown, LineX, Dip + (LineY * 20));
                }
            }
        }

        static void ProcessCommand(string Command)
        {
            Command = Command.Replace("\r", "");
            if (Command.Contains('\n'))
            {
                foreach (string Cmd in Command.Split('\n'))
                {
                    if (Command.StartsWith("#") || Command.Length == 0) continue;
                    ProcessCommand(Cmd);
                }
                return;
            }

            #region Variables

            if (!Command.StartsWith('&'))
            {
                Command = Command.Replace("$¢mx", MouseManager.X.ToString())
                    .Replace("$¢my", MouseManager.Y.ToString())
                    .Replace("$¢b", Home.Score.ToString())
                    .Replace("$¢w", Kernel.ScreenWidth.ToString())
                    .Replace("$¢h", Kernel.ScreenHeight.ToString());
                if (Command.Contains("$£r"))
                {
                    Random r = new Random();
                    int Min = Convert.ToInt32(Command.Split('£')[1].Split(' ')[0].Split(':')[1]);
                    int Max = Convert.ToInt32(Command.Split('£')[1].Split(' ')[0].Split(':')[2]);
                    Command = Command.Replace($"$£{Command.Split('£')[1].Split(' ')[0]}", r.Next(Min, Max).ToString());
                }
                foreach (string VarSpl in Command.Split('¡'))
                {
                    string VarName = VarSpl;
                    if (VarSpl.Contains(' ')) VarName = VarSpl.Split(' ')[0];
                    if (Variables.ContainsKey(VarName))
                    {
                        Command = Command.Replace($"$¡{VarName}", Variables[VarName].ToString());
                    }
                }
            }

            #endregion

            if (Command.StartsWith('@') || Command.StartsWith('&'))
            {
                if (Command.StartsWith('&')) Looped.Add(Command.TrimStart('&'));
                string[] Split = Command.Split(' ');
                switch (Split[0].ToLower().TrimStart('&'))
                {
                    default:
                        PrintLine("??? dafuq", Color.Red);
                        break;
                    #region If Statements
                    case "@?":
                        if (Split.Length > 6)
                        {
                            bool Run = false;
                            switch (Split[2])
                            {
                                default:
                                    PrintLine("=, <, >, !");
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
                            PrintLine("@? $ = $ [command], @? $ < $ [command], @? $ > $ [command], @? $ ! $ [command]");
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
                                        PrintLine("HELP PAGE 1:");
                                        PrintLine("press f1 to chain commands together");
                                        PrintLine("&@[command] - loop");
                                        PrintLine("@? - if statement");
                                        PrintLine("@exit - leave terminal");
                                        PrintLine("@sysinfo - get information about your computer");
                                        break;
                                    case 2:
                                        PrintLine("HELP PAGE 2:");
                                        PrintLine("@followmouse - make the terminal follow the mouse");
                                        PrintLine("@doarc - toggle mouse arcs");
                                        PrintLine("@c [line] - move cursor to line");
                                        PrintLine("@setenv [variable name] [value] - set an environment variable to a value");
                                        PrintLine("@setv [variable name] [value] - set a user defined variable to a value (creates if doesnt exist)");
                                        break;
                                    case 3:
                                        PrintLine("HELP PAGE 3:");
                                        PrintLine("@halt - halt the cpu");
                                        PrintLine("@shutdown - shut down");
                                        PrintLine("@reboot - reboot the cpu");
                                        PrintLine("@clear [color] - clear the screen");
                                        PrintLine("@forceclear - toggles force clear");
                                        break;
                                    case 4:
                                        PrintLine("HELP PAGE 4:");
                                        PrintLine("@draw - draw stuff on screen");
                                        PrintLine("@disp - display the screen buffer");
                                        PrintLine("@font - set the system font");
                                        PrintLine("@net - do networking stuff");
                                        PrintLine("@fetch [url] - download a file from the internet");
                                        break;
                                    case 5:
                                        PrintLine("HELP PAGE 5:");
                                        PrintLine("@rfetch [url] - download and run a file from the internet");
                                        PrintLine("@dout - toggle command output");
                                        PrintLine("@memory - do stuff with memory");
                                        PrintLine("@persist [loops/macros] - toggle macros/loops persisting when terminal is closed");
                                        PrintLine("@beep [hertz] [ms] - beep");
                                        break;
                                    case 6:
                                        PrintLine("HELP PAGE 6:");
                                        PrintLine("@macro [key] command - assign a command to a key");
                                        PrintLine("@time - get the current time");
                                        PrintLine("@hampster - wawaweewa");
                                        PrintLine("@ascend - top secret");
                                        PrintLine("@descend - top secret");
                                        break;
                                    case 7:
                                        PrintLine("HELP PAGE 7:");
                                        PrintLine("$¢mx - mouse x position environment variable (press f2 for ¢)");
                                        PrintLine("$¢my - mouse y position environment variable (press f2 for ¢)");
                                        PrintLine("$¢b - buttons pressed environment variable (press f2 for ¢)");
                                        PrintLine("$¢w - screen width (press f2 for ¢)");
                                        PrintLine("$¢h - screen height (press f2 for ¢)");
                                        break;
                                    case 8:
                                        PrintLine("HELP PAGE 8:");
                                        PrintLine("$¡[variable name] - user defined variable (press f3 for ¡)");
                                        PrintLine("$£r:[min]:[max] - random integer between the min and max (press f4 for £)");
                                        break;
                                }
                            }
                            catch
                            {
                                PrintLine("@help [page]");
                            }
                        }
                        else
                        {
                            PrintLine("@help [page]");
                        }
                        break;
                    #endregion
                    #region Basic Commands
                    case "@sysinfo":
                    case "@info":
                    case "@i":
                    case "@neofetch":
                        PrintLine("here is everything you will ever need to know:");
                        PrintLine($"TOTAL RAM: {CPU.GetAmountOfRAM() + 1} MB, USED: {(CPU.GetEndOfKernel() + 1024) / 1048576} MB, FREE: {CPU.GetAmountOfRAM() + 1 - ((CPU.GetEndOfKernel() + 1024) / 1048576)} MB");
                        PrintLine($"CPU: {CPU.GetCPUBrandString()}");
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
                        if (Split.Length > 1)
                        {
                            switch (Split[1].ToLower())
                            {
                                default:
                                    Mouse.DoArc = false;
                                    break;
                                case "true":
                                case "t":
                                case "yes":
                                case "y":
                                case "do":
                                case "arc":
                                    Mouse.DoArc = true;
                                    break;
                            }
                        }
                        else
                        {
                            Mouse.DoArc = !Mouse.DoArc;
                        }
                        break;
                    case "@c":
                    case "@cursor":
                        if (Split.Length > 1)
                        {
                            LineY = Convert.ToInt32(Split[1]);
                        }
                        else
                        {
                            PrintLine("@c [line]");
                        }
                        break;
                    case "@setenv":
                    case "@env":
                    case "@se":
                        if (Split.Length > 2)
                        {
                            try
                            {
                                switch (Split[1].ToLower())
                                {
                                    default:
                                        PrintLine("??? its just mx or my or b", Color.Red);
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
                                PrintLine("??? you did something wrong i think", Color.Red);
                            }
                        }
                        else
                        {
                            PrintLine("@setenv [variable name] [value]");
                        }
                        break;
                    case "@setv":
                    case "@setvariable":
                    case "@variable":
                    case "@setvar":
                    case "@var":
                    case "@v":
                        if (Split.Length > 2)
                        {
                            try
                            {
                                if (Variables.ContainsKey(Split[1]))
                                {
                                    Variables[Split[1]] = Convert.ToInt32(Split[2]);
                                    PrintLine($"updated variable {Split[1]} with value {Split[2]}");
                                }
                                else
                                {
                                    Variables.Add(Split[1], Convert.ToInt32(Split[2]));
                                    PrintLine($"variable {Split[1]} didnt exist so it was created with value {Split[2]}");
                                }
                            }
                            catch
                            {
                                if (Command.StartsWith('&')) Looped.RemoveAt(Looped.IndexOf(Command));
                                PrintLine("@setv [variable name] [value]");
                            }
                        }
                        else
                        {
                            PrintLine("@setv [variable name] [value]");
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
                        CPU.Reboot();
                        break;
                    case "@dooutput":
                    case "@doout":
                    case "@dout":
                    case "@do":
                        if (Split.Length > 1)
                        {
                            switch (Split[1].ToLower())
                            {
                                default:
                                    DoOutput = true;
                                    break;
                                case "f":
                                case "false":
                                case "no":
                                case "dont":
                                case "n":
                                    DoOutput = false;
                                    break;
                            }
                        }
                        else
                        {
                            DoOutput = !DoOutput;
                        }
                        break;
                    case "@memory":
                    case "@mem":
                        PrintLine($"not implemented yet {RandomFace()}");
                        break;
                    case "@persist":
                    case "@ps":
                    case "@p":
                        if (Split.Length > 1)
                        {
                            switch (Split[1].ToLower())
                            {
                                default:
                                    PrintLine("@persist [loops/macros]");
                                    break;
                                case "macros":
                                case "macro":
                                case "m":
                                    PersistMacros = !PersistMacros;
                                    if (PersistMacros)
                                    {
                                        PrintLine("macros will now persist");
                                    }
                                    else
                                    {
                                        PrintLine("macros will no longer persist");
                                    }
                                    break;
                                case "loops":
                                case "loop":
                                case "l":
                                    PersistLoops = !PersistLoops;
                                    if (PersistLoops)
                                    {
                                        PrintLine("loops will now persist");
                                    }
                                    else
                                    {
                                        PrintLine("loops will no longer persist");
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            PrintLine("@persist [loops/macros]");
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
                                if (Command.StartsWith('&')) Looped.RemoveAt(Looped.IndexOf(Command));
                                PrintLine("@beep [hertz] [ms]");
                            }
                        }
                        else
                        {
                            PrintLine("@beep [hertz] [ms]");
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
                            PrintLine($"added new macro for key {Split[1]}: {Cmd}");
                        }
                        else
                        {
                            PrintLine("@macro [key] command");
                        }
                        break;
                    case "@time":
                    case "@t":
                    case "@now":
                    case "@date":
                        PrintLine($"it is {Cosmos.HAL.RTC.Month}/{Cosmos.HAL.RTC.DayOfTheMonth}/{Cosmos.HAL.RTC.Year} and the time is {Cosmos.HAL.RTC.Hour}:{Cosmos.HAL.RTC.Minute}:{Cosmos.HAL.RTC.Second}");
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
                                PrintLine("too late", Color.DarkKhaki);
                            }
                            else
                            {
                                PrintLine("click some more buttons", Color.DarkKhaki);
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
                                PrintLine("??? that is not a COLOLR!!", Color.Red);
                                PrintLine("roygbiv (but not blue or indigo or violet i must save your eye from blue light)", Color.Red);
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
                            PrintLine("OK ITS OFF!!!!!!");
                        }
                        break;
                    case "@disp":
                    case "@display":
                    case "@dp":
                        Graphics.Canvas.Display();
                        break;
                    case "@draw":
                    case "@d":
                        if (Split.Length > 1)
                        {
                            switch (Split[1].ToLower())
                            {
                                default:
                                    PrintLine("@draw [shape]");
                                    break;
                                case "point":
                                case "p":
                                    if (Split.Length > 3)
                                    {
                                        try
                                        {
                                            Graphics.Canvas.DrawPoint(Color.FromName($"{Split[4][0].ToString().ToUpper()}{Split[4].ToString().Substring(1).ToLower()}"), Convert.ToInt32(Split[2]), Convert.ToInt32(Split[3]));
                                        }
                                        catch
                                        {
                                            if (Command.StartsWith('&')) Looped.RemoveAt(Looped.IndexOf(Command));
                                            PrintLine("@draw point [x] [y] [color]");
                                        }
                                    }
                                    else
                                    {
                                        PrintLine("@draw point [x] [y] [color]");
                                    }
                                    break;
                                case "text":
                                case "string":
                                case "s":
                                case "t":
                                    if (Split.Length > 4)
                                    {
                                        try
                                        {
                                            string Text = "";
                                            for (int i = 5; i < Split.Length; i++)
                                            {
                                                Text += Split[i] + " ";
                                            }
                                            Graphics.Canvas.DrawString($"{Text}", Kernel.DefaultFont, Color.FromName($"{Split[4][0].ToString().ToUpper()}{Split[4].ToString().Substring(1).ToLower()}"), Convert.ToInt32(Split[2]), Convert.ToInt32(Split[3]));
                                        }
                                        catch
                                        {
                                            if (Command.StartsWith('&')) Looped.RemoveAt(Looped.IndexOf(Command));
                                            PrintLine("@draw string [x] [y] [color] text");
                                        }
                                    }
                                    else
                                    {
                                        PrintLine("@draw string [x] [y] [color] text");
                                    }
                                    break;
                                case "square":
                                case "rectangle":
                                case "r":
                                    if (Split.Length > 6)
                                    {
                                        try
                                        {
                                            switch (Split[7])
                                            {
                                                default:
                                                case "false":
                                                case "no":
                                                case "n":
                                                    Graphics.Canvas.DrawRectangle(Color.FromName($"{Split[6][0].ToString().ToUpper()}{Split[6].ToString().Substring(1).ToLower()}"), Convert.ToInt32(Split[2]), Convert.ToInt32(Split[3]), Convert.ToInt32(Split[4]), Convert.ToInt32(Split[5]));
                                                    break;
                                                case "true":
                                                case "yes":
                                                case "fill":
                                                case "y":
                                                    Graphics.Canvas.DrawFilledRectangle(Color.FromName($"{Split[6][0].ToString().ToUpper()}{Split[6].ToString().Substring(1).ToLower()}"), Convert.ToInt32(Split[2]), Convert.ToInt32(Split[3]), Convert.ToInt32(Split[4]), Convert.ToInt32(Split[5]));
                                                    break;
                                            }
                                        }
                                        catch
                                        {
                                            if (Command.StartsWith('&')) Looped.RemoveAt(Looped.IndexOf(Command));
                                            PrintLine("@draw rectangle [x] [y] [width] [height] [color] [fill]");
                                        }
                                    }
                                    else
                                    {
                                        PrintLine("@draw rectangle [x] [y] [width] [height] [color] [fill]");
                                    }
                                    break;
                                case "oval":
                                case "circle":
                                case "ellipse":
                                case "c":
                                    if (Split.Length > 6)
                                    {
                                        try
                                        {
                                            switch (Split[7])
                                            {
                                                default:
                                                case "false":
                                                case "no":
                                                case "n":
                                                    Graphics.Canvas.DrawEllipse(Color.FromName($"{Split[6][0].ToString().ToUpper()}{Split[6].ToString().Substring(1).ToLower()}"), Convert.ToInt32(Split[2]), Convert.ToInt32(Split[3]), Convert.ToInt32(Split[4]), Convert.ToInt32(Split[5]));
                                                    break;
                                                case "true":
                                                case "yes":
                                                case "fill":
                                                case "y":
                                                    Graphics.Canvas.DrawFilledEllipse(Color.FromName($"{Split[6][0].ToString().ToUpper()}{Split[6].ToString().Substring(1).ToLower()}"), Convert.ToInt32(Split[2]), Convert.ToInt32(Split[3]), Convert.ToInt32(Split[4]), Convert.ToInt32(Split[5]));
                                                    break;
                                            }
                                        }
                                        catch
                                        {
                                            if (Command.StartsWith('&')) Looped.RemoveAt(Looped.IndexOf(Command));
                                            PrintLine("@draw ellipse [x] [y] [width] [height] [color] [fill]");
                                        }
                                    }
                                    else
                                    {
                                        PrintLine("@draw ellipse [x] [y] [width] [height] [color] [fill]");
                                    }
                                    break;
                                case "arch":
                                case "arc":
                                case "a":
                                    if (Split.Length > 8)
                                    {
                                        try
                                        {
                                            Graphics.Canvas.DrawArc(Convert.ToInt32(Split[2]), Convert.ToInt32(Split[3]), Convert.ToInt32(Split[4]), Convert.ToInt32(Split[5]), Color.FromName($"{Split[8][0].ToString().ToUpper()}{Split[8].ToString().Substring(1).ToLower()}"), Convert.ToInt32(Split[6]), Convert.ToInt32(Split[7]));
                                        }
                                        catch
                                        {
                                            if (Command.StartsWith('&')) Looped.RemoveAt(Looped.IndexOf(Command));
                                            PrintLine("@draw arc [x] [y] [width] [height] [start angle] [end angle] [color]");
                                        }
                                    }
                                    else
                                    {
                                        PrintLine("@draw arc [x] [y] [width] [height] [start angle] [end angle] [color]");
                                    }
                                    break;
                                case "image":
                                case "picture":
                                case "i":
                                    PrintLine($"not implemented yet {RandomFace()}");
                                    break;
                            }
                        }
                        else
                        {
                            PrintLine("@draw [shape]");
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
                                    PrintLine("@font [default/cosmos/thin/sun/aisarn/consl/kmfont/light/ramafo/small]");
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
                            PrintLine("@font [default/cosmos/thin/sun/aisarn/consl/kmfont/light/ramafo/small]");
                        }
                        break;
                    #endregion
                    #region Networking
                    case "@net":
                    case "@n":
                        if (Split.Length > 1)
                        {
                            switch (Split[1].ToLower())
                            {
                                default:
                                    PrintLine("@net [?/http/udp/tcp/ftp/icmp/dns]");
                                    break;
                                case "?":
                                case "info":
                                case "i":
                                case "help":
                                    try
                                    {
                                        if (NetworkConfiguration.Count > 0)
                                        {
                                            PrintLine($"configs: {NetworkConfiguration.Count}");
                                            PrintLine($"ip: {NetworkConfiguration.CurrentAddress.ToString()}");
                                        }
                                        else
                                        {
                                            PrintLine("no network configurations");
                                        }
                                    }
                                    catch
                                    {
                                        PrintLine("network error", Color.Red);
                                    }
                                    break;
                                case "https":
                                case "http":
                                case "h":
                                    PrintLine($"not implemented yet {RandomFace()}");
                                    break;
                                case "udp":
                                    PrintLine($"not implemented yet {RandomFace()}");
                                    break;
                                case "tcp":
                                    PrintLine($"not implemented yet {RandomFace()}");
                                    break;
                                case "ftp":
                                    PrintLine($"not implemented yet {RandomFace()}");
                                    break;
                                case "icmp":

                                    break;
                                case "dns":
                                    PrintLine($"not implemented yet {RandomFace()}");
                                    break;
                            }
                        }
                        else
                        {
                            PrintLine("@net [?/http/udp/tcp/ftp/icmp/dns]");
                        }
                        break;
                    case "@fetch":
                    case "@download":
                    case "@get":
                    case "@f":
                        if (Split.Length > 1)
                        {
                            try
                            {
                                string Path, DomainName;

                                if (Split[1].Replace("http://", "").Contains('/'))
                                {
                                    Path = Split[1].Replace("http://", "").Substring(Split[1].Replace("http://", "").IndexOf('/'));
                                    DomainName = Split[1].Replace("http://", "").Split('/')[0];
                                }
                                else
                                {
                                    Path = "/";
                                    DomainName = Split[1].Replace("http://", "");
                                }

                                PrintLine("getting ready to fetch...");

                                var DNSClient = new DnsClient();
                                DNSClient.Connect(DNSConfig.DNSNameservers[0]);
                                PrintLine("connected to name server");
                                DNSClient.SendAsk(DomainName);
                                PrintLine("sent ask to domain");
                                Address Addr = DNSClient.Receive();
                                PrintLine("got ip");
                                DNSClient.Close();

                                if (Addr == null)
                                {
                                    PrintLine("oh no the address is null", Color.Red);
                                    PrintLine("backup plan: maybe you typed in the ip itself");
                                    try
                                    {
                                        Addr = new Address(Convert.ToByte(DomainName.Split('.')[0]), Convert.ToByte(DomainName.Split('.')[1]), Convert.ToByte(DomainName.Split('.')[2]), Convert.ToByte(DomainName.Split('.')[3]));
                                        PrintLine($"address: {Addr.ToString()}");
                                    }
                                    catch
                                    {
                                        PrintLine("nope its still bad", Color.Red);
                                        PrintLine("i give up", Color.Red);
                                        return;
                                    }
                                }
                                else
                                {
                                    PrintLine($"address: {Addr.ToString()}");
                                }

                                PrintLine("now time to send the get request...");

                                HttpRequest request = new();
                                request.IP = Addr.ToString();
                                request.Domain = DomainName;
                                request.Path = Path;
                                request.Method = "GET";
                                request.Send();

                                PrintLine($"success hooray {RandomFace()}");
                                PrintLine($"CHARSET:{request.Response.Charset}");
                                PrintLine($"CONTENT LENGTH:{request.Response.ContentLength}");
                                PrintLine($"CONTENT:{request.Response.Content}");
                            }
                            catch (Exception ex)
                            {
                                PrintLine($"something went wrong when fetching {Split[1]}", Color.Red);
                                PrintLine($"{ex.HResult}:{ex.Message}", Color.Red);
                            }
                        }
                        else
                        {
                            PrintLine("@fetch [url]");
                        }
                        break;
                    case "@frun":
                    case "@runf":
                    case "@run":
                    case "@fetchrun":
                    case "@runfetch":
                    case "@fetchr":
                    case "@rfetch":
                    case "@fr":
                    case "@rf":
                    case "@r":
                        if (Split.Length > 1)
                        {
                            try
                            {
                                string Path, DomainName;

                                if (Split[1].Replace("http://", "").Contains('/'))
                                {
                                    Path = Split[1].Replace("http://", "").Substring(Split[1].Replace("http://", "").IndexOf('/'));
                                    DomainName = Split[1].Replace("http://", "").Split('/')[0];
                                }
                                else
                                {
                                    Path = "/";
                                    DomainName = Split[1].Replace("http://", "");
                                }

                                var DNSClient = new DnsClient();
                                DNSClient.Connect(DNSConfig.DNSNameservers[0]);
                                DNSClient.SendAsk(DomainName);
                                Address Addr = DNSClient.Receive();
                                DNSClient.Close();

                                if (Addr == null)
                                {
                                    PrintLine("address null, backup plan", Color.Red);
                                    try
                                    {
                                        Addr = new Address(Convert.ToByte(DomainName.Split('.')[0]), Convert.ToByte(DomainName.Split('.')[1]), Convert.ToByte(DomainName.Split('.')[2]), Convert.ToByte(DomainName.Split('.')[3]));
                                    }
                                    catch
                                    {
                                        PrintLine("you did it wrong lol", Color.Red);
                                        return;
                                    }
                                }

                                HttpRequest request = new();
                                request.IP = Addr.ToString();
                                request.Domain = DomainName;
                                request.Path = Path;
                                request.Method = "GET";
                                request.Send();

                                PrintLine($"success hooray {RandomFace()}");
                                PrintLine("attempting to run fetched commands");
                                try
                                {
                                    ProcessCommand(request.Response.Content);
                                }
                                catch
                                {
                                    PrintLine("screwed up real bad trying to run those tf did you do", Color.Red);
                                }
                            }
                            catch (Exception ex)
                            {
                                PrintLine($"something went wrong when fetching {Split[1]}", Color.Red);
                                PrintLine($"{ex.HResult}:{ex.Message}", Color.Red);
                            }
                        }
                        else
                        {
                            PrintLine("@rfetch [url]");
                        }
                        break;
                        #endregion
                }
            }
            else
            {
                PrintLine(Command);
            }
        }

        static void PrintLine(string Message, Color? Col = null)
        {
            if (!DoOutput) return;
            if (Col == null) Col = Color.Green;
            for (int i = 0; i < Message.Length; i += MaxLine)
            {
                Graphics.Canvas.DrawString($"{Message.Substring(i, Math.Min(MaxLine, Message.Length - i))}", Kernel.DefaultFont, (Color)Col, LineX, Dip + (LineY * 20));
                LineY++;
                Graphics.Canvas.Display();
            }
            //Graphics.Canvas.DrawString($"{Message}", Kernel.DefaultFont, (Color)Col, LineX, Dip + (LineY * 20));
            //LineY++;
        }

        static string RandomFace()
        {
            Random r = new Random();
            int Face = r.Next(10);
            switch (Face)
            {
                default:
                    return ":)";
                case 1:
                    return ":O";
                case 2:
                    return ";)";
                case 3:
                    return "B)";
                case 4:
                    return "8)";
                case 5:
                    return ":*)";
                case 6:
                    return ">:(";
                case 7:
                    return ">:)";
                case 8:
                    return ":]";
                case 9:
                    return ":#";
            }
        }
    }
}
