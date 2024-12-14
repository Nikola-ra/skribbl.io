using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Emit;
using System.Text;
using System.Threading;

namespace server
{
    class Server
    {
        private TcpListener _listener;
        private readonly List<TcpClient> _clients = new List<TcpClient>();
        private readonly object _lock = new object(); 
        string correct;

        //Dizionario che mi serve per capire se tutti hanno messo pronto dopo una vittoria
        private readonly Dictionary<TcpClient, bool> _readyStatus = new Dictionary<TcpClient, bool>();

        string _winner;

        public static void Main(string[] args)
        {
            Server server = new Server();
            server.Start();
        }

        public void Start()
        {
            _listener = new TcpListener(IPAddress.Any, 50000);
            _listener.Start();
            Console.WriteLine("Server started...");

            while (true)
            {
                TcpClient client = _listener.AcceptTcpClient();
                lock (_lock)
                {
                    _clients.Add(client);
                    _readyStatus[client] = false;
                }

                Console.WriteLine("Client connected...");
                Thread clientThread = new Thread(HandleClient);
                clientThread.Start(client);
            }
        }

        private void HandleClient(object obj)
        {
            TcpClient client = (TcpClient)obj;
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead;
            StringBuilder messageBuffer = new StringBuilder();

            string role;
            role = _clients.Count == 1 ? "Drawer" : "Guesser";
            string roleMessage = JsonConvert.SerializeObject(new { type = "role", message = role });
            byte[] roleData = Encoding.UTF8.GetBytes(roleMessage + "\n");
            stream.Write(roleData, 0, roleData.Length); 

            try
            {
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    messageBuffer.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));

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
                            Console.WriteLine($"Recieved: {message}");
                            Broadcast(message + "\n", client);
                            ManageMessage(message, client);

                            if (IsWin(message)) {

                                var winMessage = JsonConvert.SerializeObject(new
                                {
                                    type = "win",
                                    player = _winner
                                });
                                Console.WriteLine("Hai vinto");
                                BroadcastAll(winMessage + "\n");
                            };
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error processing message: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                lock (_lock)
                {
                    _clients.Remove(client);
                    client.Close();
                    if (_readyStatus.Values.All(status => status))
                    {
                        StartNewGame();
                    }
                }

                Console.WriteLine("Client disconnected...");
            }
        }

        private void Broadcast(string message, TcpClient sender)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);

            lock (_lock)
            {
                foreach (var client in _clients)
                {
                    if (client != sender) 
                    {
                        try
                        {
                            NetworkStream stream = client.GetStream();
                            stream.Write(data, 0, data.Length); 
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Broadcast error: {ex.Message}");
                        }
                    }
                }
            }
        }

        private void BroadcastAll(string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);

            lock (_lock)
            {
                foreach (var client in _clients)
                {
                    try
                    {
                        NetworkStream stream = client.GetStream();
                        stream.Write(data, 0, data.Length);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Broadcast error: {ex.Message}");
                    }
                }
            }
        }

        private void SendMessageToClient(string message, TcpClient client)
        {
            try
            {
                NetworkStream stream = client.GetStream();
                byte[] data = Encoding.UTF8.GetBytes(message + "\n");
                stream.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message: {ex.Message}");
            }
        }

        private bool IsWin(string message)
        {
            var data = JsonConvert.DeserializeObject<dynamic>(message);
            if (data.type == "guess" && data.word == correct) {
                _winner = data.player;
                return true;
            }
            return false;
        }
        private void ManageMessage(string message, TcpClient client)
        {
            var data = JsonConvert.DeserializeObject<dynamic>(message);
            if (data.type == "correct") correct = data.message;
            if (data.type == "guess") SendMessageToClient(message, client);

            if (data.type == "ready")
            {
                lock (_lock)
                {
                    _readyStatus[client] = true;
                    if (_readyStatus.Values.All(status => status))
                    {
                        StartNewGame();
                    }
                }
            }
        }

        private void StartNewGame()
        {
            lock (_lock)
            {
                foreach (var key in _readyStatus.Keys.ToList())
                {
                    _readyStatus[key] = false;
                }

                var newDrawerIndex = new Random().Next(_clients.Count);
                for (int i = 0; i < _clients.Count; i++)
                {
                    string role = i == newDrawerIndex ? "Drawer" : "Guesser";
                    var roleMessage = JsonConvert.SerializeObject(new { type = "role", message = role });
                    SendMessageToClient(roleMessage, _clients[i]);
                }

                correct = null;

                BroadcastAll(JsonConvert.SerializeObject(new { type = "restart" }) + "\n");
            }
        }
    }
}
