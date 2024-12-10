using Newtonsoft.Json;
using System;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace GameClient
{
    public partial class Form1 : Form
    {
        ColorDialog colorDialog = new ColorDialog();
        private TcpClient _client;
        private NetworkStream _stream;
        private Thread _receiveThread;


        public string _winner = "";
        public string _playerName = "";
        public string _serverIp = "";
        public int readyCount = 0;

        WinForm winForm;

        private bool _isDrawing = false;
        private Point _previousPoint;
        private Pen _pen = new Pen(Color.Black, 5);

        private StringBuilder messageBuffer = new StringBuilder(); 

        public Form1()
        {
            CheckForIllegalCrossThreadCalls = false;
            _pen.StartCap = _pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
            InitializeComponent();
            InitializeGame();
            ConnectToServer();
            winForm = new WinForm(this);
        }

        private void InitializeGame()
        {
            canvas.MouseDown += Canvas_MouseDown;
            canvas.MouseMove += Canvas_MouseMove;
            canvas.MouseUp += Canvas_MouseUp;
            guessTextBox.KeyDown += GuessTextBox_KeyDown;
            colorPanel.BackColor = _pen.Color;
            PopulateWords();
            wordChoice.SelectedIndex = 0;
        }

        private void PopulateWords()
        {

            //Oltre a queste parole il disegnatore può scriverne una a suo piacimento, in caso non gli piacciano quelle proposte (non esistono blacklist! ;) )
            string[] words = {"castello", "aeroplano", "bicicletta", "dinosaura", "elefante", "tornado", "laboratorio", "arcobaleno", "candelabro", "oceano",
    "astronave", "piramide", "tempesta", "vulcano", "sottomarino", "grattacielo", "laboratorio", "ciclope", "cometa", "mummia",
    "trampolino", "autostrada", "trenino", "galassia", "carro armato", "ponte", "squalo", "scoiattolo", "sottomarino", "corallo",
    "ragnatela", "finestrino", "occhiale", "frullatore", "paracadute", "capitano", "martello", "ascensore", "pianoforte", "cappello",
    "cappuccino", "mozzarella", "barca a vela", "serpente", "oceano", "cooperativa", "maschera", "granchio", "torre", "mandarino",
    "giraffa", "giocattolo", "torretta", "acquario", "martin pescatore", "lavatrice", "sedile", "sciame", "zainetto", "guerriero"};

            Random rnd = new Random();
            for (int i = 0; i < 3; i++)
            {
                wordChoice.Items.Add(words[rnd.Next(words.Length)]);
            }
        }

        private void GuessTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) 
            {
                e.SuppressKeyPress = true; 
                guessButton.PerformClick(); 
            }
        }

        private void ConnectToServer()
        {
            _serverIp = Prompt("Enter the server IP:");
            _playerName = Prompt("Enter your player name:");
            playerNameLabel.Text = _playerName;

            try
            {
                _client = new TcpClient(_serverIp, 50000);
                _stream = _client.GetStream();

                _receiveThread = new Thread(ReceiveMessages);
                _receiveThread.IsBackground = true;
                _receiveThread.Start();

                MessageBox.Show("Connected to server!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error connecting to server: {ex.Message}");
            }
        }

        private void ReceiveMessages()
        {
            byte[] buffer = new byte[1024];
            while (true)
            {
                try
                {
                    int bytesRead = _stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    messageBuffer.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));

                    ProcessMessages();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error receiving message: {ex.Message}");
                    break;
                }
            }
        }

        private void ProcessMessages()
        {
            string[] messages = messageBuffer.ToString().Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            if (messageBuffer.ToString().EndsWith("\n"))
            {
                messageBuffer.Clear();
            }
            else
            {
                messageBuffer.Clear();
                messageBuffer.Append(messages.Last());
                messages = messages.Take(messages.Length - 1).ToArray();
            }

            foreach (string message in messages)
            {
                try
                {
                    HandleServerMessage(message);
                }
                catch (JsonReaderException)
                {
                    Console.WriteLine("Invalid JSON message received");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error processing message: {ex.Message}\nMessage: {message}");
                }
            }
        }

        private void HandleServerMessage(string message)
        {
            try
            {
                var data = JsonConvert.DeserializeObject<dynamic>(message);

                if (data.type == "role")
                {
                    
                    if (data.message == "Drawer")
                    {
                        SetDrawerUI();
                    }
                    else
                    {
                        SetGuesserUI();
                    }
                }

                if (data.type == "guess")
                {
                    guessesList.Items.Add($"{data.player} : {data.word}");
                }

                if (data.type == "draw")
                {
                    //Con questo riesco a gestire il disegno in modo asincrono, per non crashare a causa di connessioni lente o problemi di Json
                    DrawFromServer((int)data.x1, (int)data.y1, (int)data.x2, (int)data.y2,
                        ColorTranslator.FromHtml((string)data.color), (float)data.size);
                }

                if (data.type == "win")
                {
                    _winner = data.player;
                    HandleWin();
                }

                
                
               if (data.type == "restart")
                {
                    guessesList.Items.Clear();
                    
                    canvas.Invalidate();
                    this.Activate();
                    this.TopMost = false;
                    MessageBox.Show("New round started!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.TopMost = true;

                }
            }
            catch (JsonReaderException)
            {
                Console.WriteLine("Invalid JSON message received, skipping...");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing message: {ex.Message}\nMessage: {message}");
            }
        }

        private void HandleWin()
        {
            this.Hide();
            winForm.ShowDialog();
        }

        private void DrawFromServer(int x1, int y1, int x2, int y2, Color color, float size)
        {
            if (canvas.InvokeRequired)
            {
                canvas.Invoke(new Action(() => DrawFromServer(x1, y1, x2, y2, color, size)));
            }
            else
            {
                Pen serverPen = new Pen(color, size)
                {
                    StartCap = System.Drawing.Drawing2D.LineCap.Round,
                    EndCap = System.Drawing.Drawing2D.LineCap.Round
                };
                Graphics g = canvas.CreateGraphics();
                g.DrawLine(serverPen, x1, y1, x2, y2);
            }
        }

        private void Canvas_MouseDown(object sender, MouseEventArgs e)
        {
            _isDrawing = true;
            _previousPoint = e.Location;
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDrawing)
            {
                var currentPoint = e.Location;
                DrawLineOnCanvas(_previousPoint.X, _previousPoint.Y, currentPoint.X, currentPoint.Y);

                SendDrawingToServer(_previousPoint.X, _previousPoint.Y, currentPoint.X, currentPoint.Y, _pen.Color, _pen.Width);

                _previousPoint = currentPoint;
            }
        }

        private void Canvas_MouseUp(object sender, MouseEventArgs e)
        {
            _isDrawing = false;
        }

        private void DrawLineOnCanvas(int x1, int y1, int x2, int y2)
        {
            if (canvas.InvokeRequired)
            {
                canvas.Invoke(new Action(() => DrawLineOnCanvas(x1, y1, x2, y2)));
            }
            else
            {
                Graphics g = canvas.CreateGraphics();
                g.DrawLine(_pen, x1, y1, x2, y2);
            }
        }

        private void SendDrawingToServer(int x1, int y1, int x2, int y2, Color color, float size)
        {
            var message = JsonConvert.SerializeObject(new
            {
                type = "draw",
                x1,
                y1,
                x2,
                y2,
                color = ColorTranslator.ToHtml(color),
                size
            });

            byte[] data = Encoding.UTF8.GetBytes(message + "\n");
            _stream.Write(data, 0, data.Length);
        }

        private void SetGuesserUI()
        {
            canvas.Enabled = false;
            colorPanel.Visible = false;
            guessTextBox.Visible = true;
            guessButton.Visible = true;
            wordChoice.Visible = false;
            penSizeSlider.Visible = false;
            eraserButton.Visible = false;
            wordLabel.Visible = false;
            penWidthLabel.Visible = false;
            colorPickerButton.Visible = false;
            roleLabel.Text = "You are the Guesser!";
        }

        private void SetDrawerUI()
        {
            canvas.Enabled = false;
            colorPanel.Visible = true;
            guessTextBox.Visible = false;
            guessButton.Visible = false;
            wordChoice.Visible = true;
            penSizeSlider.Visible = true;
            wordLabel.Visible = true;
            eraserButton.Visible = true;
            penWidthLabel.Visible = true;
            colorPickerButton.Visible = true;
            roleLabel.Text = "You are the Drawer!";
        }

        public void SendMessage(string message)
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(message + "\n");
                _stream.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error sending message: {ex.Message}");
            }
        }

        private void guessButton_Click(object sender, EventArgs e)
        {
            string guess = guessTextBox.Text;

            var message = new
            {
                type = "guess",
                player = _playerName,
                word = guess
            };


            SendMessage(Newtonsoft.Json.JsonConvert.SerializeObject(message));
            guessTextBox.Clear();
        }

        private void eraserButton_Click(object sender, EventArgs e)
        {
            _pen.Color = Color.White;
            _pen.Width = 25;
        }
        private string Prompt(string message)
        {
            return Interaction.InputBox(message,"Inserisci:", "127.0.0.1");
        }

        private void penSizeSlider_Scroll(object sender, EventArgs e)
        {
            _pen.Width = penSizeSlider.Value;
        }

        private void colorPickerButton_Click(object sender, EventArgs e)
        {
            colorDialog.ShowDialog();
            _pen.Color = colorDialog.Color;
            colorPanel.BackColor = _pen.Color;
        }

        private void wordChoice_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var message = new
            {
                type = "correct",
                message = wordChoice.SelectedItem.ToString()
            };
            wordLabel.Text = $"The word is: {wordChoice.SelectedItem}";

            SendMessage(JsonConvert.SerializeObject(message));
            wordChoice.Visible = false;
            canvas.Enabled = true;
        }
    }
}
