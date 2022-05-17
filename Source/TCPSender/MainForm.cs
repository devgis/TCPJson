using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace TCPSender
{
    public partial class MainForm : Form
    {
        const int listenPort = 30009;
        //const int sendPort = 8099;
        Thread listenthread = null;
        public MainForm()
        {
            InitializeComponent();
            listenthread = new Thread(Listen);
            listenthread.Start();
        }

        private void btStart_Click(object sender, EventArgs e)
        {
            //125.76.225.111 30009
            string ssend = File.ReadAllText(Path.Combine(Application.StartupPath, "send.txt"));
            Send2("127.0.0.1", listenPort, ssend); //192.168.0.130 "125.76.225.111"
            /*
            this.tbMessage.Invoke((EventHandler)(delegate
            {
                tbMessage.Text += "Result: " + irs + " ..." + "\r\n";
            }));
            if (btStart.Text == "开始")
            {
                //开始
                btStart.Text = "结束";
            }
            else
            {
                //结束
                btStart.Text = "开始";
            }
            */
        }

        private Encoding encode = Encoding.Default;
        /// <summary>
        /// 监听请求
        /// </summary>
        /// <param name="port"></param>
        public void Listen()
        {
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listenSocket.Bind(new IPEndPoint(IPAddress.Any, listenPort));
            listenSocket.Listen(listenPort);
            this.tbMessage.Invoke((EventHandler)(delegate
            {
                tbMessage.Text += "Listen " + listenPort + " ..." + "\r\n";
            }));
            while (true)
            {
                Socket acceptSocket = listenSocket.Accept();
                string receiveData = Receive(acceptSocket, 5000); //5 seconds timeout.
                //Console.WriteLine("Receive：" + receiveData);
                this.tbMessage.Invoke((EventHandler)(delegate
                {
                    tbMessage.Text += "Receive：" + receiveData + "\r\n";
                }));

                acceptSocket.Send(encode.GetBytes("ok"));
                DestroySocket(acceptSocket); //import
            }
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public int Send(string host, int port, string data)
        {
            string result = string.Empty;
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect(host, port);
            int iresult=clientSocket.Send(encode.GetBytes(data));
            //Console.WriteLine("Send：" + data);
            //tbMessage.Text += "Send：" + data + "\r\n";
            //result = Receive(clientSocket, 5000 * 2); //5*2 seconds timeout.
            //Console.WriteLine("Receive：" + result);
            //tbMessage.Text += "Receive：" + result + "\r\n";
            DestroySocket(clientSocket);
            return iresult;
        }

        public int Send2(string ip, int port, string message)
        {
            TcpClient client = new TcpClient();
            client.Connect(ip, port);
            int irs=client.Client.Send(encode.GetBytes(message));
            //byte[] word = new byte[1024];
            //int count = client.Client.Receive(word);
            //string msg = Encoding.Unicode.GetString(word, 0, count).ToString();
            return irs;
        }
        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        private string Receive(Socket socket, int timeout)
        {
            string result = string.Empty;
            socket.ReceiveTimeout = timeout;
            List<byte> data = new List<byte>();
            byte[] buffer = new byte[8192];
            socket.Receive(buffer);
            result = encode.GetString(buffer, 0, buffer.Length);
            /*
            string result = string.Empty;
            socket.ReceiveTimeout = timeout;
            List<byte> data = new List<byte>();
            byte[] buffer = new byte[1024];
            int length = 0;
            try
            {
                while ((length = socket.Receive(buffer)) > 0)
                {
                    for (int j = 0; j < length; j++)
                    {
                        data.Add(buffer[j]);
                    }
                    if (length < buffer.Length)
                    {
                        break;
                    }
                }
            }
            catch { }
            if (data.Count > 0)
            {
                result = encode.GetString(data.ToArray(), 0, data.Count);
            }
            */
            return result;
        }
        /// <summary>
        /// 销毁Socket对象
        /// </summary>
        /// <param name="socket"></param>
        private void DestroySocket(Socket socket)
        {
            if (socket.Connected)
            {
                socket.Shutdown(SocketShutdown.Both);
            }
            socket.Close();
        }
    }
}
