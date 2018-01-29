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
    public partial class AddLoot : Form
    {
        DataGridView DGV;
        public AddLoot(DataGridView dgv)
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
            Program.DataLoot.Add(item);
            RefreshGrid();
            string json = JsonConvert.SerializeObject(Program.DataLoot);
            File.WriteAllText("dataLootFarm.txt", json);
        }
        private void RefreshGrid()
        {
            DGV.Rows.Clear();
            foreach (var item in Program.DataLoot)
            {
                int rowId = DGV.Rows.Add();
                DataGridViewRow row = DGV.Rows[rowId];
                row.Cells["id1"].Value = item.Id;
                row.Cells["name1"].Value = item.Name;
               // row.Cells["factor1"].Value = item.Factory;
                row.Cells["price1"].Value = item.Price;
            }

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            textBox2.Text = Clipboard.GetText();
        }
    }
}
