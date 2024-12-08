using Newtonsoft.Json;
using System;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace GameClient
{
    public partial class Form1 : Form
    {
        ColorDialog colorDialog = new ColorDialog();
        private TcpClient _client;
        private NetworkStream _stream;
        private Thread _receiveThread;
        private string _playerName = "";
        private string _serverIp = "";

        private bool _isDrawing = false;
        private Point _previousPoint;
        private Pen _pen = new Pen(Color.Black, 5);

        private StringBuilder messageBuffer = new StringBuilder(); // Buffer for incoming messages

        public Form1()
        {
            InitializeComponent();
            _pen.StartCap = _pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
            InitializeGame();
            CheckForIllegalCrossThreadCalls = false;
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
            _serverIp = Prompt("Enter the server IP:");
            _playerName = Prompt("Enter your player name:");

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
            byte[] buffer = new byte[1024];
            while (true)
            {
                try
                {
                    int bytesRead = _stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    // Append received data to the buffer
                    messageBuffer.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));

                    // Process complete messages
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
            // Split the buffer into complete messages using the newline delimiter
            string[] messages = messageBuffer.ToString().Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            // If the buffer doesn't end with a newline, retain the incomplete message for next read
            if (messageBuffer.ToString().EndsWith("\n"))
            {
                messageBuffer.Clear();  // All data is processed if it ends with a newline.
            }
            else
            {
                messageBuffer.Clear();
                messageBuffer.Append(messages.Last()); // Retain the incomplete message for next round
                messages = messages.Take(messages.Length - 1).ToArray(); // Remove the last incomplete message
            }

            // Process each complete message
            foreach (string message in messages)
            {
                try
                {
                    HandleServerMessage(message);
                }
                catch (JsonReaderException)
                {
                    // Skip malformed JSON messages
                    Console.WriteLine("Invalid JSON message received, skipping...");
                }
                catch (Exception ex)
                {
                    // Log other types of errors
                    MessageBox.Show($"Error processing message: {ex.Message}\nMessage: {message}");
                }
            }
        }

        private void HandleServerMessage(string message)
        {
            try
            {
                var data = JsonConvert.DeserializeObject<dynamic>(message);

                // Ensure valid "role" type message
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

                // Handle drawing type message
                if (data.type == "draw")
                {
                    // Ensure we call DrawFromServer to render the line on the Guesser's canvas
                    DrawFromServer((int)data.x1, (int)data.y1, (int)data.x2, (int)data.y2,
                        ColorTranslator.FromHtml((string)data.color), (float)data.size);
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
                // Draw locally on the canvas
                DrawLineOnCanvas(_previousPoint.X, _previousPoint.Y, currentPoint.X, currentPoint.Y);

                // Send the drawing data to the server
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
            guessTextBox.Visible = true;
            guessButton.Visible = true;
            penSizeSlider.Visible = false;
            eraserButton.Visible = false;
            colorPickerButton.Visible = false;
            roleLabel.Text = "You are the Guesser!";
        }

        private void SetDrawerUI()
        {
            canvas.Enabled = true;
            guessTextBox.Visible = false;
            guessButton.Visible = false;
            penSizeSlider.Visible = true;
            eraserButton.Visible = true;
            colorPickerButton.Visible = true;
            roleLabel.Text = "You are the Drawer!";
        }

        private void SendMessage(string message)
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

        // Helper method to prompt for input
        private string Prompt(string message)
        {
            return Interaction.InputBox(message);
        }

        private void penSizeSlider_Scroll(object sender, EventArgs e)
        {
            _pen.Width = penSizeSlider.Value;
        }

        private void colorPickerButton_Click(object sender, EventArgs e)
        {
            colorDialog.ShowDialog();
            _pen.Color = colorDialog.Color;
        }
    }
}
