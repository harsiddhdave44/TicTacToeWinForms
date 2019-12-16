using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace TicTacToe
{
    class Communication
    {
        TcpClient tcpclnt;
        IPAddress ipAdd;
        Socket s;
        NetworkStream netstream;
        bool isListener = false;
        TcpListener listen;
        public void Connect(string data)
        {
            byte[] d = new byte[data.Length];
            d = Encoding.ASCII.GetBytes(data);
            IPHostEntry ip = Dns.GetHostEntry(Dns.GetHostName());
            ipAdd = ip.AddressList[1];
            tcpclnt = new TcpClient();
            while (!tcpclnt.Connected)
                tcpclnt.Connect("127.0.0.1", 4444);
            //listen.Stop();

            netstream = tcpclnt.GetStream();
            netstream.Write(d, 0, d.Length);
            MessageBox.Show("WROTE" + d.ToString());
            netstream.Close();
        }

        public void listener()
        {
            listen = new TcpListener(IPAddress.Parse("127.0.0.1"), 4444);
            listen.Start();
            s = listen.AcceptSocket();
            if (s != null)
                tcpclnt.Close();

            isListener = true;
            byte[] data = new byte[s.ReceiveBufferSize];
            s.Receive(data);
            string d = Encoding.ASCII.GetString(data);
            MessageBox.Show(d);
        }

    }
}
