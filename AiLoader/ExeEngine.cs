using System.Diagnostics;
using System.Text;

namespace AiLoader;
public class ExeEngine
{
    public ExeEngine(string path)
    {
        Path = path;
    }

    public delegate void EventHandler(object sender, string message);
    public event EventHandler Log;
    public event EventHandler Move;

    public Process Process { get; set; }
    public string Path { get; }

    public void Start()
    {
        Process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                FileName = Path,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                StandardInputEncoding = Encoding.ASCII,
                RedirectStandardError = true,
            }
        };
        Process.OutputDataReceived += Process_OutputDataReceived;
        Process.ErrorDataReceived += Process_ErrorDataReceived;

        Process.Start();
        Process.BeginOutputReadLine();
        Process.BeginErrorReadLine();
    }

    public void Close()
    {
        Process.StandardInput.WriteLine("q");

    }

    public static void KillAll()
    {
        var siblings = Process.GetProcessesByName("Backgammon");
        foreach (var sibling in siblings)
        {
            sibling.Kill();
        }
    }

    public void SearchBoard(string internalboard)
    {
        Process.StandardInput.WriteLine(internalboard);        
    }

    private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        var message = e.Data ?? "";
        if (string.IsNullOrEmpty(message)) return;

        if (message.StartsWith("move"))
            Move.Invoke(this, message);
        else
            Log?.Invoke(this, message);
    }

    private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
    {
        if (!string.IsNullOrEmpty(e.Data))
            throw new ApplicationException(e.Data);
    }
}
