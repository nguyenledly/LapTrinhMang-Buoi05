using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Windows.Threading;

namespace Chat_Client
{
    public partial class Form1 : Form
    {
        Socket client;
        byte[] data;
        IPEndPoint ipServer;
        int recv;
        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //ipServer = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1234);
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ipServer = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1234);
            //client.Connect(ipServer);
            client.BeginConnect(ipServer, new AsyncCallback(Connected), client);
        }
        private void Connected(IAsyncResult i)
        {
            client = ((Socket)i.AsyncState);
            client.EndConnect(i);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            /*
            //string text = textBox2.Text;
            string text;
            data = new byte[1024];
            client.Receive(data);
            //text = Encoding.ASCII.GetString(data);
            listBox1.Items.Add(Encoding.ASCII.GetString(data));


            data = new byte[1024];
            data = Encoding.ASCII.GetBytes(textBox2.Text);
            listBox1.Items.Add(textBox2.Text);
            textBox2.Text = "";
            client.Send(data);
            */

            

            data = new byte[30];
            data = Encoding.ASCII.GetBytes(textBox2.Text);
            listBox1.Items.Add(textBox2.Text);
            client.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendData), client);

           
        }

        private void SendData(IAsyncResult iar)
        {
            client = (Socket)iar.AsyncState;
            int sent = client.EndSend(iar);
            //Socket server = (Socket)iar.AsyncState;
            //int sent = server.EndSend(iar); 
            data = new byte[30];
            client.BeginReceive(data, 0, data.Length, SocketFlags.None, new AsyncCallback(ReceivedData), client);
            string receivedData = Encoding.ASCII.GetString(data);
            listBox1.Items.Add(receivedData);
        }
        private void ReceivedData(IAsyncResult iar)
        {
            client = (Socket)iar.AsyncState;
            recv = client.EndReceive(iar);
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
