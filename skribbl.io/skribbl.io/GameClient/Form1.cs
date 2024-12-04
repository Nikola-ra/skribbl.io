using System;
using System.Drawing;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;

namespace GameClient
{
    public partial class Form1 : Form
    {
        private PictureBox canvas;
        private Label roleLabel;
        private Label roundLabel;
        private Label statusLabel;
        private Panel toolboxPanel;
        private Button colorPickerButton;
        private TrackBar penSizeSlider;
        private TextBox guessTextBox;
        private Button guessButton;
        private ListBox guessesList;
        private ListBox wordOptions;
        private TextBox playerNameTextBox;
        private TextBox serverIpTextBox;
        private Button connectButton; // To connect after entering name
        private Label nameLabel;


        private TcpClient _client;
        private NetworkStream _stream;
        private Thread _receiveThread;

        private bool _isDrawing = false;
        private Point _previousPoint;
        private Pen _pen = new Pen(Color.Black, 2);
        public Form1()
        {
            InitializeComponent();
            InitializeGame();
            ConnectToServer("127.0.0.1");
        }

        private void InitializeGame()
        {
            canvas.MouseDown += Canvas_MouseDown;
            canvas.MouseMove += Canvas_MouseMove;
            canvas.MouseUp += Canvas_MouseUp;
        }

        private void ConnectToServer(string serverIp)
        {
            try
            {
                _client = new TcpClient(serverIp, 12345);
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
            try
            {
                byte[] buffer = new byte[1024];
                int bytesRead;

                while ((bytesRead = _stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    HandleServerMessage(message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error receiving message: {ex.Message}");
            }
        }

        private void HandleServerMessage(string message)
        {
            // Handle messages (e.g., drawing updates, word choices, guesses)
            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(message);

            if (data.type == "draw")
            {
                DrawFromServer((int)data.x1, (int)data.y1, (int)data.x2, (int)data.y2, ColorTranslator.FromHtml((string)data.color), (float)data.size);
            }
            else if (data.type == "word")
            {
                // Show word options to the painter
                string[] words = data.options.ToObject<string[]>();
                Invoke(new Action(() => ShowWordOptions(words)));
            }
            else if (data.type == "guess")
            {
                // Display guesses (optional UI feature)
                Invoke(new Action(() => guessesList.Items.Add($"{data.player}: {data.word}")));
            }
        }

        private void ShowWordOptions(string[] words)
        {
            wordOptions.Items.Clear();
            wordOptions.Items.AddRange(words);
            wordOptions.Visible = true;
        }

        private void DrawFromServer(int x1, int y1, int x2, int y2, Color color, float size)
        {
            var pen = new Pen(color, size);
            using (var g = canvas.CreateGraphics())
            {
                g.DrawLine(pen, x1, y1, x2, y2);
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
                using (var g = canvas.CreateGraphics())
                {
                    g.DrawLine(_pen, _previousPoint, currentPoint);
                }

                // Send the drawing update to the server
                SendDrawingUpdate(_previousPoint, currentPoint, _pen.Color, _pen.Width);
                _previousPoint = currentPoint;
            }
        }

        private void Canvas_MouseUp(object sender, MouseEventArgs e)
        {
            _isDrawing = false;
        }

        private void SendDrawingUpdate(Point start, Point end, Color color, float size)
        {
            var message = new
            {
                type = "draw",
                x1 = start.X,
                y1 = start.Y,
                x2 = end.X,
                y2 = end.Y,
                color = ColorTranslator.ToHtml(color),
                size
            };

            SendMessage(Newtonsoft.Json.JsonConvert.SerializeObject(message));
        }

        private void SendMessage(string message)
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                _stream.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error sending message: {ex.Message}");
            }
        }

        private void GuessButton_Click(object sender, EventArgs e)
        {
            string guess = guessTextBox.Text;

            var message = new
            {
                type = "guess",
                player = playerNameTextBox.Text,
                word = guess
            };

            SendMessage(Newtonsoft.Json.JsonConvert.SerializeObject(message));
            guessTextBox.Clear();
        }

        private void WordOption_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (wordOptions.SelectedItem != null)
            {
                string selectedWord = wordOptions.SelectedItem.ToString();
                var message = new { type = "wordSelection", word = selectedWord };
                SendMessage(Newtonsoft.Json.JsonConvert.SerializeObject(message));
                wordOptions.Visible = false;
            }
        } 
    }
}
