using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

class Server
{
    TcpListener server = null;
    List<TcpClient>  clients = new List<TcpClient>();
    public Server(string ip, int port)
    {
        IPAddress localAddr = IPAddress.Parse(ip);
        server = new TcpListener(localAddr, port);
        server.Start();
        StartListener();
    }

    public void StartListener()
    {
        try
        {
            while (true)
            {
                Console.WriteLine("Waiting for a connection...");
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Connected!");

                Thread t = new Thread(new ParameterizedThreadStart(HandleDeivce));
                t.Start(client);
                clients.Add(client);
            }
        }
        catch (SocketException e)
        {
            Console.WriteLine("SocketException: {0}", e);
            server.Stop();
        }
    }

    public void HandleDeivce(Object obj)
    {
        TcpClient client = (TcpClient)obj;
        var stream = client.GetStream();
        string imei = String.Empty;

        string data = null;
        Byte[] bytes = new Byte[256];
        Nullable<int> i;
        try
        {
            var msg = "";
            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                string hex = BitConverter.ToString(bytes);
                data = Encoding.ASCII.GetString(bytes, 0, i.Value);
                msg = string.Concat(msg, data);

                if(data == ";")
                {
                    Console.WriteLine($"msg {msg}");
                    Byte[] reply = System.Text.Encoding.ASCII.GetBytes(msg);
                    clients.ForEach(cl => {
                        Task.Run(async () => await cl.GetStream().WriteAsync(reply, 0, reply.Length));
                    });

                    i = null;
                }
            }            
        }
        catch(Exception e)
        {
            Console.WriteLine("Exception: {0}", e.ToString());
            client.Close();
        }
    }
}