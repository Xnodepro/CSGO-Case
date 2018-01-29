using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebSocketSharp;
using WebSocket4Net;

using System.Net;
using WebSocketSharp.Server;
using System.Security.Cryptography.X509Certificates;

namespace CSMONEY
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            timer1.Start();
        }

        List<string> ProxyList = new List<string>();
        int Id = 0;
        int IdLoot = 0;
        int IdCsTrade = 0;
        int IdTSF = 0;
        int IdDeals = 0;
 
        private void button1_Click(object sender, EventArgs e)
        {
            Work w = new Work( Id);
            new System.Threading.Thread(delegate () {
                w.INI();
            }).Start();
            Id++;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            // textBox1.Text = driver.PageSource;
            var item = new Program.Dat {
                Id = unixTimestamp,
                Name = textBox2.Text,
             //   Factory = comboBox1.Text,
                Price = Convert.ToDouble(textBox3.Text.Replace(".",","))};
                Program.Data.Add(item);
            RefreshGrid();
            string json = JsonConvert.SerializeObject(Program.Data);
            File.WriteAllText("data.txt", json);
        }
        #region Refresh
        private void RefreshGrid()
        {
            dataGridView1.Rows.Clear();
            foreach (var item in Program.Data)
            {
                int rowId = dataGridView1.Rows.Add();
                DataGridViewRow row = dataGridView1.Rows[rowId];
                row.Cells["id2"].Value = item.Id;
                row.Cells["Name"].Value = item.Name;
                row.Cells["Factor"].Value = item.Factory;
                row.Cells["Price"].Value = item.Price;
            }

        }
      
        #endregion
        #region rTimer
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (Program.Mess.Count != 0)
                {
                    for (int i = 0; i < Program.Mess.Count; i++)
                    {
                        listBox1.Items.Insert(0, Program.Mess.Dequeue());
                        // textBox1.Text = Program.Mess.Dequeue() + Environment.NewLine + textBox1.Text;
                    }
                }
                label3.Text = $"Cs.Money DataBase Items: {Program.DataMoney.Count.ToString()}";

            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }

        }
        private void timer2_Tick(object sender, EventArgs e)
        {
            string tmp = "";
            foreach (var item in listBox1.Items)
            {
                tmp = tmp + item + Environment.NewLine;
            }
            File.WriteAllText("./logCsMoney/" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff") + ".txt", tmp);
            listBox1.Items.Clear();

           
        }
        #endregion
        #region Enebl
        private void enableCSMoney()
        {
            if (Properties.Settings.Default.csmoney != "")
            {
                //  var a = Properties.Settings.Default.lootfarm;
                button1.Enabled = true;
                button4.Enabled = true;
                button11.Enabled = true;
                textBox4.Enabled = true;
            }
        }
      
        #endregion

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            Program.sleepMSecond = Convert.ToInt32(textBox4.Text);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox13.Text = Properties.Settings.Default.ApiKey;
            enableCSMoney();

            timer2.Start();
            try
            {
                string jj = File.ReadAllText("data.txt");
                if (jj != "")
                {
                    Program.Data = JsonConvert.DeserializeObject<List<Program.Dat>>(jj);
                }
            }
            catch (Exception ex) { Program.Mess.Enqueue("" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "|" + ex.Message); }
            try
            {
                string jj = File.ReadAllText("dataBaseMoney.txt");
                if (jj != "")
                {
                    Program.DataMoney = JsonConvert.DeserializeObject<List<DataForMoney>>(jj);
                }
            }
            catch (Exception ex) { Program.Mess.Enqueue("" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "|" + ex.Message); }
            RefreshGrid();

        }
       
    
        private void button4_Click(object sender, EventArgs e)
        {
            Add a = new Add(dataGridView1);
            a.Show();
        }

      
       
        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int index = dataGridView1.CurrentCell.RowIndex;
            dataGridView1.Rows.RemoveAt(index);
            Program.Data.RemoveAt(index);
            string json = JsonConvert.SerializeObject(Program.Data);
            File.WriteAllText("data.txt", json);
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);

        }
       
        private void button11_Click(object sender, EventArgs e)
        {
            if (!Program.pauseMoney)
            {
                Program.pauseMoney = true;
                button11.BackColor = Color.YellowGreen;
                Program.Mess.Enqueue( DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "|" + "Пауза установлена!");
            }
            else {
                Program.pauseMoney = false;
                button11.BackColor = Color.White;
                Program.Mess.Enqueue(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "|" + "Пауза снята!");
            }
        }
       
        public struct Dat
        {
            public string Event { get; set; }
            data data { get; set; }
           
        }
        public struct data
        {
            List<Data> Data { get; set; }
        }
        public struct Data
        {
            public List<string> b { get; set; }
            public List<string> id { get; set; }
            public string m { get; set; }
            public string e { get; set; }
            public double p { get; set; }
            public Data(List<string> B, List<string> ID, string M ,string E, double P)
            {
                b = B;
                id = ID;
                m = M;
                e = E;
                p = P;


            }
            //  public string id { get; set; }
        }
        private void button14_Click(object sender, EventArgs e)
        {
            string json = File.ReadAllText("jj.txt");
            var da = JsonConvert.DeserializeObject<dynamic>(json.Replace("event","Event"));
            var aa =da.Event.Value;
            foreach (var item in da.data)
            {
                var b = item.b[0].Value;
                var id = item.id[0].Value;
                var ee = item.e.Value;
                var m = item.m.Value;
                var p = item.p.Value;
            }

        }
      
       
        private void textBox13_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ApiKey = textBox13.Text;
            Properties.Settings.Default.Save();
        }

        

        

       

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                checkBox1.Text = "Отключить";
                Program.autoConfirm = true;
                textBox13.Enabled = true;
            }
            else {
                checkBox1.Text = "Включить";
                Program.autoConfirm = false;
                textBox13.Enabled = false;
            }
        }

        private void button19_Click(object sender, EventArgs e)
        {
            ViewBadPrice VBP = new ViewBadPrice();
            VBP.Show();
        }

        private void изменитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int index = dataGridView1.CurrentCell.RowIndex;
            Program.Dat neww  = Program.Data[index];
            neww.Price = Convert.ToDouble(dataGridView1.Rows[index].Cells[3].Value.ToString());
            dataGridView1.Rows.RemoveAt(index);
            Program.Data.RemoveAt(index);
            Program.Data.Add(neww);
            string json = JsonConvert.SerializeObject(Program.Data);
            File.WriteAllText("data.txt", json);
            RefreshGrid();
            MessageBox.Show("Успешно изменено!");
        }

        private void textBox12_TextChanged(object sender, EventArgs e)
        {

        }

       
       

        

        
        private void button20_Click(object sender, EventArgs e)
        {
            ProxyList.Clear();
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            // получаем выбранный файл
            string filename = openFileDialog1.FileName;
            // читаем файл в строку
            string[] proxy = System.IO.File.ReadAllLines(filename);
            foreach (var item in proxy)
            {
                ProxyList.Add(item);
            }
            MessageBox.Show("подгрузил прокси в количестве:" + ProxyList.Count);
        }

        private void button21_Click(object sender, EventArgs e)
        {
            ProxyList.Clear();
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            // получаем выбранный файл
            string filename = openFileDialog1.FileName;
            // читаем файл в строку
            string[] proxy = System.IO.File.ReadAllLines(filename);
            foreach (var item in proxy)
            {
                ProxyList.Add(item);
            }
            MessageBox.Show("подгрузил прокси в количестве:" + ProxyList.Count);
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                Program.BrowesrQuery = true;
            }
            else { Program.BrowesrQuery = false; }
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Program.threadCount = Convert.ToInt32(textBox5.Text);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            // получаем выбранный файл
            string filename = openFileDialog1.FileName;
            // читаем файл в строку
            string[] fileText = System.IO.File.ReadAllLines(filename);
            foreach (var item in fileText)
            {
                Program.ProxyList.Enqueue(item);
                Program.ProxyListFix.Add(item);
            }
            MessageBox.Show(fileText.Length.ToString());
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Money money = new Money();
            money.INI();
        }

        private void настройкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingProp SP= new SettingProp();
            SP.Show();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked) { Program.AutoRefreshPage = true; }
            else { Program.AutoRefreshPage = false; }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Parsing Par = new Parsing();
            Par.Show();
        }
    }

}
