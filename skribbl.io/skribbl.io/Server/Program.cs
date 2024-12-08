using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace server
{
    class Server
    {
        private TcpListener _listener;
        private readonly List<TcpClient> _clients = new List<TcpClient>();
        private readonly object _lock = new object(); // For thread safety
        string correct = "agazzi"; // Placeholder for the correct guess

        public static void Main(string[] args)
        {
            Server server = new Server();
            server.Start();
        }

        public void Start()
        {
            _listener = new TcpListener(IPAddress.Any, 12345);
            _listener.Start();
            Console.WriteLine("Server started...");

            while (true)
            {
                TcpClient client = _listener.AcceptTcpClient();
                lock (_lock)
                {
                    if (_clients.Count < 2) _clients.Add(client);
                    else Console.WriteLine("Too many clients!");
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
            stream.Write(roleData, 0, roleData.Length);  // Send role to client

            try
            {
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    messageBuffer.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));

                    // Split messages by newline
                    string[] messages = messageBuffer.ToString().Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                    // Clear message buffer and retain any incomplete message for the next round
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

                    // Process each complete message
                    foreach (string message in messages)
                    {
                        try
                        {
                            // Broadcast all received messages (drawing, guesses, etc.)
                            Broadcast(message + "\n", client);
                            if (IsWin(message)) Console.WriteLine("Hai vinto");
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
                }

                client.Close();
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
                    if (client != sender)  // Don't send back to the sender
                    {
                        try
                        {
                            NetworkStream stream = client.GetStream();
                            stream.Write(data, 0, data.Length);  // Send the message to the client
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Broadcast error: {ex.Message}");
                        }
                    }
                }
            }
        }

        private bool IsWin(string message)
        {
            var data = JsonConvert.DeserializeObject<dynamic>(message);
            if (data == null) return false;
            if (data.type == "guess" && data.word == correct) return true;
            return false;
        }
    }
}
