﻿using SimpleTCP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TCPIPdemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        SimpleTcpServer server;

        private void Form1_Load(object sender, EventArgs e)
        {
            server = new SimpleTcpServer();
            server.Delimiter = 0x13;//enter
            server.StringEncoder = Encoding.UTF32;
            server.DataReceived += Server_DataReceived; 
        }

        private void Server_DataReceived(object sender, SimpleTCP.Message e)
        {
            txtStatus.Invoke((MethodInvoker)delegate () {
                txtStatus.Text += e.MessageString;
                e.ReplyLine(string.Format("You said : {0}", e.MessageString));
            });
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            txtStatus.Text += "Server starting...";
            long s;
            try
            {
                System.Net.IPAddress ip = new System.Net.IPAddress(long.Parse(txtHost.Text));
                server.Start(ip, Convert.ToInt32(txtPort.Text));
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        private void BtnStop_Click(object sender, EventArgs e)
        {
            if (server.IsStarted)
            {
                server.Stop();
            }
        }
    }
}
