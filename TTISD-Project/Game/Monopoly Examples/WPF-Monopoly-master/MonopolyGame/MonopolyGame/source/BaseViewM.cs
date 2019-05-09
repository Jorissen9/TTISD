using System;
using System.Collections.Generic;
using System.ComponentModel;
using MonopolyGame.source;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Collections.Specialized;

namespace MonopolyGame.source
{
    public class ViewMono : INotifyPropertyChanged {
        Monopoly Game = Monopoly.Instance;
        public int diceroll;
        Thickness[] playersmargin;
    
        private ViewMono()
        {
            QuestionEna = Visibility.Hidden;
            StartEna = Visibility.Visible;
            RollEna = Visibility.Hidden;
            ResetEna = Visibility.Hidden;
            ConfirmEna = Visibility.Hidden;
            EndEna = Visibility.Hidden;
            createmap();
            createmap2();
            Player4margin = new Thickness(-56, -100, 0, 0);
            Player3margin = new Thickness(-56, -100, 0, 0);
            Player2margin = new Thickness(-56, -100, 0, 0);
            Player1margin = new Thickness(-56, -100, 0, 0);
            playersmargin = new Thickness[] { Player1margin, Player2margin, Player3margin, Player4margin };
            Player24margin = new Thickness(-56, -100, 0, 0);
            Player23margin = new Thickness(-56, -100, 0, 0);
            Player22margin = new Thickness(-56, -100, 0, 0);
            Player21margin = new Thickness(-56, -100, 0, 0);
    //        playersmargin = new Thickness[] { Player1margin, Player2margin, Player3margin, Player4margin };
            for (int i = 0; i < 4; i++) {
                Pcash.Add(0);
                Pwealth.Add(0);
                Pprop.Add("");
                PEna.Add(Visibility.Visible);
            }
        }



        public BindingList<Act> history = new BindingList<Act>();
        
        public void addhistory(int Turn, string pl, string action) {
            if (pl.Length > 6) pl = pl.Remove(0, 6);
        
            history.Insert(0,new Act(Turn, pl, action));
            RaisePropertyChangedEvent("history");
        }
        public class Act {
            public int turn { get; set; }
            public string player { get; set; }
            public string action { get; set; }
            public Act(int turn, string player, string action) {
                this.turn = turn;
                this.player = player;
                this.action = action;
            }
        }



        private static ViewMono ins;
        public static ViewMono Ins {
            get
            {
                if (ins == null)
                {
                    ins = new ViewMono();        //private constructor
                }
                return ins;
            }
        }

        private BindingList<double> pcash = new BindingList<double>();
        public BindingList<double> Pcash { get { return pcash; } set { pcash = value; RaisePropertyChangedEvent(); } }
        private BindingList<double> pwealth = new BindingList<double> ();
        public BindingList<double> Pwealth { get { return pwealth; } set { pwealth = value; RaisePropertyChangedEvent(); } }
        private BindingList<string> pprop = new BindingList<string> ();
        public BindingList<string>Pprop { get { return pprop; } set { pprop = value; RaisePropertyChangedEvent(); } }

       

        private BindingList<Visibility> pEna = new BindingList<Visibility>();
        public BindingList<Visibility> PEna { get { return pEna; } set { pEna = value;RaisePropertyChangedEvent(); } }





        private Visibility rollEna;
        public Visibility RollEna                                 //Enable rolling button??
        {
            get { return rollEna; }
            set
            {
                rollEna = value;
                RaisePropertyChangedEvent();
            }
        }

        private Visibility resetEna;
        public Visibility ResetEna                                 //Enable reset button??
        {
            get { return resetEna; }
            set
            {
                resetEna = value;
                RaisePropertyChangedEvent();
            }
        }

        private Visibility startEna;
        public Visibility StartEna                                 //Enable reset button??
        {
            get { return startEna; }
            set
            {
                startEna = value;
                RaisePropertyChangedEvent();
            }
        }

        private Visibility questionEna;
        public Visibility QuestionEna                                 //Enable Question??
        {
            get { return questionEna; }
            set
            {
                questionEna = value;
                RaisePropertyChangedEvent();
            }
        }

        
        private string questionMes;                                  //Question in Questionbox                              
        public string QuestionMes {
            get { return questionMes; }
            set {
                questionMes = value;
                RaisePropertyChangedEvent();
            }
        }

        private Visibility confirmEna;
        public Visibility ConfirmEna                                 //Enable Confirm Message???
        {
            get { return confirmEna; }
            set
            {
                confirmEna = value;
                RaisePropertyChangedEvent();
            }
        }


        private string confirmMes;                                  //ConfirmMessage                              
        public string ConfirmMes
        {
            get { return confirmMes; }
            set
            {
                confirmMes = value;
                RaisePropertyChangedEvent();
            }
        }

        private Visibility endEna;
        public Visibility EndEna                                 //Enable Confirm Message???
        {
            get { return endEna; }
            set
            {
                endEna = value;
                RaisePropertyChangedEvent();
            }
        }


        private string endMes;                                  //ConfirmMessage                              
        public string EndMes
        {
            get { return endMes; }
            set
            {
                endMes = value;
                RaisePropertyChangedEvent();
            }
        }



        private string previousMes;                                  //showing what the previous one ppl do                               
        public string PreviousMes
        {
            get { return previousMes; }
            set
            {
                previousMes = value;
                RaisePropertyChangedEvent();
            }
        }

        private string statusMes;                                  //showing what the current  player status before roll                            
        public string StatusMes
        {
            get { return statusMes; }
            set
            {
                statusMes = value;
                RaisePropertyChangedEvent();
            }
        }
        private string actionMes;                                  //showing what the current  player action after rool                             
        public string ActionMes
        {
            get { return actionMes; }
            set
            {
                actionMes = value;
                RaisePropertyChangedEvent();
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChangedEvent([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }



        

        private Thickness player1margin;
        public Thickness Player1margin { 
            get { return player1margin; }
            set { player1margin = value  ;
                RaisePropertyChangedEvent();
            }
        }
        private Thickness player2margin;
        public Thickness Player2margin
        {
            get { return player2margin; }
            set
            {
                player2margin = value;
                RaisePropertyChangedEvent();
            }
        }
        private Thickness player3margin;
        public Thickness Player3margin
        {
            get { return player3margin; }
            set
            {
                player3margin = value;
                RaisePropertyChangedEvent();
            }
        }
        private Thickness player4margin;
        public Thickness Player4margin
        {
            get { return player4margin; }
            set
            {
                player4margin = value;
                RaisePropertyChangedEvent();
            }
        }

        private Thickness player21margin;
        public Thickness Player21margin
        {
            get { return player21margin; }
            set
            {
                player21margin = value;
                RaisePropertyChangedEvent();
            }
        }
        private Thickness player22margin;
        public Thickness Player22margin
        {
            get { return player22margin; }
            set
            {
                player22margin = value;
                RaisePropertyChangedEvent();
            }
        }
        private Thickness player23margin;
        public Thickness Player23margin
        {
            get { return player23margin; }
            set
            {
                player23margin = value;
                RaisePropertyChangedEvent();
            }
        }
        private Thickness player24margin;
        public Thickness Player24margin
        {
            get { return player24margin; }
            set
            {
                player24margin = value;
                RaisePropertyChangedEvent();
            }
        }
        
        public void createmap()
        {

            for (int i = 0; i < 10; i++)
                positions.Add(new Thickness(56, (674 - i * 60), 0, 0));

            for (int i = 10; i < 20; i++)
                positions.Add(new Thickness(56 + 56 * (i - 10), 74, 0, 0));

            for (int i = 20; i < 30; i++)
                positions.Add(new Thickness(616, 74 + (i - 20) * 60, 0, 0));

            for (int i = 30; i < 40; i++)
                positions.Add(new Thickness(616 - 56 * (i - 30), 674, 0, 0));


        }
        public void createmap2()
        {

            for (int i = 0; i < 10; i++)
                positions2.Add(new Thickness(35, 451-40*i, 0, 0));

            for (int i = 10; i < 20; i++)
                positions2.Add(new Thickness(35+40*(i-10), 51, 0, 0));

            for (int i = 20; i < 30; i++)
                positions2.Add(new Thickness(435, 51+(i-20)*40, 0, 0));

            for (int i = 30; i < 40; i++)
                positions2.Add(new Thickness(435 - 40 * (i - 30), 451, 0, 0));


        }
        List<Thickness> positions = new List<Thickness>();
        List<Thickness> positions2 = new List<Thickness>();
      

        public void move()
        {


            if (Game.NumPlayers == 2) { 
                Player1margin = positions[Game.players[0].position];
                Player2margin = positions[Game.players[1].position];
            }
            else if (Game.NumPlayers == 3) {
                Player1margin = positions[Game.players[0].position];
                Player2margin = positions[Game.players[1].position];
                Player3margin = positions[Game.players[2].position];            
            } else {
                Player1margin = positions[Game.players[0].position];
                Player2margin = positions[Game.players[1].position];
                Player3margin = positions[Game.players[2].position]; 
                Player4margin = positions[Game.players[3].position];
            }
 
        }


        public void move2()
        {


            if (Game.NumPlayers == 2)
            {
                Player21margin = positions2[Game.players[0].position];
                Player22margin = positions2[Game.players[1].position];
            }
            else if (Game.NumPlayers == 3)
            {
                Player21margin = positions2[Game.players[0].position];
                Player22margin = positions2[Game.players[1].position];
                Player23margin = positions2[Game.players[2].position];
            }
            else
            {
                Player21margin = positions2[Game.players[0].position];
                Player22margin = positions2[Game.players[1].position];
                Player23margin = positions2[Game.players[2].position];
                Player24margin = positions2[Game.players[3].position];
            }
        }


        public void GameStart(string x)
        {
            history.Clear();
            StartEna = Visibility.Hidden;
            RollEna = Visibility.Visible;
            ResetEna = Visibility.Visible;
            EndEna = Visibility.Hidden;
            int num;
            Int32.TryParse(x, out num);
            for (int i = num - 1; i >= 0; i--) playersmargin[i] = positions[0];
            for (int i = 0; i < 4; i++) PEna[i] = Visibility.Hidden;
            for (int i = 0; i < num; i++) PEna[i] = Visibility.Visible;
            RaisePropertyChangedEvent("PEna");

            Game.Init(num);
            Player a = Game.players[0];
            StatusMes = "Turn " + (Game.totalturns + 1) + " :" + a.name + "\nYou have $" + a.cash;
            updatePstat();
            move();
            move2();
        }

        public void updatePstat() {
            for (int i = 0; i < Game.NumPlayers; i++) {
                Pcash[i] = Game.players[i].cash;
                RaisePropertyChangedEvent("Pcash");
                Pwealth[i] = Game.players[i].wealth;
                RaisePropertyChangedEvent("Pwealth");
                string PropM = "";
                int j = 1;
                foreach (land L in Game.players[i].property) {
                    PropM +=j+") "+ L.name + " " + (L.totalhouse>0?"(" +L.totalhouse + "H)":"")+"\n";
                    j++;
                }
                Pprop[i] = PropM;
                RaisePropertyChangedEvent("Pprop");

            }
        }

        public int curplayposition()
        {
            return Game.players[Game.currentplayer].position;
        }


        public void NextTurn()
        {
          
                RollEna = Visibility.Hidden;
                bool acrossgo = Game.Next();
                ActionMes = "";

                Player a = Game.players[Game.currentplayer];
                if (acrossgo)
                {
                    ActionMes += a.name + "gain 200 salary\n";
                    addhistory(Game.totalturns, a.name, "across go,gain 200");
                }

                ActionMes += a.name + " moves to " + (Game.list[a.position] as tile).name + "\n";
                addhistory(Game.totalturns, a.name, "moves to " + (Game.list[a.position] as tile).name);
            move();
            move2();
            whattodo(a.position);
        }

        public void EndTurn()
        {   if (Game.totalturns >= 150) { EndGame(); }
        else if (Game.numofbankrupt == Game.NumPlayers - 1) { EndGame(); }
            else
            {
                Player now = Game.players[Game.currentplayer];
                if (now.wealth < 0)
                {
                    Game.bankrupt(now);
                    addhistory(Game.totalturns, now.name, "bankrupt now!!");
              

                }
                updatePstat();
       
              
                
                Player next = Game.players[(Game.currentplayer + 1) % Game.NumPlayers];


                while (!next.online || next.jailcount > 0)
                { if (!next.online)
                    {
                        addhistory(Game.totalturns, next.name, "bankrupted,cant move");
                        Game.skipnext();
                        next = Game.players[(Game.currentplayer + 1) % Game.NumPlayers];
                    }
                    else {
                        
                        ActionMes = next.name + " is in jail " + ", wait for " + next.jailcount + " more round\n";
                        addhistory(Game.totalturns, next.name, " in jail," + next.jailcount + " round left");
                        Game.jailskip();
                        next = Game.players[(Game.currentplayer + 1) % Game.NumPlayers];
                    }
                }

                RollEna = Visibility.Visible;
                PreviousMes = ActionMes;
                ActionMes = "";
                StatusMes = "Turn " + (Game.totalturns + 1) + ": " + next.name + "\nYou have $" + next.cash;
            }
        }

        public void EndGame() {
            List<Player> P = Game.Getrank();
            string Mes="";
            for(int i = Game.NumPlayers -1; i>=0 ;i--) Mes += "Rank " +(Game.NumPlayers -i) +": "+P[i].name + " cash:"+P[i].cash +" wealth:"+P[i].wealth + "\n";
            EndEna = Visibility.Visible;
            EndMes = Mes;
            addhistory(0,"","End");
            
        }

        public void Restart() {
            QuestionEna = Visibility.Hidden;
            StartEna = Visibility.Visible;
            RollEna = Visibility.Hidden;
            ResetEna = Visibility.Hidden;
            ConfirmEna = Visibility.Hidden;
            EndEna = Visibility.Hidden;

            Player4margin = new Thickness(-56, -100, 0, 0);
            Player3margin = new Thickness(-56, -100, 0, 0);
            Player2margin = new Thickness(-56, -100, 0, 0);
            Player1margin = new Thickness(-56, -100, 0, 0);
        }

        public void whattodo(int position)
        {
            if (Game.list[position] is land) { onproperty(); }

            else if (Game.list[position] is gotojail)
            {
                Game.gotojail();
                ActionMes += Game.players[Game.currentplayer].name + " need to go to jail\n";
                addhistory(Game.totalturns, Game.players[Game.currentplayer].name, " locked in jail");
                EndTurn();
            }

            else if (Game.list[position] is chance)
            {
                int i = Game.takechance();
                ActionMes += Game.players[Game.currentplayer].name + " take chance card!! ";
                if (i == -1) { ActionMes += "Unlucky, goto jail\n";
                    addhistory(Game.totalturns, Game.players[Game.currentplayer].name, "jail card, locked in J");
                }
                else if (i == 0) { ActionMes += "unlucky, deduct $30\n";
                    addhistory(Game.totalturns, Game.players[Game.currentplayer].name, "loss card, lose 30");
                }
                else { ActionMes += "lucky! gain $50\n";
                    addhistory(Game.totalturns, Game.players[Game.currentplayer].name, "gain card, gain 50");
                }
                EndTurn();
            }

            else
            {
                ActionMes += Game.players[Game.currentplayer].name + " do nothing\n";
                EndTurn();
            }
        }

        public void onproperty(){
            if (Game.currentplayercanbuy())
            {
                QuestionEna = Visibility.Visible;
                RollEna = Visibility.Hidden;
                QuestionMes = "Hi " + Game.players[Game.currentplayer].name + " do you want to buy " + (Game.list[Game.players[Game.currentplayer].position] as land).name;


                doyes = () =>
                {
                    addhistory(Game.totalturns, Game.players[Game.currentplayer].name,  "buy " + (Game.list[Game.players[Game.currentplayer].position] as land).name);
                    ActionMes += Game.players[Game.currentplayer].name + " buy " + (Game.list[Game.players[Game.currentplayer].position] as land).name+"\n";
                    Game.buyland();
                };


                dono = () => { ActionMes += Game.players[Game.currentplayer].name + "dont buy " + (Game.list[Game.players[Game.currentplayer].position] as land).name +"\n"; };
            }

            else if (Game.canbuild())
            {
                QuestionEna = Visibility.Visible;
                RollEna = Visibility.Hidden;
                QuestionMes = "Hi " + Game.players[Game.currentplayer].name + " do you want to build a house on " + (Game.list[Game.players[Game.currentplayer].position] as land).name; ;


                doyes = () =>
                {
                    addhistory(Game.totalturns, Game.players[Game.currentplayer].name, "build a house on " + (Game.list[Game.players[Game.currentplayer].position] as land).name);
                    ActionMes += Game.players[Game.currentplayer].name + " build a house on " + (Game.list[Game.players[Game.currentplayer].position] as land).name +"\n";
                    Game.buildhouse();
                };
                dono = () => { ActionMes += Game.players[Game.currentplayer].name + "dont build houses " + (Game.list[Game.players[Game.currentplayer].position] as land).name +"\n"; };


            }

            else if (Game.needpay())
            {

                string [] mess = Game.payrent();
                ActionMes += mess[0] + " pay "+ mess[1] + " $"+mess[2]+" for rent\n";
                addhistory(Game.totalturns, Game.players[Game.currentplayer].name, "pay " + mess[1] + " $" + mess[2] + " for rent");
                EndTurn();
            }
            else {
                ActionMes += Game.players[Game.currentplayer].name + " can do nothing\n";
                EndTurn();

            }
       }

        public delegate void useransno();
        useransno dono;


        public delegate void useransyes();
        useransyes doyes;
        public void userclickyes(){
            QuestionEna = Visibility.Hidden;
            doyes();
            doyes = null;
            EndTurn();
        }

        public void userclickno()
        {
            QuestionEna = Visibility.Hidden;
            dono();
            dono = null;
            EndTurn();
        } 




    }

  

}