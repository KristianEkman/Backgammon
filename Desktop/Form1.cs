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

            this.Game.Reset();
            this.Game.RollDice();
            Start();
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
                richTextBox2.Clear();
                Engine2.Log += Engine2_Log;
                Engine2.Move += Engine2_Move;
                Engine2.Start();
            }
        }

        private void Engine1_Move(object sender, string message)
        {
            if (richTextBox1.InvokeRequired)
            {
                richTextBox1.Invoke(() => {
                    richTextBox1.AppendText(message + Environment.NewLine);
                });
            }
            // todo: parse move
            // make move on game
            // send game string to other engine
        }

        private void Engine1_Log(object sender, string message)
        {
            if (richTextBox1.InvokeRequired)
            {
                richTextBox1.Invoke(() => {
                    richTextBox1.AppendText(message + Environment.NewLine);
                });
            }
        }


        private void Engine2_Move(object sender, string message)
        {
            if (richTextBox2.InvokeRequired)
            {
                richTextBox2.Invoke(() => {
                    richTextBox2.AppendText(message + Environment.NewLine);
                });
            }

            // todo: parse move
            // make move on game
            // send game string to other engine
        }

        private void Engine2_Log(object sender, string message)
        {
            if (richTextBox2.InvokeRequired)
            {
                richTextBox2.Invoke(() => {
                    richTextBox2.AppendText(message + Environment.NewLine);
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
            Engine1.SearchBoard("board 0 b2 0 0 0 0 w5 0 w3 0 0 0 b5 w5 0 0 0 b3 0 b5 0 0 0 0 w2 0 0 0 b 5 6");
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void buttonSearch2_Click(object sender, EventArgs e)
        {
            Engine2.SearchBoard("board 0 b2 0 0 0 0 w5 0 w3 0 0 0 b5 w5 0 0 0 b3 0 b5 0 0 0 0 w2 0 0 0 b 5 6");

        }

        private void buttonPlay_Click(object sender, EventArgs e)
        {
            Game.RollDice();
            var s = Game.GameString();
            Engine1.SearchBoard(s);
        }
    }
}