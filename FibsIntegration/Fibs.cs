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
            Console.WriteLine(Read("login:"));
            Send("kristianekman");
            Console.WriteLine(Read("\u0001"));

            var pw = File.ReadAllText("fibspw.txt");
            Send(pw);
            var reply = Read(">");
            Console.WriteLine(reply);
            if (!reply.Contains("User kristianekman authenticated."))
            {
                throw new ApplicationException("Failed to connect to fibs");
            }
            Send("set boardstyle 3");
            Console.WriteLine(Read(">"));
        }

        public void Disconnect()
        {
            Send("Bye");
            socket.Disconnect(false);
        }

        public void Send(string data)
        {
            Thread.Sleep(500);
            var sendBuffer = Encoding.ASCII.GetBytes(data + "\r\n");
            socket.Send(sendBuffer);
        }

        public string Read(string terminator)
        {
            Thread.Sleep(500);
            string text = "";
            do
            {
                var buffer = new byte[256];
                int bytesCount = socket.Receive(buffer, buffer.Length, SocketFlags.None);
                if (bytesCount == 0)
                    Thread.Sleep(1000);
                else
                    text += Encoding.ASCII.GetString(buffer, 0, bytesCount);
            }
            while (!text.TrimEnd().EndsWith(terminator));
            return text;
        }
    }
}
