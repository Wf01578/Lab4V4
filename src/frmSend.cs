using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab4V4
{
    public partial class frmSend : Form
    {

        public static int instantiations = 0;
        public frmSend()
        {
            InitializeComponent();
            instantiations++;
        }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Text Files|*.txt|All Files|*.*";
            openFileDialog1.Title = "Open the captured packets";
            openFileDialog1.ShowDialog();
            if (openFileDialog1.FileName != "")
            {
                txtPacket.Text = System.IO.File.ReadAllText(openFileDialog1.FileName);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Text Files|*.txt|All Files|*.*";
            saveFileDialog1.Title = "Save the captured packets";
            saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName != "")
            {
                System.IO.File.WriteAllText(saveFileDialog1.FileName, txtPacket.Text);
            }

        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string stringBytes = "";
            foreach (string s in txtPacket.Lines){
                string[] noComments = s.Split('#');
                string s1 = noComments[0];
                stringBytes += s1 + Environment.NewLine;
            }
        string[] sBytes = stringBytes.Split(new string[] { "\n", "\r\n", " ", "\t" }, StringSplitOptions.RemoveEmptyEntries);
            byte[] packet = new byte[sBytes.Length];
            int i = 0;
            foreach (string s in sBytes) {
                packet[i] = Convert.ToByte(s, 16);
                i++;
            }

            try
            {
                frmCapture.device.SendPacket(packet);
            }
            catch(Exception exp) { 
            //do something for error
            }

        }
        private void frmSend_FormClosed(object sender, FormClosedEventArgs e)
        {
            instantiations--;
        }
    }
}
