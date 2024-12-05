using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        string correct = "agazzi";


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

            try
            {
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    if (IsWin(message)) Console.WriteLine("Hai vinto");
                    Console.WriteLine($"Received: {message}");
                    Broadcast(message+"\n", client);
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

        private bool IsWin(string message)
        {
            var data = JsonConvert.DeserializeObject<dynamic>(message);
            if (data == null) return false;
            if (data.type == "guess" && data.word == correct) return true;
            return false;
        }
    }
}
