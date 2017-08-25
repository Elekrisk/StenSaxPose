using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace StenSaxPoseV2_Client
{
    static class Vars
    {
        static public string path = Path.Combine(Environment.CurrentDirectory, "saves.ssps");
    }

    class FileHandler
    { 

        public void Write(LocalGameSave[] games)
        {
            int num = 0;
            foreach (LocalGameSave g in games)
            {
                num += g.playerNum + 1;
            }

            string[] lines = new string[num];
            int c = 0;
            foreach (LocalGameSave g in games)
            {
                lines[c] = g.id + "|" + g.name + "|" + g.pointCap + "|" + g.playerNum;
                c++;
                foreach (LocalPlayer p in g.players)
                {
                    lines[c] = " " + p.id + "|" + p.name + "|" + p.points + "|" + (int)p.move;
                    c++;
                }
            }

            /*if (File.Exists(Vars.path))
            {
                File.Delete(Vars.path);
            }*/

            File.WriteAllLines(Vars.path, lines);
        }

        public LocalGameSave[] Read(string path)
        {
            if (!File.Exists(path))
            {
                File.Create(path);
                return new LocalGameSave[0];
            }

            string[] lines = File.ReadAllLines(path);

            List<LocalGameSave> games = new List<LocalGameSave>();
            LocalGameSave tempGame = LocalGameSave.Null;

            foreach (string line in lines)
            {
                if (line[0] != ' ')
                {
                    if (tempGame != LocalGameSave.Null)
                    {
                        games.Add(tempGame);
                    }
                    string[] newGame = line.Trim(' ').Split('|');
                    tempGame = new LocalGameSave(newGame[1], int.Parse(newGame[3]), int.Parse(newGame[2]), int.Parse(newGame[0]));
                }
                else
                {
                    if (tempGame != LocalGameSave.Null)
                    {
                        string[] newPlayer = line.Trim(' ').Split('|');
                        LocalPlayer tempPlayer = new LocalPlayer(int.Parse(newPlayer[0]), newPlayer[1])
                        {
                            points = int.Parse(newPlayer[2]),
                            move = (Moves)int.Parse(newPlayer[3])
                        };
                        tempGame.AddLocalPlayer(tempPlayer);
                    }
                }
            }
            if (tempGame != LocalGameSave.Null)
                games.Add(tempGame);

            return games.ToArray();
        }

        public int GetHighestId()
        {
            LocalGameSave[] lgs = Read(Vars.path);

            int c = -1;
            for (int i = 0; i < lgs.Length; i++)
            {
                if (lgs[i].id > c)
                {
                    c = lgs[i].id;
                }
            }
            return c;
        }
    }

    public class LocalGameSave
    {
        public string name;
        public int id;
        public int playerNum;
        public int addedPlayerNum;
        public LocalPlayer[] players;
        public int pointCap;

        static public LocalGameSave Null = new LocalGameSave("null", -1, -1, -1);

        public LocalGameSave(string name, int playerNum, int pointCap, int id = -2)
        {
            if (playerNum > 0)
            {
                this.name = name;
                this.playerNum = playerNum;
                players = new LocalPlayer[playerNum];
                this.pointCap = pointCap;
                if (id == -2)
                {
                    FileHandler f = new FileHandler();
                    int hid = f.GetHighestId();
                    this.id = hid + 1;
                }
                else
                {
                    this.id = id;
                }
            }
        }

        public void AddLocalPlayer(LocalPlayer player, int id = -1)
        {
            if (id >= 0)
                players[id] = player;
            else
                players[addedPlayerNum] = player;
            addedPlayerNum++;
        }

        public void PlayerMove(LocalPlayer lp, Moves ms)
        {
            lp.move = ms;
        }

        public void PlayerPoint(LocalPlayer lp)
        {
            lp.AddPoint();
        }

        public bool Save()
        {
            FileHandler f = new FileHandler();
            LocalGameSave[] saves = f.Read(Vars.path);

            bool c = false;

            for (int i = 0; i < saves.Length; i++)
            {
                if (saves[i].id == id)
                {
                    saves[i] = this;
                    c = true;
                    break;
                }
            }
            if (!c)
            {
                Array.Resize(ref saves, saves.Length + 1);
                saves[saves.Length - 1] = this;
            }

            f.Write(saves);

            return true;
        }

        public bool Delete()
        {
            FileHandler f = new FileHandler();
            LocalGameSave[] saves = f.Read(Vars.path);

            for (int i = 0; i < saves.Length; i++)
            {
                if (saves[i].id == id)
                {
                    List<LocalGameSave> foo = new List<LocalGameSave>(saves);
                    foo.RemoveAt(i);
                    saves = foo.ToArray();
                }
            }

            f.Write(saves);

            return true;
        }
    }

    public enum Moves { Rock, Paper, Scissors, Null = -1, Idle = -2}

    public class LocalPlayer
    {
        public int id;
        public string name;
        public int points;
        public Moves move;

        static public LocalPlayer Null = new LocalPlayer(-1, "null");

        public LocalPlayer(int ID, string Name)
        {
            id = ID;
            name = Name;
            points = 0;
            move = Moves.Null;
        }

        public void AddPoint()
        {
            points++;
        }
    }

    class GameInfo
    {

    }
}
