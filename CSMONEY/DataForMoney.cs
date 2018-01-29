using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSMONEY
{
    class DataForMoney
    {
        string name;
        double price;
        string id;
        string bot;
        DateTime dt;
        public string Name { get => name; set => name = value; }
        public double Price { get => price; set => price = value; }
        public string Id { get => id; set => id = value; }
        public string Bot { get => bot; set => bot = value; }
        public DateTime Dt { get => dt; set => dt = value; }
    }
}
