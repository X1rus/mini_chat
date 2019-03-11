using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace chat_app
{
    public partial class Form1 : Form
    {
        Socket sck;
        EndPoint epLocal, epRemote;
        byte[] buffer;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            //set up socket
            sck = new Socket(AddressFamily.InterNetwork,SocketType.Dgram, ProtocolType.Udp);
            sck.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress,true);

            //get user IP
            textLocalIp.Text = GetLocalIP();
            textRemoteIp.Text = GetLocalIP();

        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            //binding socket
            epLocal = new IPEndPoint(IPAddress.Parse(textLocalIp.Text), Convert.ToInt32(textLocalPort.Text));
            sck.Bind(epLocal);
            //connect to remote ip
            epRemote = new IPEndPoint(IPAddress.Parse(textRemoteIp.Text),Convert.ToInt32(textRemotePort.Text));
            sck.Connect(epRemote);
            //listening the specific port
            buffer = new byte[1500];
            sck.BeginReceiveFrom(buffer,0,buffer.Length,SocketFlags.None,ref epRemote,new AsyncCallback(MessageCallBack),buffer);

        }

        private void MessageCallBack(IAsyncResult aResult)
        {
            try
            {
                byte[] receivedData = new byte[1500];
                receivedData = (byte[])aResult.AsyncState;
                //converting byte[] to string
                ASCIIEncoding aEncoding = new ASCIIEncoding();
                string receivedMessage = aEncoding.GetString(receivedData);

                //adding this mesage into listbox
                listMessage.Items.Add("Friend: " + receivedMessage);

                buffer = new byte[1500];
                sck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            //convert string message to byte[]
            ASCIIEncoding aEncoding = new ASCIIEncoding();
            byte[] sendingMessage = new byte[1500];
            sendingMessage = aEncoding.GetBytes(textMassege.Text);

            //sending the encoded message
            sck.Send(sendingMessage);
            listMessage.Items.Add("Me: "+textMassege.Text);
            textMassege.Text = "";
        }

        private string GetLocalIP()
        {
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress  iP in host.AddressList)
            {
                if (iP.AddressFamily == AddressFamily.InterNetwork)
                    return iP.ToString();
            }
            {

            }
            return "127.0.0.1";
        }
    }
}
