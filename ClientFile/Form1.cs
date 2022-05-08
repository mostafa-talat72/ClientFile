using System;
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Text;
namespace ClientFile
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void ConnectBTN_Click(object sender, EventArgs e)
        {
            int recv;
            byte[] data = new byte[1024];
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 9050);
            Socket newsock = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream, ProtocolType.Tcp);
            newsock.Bind(ipep);
            newsock.Listen(10);
            InfoTxt.Text += "Waiting for a client...\n";
            Socket client = newsock.Accept();
            IPEndPoint clientep = (IPEndPoint)client.RemoteEndPoint;
            string welcome = "Welcome to my test server";
            data = Encoding.ASCII.GetBytes(welcome);
            client.Send(data, data.Length, SocketFlags.None);
            int cnt = 0;
            StreamWriter streamWriter;
            string fileName = "";
            while (true)
            {
                data = new byte[1024];
                recv = client.Receive(data);
                if (recv == 0)
                    break;
                if (cnt == 1)
                {
                    Stream file = new FileStream(Encoding.ASCII.GetString(data, 0, recv), FileMode.OpenOrCreate);
                    fileName = Encoding.ASCII.GetString(data, 0, recv);
                    file.Close();
                }
                else if (cnt > 1)
                {
                    streamWriter = new StreamWriter(fileName);
                    streamWriter.Write(Encoding.ASCII.GetString(data, 0, recv));
                    streamWriter.Close();
                }
                InfoTxt.Text += Encoding.ASCII.GetString(data, 0, recv);
                client.Send(data, recv, SocketFlags.None);
                cnt++;
            }
            InfoTxt.Text += "Disconnected from " + clientep.Address + "\n";
            client.Close();
            newsock.Close();
        }
    }
}