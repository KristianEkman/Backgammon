using FibsIntegration;
using System;
using System.Diagnostics;
using System.Threading;

namespace TestConsole
{
    class Program
    {
        static Process AIinC;
        static FibsIntegration.Fibs Fibs;

        static void Main(string[] args)
        {
            // The purpose of this client is to connect to fibs and use my AI to compete.
            // First step is to semi automate a game.

            // Manualy join. etc44

            Console.WriteLine("Backgammon test console");
            StartAi();

            //var text = Console.ReadLine();
            //AIinC.StandardInput.WriteLine(text);

            Fibs = new FibsIntegration.Fibs();
            Fibs.Connect();

            while (true)
            {
                var crl = Console.ReadLine();
                if (crl.Equals("bye", StringComparison.OrdinalIgnoreCase))
                {
                    Fibs.Disconnect();
                    AIinC.StandardInput.WriteLine("q");

                    //AIinC.Close();
                    //AIinC.Kill();4
                    return;
                }

                if (crl.Equals("game", StringComparison.OrdinalIgnoreCase))
                {
                    AIinC.StandardInput.WriteLine("game");
                    continue;
                }

                Fibs.Send(crl);
                var reply = Fibs.Read(">");
                Console.WriteLine(reply);
                var lines = reply.Split("\n");
                foreach (var line in lines)
                {
                    var boardIndex = line.IndexOf("board:");
                    if (boardIndex > -1)
                    {
                        var board = Board.Parse(line.Substring(boardIndex));
                        if (board.YourColor == board.Turn)
                        {                            
                            var intString = board.ToInternal();
                            AIinC.StandardInput.WriteLine(intString);
                            AIinC.StandardInput.Flush();
                            Thread.Sleep(500);
                            AIinC.StandardInput.WriteLine("search");
                            AIinC.StandardInput.Flush();
                        }
                    }
                }
            }
        }

        private static void StartAi()
        {
            AIinC = new Process();
            AIinC.StartInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                FileName = "Backgammon.exe",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                StandardInputEncoding = System.Text.Encoding.ASCII,
            };
            AIinC.OutputDataReceived += AIinC_OutputDataReceived;
            AIinC.Start();
            AIinC.BeginOutputReadLine();
        }

        private static void AIinC_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine("AI-> " + e.Data);
        }
    }
}
