using System;
using System.Collections.Generic;
using MonopolyGame;
using System.ComponentModel;

namespace MonopolyGame.source           //need player, gameboard, history
{
    public class Monopoly
    {          //Model use singleton
        private static Monopoly instance;
        public int NumPlayers;
        public int currentplayer;
        public int totalturns;
        public int samerollcnt;
        public Player[] players;
        public int[] dice;
        //gameboard

        Random rnd = new Random();
        List<string> positions = new List<string>();

        public bool currentplayercanbuy()
        {
            if (list[players[currentplayer].position] is land && players[currentplayer].cash >= (list[players[currentplayer].position] as land).price)
                return (list[players[currentplayer].position] as land).owner == null;
            return false;
        }

        public bool canbuild()
        {
            int sum = 0;
            foreach (var land in players[currentplayer].property)
            {
                if ((list[players[currentplayer].position] as land).color == land.color)
                    sum++;
            }
            if ((list[players[currentplayer].position] as land).color == "darkblue" || (list[players[currentplayer].position] as land).color == "brown")
                return sum == 2;
            else
                return sum == 3;
        }
        public void buyland()
        {
            players[currentplayer].cash -= (list[players[currentplayer].position] as land).price;
            (list[players[currentplayer].position] as land).name = players[currentplayer].name;

            players[currentplayer].property.Add(list[players[currentplayer].position] as land);
        }

        public void buildhouse()
        {
            (list[players[currentplayer].position] as land).totalhouse++;
            (list[players[currentplayer].position] as land).rent += 10 * (list[players[currentplayer].position] as land).totalhouse;

        }

        public bool needpay()
        {
            return (list[players[currentplayer].position] as land).owner != null && (list[players[currentplayer].position] as land).owner != players[currentplayer];
        }

        public void payrent()
        {

            players[currentplayer].cash -= (list[players[currentplayer].position] as land).rent;
            (list[players[currentplayer].position] as land).owner.cash += (list[players[currentplayer].position] as land).rent;
        }
        public bool ischance()
        {


        }












        private Monopoly()          //private constructor
        {

        }
        public static Monopoly Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Monopoly();        //private constructor
                }
                return instance;
            }
        }

        List<object> list = new List<object>();
        land brown1 = new land(15, 5, "brown", "VINE STREET");
        land brown2 = new land(57, 10, "brown", "COVENTRY STREET");
        land lightblue1 = new land(68, 10, "lightblue", "LEICESTER STREET");
        land lightblue2 = new land(71, 10, "lightblue", "BOW STREET");
        land lightblue3 = new land(81, 10, "lightblue", "VWHITECHAPEL ROAD");
        land pink1 = new land(91, 10, "pink", "ANGEL ISLINGTON");
        land pink2 = new land(97, 10, "pink", "TRAFALGAR SQUARE");
        land pink3 = new land(112, 10, "pink", "NORTHUMRLD AVENUE");
        land orange1 = new land(125, 10, "orange", "M'BOROUGH STREET");
        land orange2 = new land(148, 10, "orange", "FLEET STREET");
        land orange3 = new land(208, 15, "orange", "OLD KENT ROAD");
        land red1 = new land(211, 15, "red", "WHITEHALL ROAD");
        land red2 = new land(215, 15, "red", "PENTONVILLE");
        land red3 = new land(228, 15, "red", "PALL MALL");
        land yellow1 = new land(271, 15, "yellow", "BOND STREET");
        land yellow2 = new land(320, 15, "yellow", "STRAND");
        land yellow3 = new land(370, 15, "yellow", "REGENT STREET");
        land green1 = new land(404, 20, "green", "EUSTON ROAD");
        land green2 = new land(440, 20, "green", "PICCADILLY");
        land green3 = new land(550, 20, "green", "OXFORD STREET");
        land darkblue1 = new land(562, 20, "darkblue", "PARK LANE");
        land darkblue2 = new land(1800, 30, "darkblue", "MAYFAIR");

        public void createlist()
        {
            list.Add(brown1); list.Add(brown2); list.Add(lightblue1); list.Add(lightblue2); list.Add(lightblue3); list.Add(pink1); list.Add(pink2);
            list.Add(pink3); list.Add(orange1); list.Add(orange2); list.Add(orange3); list.Add(red1); list.Add(red2); list.Add(red3); list.Add(yellow1);
            list.Add(yellow1); list.Add(yellow2); list.Add(yellow3); list.Add(green1); list.Add(green2); list.Add(green3); list.Add(darkblue1); list.Add(darkblue2);
        }

        public void Init(int num)
        {
            createlist();
            NumPlayers = num;
            currentplayer = num - 1;
            totalturns = 200;

            players = new Player[num];
            for (int i = 0; i < num; i++)
                players[i] = new Player("player" + (i + 1));
        }


        public void Next()
        {
            dice = Roll();
            currentplayer = (currentplayer + 1) % NumPlayers;
            players[currentplayer].position += dice[0] + dice[1];
            //board[position].Action(players[whosturn])      
            totalturns++;
        }

        public int[] Roll()
        {
            int[] result = new int[2];
            result[0] = rnd.Next(1, 7);
            result[1] = rnd.Next(1, 7);
            return result;
        }

        public class land
        {
            public Player owner = null;
            public double price;
            public double rent;
            public int totalhouse = 0;
            public string color;
            public string name;
            //public bool istaken = false;
            public land(double price, double rent, string color, string name)
            {
                this.price = price;
                this.rent = rent;
                this.color = color;
                this.name = name;
            }
        }

        public class jail
        { 
        
        
        
        
        
        
        }
        
        
        
        
        
        
        
        
        public class chance
        { 
            
        
        
        
        
        
        }


        public class Player
        {
            public string name;
            public double cash;
            public int position;
            public List<land> property = new List<land>();
            // public List<property> itemsown;
            public Player(string name)
            {
                this.name = name;
                cash = 200;
                position = 0;
            }
        }

        public class gamehistory
        {

        }


        public class board { }

    }
}