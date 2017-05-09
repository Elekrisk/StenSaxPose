using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace StenSaxPose
{
    public class Player
    {
        public int ID;
        public string Name;
        public int Score;

        public Player(string name, int id)
        {
            Name = name;
            ID = id;
        }
    }

    enum Menus { MainMenu, LocalChoose, LocalLoad, LocalSave, LocalSetup, LocalPlayerNum, LocalPlayerScore, OnlineSetup1, InLocalGame, InOnlineGame, Music }

    class Program
    {
        static string path;
        static int localGameID;
        static Menus CurrentMenu = Menus.MainMenu;

        static string nextAdd = "";
        static int localPlayerNum = 2;
        static int localPlayerScoreLimit = 3;
        static string cLocalCharID = "00000";

        static LocalGamePlay activeLocalGame;

        static Music backgroundMusic;
        
        

        static void Main(string[] args)
        {
            backgroundMusic = new Music("test.wav");
            backgroundMusic.Play();

            path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\";

            WriteWelcomeMessage();
            while (true)
            {
                Console.Clear();
                switch (CurrentMenu)
                {
                    case Menus.MainMenu:
                        Console.WriteLine("--- Main Menu ---");
                        Console.WriteLine("1. Play Local Game");
                        Console.WriteLine("2. Play Online Game");
                        Console.WriteLine("3. Music Controller");
                        Console.WriteLine("4. Quit");
                        Console.Write(">");
                        DoCommand(Console.ReadLine());
                        break;
                    case Menus.LocalSetup:
                        Console.WriteLine("--- New Local Game ---");
                        Console.WriteLine("1. Number of players: " + localPlayerNum);
                        Console.WriteLine("2. Score limit: " + localPlayerScoreLimit);
                        Console.WriteLine("3. ASCII Name (max 10 char): " + cLocalCharID);
                        Console.WriteLine("4. Start Game");
                        Console.WriteLine("5. Back to Main Menu");
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
                    case Menus.LocalChoose:
                        Console.WriteLine("--- Local Game ---");
                        Console.WriteLine("1. Create new local game");
                        Console.WriteLine("2. Load a local game");
                        Console.WriteLine("3. Back");
                        Console.Write(">");
                        DoCommand(Console.ReadLine());
                        break;
                    case Menus.InLocalGame:
                        LocalGameFunc();
                        break;
                    case Menus.Music:
                        Console.WriteLine("1. Start/Stop");
                        Console.WriteLine("2. Back");
                        DoCommand(Console.ReadLine());
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

        static void CreateLocalGame()
        {
            

            for (int i = 0; i < localPlayerNum; i++)
            {
                
            }

            //activeLocalGame = new LocalGamePlay(localPlayerNum, );
            
            if (!File.Exists(path + "savefiles.sspl"))
            {
                File.Create(path + "savefiles.sspl");
            }

            string savePath = path + "savefiles.sspl";

            string saveData = "";

            saveData += (char)211;


            FileStream f = File.Open(savePath, FileMode.Open);

            int c = 0;
            while (true)
            {
                int b = f.ReadByte();
                if (b == 211)
                {
                    c++;
                }
                else if (b == -1)
                {
                    break;
                }
            }

            saveData += c + 1;

            saveData += cLocalCharID;

            saveData += (char)localPlayerNum;

            for (int i = 0; i < localPlayerNum; i++)
            {
                saveData += (char)0;
            }
            Random r = new Random();
            saveData += (char)r.Next(0, localPlayerNum);
            byte[] stream = Encoding.ASCII.GetBytes(saveData);
            f.Write(stream, 0, 14+localPlayerNum);
        }

        static void LocalGameFunc()
        {
            
        }

        static void WriteWelcomeMessage()
        {
            Console.Write("  ______                      ______                ______                 \n" +
                          " / _____) _                  / _____)              (_____ \\                \n" +
                          "( (____ _| |_ _____ ____    ( (____  _____ _   _    _____) )__   ___ _____ \n" +
                          " \\____ (_   _) ___ |  _ \\    \\____ \\(____ ( \\ / )  |  ____/ _ \\ /___) ___ |\n" +
                          " _____) )| |_| ____| | | |   _____) ) ___ |) X (   | |   | |_| |___ | ____|\n" +
                          "(______/  \\__)_____)_| |_|  (______/\\_____(_/ \\_)  |_|    \\___/(___/|_____)\n" +
                          "===========================================================================\n");

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
                    
                    case Menus.LocalChoose:
                    case Menus.OnlineSetup1:
                        CurrentMenu = Menus.MainMenu;
                        break;
                    case Menus.LocalLoad:
                    case Menus.LocalSetup:
                        CurrentMenu = Menus.LocalChoose;
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
                            CurrentMenu = Menus.LocalChoose;
                            break;
                        case "2":
                            CurrentMenu = Menus.OnlineSetup1;
                            break;
                        case "3":
                            CurrentMenu = Menus.Music;
                            break;
                        case "4":
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
                        case "3":
                            CreateLocalGame();
                            CurrentMenu = Menus.InLocalGame;
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
                case Menus.LocalChoose:
                    switch (s)
                    {
                        case "1":
                            CurrentMenu = Menus.LocalSetup;
                            break;
                        case "2":
                            CurrentMenu = Menus.LocalLoad;
                            break;
                        case "3":
                            CurrentMenu = Menus.MainMenu;
                            break;
                        default:
                            break;
                    }
                    break;
                case Menus.Music:
                    switch (s)
                    {
                        case "1":
                            if (backgroundMusic.Playing())
                                backgroundMusic.Stop();
                            else
                                backgroundMusic.Play();

                            break;
                        case "2":
                            CurrentMenu = Menus.MainMenu;
                            break;
                        default:
                            break;
                    }
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
