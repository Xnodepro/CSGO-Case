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
    public partial class ViewBadPrice : Form
    {
        public ViewBadPrice()
        {
            InitializeComponent();
        }

        private void ViewBadPrice_Load(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            foreach (var item in Program.BadPrice)
            {
                ListViewItem lvi = new ListViewItem(item.Date);
                lvi.SubItems.Add(item.Site);
                lvi.SubItems.Add(item.Name);
                lvi.SubItems.Add(item.OldPrice);
                lvi.SubItems[3].BackColor = System.Drawing.Color.OrangeRed;
                lvi.SubItems.Add(item.NewPrice);
                lvi.SubItems[4].BackColor = System.Drawing.Color.LawnGreen;
                lvi.UseItemStyleForSubItems = false;
                listView1.Items.Add(lvi);
            }
        }
    }
}
