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
using System.IO;
using System.Net.NetworkInformation;
using System.Windows.Threading;
using System.Threading;

namespace Server
{
    public partial class Form1 : Form
    {
        Socket server, client;
        byte[] data;
        IPEndPoint ipClient;
        int recv;
        ManualResetEvent allDone = new ManualResetEvent(false);
        string response = string.Empty;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           
            IPEndPoint ipServer = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1234);
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(ipServer);
            server.Listen(5);

              //  allDone.Reset();
                server.BeginAccept(new AsyncCallback(CallAccept), server);
            //    allDone.WaitOne();
            
           
            //client = server.Accept();
            //data = new byte[1024];
            //client.Receive(data);
            //listBox1.Items.Add(Encoding.ASCII.GetString(data));
            
        }
        private void CallAccept(IAsyncResult i)
        {
            allDone.Set();
            client = ((Socket)i.AsyncState).EndAccept(i);

            //StateObject state = new StateObject();
            //state.workSocket = client;
            //client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
            //byte[] data = new byte[1024];
            //client.BeginReceive(data, 0, data.Length, SocketFlags.None, new AsyncCallback(ReceiveData), client);
        }
        public void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            // Read data from the client socket.   
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                // There  might be more data, so store the data received so far.  
                state.sb.Append(Encoding.ASCII.GetString(
                    state.buffer, 0, bytesRead));

                // Check for end-of-file tag. If it is not there, read   
                // more data.  
                content = state.sb.ToString();

                // All the data has been read from the   
                // client. Display it on the console.  
                //Console.WriteLine("Read {0} bytes from socket. \n Data : {1}",
                //    content.Length, content);
                listBox1.Items.Add(content);
                    // Echo the data back to the client.  
                    Send(handler, content);
                
                //else
                //{
                //    // Not all data received. Get more.  
                //    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                //    new AsyncCallback(ReadCallback), state);
                //}
            }
        }
        private  void Send(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.  
            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback),handler);
        }
        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                //Socket handler = (Socket)ar.AsyncState;
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = handler.EndSend(ar);
                //Console.WriteLine("Sent {0} bytes to client.", bytesSent);

                Receive(handler);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        //private void ReceiveData(IAsyncResult i)
        //{
        //    ((Socket)i.AsyncState).EndReceive(i);
        //    listBox1.Items.Add("Client: " + Encoding.ASCII.GetString(data));
        //}

        private void Receive(Socket handler)
        {
            try
            {
                // Create the state object.  
                StateObject state = new StateObject();
                state.workSocket = handler;

                // Begin receiving the data from the remote device.  
                handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket   
                // from the asynchronous state object.  
                StateObject state = (StateObject)ar.AsyncState;
                Socket handler = state.workSocket;

                // Read data from the remote device.  
                int bytesRead = handler.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.  
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                    // Get the rest of the data.  
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), state);
                }
                else
                {
                    // All the data has arrived; put it in response.  
                    if (state.sb.Length > 1)
                    {
                        response = state.sb.ToString();
                    }
                    // Signal that all bytes have been received.  
                    //receiveDone.Set();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            /*
            string text = textBox1.Text;
            listBox1.Items.Add(text);
            textBox1.Text = "";
            data = new byte[1024];
            data = Encoding.ASCII.GetBytes(text);
            client.Send(data);
            
            data = new byte[1024];
            client.Receive(data);
            listBox1.Items.Add(Encoding.ASCII.GetString(data));
            */

            data = new byte[30];
            data = Encoding.ASCII.GetBytes(textBox1.Text);
            listBox1.Items.Add(textBox1.Text);
            client.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendData), client);
            //Send(client, textBox1.Text);



        }
        private void SendData(IAsyncResult iar)
        {
            client = (Socket)iar.AsyncState;
            int sent = client.EndSend(iar);
            data = new byte[30];
            client.BeginReceive(data, 0, data.Length, SocketFlags.None, new AsyncCallback(ReceivedData), client);
            string receivedData = Encoding.ASCII.GetString(data, 0, recv);
            listBox1.Items.Add(receivedData);
        }
        private void ReceivedData(IAsyncResult iar)
        {
            client = (Socket)iar.AsyncState; 
            recv = client.EndReceive(iar);
            
        }
    }
}
