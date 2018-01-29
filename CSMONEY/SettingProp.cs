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
    public partial class SettingProp : Form
    {
        public SettingProp()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.UsetAgent = textBox1.Text;
            Properties.Settings.Default.Save();
        }

        private void SettingProp_Load(object sender, EventArgs e)
        {
            textBox1.Text = Properties.Settings.Default.UsetAgent;
        }
    }
}
