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
    public partial class AddDeals : Form
    {
        DataGridView DGV;
        public AddDeals(DataGridView dgv)
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
            Program.DataDeals.Add(item);
            RefreshGrid();
            string json = JsonConvert.SerializeObject(Program.DataDeals);
            File.WriteAllText("dataCsDeals.txt", json);
        }
        private void RefreshGrid()
        {
            DGV.Rows.Clear();
            foreach (var item in Program.DataDeals)
            {
                int rowId = DGV.Rows.Add();
                DataGridViewRow row = DGV.Rows[rowId];
                row.Cells["id5"].Value = item.Id;
                row.Cells["name5"].Value = item.Name;
                //    row.Cells["factory3"].Value = item.Factory;
                row.Cells["price5"].Value = item.Price;
            }

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            textBox2.Text = Clipboard.GetText();
        }
    }
}
