using System;
using System.Collections.Generic;
using MonopolyGame;
using System.ComponentModel;

namespace MonopolyGame.source           //need player, gameboard, history
{
    public class Monopoly
    {
        private static Monopoly instance;
        public int NumPlayers;
        public int currentplayer;
        public int totalturns;
 //       public int samerollcnt;
        public List<Player> players;
        public int[] dice;
        public int numofbankrupt;
        //gameboard

        Random rnd = new Random();
      //  List<string> positions = new List<string>();

        public bool currentplayercanbuy()
        {
            if (list[players[currentplayer].position] is land && players[currentplayer].cash >= (list[players[currentplayer].position] as land).price)
                return (list[players[currentplayer].position] as land).owner == null;
            return false;
        }

        public bool canbuild()
        {
            if ((players[currentplayer].cash) <= 50 || (list[players[currentplayer].position] as land).totalhouse >=4) { return false;  } else {
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
        }
        public void buyland()
        {
            players[currentplayer].cash -= (list[players[currentplayer].position] as land).price;
            (list[players[currentplayer].position] as land).owner = players[currentplayer];

            players[currentplayer].property.Add(list[players[currentplayer].position] as land);
        }

        public void buildhouse()
        {
            (list[players[currentplayer].position] as land).totalhouse++;
            (list[players[currentplayer].position] as land).rent += 10;
            players[currentplayer].cash -= 50;
            

        }

        public bool needpay()
        {
            return (list[players[currentplayer].position] as land).owner != null && (list[players[currentplayer].position] as land).owner != players[currentplayer];
        }

        public string[] payrent()
        {
            players[currentplayer].cash -= (list[players[currentplayer].position] as land).rent;
            (list[players[currentplayer].position] as land).owner.cash += (list[players[currentplayer].position] as land).rent;
            return new string[]{ players[currentplayer].name, (list[players[currentplayer].position] as land).owner.name, (list[players[currentplayer].position] as land).rent.ToString()};
        }
        public int takechance()
        {
            int a = rnd.Next(1, 4);
            if (a == 1) {
                players[currentplayer].cash += 50;
                return 1;
            }
            else if (a == 2)
            {
                players[currentplayer].cash -= 30;
                return 0;
            }
            else
            {
                gotojail();
                return -1;
            }
        }

        




        
        public void gotojail()
        {

            players[currentplayer].position = 10;
            players[currentplayer].jailcount += 2;

        }
        public void jailskip()
        {
            currentplayer = (currentplayer + 1) % NumPlayers;
            players[currentplayer].jailcount--;

            totalturns++;

        }

        private Monopoly()          //private constructor
        {
            createlist();
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

        public List<object> list = new List<object>();
        land brown1 = new land(15, 500, "brown", "VINE STREET");
        land brown2 = new land(57, 1000, "brown", "COVENTRY STREET");
        land lightblue1 = new land(68, 1000, "lightblue", "LEICESTER STREET");
        land lightblue2 = new land(71, 1000, "lightblue", "BOW STREET");
        land lightblue3 = new land(81, 1000, "lightblue", "VWHITECHAPEL ROAD");
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
        gotojail gotojail1 = new gotojail();
        chance chance1 = new chance();
        chance chance2 = new chance();
        chance chance3 = new chance();
        jail ja = new jail();
        starting go = new starting();
        freepark freepark1 = new freepark();
        freepark freepark2 = new freepark();
        freepark freepark3 = new freepark();
        freepark freepark4 = new freepark();
        freepark freepark5 = new freepark();
         

        public void createlist()
        {
            list.Add(go); list.Add(brown1); list.Add(freepark1); list.Add(brown2); list.Add(freepark2); list.Add(freepark3); list.Add(lightblue1); list.Add(chance1); list.Add(lightblue2); list.Add(lightblue3);
            list.Add(ja); list.Add(pink1); list.Add(freepark4); list.Add(pink2); list.Add(pink3); list.Add(freepark5); list.Add(orange1); list.Add(freepark5); list.Add(orange2); list.Add(orange3);
            list.Add(freepark5); list.Add(red1); list.Add(chance2); list.Add(red2); list.Add(red3); list.Add(freepark5); list.Add(yellow1);  list.Add(yellow2); list.Add(freepark5); list.Add(yellow3);
            list.Add(gotojail1); list.Add(green1); list.Add(green2); list.Add(freepark5); list.Add(green3); list.Add(freepark5); list.Add(chance3); list.Add(darkblue1); list.Add(freepark5); list.Add(darkblue2);
           
        }

        public void Init(int num)
        {
            
            NumPlayers = num;
            currentplayer = num - 1;
            totalturns = 0;

            players = new List<Player>();
            for (int i = 0; i < num; i++)
            {
                string a = "player" + (i + 1);
                players.Add( new Player(a));
            }
            numofbankrupt = 0;
        }


        public bool Next()
        {
            dice = Roll();
            totalturns++;
            currentplayer = (currentplayer + 1) % NumPlayers;
            players[currentplayer].position +=  dice[0] + dice[1];
            if (players[currentplayer].position > 39) {
                players[currentplayer].position = players[currentplayer].position % 40;
                players[currentplayer].cash += 200;
                return true;
            }
            else return false;
            
        }

        public int[] Roll()
        {
            int[] result = new int[2];
            result[0] = rnd.Next(1, 7);
            result[1] = rnd.Next(1, 7);
            return result;
        }

        public List<Player> Getrank() {
            List<Player>  P = players;
            P.Sort();
            return P;
        }

        public void bankrupt(Player P){
            if (P.online) {
                P.cash = P.wealth;
                foreach (land L in P.property)
                {
                    L.owner = null;
                    L.totalhouse = 0;
                }
                P.property = new List<land>();
                P.online = false;
                P.position = 0;
                P.jailcount = 0;
                numofbankrupt++;
            }

        }

        public void skipnext() {
           currentplayer = (currentplayer + 1) % NumPlayers;
        }


    }


    public class Player:IComparable<Player>
    {
        public string name;
        public double cash;
        public bool online;
        public double wealth {
            get {
                double sum = 0;
                if (property != null)
                {
                    foreach (land L in property)
                    {
                        sum += L.price + L.totalhouse * 50;
                    }
                }
                return cash+sum  ; }
        }
        public int position;
        public int jailcount = 0;
        public List<land> property = new List<land>();
        // public List<property> itemsown;
        public Player(string name)
        {
            this.name = name;
            cash = 200;
            position = 0;
            online = true;
        }
        public int CompareTo(Player P) {
            if (wealth > P.wealth) return 1;
            else if (wealth == P.wealth) return 0;
            else return -1;

        }
    }

    public class land:tile
    {
        public double price;
        public double rent;
        public int totalhouse = 0;
        public string color;
  
        //public bool istaken = false;
        public land(double price, double rent, string color, string name)
        {
            this.price = price;
            this.rent = rent;
            this.color = color;
            base.name = name;
            base.owner = null;
        }
    }

    //public class communitychest : tile {          left for expansion
    //    public communitychest() {
    //        base.name = "community chest";
    //        base.owner = null;
    //    }
    //}

    public class gotojail : tile
    {
        public gotojail() {
        base.name = "goto jail";
            base.owner = null;
        }
    }

    public class jail : tile
    {
        public  jail() {
        base.name = "jail";
            base.owner = null;
        }
    }






    public class chance : tile
    {
        public chance() {
        base.name = "chance";
            base.owner = null;
        }
    }


    public class freepark : tile {
        public freepark(){
         base.name = "freeparking";
            base.owner = null;
        }
    }

    public class starting:tile {
        public starting()
        {
            base.name = "Go point";
            base.owner = null;
        }
    }
    public abstract class tile
    {
        public string name;
        public Player owner;
    }







}