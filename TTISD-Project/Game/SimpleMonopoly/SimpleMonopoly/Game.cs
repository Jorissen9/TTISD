using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMonopoly
{
    class Game
    {
        private const int maxFields = 39;
        private Player currentPlayer;
        private List<Player> players;

        public Game()
        {
            InitPlayers();
            currentPlayer = players[0];
        }

        private void InitPlayers()
        {
            // add players
            // TODO: change where to get player amount
            int playerAmount = 2;
            players = new List<Player>();
            for (int i = 1; i <= playerAmount; i++)
            {
                Player player = new Player(i);
                players.Add(player);
            }
        }
    }
}
