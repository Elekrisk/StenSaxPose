using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StenSaxPose
{
    public class LocalGamePlay
    {
        int playerNum;
        Player[] players;
        int scoreLimit;
        public int localGameID;
        int[] score;
        int turn;

        public LocalGamePlay(int n, Player[] pls, int sl, int id)
        {
            playerNum = n;
            players = pls;
            scoreLimit = sl;
            localGameID = id;
            score = new int[playerNum];
            turn = 0;
        }
    }
}
