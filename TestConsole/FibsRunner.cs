using FibsIntegration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestConsole
{
    // The purpose of this client is to connect to fibs and use my AI to compete.
    // First step is to semi automate a game.
    static class FibsRunner
    {
        static Process AIinC;
        static FibsIntegration.Fibs Fibs;
        public static bool FibsConnected { get; set; }

        public static void RunFibs()
        {
            StartAi();
            Fibs = new FibsIntegration.Fibs();
            Fibs.Connect();
            FibsConnected = true;
            var thread = new Thread(ReadJob);
            thread.Start();
            ConsoleReadLoop();
            thread.Interrupt();
        }

        // Read console and sends the commands to Fibs or AI
        private static void ConsoleReadLoop()
        {
            while (true)
            {

                var line = Console.ReadLine();
                if (line.Equals("bye", StringComparison.OrdinalIgnoreCase))
                {
                    Fibs.Disconnect();
                    FibsConnected = false;
                    AIinC.StandardInput.WriteLine("q");
                    AIinC.Kill();
                    return;
                }

                // AI commands
                if (line.Equals("game", StringComparison.OrdinalIgnoreCase))
                {
                    AIinC.StandardInput.WriteLine("game");
                    continue;
                }

                // todo: invite/join
                Fibs.Send(line);
            }
        }

        // Reads Fibs endpoint
        // When a board is found calls AI to calculate best move.
        private static void ReadJob()
        {
            try
            {
                while (FibsConnected)
                {
                    var reply = Fibs.Read(">");
                    Console.WriteLine(reply);
                    var rows = reply.Split("\n");
                    foreach (var row in rows)
                    {
                        var boardIndex = row.IndexOf("board:");
                        if (boardIndex > -1)
                        {
                            var board = Board.Parse(row.Substring(boardIndex));
                            if (board.Turn == Players.Player && board.PlayerDice[0] > 0 && board.PlayerDice[1] > 0 && board.CanMove > 0)
                            {
                                var intString = board.ToInternal();
                                AIinC.StandardInput.WriteLine(intString);
                                Thread.Sleep(500);
                                AIinC.StandardInput.WriteLine("search");
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
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
            // todo: send the move automatically
            //if (e.Data.StartsWith("move "))
            //{
            //    Fibs.Send(e.Data);
            //}
        }
    }
}
