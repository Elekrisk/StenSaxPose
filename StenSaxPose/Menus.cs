using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace StenSaxPose
{
    public static class Constants
    {
        public static int Header = 127;
    }

    /// <summary>
    /// The class for a locak player, game specific
    /// </summary>
    public class LocalPlayer
    {
        /// <summary>
        /// The ID of the local player
        /// </summary>
        public int ID;
        /// <summary>
        /// The score of the player
        /// </summary>
        public int Score;

        public Moves LastMove;

        /// <summary>
        /// Creates a new local player with the specified ID
        /// </summary>
        /// <param name="id">The ID of the local player</param>
        public LocalPlayer(int id)
        {
            ID = id;
        }
    }

    public enum Moves { Null, Rock, Paper, Scissors }

    /// <summary>
    /// Menu enum
    /// </summary>
    enum Menus { MainMenu, LocalChoose, LocalLoad, LocalSave, LocalGameName, LocalSetup, LocalPlayerNum, LocalPlayerScore, OnlineSetup1, InLocalGame, InOnlineGame, Music }

    /// <summary>
    /// Main Program, contains the bulk of the game
    /// </summary>
    class Program
    {
        /// <summary>
        /// The path to the .exe at runtime
        /// </summary>
        static string path;
        /// <summary>
        /// The current menu, or the menu to be shown next redraw
        /// </summary>
        static Menus CurrentMenu = Menus.MainMenu;

        /// <summary>
        /// Extra information to add on the next redraw
        /// </summary>
        static string nextAdd = "";
        /// <summary>
        /// Number of players in created local games
        /// </summary>
        static int localPlayerNum = 2;
        /// <summary>
        /// Score limit of created local games
        /// </summary>
        static int localPlayerScoreLimit = 3;
        /// <summary>
        /// NAme of created local games
        /// </summary>
        static string cLocalCharID = "";

        /// <summary>
        /// The currently playing local game, if any
        /// </summary>
        static LocalGamePlay activeLocalGame;

        /// <summary>
        /// The background music. 10/10
        /// </summary>
        static Music backgroundMusic;
        
        
        /// <summary>
        /// The ASCII seperator
        /// </summary>
        static string SEPERATOR;

        static int MENUSTART;

        /// <summary>
        /// Entry point of the program
        /// </summary>
        /// <param name="args">I have no idea what this is</param>
        static void Main(string[] args)
        {
            Console.SetWindowSize((Console.LargestWindowWidth / 2), (Console.LargestWindowHeight / 2));

            // Assigning variables
            backgroundMusic = new Music("test.wav");
            backgroundMusic.Play();

            path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\";

            // Writing the welcome message with ASCII Art by /SaftLasso
            WriteWelcomeMessage();

            // Handles the rewrite and the menus, except for the in-game one and the load menu
            while (true)
            {
                Console.Clear();
                switch (CurrentMenu)
                {
                    case Menus.MainMenu:
                        WriteHeader();
                        WriteCentre("--- Main Menu ---");
                        //Console.WriteLine("--- Main Menu ---");
                        WriteMenu("1. Play Local Game");
                        WriteMenu("2. Play Online Game");
                        WriteMenu("3. Music Controller");
                        WriteMenu("4. Quit");
                        Console.Write(">");
                        DoCommand(Console.ReadLine());
                        break;
                    case Menus.LocalSetup:
                        WriteHeader();
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
                        WriteHeader();
                        Console.Write("Number of players: ");
                        DoCommand(Console.ReadLine());
                        CurrentMenu = Menus.LocalSetup;
                        break;
                    case Menus.LocalPlayerScore:
                        WriteHeader();
                        Console.Write("Score Limit: ");
                        DoCommand(Console.ReadLine());
                        CurrentMenu = Menus.LocalSetup;
                        break;
                    case Menus.LocalChoose:
                        WriteHeader();
                        Console.WriteLine("--- Local Game ---");
                        Console.WriteLine("1. Create new local game");
                        Console.WriteLine("2. Load a local game");
                        Console.WriteLine("3. Back");
                        Console.Write(">");
                        DoCommand(Console.ReadLine());
                        break;
                    case Menus.InLocalGame:
                        WriteHeader();
                        LocalGameFunc();
                        break;
                    case Menus.Music:
                        WriteHeader();
                        Console.WriteLine("--- Music Controller ---");
                        Console.WriteLine("1. Start/Stop");
                        Console.WriteLine("2. Back");
                        DoCommand(Console.ReadLine());
                        break;
                    case Menus.LocalGameName:
                        WriteHeader();
                        Console.Write("ASCII Name (10 char max): ");
                        string s = Console.ReadLine();
                        if (CheckAscii(s, out s))
                        {
                            cLocalCharID = s;
                        }
                        CurrentMenu = Menus.LocalSetup;
                        break;
                    case Menus.LocalLoad:
                        WriteHeader();
                        ListLocalGames();
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

        /// <summary>
        /// Reads all games from file and displays them for loading.
        /// </summary>
        static void ListLocalGames()
        {
            // Checking if there is a save file and, if there is, opening it
            string savePath = path + "savefiles.sspl";
            if (!File.Exists(savePath))
            {
                CurrentMenu = Menus.LocalChoose;
                nextAdd += "No save games found. Create a new local game.";
                return;
            }

            FileStream f = File.Open(savePath, FileMode.Open);

            // Creating a list to hold all of the saved games
            List<LocalGamePlay> games = new List<LocalGamePlay>();

            // Reading one game each loop, adding to the above list
            // Byte order:
            //
            // Header (211)         1 byte
            // Game ID              1 byte
            // Name in ASCII        10 bytes
            // Number of players    1 bytebyte
            // Score limit          1 byte
            // Player score         1 byte per player
            // Player turn          1 byte
            // Player last move     1 byte per player
            // 
            // Total: 15 + number of players * 2
            while (true)
            {
                bool l = false;
                while (true)
                {
                    int z = f.ReadByte();
                    if (z == -1)
                    {
                        l = true;
                        break;
                    }
                    else if (z == Constants.Header)
                    {
                        break;
                    }
                }
                if (l)
                {
                    break;
                }
                
                int id = f.ReadByte();
                string name = "";
                for (int i = 0; i < 10; i++)
                {
                    name += (char)f.ReadByte();
                }
                int playerNum = f.ReadByte();
                int scoreLimit = f.ReadByte();
                LocalPlayer[] players = new LocalPlayer[playerNum];
                for (int i = 0; i < playerNum; i++)
                {
                    players[i] = new LocalPlayer(f.ReadByte());
                }
                int playerTurn = f.ReadByte();
                Moves[] lastMoves = new Moves[playerNum];
                for (int i = 0; i < playerNum; i++)
                {
                    lastMoves[i] = (Moves)f.ReadByte();
                }

                games.Add(new LocalGamePlay(playerNum, players, scoreLimit, id, name));
            }
            f.Close();

            // Initializing variables for use in displaying the above list
            int listSize = 10;
            int page = 0;

            // Displaying the list
            bool t = true;
            while (t)
            {
                Console.Clear();
                Console.WriteLine("--- Load Local Game ---");
                for (int i = page * listSize; i < page * listSize + listSize; i++)
                {
                    if (i < games.Count)
                    {
                        Console.WriteLine(games[i].localGameID + ". " + games[i].Name + " - " + games[i].playerNum + " players - score limit " + games[i].scoreLimit);
                    }
                }
                Console.WriteLine();
                Console.WriteLine("a. Save game options");
                Console.WriteLine("b. Back");
                Console.Write(">");
                string s = Console.ReadLine();

                switch (s)
                {
                    case "a":
                        Console.Clear();
                        Console.WriteLine("1. Clear all saves");
                        Console.WriteLine("2. Change page size");
                        Console.Write(">");
                        string s2 = Console.ReadLine();
                        switch (s2)
                        {
                            case "1":
                                File.Delete(savePath);
                                CurrentMenu = Menus.LocalChoose;
                                t = false;
                                break;
                            case "2":
                                Console.Clear();
                                Console.Write("Page size: ");
                                string s3 = Console.ReadLine();
                                int h2;
                                if (int.TryParse(s3, out h2))
                                {
                                    listSize = h2;
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    case "b":
                        CurrentMenu = Menus.LocalChoose;
                        t = false;
                        break;
                    default:
                        int h;
                        if (int.TryParse(s, out h))
                        {
                            if (h >= page * listSize && h < page * listSize + listSize)
                            {
                                foreach (LocalGamePlay g in games)
                                {
                                    if (h == g.localGameID)
                                    {
                                        activeLocalGame = g;
                                        CurrentMenu = Menus.InLocalGame;
                                        t = false;
                                    }
                                }
                            }
                        }
                        break;
                }
            }
            
        }

        /// <summary>
        /// Function to check if a string is ASCII and also making sure it is exactly 10 characters long
        /// </summary>
        /// <param name="s">The input string</param>
        /// <param name="so">The output string, 10 characters long</param>
        /// <returns></returns>
        static bool CheckAscii(string s, out string so)
        {
            // Limiting the string to 10 characters or less
            string ss = "";
            for (int i = 0; i < (s.Length <= 10 ? s.Length : 10); i++)
            {
                ss += s[i];
            }

            // Filling out the string to 10 characters
            if (ss.Length < 10)
            {
                for (int i = s.Length; i < 10; i++)
                {
                    ss += " ";
                }
            }

            // Outputting the string
            so = ss;

            // Turning the string into a char array
            char[] c = new char[10];
            for (int i = 0; i < 10; i++)
                c[i] = ss[i];

            // Checkin ASCII Value
            foreach (char ch in c)
            {
                if ((int)ch > 127)
                {
                    nextAdd += (int)ch;
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Creating a local game
        /// </summary>
        static void CreateLocalGame()
        {
            // Setting the path to the save file
            string savePath = path + "savefiles.sspl";
            
            if (!File.Exists(savePath))
            {
                File.Create(savePath).Close();
            }
            
            // Encoding data to a string
            // Byte order can be found in the load function
            string saveData = "";

            saveData += (char)Constants.Header;


            FileStream f = File.Open(savePath, FileMode.Open);

            // Counting the headers to calculate the next game ID
            int c = 0;
            while (true)
            {
                int b = f.ReadByte();
                if (b == Constants.Header)
                {
                    c++;
                }
                else if (b == -1)
                {
                    break;
                }
            }

            saveData += (char)c;

            saveData += cLocalCharID;

            saveData += (char)localPlayerNum;

            saveData += (char)localPlayerScoreLimit;

            for (int i = 0; i < localPlayerNum; i++)
            {
                saveData += (char)0;
            }
            Random r = new Random();
            saveData += (char)r.Next(0, localPlayerNum);

            for (int i = 0; i < localPlayerNum; i++)
            {
                saveData += (char)Moves.Null;
            }
            byte[] stream = Encoding.ASCII.GetBytes(saveData);
            f.Write(stream, 0, stream.Length);
            f.Close();

            // Creating local players and setting the active local game to this one
            LocalPlayer[] tpls = new LocalPlayer[localPlayerNum];

            for (int i = 0; i < localPlayerNum; i++)
            {
                tpls[i] = new LocalPlayer(i);
            }

            activeLocalGame = new LocalGamePlay(localPlayerNum, tpls, localPlayerScoreLimit, c, cLocalCharID);
        }

        static void SaveGame(LocalGamePlay g)
        {
            string savePath = path + "savefiles.sspl";

            if (!File.Exists(savePath))
            {
                File.Create(savePath).Close();
            }

            string file = File.ReadAllText(savePath);

            FileStream f = File.Open(savePath, FileMode.Open);
            FileStream t = File.Open(savePath + ".tmp", FileMode.Create);

            while (true)
            {
                int x;
                int h = f.ReadByte();
                if (h == Constants.Header)
                {
                    string saveData = "";
                    int id = f.ReadByte();
                    bool v = false;
                    if (id == g.localGameID)
                        v = true;
                    saveData += (char)id;
                    for (int i = 0; i < 10; i++)
                    {
                        saveData += (char)f.ReadByte();
                    }
                    int np = f.ReadByte();
                    saveData += (char)np;
                    saveData += (char)f.ReadByte();
                    for (int i = 0; i < np; i++)
                    {
                        x = f.ReadByte();
                        saveData += v? g.score[i] : x;
                    }
                    x = f.ReadByte();
                    saveData += v ? g.turn : x;
                    for (int i = 0; i < np; i++)
                    {
                        x = f.ReadByte();
                        saveData += v ? (char)g.players[i].LastMove : (char)x;
                    }
                    byte[] bs = Encoding.ASCII.GetBytes(saveData);
                    t.Write(bs, 0, bs.Length);
                }
            }
        }

        // Handling in-game logic (WIP)
        static void LocalGameFunc()
        {
            if (activeLocalGame == null)
            {
                CurrentMenu = Menus.MainMenu;
                nextAdd += "No active game; please load one or create a new one";
                return;
            }

            bool running = true;

            while (running)
            {
                Console.WriteLine("----- LOCAL GAME " + activeLocalGame.Name + " " + activeLocalGame.playerNum + " PLAYERS -----");
                Console.WriteLine("PLAYER " + activeLocalGame.turn + 1 + "'S TURN");
                Console.WriteLine("1. See scores");
                Console.WriteLine("2. Play -ROCK-");
                Console.WriteLine("3. Play -PAPER-");
                Console.WriteLine("4. Play -SCICCORS-");
                Console.WriteLine("5. Save and Quit");
                Console.Write(">");
                string s = Console.ReadLine();
                switch (s)
                {
                    case "1":
                        foreach (LocalPlayer p in activeLocalGame.players)
                        {
                            Console.WriteLine("Player " + p.ID + " - " + activeLocalGame.score[p.ID]);
                        }
                        Console.WriteLine("Press any button to return...");
                        Console.ReadKey();
                        break;
                    case "2":
                        break;
                    case "3":
                        break;
                    case "4":
                        break;
                    case "5":
                        SaveGame(activeLocalGame);
                        activeLocalGame = null;
                        CurrentMenu = Menus.MainMenu;
                        running = false;
                        break;
                    default:
                        break;
                }
            }
        }

        // Writes a welcome message with accompanying ASCII art by /SaftLasso
        static void WriteWelcomeMessage()
        {

            CalculateHeader();

            WriteHeader();

            //Console.Write(SEPERATOR);
            //Console.Write(HEADER);
            //Console.WriteLine(SEPERATOR);



            Console.WriteLine("If you need help with anything, just write '!h' for contextual help!");
            Console.WriteLine("Press any button to continue.");
            Console.ReadKey();
        }

        // Handles menu choices except those in-game and in the load menu
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
                            CurrentMenu = Menus.LocalGameName;
                            break;
                        case "4":
                            CreateLocalGame();
                            CurrentMenu = Menus.InLocalGame;
                            break;
                        case "5":
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

        // Writes help (WIP - low priority)
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

        static void WriteHeader()
        {
            Console.Write(SEPERATOR);
            Console.Write(HEADER);
            Console.WriteLine(SEPERATOR);
        }

        static void CalculateHeader()
        {
            for (int i = 0; i < Console.WindowWidth; i++)
            {
                SEPERATOR += "#";
            }

            int a = (Console.WindowWidth - 75) / 2; // 75 = string b length

            string space = "";

            for (int i = 0; i < a; i++)
            {
                space += " ";
            }

            string b = "  ______                      ______                ______                 \n";
            string c = " / _____) _                  / _____)              (_____ \\               \n";
            string d = "( (____ _| |_ _____ ____    ( (____  _____ _   _    _____) )__   ___ _____ \n";
            string e = " \\____ (_   _) ___ |  _ \\    \\____ \\(____ ( \\ / )  |  ____/ _ \\ /___) ___ |\n";
            string f = " _____) )| |_| ____| | | |   _____) ) ___ |) X (   | |   | |_| |___ | ____|\n";
            string g = "(______/  \\__)_____)_| |_|  (______/\\_____(_/ \\_)  |_|    \\___/(___/|_____)\n";

            HEADER = space + b + space + c + space + d + space + e + space + f + space + g;
        }

        static void WriteCentre(string str)
        {

            MENUSTART = (Console.WindowWidth - str.Length) / 2;
            Console.SetCursorPosition(MENUSTART, Console.CursorTop);
            Console.WriteLine(str);

            //Console.WriteLine(String.Format("{0," + ((Console.WindowWidth / 2) + (str.Length / 2)) + "}"), str);
        }

        static void WriteMenu(string str)
        {
            Console.SetCursorPosition(MENUSTART, Console.CursorTop);
            Console.WriteLine(str);
        }

    }
}
