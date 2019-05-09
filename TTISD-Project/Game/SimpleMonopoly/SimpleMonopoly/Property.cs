using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMonopoly
{
    class Property
    {
        private bool mortgageProperty;
        private int mortgage;

        public Property()
        {

        }

        public void SetAsMortgageProperty()
        {
            mortgageProperty = true;
        }

        public void SetAsNormalProperty()
        {
            mortgageProperty = false;
        }

        public int GetMortgage()
        {
            return mortgage;
        }
    }
}
