using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSMONEY
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool chek = false;
            var dt = DataTa.GetTable();
            bool checkcsMoney = false;
            bool checkcsloot = false;
            bool checkcstrade = false;
            bool checkcstsf = false;
            bool checkcdeals = false;
            foreach (DataRow item in dt.Rows)
            {
                var s = item.ItemArray;
                if (s[0].ToString() == textBox1.Text)
                {
                    if (s[4].ToString() == "https://skinsjar.com/ru")
                    {
                        chek = true;
                        checkcsMoney = true;
                        Properties.Settings.Default.csmoney = s[1].ToString();
                        Properties.Settings.Default.name = textBox1.Text;
                        Properties.Settings.Default.Save();
                        if (s[3].ToString() != Properties.Settings.Default.csmoneyVersion)
                        {
                            MessageBox.Show("Версия ПО устарела");
                            Application.Exit();
                        }
                    }
                   
                    //Program.sleepILoot = Convert.ToInt32(s[2].ToString());
                    
                }
            }
            if(checkcsloot == false)
            {
                Properties.Settings.Default.lootfarm = "";
                Properties.Settings.Default.Save();
            }
            if (checkcsMoney == false)
            {
                Properties.Settings.Default.csmoney = "";
                Properties.Settings.Default.Save();
            }
            if (checkcstrade == false)
            {
                Properties.Settings.Default.CsTrade = "";
                Properties.Settings.Default.Save();
            }
            if (checkcstsf == false)
            {
                Properties.Settings.Default.CsTSF = "";
                Properties.Settings.Default.Save();
            }
            if (checkcdeals == false)
            {
                Properties.Settings.Default.Deals = "";
                Properties.Settings.Default.Save();
            }
            if (chek == false)
            {
                MessageBox.Show("Программа не лицензированная");
                Application.Exit();
            }
            else { Form1 f1 = new Form1();f1.Show(); this.Hide(); }
        }

        private void Login_Load(object sender, EventArgs e)
        {

        }
    }
}
