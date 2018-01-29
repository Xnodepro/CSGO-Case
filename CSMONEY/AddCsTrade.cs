using Newtonsoft.Json;
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

namespace CSMONEY
{
    public partial class AddCsTrade : Form
    {
        DataGridView DGV;
        public AddCsTrade(DataGridView dgv)
        {
            DGV = dgv;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            var item = new Program.Dat
            {
                Id = unixTimestamp,
                Name = textBox2.Text,
                //Factory = comboBox1.Text,
                Price = Convert.ToDouble(textBox3.Text.Replace(".", ","))
            };
            Program.DataCsTrade.Add(item);
            RefreshGrid();
            string json = JsonConvert.SerializeObject(Program.DataCsTrade);
            File.WriteAllText("dataCsTrade.txt", json);
        }
        private void RefreshGrid()
        {
            DGV.Rows.Clear();
            foreach (var item in Program.DataCsTrade)
            {
                int rowId = DGV.Rows.Add();
                DataGridViewRow row = DGV.Rows[rowId];
                row.Cells["id3"].Value = item.Id;
                row.Cells["name3"].Value = item.Name;
            //    row.Cells["factory3"].Value = item.Factory;
                row.Cells["price3"].Value = item.Price;
            }

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            textBox2.Text = Clipboard.GetText();
        }

        private void AddCsTrade_Load(object sender, EventArgs e)
        {

        }
    }
}
