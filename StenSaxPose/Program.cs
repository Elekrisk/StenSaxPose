using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StenSaxPose
{
    enum Menus { MainMenu, LocalSetup, LocalPlayerNum, LocalPlayerScore, OnlineSetup1, InGame }

    class Program
    {
        static Menus CurrentMenu = Menus.MainMenu;

        static string nextAdd = "";
        static int localPlayerNum = 2;
        static int localPlayerScoreLimit = 3;

        static void Main(string[] args)
        {
            WriteWelcomeMessage();
            while (true)
            {
                Console.Clear();
                switch (CurrentMenu)
                {
                    case Menus.MainMenu:
                        Console.WriteLine("--- Main Menu ---");
                        Console.WriteLine("");
                        Console.WriteLine("1. Play Local Game");
                        Console.WriteLine("2. Play Online Game");
                        Console.WriteLine("3. Quit");
                        Console.Write(">");
                        DoCommand(Console.ReadLine());
                        break;
                    case Menus.LocalSetup:
                        Console.WriteLine("--- Local Game ---");
                        Console.WriteLine("1. Number of players: " + localPlayerNum);
                        Console.WriteLine("2. Score limit: " + localPlayerScoreLimit);
                        Console.WriteLine("3. Start Game");
                        Console.WriteLine("4. Back to Main Menu");
                        Console.Write(">");
                        DoCommand(Console.ReadLine());
                        break;
                    case Menus.LocalPlayerNum:
                        Console.Write("Number of players: ");
                        DoCommand(Console.ReadLine());
                        CurrentMenu = Menus.LocalSetup;
                        break;
                    case Menus.LocalPlayerScore:
                        Console.Write("Score Limit: ");
                        DoCommand(Console.ReadLine());
                        CurrentMenu = Menus.LocalSetup;
                        break;
                    default:
                        break;
                }
                if (nextAdd != "")
                {
                    Console.WriteLine(nextAdd);
                    nextAdd = "";
                    Console.WriteLine("Press any button to continue");
                    Console.ReadKey();
                }
            }
        }

        static void WriteWelcomeMessage()
        {
            Console.WriteLine("Welcome to Sten Sax Påse Online!");
            Console.WriteLine("If you need help with anything, just write '!h' for contextual help!");
            Console.WriteLine("Press any button to continue.");
            Console.ReadKey();
        }

        static void DoCommand(string s)
        {
            if (s == "!h")
            {
                WriteHelp();
            }
            else if (s == "!b" || s == "back")
            {
                switch (CurrentMenu)
                {
                    case Menus.InGame:
                    case Menus.LocalSetup:
                    case Menus.OnlineSetup1:
                        CurrentMenu = Menus.MainMenu;
                        break;
                    default:
                        break;
                }
            }

            switch (CurrentMenu)
            {
                case Menus.MainMenu:

                    switch (s)
                    {
                        case "1":
                            CurrentMenu = Menus.LocalSetup;
                            break;
                        case "2":
                            CurrentMenu = Menus.OnlineSetup1;
                            break;
                        case "3":
                            Environment.Exit(0);
                            break;
                        default:
                            break;
                    }

                    break;
                case Menus.LocalSetup:

                    switch (s)
                    {
                        case "1":
                            CurrentMenu = Menus.LocalPlayerNum;
                            break;
                        case "2":
                            CurrentMenu = Menus.LocalPlayerScore;
                            break;
                        case "4":
                            CurrentMenu = Menus.MainMenu;
                            break;
                        default:
                            break;
                    }

                    break;
                case Menus.LocalPlayerNum:
                    int.TryParse(s, out localPlayerNum);
                    break;
                case Menus.LocalPlayerScore:
                    int.TryParse(s, out localPlayerScoreLimit);
                    break;
                default:
                    break;
            }
        }

        static void WriteHelp()
        {
            switch (CurrentMenu)
            {
                case Menus.MainMenu:
                    nextAdd = "\nThis is the main menu. Type one of the numbers shown on screen to choose that option.";
                    break;
                default:
                    break;
            }
        }
    }
}
