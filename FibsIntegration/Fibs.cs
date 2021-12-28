using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace FibsIntegration
{
    public class Fibs
    {
        Socket socket;
        public void Connect()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect("fibs.com", 4321);
            Read();
            Send("kristianekman");
            Read();

            var pw = File.ReadAllText("fibspw.txt");            
            Send(pw);
            var reply = Read();
            if (!reply.Contains("User kristianekman authenticated."))
            {
                throw new ApplicationException("Failed to connect to fibs");
            }
        }

        public void Disconnect()
        {
            Send("Bye");
            socket.Disconnect(false);
        }

        private void Send(string data)
        {
            Thread.Sleep(2000);
            var sendBuffer = Encoding.ASCII.GetBytes(data + "\r\n");
            socket.Send(sendBuffer);

        }

        private string Read()
        {
            Thread.Sleep(2000);
            var readBuffer = new byte[1000];
            var length = socket.Receive(readBuffer);
            var text = Encoding.ASCII.GetString(readBuffer.Take(length).ToArray());
            Debug.WriteLine(text);
            return text;
        }

        //todo: ai:t ska kunna spela en match mot bot som man väljer.
        //


    }
}
