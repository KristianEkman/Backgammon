using AiLoader;
using Backend.Rules;

namespace Desktop
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            ExeEngine.KillAll();
            InitEngines();
            Start();
        }

        ~Form1()
        {
            ExeEngine.KillAll();
        }

        private Settings Settings { get; set; }
        private ExeEngine Engine1 { get; set; }
        private ExeEngine Engine2 { get; set; }

        private readonly Game Game = Game.Create(false);

        private void InitEngines()
        {
            this.Settings = Settings.Load();
            openFileDialog1.FileName = Settings.Engine1;
            openFileDialog2.FileName = Settings.Engine2;
            labelEngine1.Text = openFileDialog1.FileName;
            labelEngine2.Text = openFileDialog2.FileName;
        }

        private void Start()
        {
            if (File.Exists(Settings.Engine1))
            {
                Engine1 = new ExeEngine(Settings.Engine1);
                richTextBox1.Clear();
                Engine1.Log += Engine1_Log;
                Engine1.Move += Engine1_Move;
                Engine1.Start();
            }

            if (File.Exists(Settings.Engine2))
            {
                Engine2 = new ExeEngine(Settings.Engine1);
                Engine2.Log += Engine2_Log;
                Engine2.Move += Engine2_Move;
                Engine2.Start();
            }
        }

        private void Engine1_Move(object sender, string move)
        {
            if (richTextBox1.InvokeRequired)
            {
                richTextBox1.Invoke(() =>
                {
                    richTextBox1.AppendText("1. " + move + Environment.NewLine);
                    if (DoAndSwitch(move))
                    {
                        var gs = Game.GameString();
                        richTextBox1.AppendText("1. " + gs + Environment.NewLine);
                        Engine2.SearchBoard(gs);
                    }
                });
            }
        }

        private void Engine1_Log(object sender, string message)
        {
            if (message.StartsWith("move"))
                return;
            if (richTextBox1.InvokeRequired)
            {
                richTextBox1.Invoke(() =>
                {
                    richTextBox1.AppendText("1. " + message + Environment.NewLine);
                });
            }
        }


        private void Engine2_Move(object sender, string move)
        {
            if (richTextBox1.InvokeRequired)
            {
                richTextBox1.Invoke(() =>
                {
                    richTextBox1.AppendText("2. " + move + Environment.NewLine);
                    if (DoAndSwitch(move))
                    {
                        var gs = Game.GameString();
                        richTextBox1.AppendText("2. " + gs + Environment.NewLine);
                        Engine1.SearchBoard(gs);
                    }
                });
            }
        }

        private bool DoAndSwitch(string message)
        {
            if (message.StartsWith("move"))
            {
                var moves = message.Substring(5).Split(' ');
                if (!message.StartsWith("move none"))
                {
                    foreach (var move in moves)
                    {
                        var temp = move.Split('-');
                        var bar = Game.CurrentPlayer == Player.Color.Black ? 0 : 25;
                        var off = Game.CurrentPlayer == Player.Color.Black ? 25 : 0;
                        var from = temp[0] == "bar" ? bar : int.Parse(temp[0]);
                        var to = temp[1] == "off" ? off : int.Parse(temp[1]);
                        Game.MakeMove(new Move
                        {
                            Color = Game.CurrentPlayer,
                            From = Game.Points[from],
                            To = Game.Points[to]
                        });
                    }
                }

                if (Game.WhitePlayer.PointsLeft == 0)
                {
                    richTextBox1.AppendText("White won");
                    return false;
                }

                if (Game.BlackPlayer.PointsLeft == 0)
                {
                    richTextBox1.AppendText("Black won");
                    return false;
                }

                Game.SwitchPlayer();
                Game.RollDice();
            }
            return true;
        }

        private void Engine2_Log(object sender, string message)
        {
            if (message.StartsWith("move"))
                return;
            if (richTextBox1.InvokeRequired)
            {
                richTextBox1.Invoke(() =>
                {
                    richTextBox1.AppendText("2. " + message + Environment.NewLine);
                });
            }
        }

        private void buttonLoad1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                labelEngine1.Text = openFileDialog1.FileName;
                Settings.Engine1 = openFileDialog1.FileName;
                Settings.Save();
            }
        }

        private void buttonLoad2_Click(object sender, EventArgs e)
        {
            if (openFileDialog2.ShowDialog(this) == DialogResult.OK)
            {
                labelEngine2.Text = openFileDialog2.FileName;
                Settings.Engine2 = openFileDialog2.FileName;
                Settings.Save();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Engine1?.Close();
            Engine2?.Close();
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            Engine1.SearchBoard("board b1 w3 w4 w1 0 w2 w2 0 w1 0 0 b1 0 0 0 0 0 b2 0 b2 b3 b4 0 w2 b2 w0 0 0 b 5 2");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ExeEngine.KillAll();
        }

        private void buttonSearch2_Click(object sender, EventArgs e)
        {
            Engine2.SearchBoard("board 0 b2 0 0 0 0 w5 0 w3 0 0 0 b5 w5 0 0 0 b3 0 b5 0 0 0 0 w2 0 0 0 b 5 6");

        }

        private void buttonPlay_Click(object sender, EventArgs e)
        {
            Game.Reset();
            Game.RollDice();
            richTextBox1.Clear();
            var s = Game.GameString();
            richTextBox1.AppendText(s + Environment.NewLine);

            if (Game.CurrentPlayer == Player.Color.Black)
                Engine1.SearchBoard(s);
            else         
                Engine2.SearchBoard(s);
        }
    }
}