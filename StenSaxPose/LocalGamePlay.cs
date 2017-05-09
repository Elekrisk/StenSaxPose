using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StenSaxPose
{
    public class LocalGamePlay
    {
        public int playerNum;
        public Player[] players;
        public int scoreLimit;
        public int localGameID;
        public int[] score;
        public int turn;
        public string Name = "          ";

        public LocalGamePlay(int n, Player[] pls, int sl, int id, string name)
        {
            playerNum = n;
            players = pls;
            scoreLimit = sl;
            localGameID = id;
            score = new int[playerNum];
            turn = 0;
            Name = name;
        }
    }
}
