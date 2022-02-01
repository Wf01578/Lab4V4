using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PacketDotNet;
using PacketDotNet;
using SharpPcap;


namespace Lab4V4
{
    public partial class frmCapture : Form
    {
        CaptureDeviceList devices;
        public static ICaptureDevice device;
        public static string  stringPackets = "";
        public static int numPackets;
        frmSend fSend;
        public frmCapture()
        {
            InitializeComponent();
            devices = CaptureDeviceList.Instance;
            if (devices.Count < 1) {
                MessageBox.Show("No valid Capture devices found!");
                Application.Exit();
            }

            foreach (ICaptureDevice dev in devices) {
                comboBox1.Items.Add(dev.Description);
            }
            device = devices[0];
            comboBox1.Text = device.Description;

            device.OnPacketArrival += new SharpPcap.PacketArrivalEventHandler(device_OnPacketArrival);


            int readTimeoutMilliseconds = 1000;
            device.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);


        }
        private static void device_OnPacketArrival(Object sender, CaptureEventArgs packet) {

            numPackets++;

            stringPackets += "Packet Number: " + Convert.ToString(numPackets);
            stringPackets += Environment.NewLine;
            //parse packets

            byte[] data = packet.Packet.Data;
            int byteCounter = 0;
            stringPackets += "Destination MAC Address: ";

            foreach (byte b in data)
            {
                if (byteCounter <= 13) stringPackets += b.ToString("X2") + " " ;
                //stringPackets += b.ToString("X2") + " ";
                byteCounter++;
                switch (byteCounter)
                {
                    case 6: stringPackets += Environment.NewLine;
                        stringPackets += "Source MAC Address: ";
                        break;
                    case 12: stringPackets += Environment.NewLine;
                        stringPackets += "Ethertype: ";
                        break;
                    case 14:
                        if (data[12] == 8)
                        {
                            if (data[13] == 0) {
                                stringPackets += "(IP)";
                            }
                            if (data[13] == 6) {
                                stringPackets += "(ARP)";
                            }
                            
                        }

                        break;        
                }

            }

                data = packet.Packet.Data;
                byteCounter = 0;
            stringPackets += Environment.NewLine;
            stringPackets += Environment.NewLine;
            stringPackets += "Raw Data: " + Environment.NewLine;
                
            //process and display each byte in our packets
            foreach (byte b in data) {
                stringPackets += b.ToString("X2") + " " ;
                byteCounter++;
                if (byteCounter == 16) {
                    byteCounter = 0;
                    stringPackets += Environment.NewLine;
                
                }
            
            }
            stringPackets += Environment.NewLine;
            stringPackets += Environment.NewLine;


        }

        private void txtCaptureData_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try {
                if (button1.Text == "Start") {

                    device.StartCapture();
                    timer1.Enabled = true;
                    button1.Text = "Stop";
                }
                else {
                    device.StopCapture();
                    timer1.Enabled = false;
                    button1.Text = "Start";

                }

            }
            catch (Exception exp) {

            
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            txtCaptureData.AppendText(stringPackets);
            stringPackets = "";
            txtNumPackets.Text = Convert.ToString(numPackets);
        }

        private void comboBox1_SelectedIndexChanged(Object sender, EventArgs e) {
            device = devices[comboBox1.SelectedIndex];
            comboBox1.Text = device.Description;
            device.OnPacketArrival += new SharpPcap.PacketArrivalEventHandler(device_OnPacketArrival);
            int readTimeoutMilliseconds = 1000;
            device.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);


        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Text Files|*.txt|All Files|*.*";
            saveFileDialog1.Title = "Save the captured packets";
            saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName != "") {
                System.IO.File.WriteAllText(saveFileDialog1.FileName, txtCaptureData.Text);
            }

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Text Files|*.txt|All Files|*.*";
            openFileDialog1.Title = "Open the captured packets";
            openFileDialog1.ShowDialog();
            if (openFileDialog1.FileName != "")
            {
                txtCaptureData.Text = System.IO.File.ReadAllText(openFileDialog1.FileName);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void sendWindoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (frmSend.instantiations == 0)
            {
                fSend = new frmSend();
                fSend.Show();
            }
        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            device = devices[comboBox1.SelectedIndex];
            comboBox1.Text = device.Description;
            device.OnPacketArrival += new SharpPcap.PacketArrivalEventHandler(device_OnPacketArrival);
            int readTimeoutMilliseconds = 1000;
            device.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);
            textBox1.Text = device.Name;
        }
    }
}
