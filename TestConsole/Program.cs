using System;
using System.Diagnostics;

namespace TestConsole
{
    class Program
    {
        static Process AIinC;
        static void Main(string[] args)
        {
            Console.WriteLine("Backgammon test console");
            StartAi();

            var text = Console.ReadLine();
            AIinC.StandardInput.WriteLine(text);
            
            Console.ReadLine();
            AIinC.Kill();
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
                StandardInputEncoding = System.Text.Encoding.ASCII
            };
            AIinC.OutputDataReceived += AIinC_OutputDataReceived;
            AIinC.Start();
            AIinC.BeginOutputReadLine();
        }
        
        private static void AIinC_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine(e.Data);
        }
    }
}
