using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace ServerClient
{
    public partial class Form1 : Form
    {
        private TcpClient client;
        public StreamReader STR;
        public StreamWriter STW;
        public string receive;
        public string text_to_send;
        public static string ip = "";

        public Form1()
        {
            InitializeComponent();

            IPAddress[] localIP = Dns.GetHostAddresses(Dns.GetHostName()); //get own IP
            foreach (IPAddress address in localIP)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    txtIPServer.Text = address.ToString();
                    ip = txtIPServer.Text;
                }
            }
        }

        private void BtnStartServer_Click(object sender, EventArgs e)
        {
            TcpListener listener = new TcpListener(IPAddress.Any, int.Parse(txtPortServer.Text));
            listener.Start();
            client = listener.AcceptTcpClient();
            STR = new StreamReader(client.GetStream());
            STW = new StreamWriter(client.GetStream());
            STW.AutoFlush = true;

            backgroundWorker1.RunWorkerAsync();  // Start receiving data in background

            backgroundWorker2.WorkerSupportsCancellation = true; // ability to cancel thread


        }

        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e) // receive data
        {
            while (client.Connected)
            {
                try
                {
                    receive = STR.ReadLine();
                    this.textBox2.Invoke(new MethodInvoker(delegate () 
                    {
                        textBox2.AppendText("Server :" + receive + "\n");
                        if (this.WindowState == FormWindowState.Minimized)
                        {
                            this.Show();
                            this.WindowState = FormWindowState.Normal;
                            notifyIcon1.Visible = false;
                        }
                    }
                    ));
                    receive = "";
                }
                catch (Exception x)
                {

                    MessageBox.Show(x.Message.ToString());
                }
            }
        }

        private void BackgroundWorker2_DoWork(object sender, DoWorkEventArgs e) // send data 
        {
            if (client.Connected)
            {
                STW.WriteLine(text_to_send);
                this.textBox2.Invoke(new MethodInvoker(delegate ()
                {
                    textBox2.AppendText("Client :" + text_to_send + "\n");
                }));
            }
            else
            {
                MessageBox.Show("Send fail!");
            }
            backgroundWorker2.CancelAsync();
        }

        

        private void Button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                text_to_send = textBox1.Text;
                backgroundWorker2.RunWorkerAsync();
            }
            textBox1.Text = "";
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon1.Visible = true;
            }
        }

        private void NotifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }
    }
}
