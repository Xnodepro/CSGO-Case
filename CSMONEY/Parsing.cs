using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSMONEY
{
    public partial class Parsing : Form
    {
        public Parsing()
        {
            InitializeComponent();
            Form.CheckForIllegalCrossThreadCalls = false;
        }
        bool stopWork = false;
        bool getFullTable = false ;
        int minPriceToColor = 5;
        public struct Dat
        {
            public string Name { get; set; }

            public double PriceMoney { get; set; }
            public double PriceJar { get; set; }
            public double Percent{ get; set; }
        }
        List<Dat> It = new List<Dat>();

        public bool GetFullTable {
            get => getFullTable;
            set {
                try
                {
                    if (value == true && stopWork == false)
                    {
                        new System.Threading.Thread(delegate () {
                            WriteToGrid();
                        }).Start();

                    }

                }
                catch (Exception)
                {

                    throw;
                }
                
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }
        void Work()
        {
            

                while (true)
                {
                    if (stopWork == false)
                    {
                        try
                        {
                        It.Clear();
                            string ss = "";
                            //var res = Program.DataJar.items.GroupBy(x => x.name).Select(x => x.First()).ToList();

                            //foreach (var item in res)
                            //{
                            //    var moneyItems = Program.DataMoney.Find(n => (n.Name.Replace(" ", "") == item.name.Replace(" ", "")));
                            //    if (moneyItems != null)
                            //    {
                            //        double tmpMoneyPrice = moneyItems.Price * 0.97;
                            //        double _Percent = ((tmpMoneyPrice / item.price) - 1) * 100;
                            //        //  ss = $"Название:{ item.name }  Цена мани:{ tmpMoneyPrice}  Цена Джар:{ item.price}---Процент:{_Percent}" + Environment.NewLine + ss;
                            //        Dat D = new Dat()
                            //        {
                            //            Name = item.name,
                            //            PriceMoney = tmpMoneyPrice,
                            //            PriceJar = item.price,
                            //            Percent = _Percent
                            //        };
                            //        It.Add(D);
                            //    }

                            //}
                        GetFullTable = true;
                        Thread.Sleep(1000);

                        }
                        catch (Exception ex)
                        {
                            Program.Mess.Enqueue(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "|Ошибка999 :" + ex.Message);
                        }
                    }
                    else { Thread.Sleep(1000); }

                }
                
            
        }

        private void Parsing_Load(object sender, EventArgs e)
        {
            try
            {
                new System.Threading.Thread(delegate () {
                    Work();
                }).Start();
            }
            catch (Exception)
            {
                throw;
            }

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                stopWork = true;
            }
            else { stopWork = false; }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
           
        }
        void WriteToGrid()
        {
            try
            {
                var aa = It.OrderByDescending(x => x.Percent);
                // textBox1.Text = ss;
                
                this.listView1.Items.Clear();

                this.listView1.BeginUpdate();
                foreach (var item in aa)
                {
                    if (item.Percent > -1)
                    {
                        
                        string[] row = { item.Name.ToString(), item.PriceMoney.ToString(), item.PriceJar.ToString(), item.Percent.ToString() };
                        var listViewItem = new ListViewItem(row);
                        if (item.PriceMoney > minPriceToColor)
                        { listViewItem.BackColor = Color.LawnGreen; }
                        listView1.Items.Add(listViewItem);
                       // dataGridView1.Rows.Add(item.Name.ToString(), item.PriceMoney.ToString(), item.PriceJar.ToString(), item.Percent.ToString());
                    }
                }
                this.listView1.EndUpdate();
            }
            catch (Exception)
            {

                throw;
            }
           
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            Clipboard.SetText(e.Item.Text);
     
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            minPriceToColor = Convert.ToInt32(textBox1.Text);
        }
    }
}
