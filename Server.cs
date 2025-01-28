using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

class Message
{
    public string type = "";
    public string message = "";
    public string sender = "";
}

class Connection
{
    public string ip = "";
    public int port;
}

class Server
{
    IPAddress IP;
    IPEndPoint EP;
    Socket S;

    List<Socket> Clients;
    List<Task> Tasks;

    List<string> ids;

    public async Task Start(string ip, int port)
    {
        IP = IPAddress.Parse(ip);
        EP = new IPEndPoint(IP, port);
        S = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

        Clients = new List<Socket>();
        Tasks = new List<Task>();
        ids = new List<string>();

        S.Bind(EP);
        S.Listen(10);

        Task ping = Task.Run(Ping);

        Console.WriteLine($"{DateTime.Now.ToString()}: Server Started\n");

        try
        {
            while (true)
            {
                Socket ns = S.Accept();

                string id = ns.RemoteEndPoint.ToString();

                Console.WriteLine($"{id} joined at {DateTime.Now}\ntotal clients count: {Clients.Count + 1}\n");
                ids.Add(id);

                await ns.SendAsync(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(new Message { type = "ids", message = JsonConvert.SerializeObject(ids) })));

                Clients.Add(ns);
                Tasks.Add(Task.Run(async () => await HandleClient(ns)));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    async Task HandleClient(Socket cs)
    {
        while (true)
        {
            try
            {
                byte[] buffer = new byte[1024];
                int receivedBytesCount = await cs.ReceiveAsync(buffer);
                string str = Encoding.ASCII.GetString(buffer, 0, receivedBytesCount);
                Message msg = JsonConvert.DeserializeObject<Message>(str);
                msg.sender = cs.RemoteEndPoint.ToString();
                string json = JsonConvert.SerializeObject(msg);
                byte[] bytes = Encoding.ASCII.GetBytes(json);

                for (int i = 0; i < Clients.Count; i++)
                {
                    if (Clients[i] == cs) continue;

                    try
                    {
                        await Clients[i].SendAsync(bytes);
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            catch
            {
                continue;
            }
        }
    }

    async Task Ping()
    {
        Message msg = new Message { type = "ping" };
        string json = JsonConvert.SerializeObject(msg);
        byte[] bytes = Encoding.ASCII.GetBytes(json);

        while (true)
        {
            try
            {
                for (int i = 0; i < Clients.Count; i++)
                {
                    try
                    {
                        await Clients[i].SendAsync(bytes);
                    }
                    catch
                    {
                        Message left = new Message { type = "playerleft", sender = ids[i] };
                        string js = JsonConvert.SerializeObject(left);
                        byte[] by = Encoding.ASCII.GetBytes(js);

                        Clients.RemoveAt(i);
                        Tasks.RemoveAt(i);
                        ids.RemoveAt(i);
                        i--;

                        for (int j = 0; j < Clients.Count; j++)
                        {
                            try
                            {
                                await Clients[j].SendAsync(by);
                            }
                            catch
                            {
                                continue;
                            }
                        }

                        Console.WriteLine($"{Clients[i].RemoteEndPoint} left at {DateTime.Now}\ntotal clients count: {Clients.Count}\n");
                    }
                }
            }
            catch
            {
                continue;
            }

            Task.Delay(1000).Wait();
        }
    }
}

class Program
{
    static async Task Main(string[] args)
    {
        string str = File.ReadAllText("connection.json");
        Connection c = JsonConvert.DeserializeObject<Connection>(str);

        Server s = new Server();
        await s.Start(c.ip, c.port);

        while (true) { }
    }
}