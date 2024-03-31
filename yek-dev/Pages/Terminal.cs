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
        static bool AllowInput = true;

        public static List<string> Looped = new();
        public static Dictionary<int, string> SetLooped = new();
        public static Dictionary<int, string> Macros = new();

        static bool PersistLoops = false;
        static bool PersistMacros = true;

        static List<string> PreviousContent = new();
        static int PreviousContentCycles = 0;

        static Dictionary<string, int> Variables = new();
        static Dictionary<string, string> Functions = new();

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
                    if (Active && AllowInput && char.IsAscii(k.KeyChar)) Content += k.KeyChar;
                    break;
                case ConsoleKeyEx.F1:
                    if (AllowInput) Content += "\n";
                    break;
                case ConsoleKeyEx.Escape:
                    if (HurtEyes.Active || AncientInterface.Active || Infinity.Active) break;
                    Active = !Active;
                    if (Active)
                    {
                        Kernel.ClearColor = Color.Beige;
                    }
                    else
                    {
                        Kernel.ClearColor = Color.Black;
                    }
                    AllowInput = true;
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
                    if (Content.ToLower() == "infinity")
                    {
                        Infinity.Active = true;
                        Active = false;
                        Content = "";
                        LineY = 0;
                        Dip = 65;
                        LineX = 5;
                        Fm = false;
                        if (!PersistLoops) Looped.Clear();
                        if (!PersistMacros) Macros.Clear();
                    }
                    if (Active && AllowInput && Content != "")
                    {
                        LineY++;
                        if (LineY > (Kernel.ScreenHeight / (Kernel.DefaultFont.Height + 4)) - 4)
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
                    if (AllowInput && Content.Length > 0) Content = Content.Remove(Content.Length - 1);
                    break;
                case ConsoleKeyEx.Tab:
                    if (AllowInput) Content += "    ";
                    break;
                case ConsoleKeyEx.UpArrow:
                    if (AllowInput && PreviousContent.Count > PreviousContentCycles)
                    {
                        PreviousContentCycles++;
                        Content = PreviousContent[PreviousContent.Count - PreviousContentCycles];
                    }
                    break;
                case ConsoleKeyEx.DownArrow:
                    if (AllowInput && PreviousContentCycles > 1)
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
                catch (Exception ex)
                {
                    Graphics.Canvas.DrawString($"LOOP ERROR {Looped.Count} {ex.Message}", Kernel.DefaultFont, Color.Red, 0, 0);
                }
            }

            foreach (int Key in SetLooped.Keys)
            {
                int Iterations = Convert.ToInt32(SetLooped[Key].Split('%')[0]);
                if (Iterations <= 0)
                {
                    SetLooped.Remove(Key);
                    continue;
                }
                try
                {
                    ProcessCommand(SetLooped[Key].Replace("$*l", Iterations.ToString()));
                }
                catch (Exception ex)
                {
                    Graphics.Canvas.DrawString($"SET LOOP ERROR {Key} {SetLooped.Count} {ex.Message}", Kernel.DefaultFont, Color.Red, 0, 0);
                }
                SetLooped[Key] = SetLooped[Key].Replace($"{Iterations}%", $"{Iterations - 1}%");
            }

            if (Active && AllowInput)
            {
                if (Fm)
                {
                    Dip = (int)MouseManager.Y;
                    LineX = (int)MouseManager.X;
                }

                if (Content.Length < MaxLine)
                {
                    Graphics.Canvas.DrawString($"{Content}", Kernel.DefaultFont, Color.Brown, LineX, Dip + (LineY * (Kernel.DefaultFont.Height + 4)));
                }
                else
                {
                    Graphics.Canvas.DrawString($"{Content.Substring(Content.Length - MaxLine)}", Kernel.DefaultFont, Color.Brown, LineX, Dip + (LineY * (Kernel.DefaultFont.Height + 4)));
                }
            }
        }

        static string ProcessVariables(string Command)
        {
            Command = Command.Replace("$$mx", MouseManager.X.ToString())
                        .Replace("$$my", MouseManager.Y.ToString())
                        .Replace("$$b", Home.Score.ToString())
                        .Replace("$$w", Kernel.ScreenWidth.ToString())
                        .Replace("$$h", Kernel.ScreenHeight.ToString());
            if (Command.Contains("$?"))
            {
                try
                {
                    if (Command.Contains('(') && Command.Contains(')'))
                    {
                        Command = Command.Replace(Command.Split("$?(")[1].Split(')')[0], ProcessVariables(Command.Split("$?(")[1].Split(')')[0]));
                    }
                    else if (Command.Contains('[') && Command.Contains(']'))
                    {
                        Command = Command.Replace(Command.Split("$?[")[1].Split(']')[0], ProcessVariables(Command.Split("$?(")[1].Split(')')[0]));
                    }
                    else
                    {
                        Command = Command.Replace(Command.Split("$?{")[1].Split('}')[0], ProcessVariables(Command.Split("$?(")[1].Split(')')[0]));
                    }
                }
                catch
                {
                    PrintLine($"failed to process variables ({Command})", Color.Red);
                }
            }
            //Todo for random and math: they break if there is more : in the command that isnt related to them
            if (Command.Contains("$~r"))
            {
                Random r = new();
                int Min = Convert.ToInt32(Command.Split("$~r")[1].Split(' ')[0].Split(':')[1]);
                int Max = Convert.ToInt32(Command.Split("$~r")[1].Split(' ')[0].Split(':')[2]);
                Command = Command.Replace($"$~r{Command.Split("$~r")[1].Split(' ')[0]}", r.Next(Min, Max).ToString());
            }
            if (Command.Contains("$^"))
            {
                try
                {
                    switch (Command.Split(':')[1])
                    {
                        case "+":
                            Command = Command.Replace($"$^{Command.Split("$^")[1].Split(':')[0]}:+:{Command.Split(':')[2].Split(' ')[0]}", (Convert.ToInt32(ProcessVariables(Command.Split("$^")[1].Split(':')[0])) + Convert.ToInt32(ProcessVariables(Command.Split(':')[2].Split(' ')[0]))).ToString());
                            break;
                        case "-":
                            Command = Command.Replace($"$^{Command.Split("$^")[1].Split(':')[0]}:-:{Command.Split(':')[2].Split(' ')[0]}", (Convert.ToInt32(ProcessVariables(Command.Split("$^")[1].Split(':')[0])) - Convert.ToInt32(ProcessVariables(Command.Split(':')[2].Split(' ')[0]))).ToString());
                            break;
                        case "*":
                            Command = Command.Replace($"$^{Command.Split("$^")[1].Split(':')[0]}:*:{Command.Split(':')[2].Split(' ')[0]}", (Convert.ToInt32(ProcessVariables(Command.Split("$^")[1].Split(':')[0])) * Convert.ToInt32(ProcessVariables(Command.Split(':')[2].Split(' ')[0]))).ToString());
                            break;
                        case "/":
                            Command = Command.Replace($"$^{Command.Split("$^")[1].Split(':')[0]}:/:{Command.Split(':')[2].Split(' ')[0]}", (Convert.ToInt32(ProcessVariables(Command.Split("$^")[1].Split(':')[0])) / Convert.ToInt32(ProcessVariables(Command.Split(':')[2].Split(' ')[0]))).ToString());
                            break;
                        case "%":
                            Command = Command.Replace($"$^{Command.Split("$^")[1].Split(':')[0]}:%:{Command.Split(':')[2].Split(' ')[0]}", (Convert.ToInt32(ProcessVariables(Command.Split("$^")[1].Split(':')[0])) % Convert.ToInt32(ProcessVariables(Command.Split(':')[2].Split(' ')[0]))).ToString());
                            break;
                    }
                }
                catch
                {
                    PrintLine($"failed to do math ({Command})", Color.Red);
                }
            }
            foreach (string VarSpl in Command.Split("$%"))
            {
                string VarName = VarSpl;
                if (VarSpl.Contains(' ')) VarName = VarSpl.Split(' ')[0];
                if (Variables.ContainsKey(VarName))
                {
                    Command = Command.Replace($"$%{VarName}", Variables[VarName].ToString());
                }
            }

            return Command;
        }

        static void ProcessCommand(string Command)
        {
            #region Multiple Lines

            Command = Command.Replace("\r", "");
            if (Command.Contains('\n'))
            {
                foreach (string Cmd in Command.Split('\n'))
                {
                    if (!(Cmd.StartsWith("#") || Cmd.Length == 0)) ProcessCommand(Cmd);
                }
                return;
            }

            #endregion

            if (!(Command.StartsWith('*') || Command.StartsWith('&') || Command.StartsWith("@@")) || Looped.Contains(Command) || SetLooped.ContainsValue(Command) || Functions.ContainsValue(Command)) Command = ProcessVariables(Command);

            if (Command.StartsWith('@') || Command.StartsWith('&') || Command.StartsWith('*'))
            {
                if (Command.StartsWith('&'))
                {
                    Looped.Add(Command.TrimStart('&'));
                    return;
                }
                if (Command.StartsWith('*'))
                {
                    string Cmd = "";
                    if (Command.Split(':').Length > 2)
                    {
                        for (int i = 1; i < Command.Split(':').Length; i++)
                        {
                            Cmd += Command.Split(':')[i] + ':';
                        }
                        Cmd = Cmd.TrimEnd(':');
                    }
                    else
                    {
                        Cmd = Command.Split(':')[1];
                    }
                    Random r = new();
                    SetLooped.Add(SetLooped.Count * r.Next(1000), $"{Convert.ToInt32(ProcessVariables(Command.TrimStart('*').Split(':')[0]))}%{Cmd}");
                    return;
                }
                string[] Split = Command.Split(' ');
                switch (Split[0].ToLower().TrimStart('&'))
                {
                    default:
                        PrintLine($"??? dafuq (({Command}))", Color.Red);
                        break;
                    #region If Statements
                    case "@?":
                        if (Split.Length > 4)
                        {
                            try
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

                                PrintLine($"no error with ur if statement ({Command})", Color.Blue);
                            }
                            catch
                            {
                                PrintLine($"error with ur if statement ({Command})", Color.Red);
                                PrintLine("make sure the variables being compared have their correct prefixes like $% for user defined", Color.Red);
                            }
                        }
                        else
                        {
                            PrintLine("@? $ = $ [command], @? $ < $ [command], @? $ > $ [command], @? $ ! $ [command]");
                        }
                        break;
                    #endregion
                    #region Functions
                    case "@@":
                        if (Split.Length > 2)
                        {
                            string Cmd;
                            switch (Split[1].ToLower())
                            {
                                default:
                                    try
                                    {
                                        if (Split.Length > 3)
                                        {
                                            string[] Args = Functions[Split[2]].Split('|');
                                            string Function = Functions[Split[2]];
                                            for (int i = 0; i < Args.Length - 1; i++)
                                            {
                                                Function = Function.Replace(Args[i] + "|", "");
                                                Function = Function.Replace(Args[i], Split[3 + i]);
                                            }
                                            ProcessCommand(Function);
                                        }
                                        else
                                        {
                                            ProcessCommand(Functions[Split[2]]);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        PrintLine($"failed to call function {Split[2]} ({Command})", Color.Red);
                                        PrintLine($"{ex.HResult}:{ex.Message}", Color.Red);
                                    }
                                    break;
                                case "arg":
                                case "argument":
                                    try
                                    {
                                        Functions[Split[2]] = $"{Split[3]}|" + Functions[Split[2]];
                                    }
                                    catch
                                    {
                                        PrintLine($"failed to add argument ({Command})", Color.Red);
                                    }
                                    break;
                                case "add":
                                case "append":
                                    try
                                    {
                                        string Prev = Functions[Split[2]];
                                        Functions.Remove(Split[2]);
                                        Cmd = "";
                                        for (int i = 3; i < Split.Length; i++)
                                        {
                                            Cmd += Split[i] + " ";
                                        }
                                        Functions.Add(Split[2], Prev + "\n" + Cmd);
                                    }
                                    catch
                                    {
                                        PrintLine($"failed to add command to function ({Command})", Color.Red);
                                    }
                                    break;
                                case "create":
                                case "c":
                                case "new":
                                case "n":
                                    try
                                    {
                                        if (Functions.ContainsKey(Split[2])) Functions.Remove(Split[2]);
                                        Cmd = "";
                                        for (int i = 3; i < Split.Length; i++)
                                        {
                                            Cmd += Split[i] + " ";
                                        }
                                        Functions.Add(Split[2], Cmd);
                                    }
                                    catch
                                    {
                                        PrintLine($"failed to create function ({Command})", Color.Red);
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            PrintLine("@@ [new/add/call/arg] [name] command/[arg]");
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
                                        PrintLine("&@command - loop");
                                        PrintLine("*[count]:@command - loop specified amount of times");
                                        PrintLine("$*l - loop control variable (specified loops only)");
                                        PrintLine("$^[variable]:[operator]:[value] - math");
                                        break;
                                    case 2:
                                        PrintLine("HELP PAGE 2:");
                                        PrintLine("@? - if statement");
                                        PrintLine("@@ [new/add/call/arg] [name] command/arg - function commands");
                                        PrintLine("@exit - leave terminal");
                                        PrintLine("@sysinfo - get information about your computer");
                                        PrintLine("@followmouse - make the terminal follow the mouse");
                                        break;
                                    case 3:
                                        PrintLine("HELP PAGE 3:");
                                        PrintLine("@doarc - toggle mouse arcs");
                                        PrintLine("@c [line] - move cursor to line");
                                        PrintLine("@setenv [variable name] [value] - set an environment variable to a value");
                                        PrintLine("@setv [variable name] [value] - set a user defined variable to a value (creates if doesnt exist)");
                                        PrintLine("@halt - halt the cpu");
                                        break;
                                    case 4:
                                        PrintLine("HELP PAGE 4:");
                                        PrintLine("@shutdown - shut down");
                                        PrintLine("@reboot - reboot the cpu");
                                        PrintLine("@clear [color] - clear the screen");
                                        PrintLine("@forceclear - toggles force clear");
                                        PrintLine("@draw - draw stuff on screen");
                                        break;
                                    case 5:
                                        PrintLine("HELP PAGE 5:");
                                        PrintLine("@disp - display the screen buffer");
                                        PrintLine("@font - set the system font");
                                        PrintLine("@net - do networking stuff");
                                        PrintLine("@fetch [url] - download a file from the internet");
                                        PrintLine("@rfetch [url] - download and run a file from the internet");
                                        break;
                                    case 6:
                                        PrintLine("HELP PAGE 6:");
                                        PrintLine("@dout - toggle command output");
                                        PrintLine("@din - toggle command input");
                                        PrintLine("@print [color] text - print output to the terminal (ignores dout)");
                                        PrintLine("@persist [loops/macros] - toggle macros/loops persisting when terminal is closed");
                                        PrintLine("@beep [hertz] [ms] - beep");
                                        break;
                                    case 7:
                                        PrintLine("HELP PAGE 7:");
                                        PrintLine("@macro [key] command - assign a command to a key");
                                        PrintLine("@time - get the current time");
                                        PrintLine("@add [variable] [amount] - add to a variable");
                                        PrintLine("@sub [variable] [amount] - subtract from a variable");
                                        PrintLine("@mul [variable] [amount] - multiply a variable");
                                        break;
                                    case 8:
                                        PrintLine("HELP PAGE 8:");
                                        PrintLine("@div [variable] [amount] - divide a variable");
                                        PrintLine("@hampster - wawaweewa");
                                        PrintLine("@ascend - top secret");
                                        PrintLine("@descend - top secret");
                                        PrintLine("$$mx - mouse x position environment variable");
                                        break;
                                    case 9:
                                        PrintLine("HELP PAGE 9:");
                                        PrintLine("$$my - mouse y position environment variable");
                                        PrintLine("$$b - buttons pressed environment variable");
                                        PrintLine("$$w - screen width");
                                        PrintLine("$$h - screen height");
                                        PrintLine("$%[variable name] - user defined variable");
                                        break;
                                    case 10:
                                        PrintLine("HELP PAGE 10:");
                                        PrintLine("$~r:[min]:[max] - random integer between the min and max");
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
                                        PrintLine($"??? its just mx or my or b ({Command})", Color.Red);
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
                                PrintLine($"??? you did something wrong i think ({Command})", Color.Red);
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
                    case "@doinput":
                    case "@doinp":
                    case "@doin":
                    case "@dinput":
                    case "@dinp":
                    case "@din":
                    case "@di":
                        if (Split.Length > 1)
                        {
                            switch (Split[1].ToLower())
                            {
                                default:
                                    AllowInput = true;
                                    break;
                                case "f":
                                case "false":
                                case "no":
                                case "dont":
                                case "n":
                                    AllowInput = false;
                                    break;
                            }
                        }
                        else
                        {
                            AllowInput = !AllowInput;
                        }
                        break;
                    case "@print":
                        if (Split.Length > 2)
                        {
                            string Text = "";
                            for (int i = 2; i < Split.Length; i++)
                            {
                                Text += Split[i] + " ";
                            }
                            PrintLine(Text, Color.FromName($"{Split[4][0].ToString().ToUpper()}{Split[4].ToString().Substring(1).ToLower()}"), true);
                        }
                        else
                        {
                            PrintLine("@print [color] text");
                        }
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
                                    if (Split.Length > 2)
                                    {
                                        switch (Split[2].ToLower())
                                        {
                                            default:
                                                PersistMacros = false;
                                                PrintLine("macros will no longer persist");
                                                break;
                                            case "true":
                                            case "t":
                                            case "yes":
                                            case "y":
                                            case "do":
                                                PersistMacros = true;
                                                PrintLine("macros will now persist");
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        PersistMacros = !PersistMacros;
                                        if (PersistMacros)
                                        {
                                            PrintLine("macros will now persist");
                                        }
                                        else
                                        {
                                            PrintLine("macros will no longer persist");
                                        }
                                    }
                                    break;
                                case "loops":
                                case "loop":
                                case "l":
                                    if (Split.Length > 2)
                                    {
                                        switch (Split[2].ToLower())
                                        {
                                            default:
                                                PersistLoops = false;
                                                PrintLine("loops will no longer persist");
                                                break;
                                            case "true":
                                            case "t":
                                            case "yes":
                                            case "y":
                                            case "do":
                                                PersistLoops = true;
                                                PrintLine("loops will now persist");
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        PersistLoops = !PersistLoops;
                                        if (PersistLoops)
                                        {
                                            PrintLine("loops will now persist");
                                        }
                                        else
                                        {
                                            PrintLine("loops will no longer persist");
                                        }
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
                            if (Macros.ContainsKey(Convert.ToInt32(Split[1])))
                            {
                                Macros[Convert.ToInt32(Split[1])] = Cmd;
                                PrintLine("there was already a macro for that key! it has been replaced");
                            }
                            else
                            {
                                Macros.Add(Convert.ToInt32(Split[1]), Cmd);
                                PrintLine($"added new macro for key {Split[1]}: {Cmd}");
                            }
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
                    #region Math
                    case "@add":
                    case "@+":
                        if (Split.Length > 2)
                        {
                            try
                            {
                                Variables[Split[1]] += Convert.ToInt32(Split[2]);
                                PrintLine($"new value of {Split[1]}: {Variables[Split[1]]}");
                            }
                            catch
                            {
                                PrintLine("@add [variable] [amount]");
                            }
                        }
                        else
                        {
                            PrintLine("@add [variable] [amount]");
                        }
                        break;
                    case "@sub":
                    case "@subtract":
                    case "@-":
                        if (Split.Length > 2)
                        {
                            try
                            {
                                Variables[Split[1]] -= Convert.ToInt32(Split[2]);
                                PrintLine($"new value of {Split[1]}: {Variables[Split[1]]}");
                            }
                            catch
                            {
                                PrintLine("@sub [variable] [amount]");
                            }
                        }
                        else
                        {
                            PrintLine("@sub [variable] [amount]");
                        }
                        break;
                    case "@mul":
                    case "@multiply":
                    case "@*":
                        if (Split.Length > 2)
                        {
                            try
                            {
                                Variables[Split[1]] *= Convert.ToInt32(Split[2]);
                                PrintLine($"new value of {Split[1]}: {Variables[Split[1]]}");
                            }
                            catch
                            {
                                PrintLine("@mul [variable] [amount]");
                            }
                        }
                        else
                        {
                            PrintLine("@mul [variable] [amount]");
                        }
                        break;
                    case "@div":
                    case "@divide":
                    case "@/":
                        if (Split.Length > 2)
                        {
                            try
                            {
                                Variables[Split[1]] /= Convert.ToInt32(Split[2]);
                                PrintLine($"new value of {Split[1]}: {Variables[Split[1]]}");
                            }
                            catch
                            {
                                PrintLine("@div [variable] [amount]");
                            }
                        }
                        else
                        {
                            PrintLine("@div [variable] [amount]");
                        }
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
                                PrintLine($"??? that is not a COLOLR!! ({Command})", Color.Red);
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
                                            Graphics.Canvas.DrawString($"{ProcessVariables(Text)}", Kernel.DefaultFont, Color.FromName($"{Split[4][0].ToString().ToUpper()}{Split[4].ToString().Substring(1).ToLower()}"), Convert.ToInt32(Split[2]), Convert.ToInt32(Split[3]));
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
                                    PrintLine("@font [default/cosmos/thin/tiny/sun/modern/medieval/aisarn/consl/kmfont/light/ramafo/small]");
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
                                case "tiny":
                                    Kernel.DefaultFont = ResourceLoader.FontTiny;
                                    break;
                                case "sun":
                                    Kernel.DefaultFont = ResourceLoader.FontSun;
                                    break;
                                case "modern":
                                    Kernel.DefaultFont = ResourceLoader.FontModern;
                                    break;
                                case "medieval":
                                    Kernel.DefaultFont = ResourceLoader.FontMedieval;
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
                            PrintLine("@font [default/cosmos/thin/tiny/sun/modern/medieval/aisarn/consl/kmfont/light/ramafo/small]");
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
                                        PrintLine($"network error ({Command})", Color.Red);
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
                                    PrintLine($"oh no the address is null ({Command})", Color.Red);
                                    PrintLine("backup plan: maybe you typed in the ip itself");
                                    try
                                    {
                                        Addr = new Address(Convert.ToByte(DomainName.Split('.')[0]), Convert.ToByte(DomainName.Split('.')[1]), Convert.ToByte(DomainName.Split('.')[2]), Convert.ToByte(DomainName.Split('.')[3]));
                                        PrintLine($"address: {Addr.ToString()}");
                                    }
                                    catch
                                    {
                                        PrintLine($"nope its still bad ({Command})", Color.Red);
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
                                PrintLine($"something went wrong when fetching {Split[1]} ({Command})", Color.Red);
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
                                    PrintLine($"address null, backup plan ({Command})", Color.Red);
                                    try
                                    {
                                        Addr = new Address(Convert.ToByte(DomainName.Split('.')[0]), Convert.ToByte(DomainName.Split('.')[1]), Convert.ToByte(DomainName.Split('.')[2]), Convert.ToByte(DomainName.Split('.')[3]));
                                    }
                                    catch
                                    {
                                        PrintLine($"you did it wrong lol ({Command})", Color.Red);
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
                                    PrintLine($"screwed up real bad trying to run those rfetch stuffs tf did you do ({Command})", Color.Red);
                                }
                            }
                            catch (Exception ex)
                            {
                                PrintLine($"something went wrong when fetching {Split[1]} ({Command})", Color.Red);
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
                PrintLine(ProcessVariables(Command));
            }
        }

        static void PrintLine(string Message, Color? Col = null, bool Force = false)
        {
            if (!DoOutput && !Force) return;
            if (Col == null) Col = Color.Green;
            for (int i = 0; i < Message.Length; i += MaxLine)
            {
                Graphics.Canvas.DrawString($"{Message.Substring(i, Math.Min(MaxLine, Message.Length - i))}", Kernel.DefaultFont, (Color)Col, LineX, Dip + (LineY * (Kernel.DefaultFont.Height + 4)));
                LineY++;
                if (LineY > (Kernel.ScreenHeight / (Kernel.DefaultFont.Height + 4)) - 4)
                {
                    LineY = 0;
                    if (Dip == 65) Dip = 5;
                }
                Graphics.Canvas.Display();
            }
        }

        static string RandomFace()
        {
            Random r = new();
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
