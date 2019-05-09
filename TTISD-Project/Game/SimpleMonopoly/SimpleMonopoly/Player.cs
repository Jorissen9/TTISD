using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMonopoly
{
    class Player
    {
        private int number;
        private int position;
        private int money;
        private List<Property> properties;

        public Player(int number)
        {
            this.number = number;
            this.position = 0;
            this.money = 1500;
            this.properties = new List<Property>();
        }

        public void MovePlayer(int newPos)
        {
            this.position = newPos;
        }

        public int GetPosition()
        {
            return position;
        }

        public void EarnMoney(int amount)
        {
            this.money += amount;
        }

        public void PayMoney(int amount)
        {
            this.money -= amount;
        }

        public int GetMoney()
        {
            return money;
        }

        public void EarnProperty(Property property)
        {
            this.properties.Add(property);
        }

        public void SellProperty(int index)
        {
            properties[index].SetAsMortgageProperty();
            this.money += properties[index].GetMortgage();
        }

        public void ReBuyProperty(int index)
        {
            this.money -= Convert.ToInt32(properties[index].GetMortgage() * 1.1);
            properties[index].SetAsNormalProperty();
        }

        public List<Property> GetProperties()
        {
            return properties;
        }

        public void GoBroke()
        {

        }
    }
}
