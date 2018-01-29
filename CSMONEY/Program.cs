using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace CSMONEY
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
        
        public static List<Dat> Data = new List<Dat>();
        public static List<DataForMoney> DataMoney = new List<DataForMoney>();
        public static Work.Dat DataJar= new Work.Dat();
        public static List<Dat> DataLoot = new List<Dat>();
        public static List<Dat> DataCsTrade = new List<Dat>();
        public static List<Dat> DataTSF = new List<Dat>();
        public static List<Dat> DataDeals = new List<Dat>();
        public static Queue<string> Mess = new Queue<string>();
        public static List<DataViewBadPrice> BadPrice = new List<DataViewBadPrice>();
        public static Queue<string> MessTelegram = new Queue<string>();
        public static Queue<string> MessLoot = new Queue<string>();
        public static Queue<string> MessCsTrade = new Queue<string>();
        public static Queue<string> MessTSF = new Queue<string>();
        public static Queue<string> MessDeals = new Queue<string>();
        public static Queue<string> ProxyList = new Queue<string>();
        public static List<string> ProxyListFix = new List<string>();
        public static bool BrowesrQuery = false;
        public static bool pauseMoney = false;
        public static bool pauseLoot = false;
        public static bool pauseCsTrade = false;
        public static bool pauseTSF = false;
        public static bool pauseDeals = false;
        public static int sleepMSecond = 500;
        public static int sleepMSecondLoot = 500;
        public static int sleepMCsTrade = 500;
        public static int sleepMTSF = 500;
        public static int sleepMDeals = 500;
        public static int sleepTSF = 500;
        public static int sleepIMONEY = 0;
        public static int sleepILoot = 0;
        public static int sleepICsTrade = 0;
        public static int sleepITSF = 0;
        public static int sleepIDeals = 0;
        public static bool autoConfirm = false;
        public static int threadCount = 7;
        public static bool AutoRefreshPage = false;
        public struct Dat
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Factory { get; set; }
            public double Price { get; set; }
        }
        public struct DataViewBadPrice
        {
            public string Date { get; set; }
            public string Site { get; set; }
            public string Name { get; set; }
            public string OldPrice { get; set; }
            public string NewPrice { get; set; }
        }
        //public struct DataForMoney
        //{
        //    public string Name { get; set; }
        //    public double Price { get; set; }
        //    public string Id { get; set; }
        //    public string Bot { get; set; }
        //}
        public static  void SetListBadPrice(string _Data, string _Site, string _Name, string _OldPrice, string _NewPrice)
        {
            Program.DataViewBadPrice item = new Program.DataViewBadPrice()
            {
                Date = _Data,
                Site = _Site,
                Name = _Name,
                OldPrice = _OldPrice,
                NewPrice = _NewPrice
            };
            Program.BadPrice.Add(item);
        }

    }
    
}
