using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using OpenTK.Mathematics;

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

class TransformTransferObject
{
    public float[] t = new float[9];

    public TransformTransferObject() { }

    public TransformTransferObject(Vector3 Position, Vector3 Scale, Vector3 Rotation)
    {
        SetTransform(Position, Scale, Rotation);
    }

    public void SetTransform(Vector3 Position, Vector3 Scale, Vector3 Rotation)
    {
        t[0] = Position.X;
        t[1] = Position.Y;
        t[2] = Position.Z;
        t[3] = Scale.X;
        t[4] = Scale.Y;
        t[5] = Scale.Z;
        t[6] = Rotation.X;
        t[7] = Rotation.Y;
        t[8] = Rotation.Z;
    }

    public (Vector3, Vector3, Vector3) GetTransform()
    {
        return (new Vector3(t[0], t[1], t[2]), new Vector3(t[3], t[4], t[5]), new Vector3(t[6], t[7], t[8]));
    }
}

class UpdatePackage
{
    public TransformTransferObject player = new TransformTransferObject();
    public List<TransformTransferObject> newTraces = new List<TransformTransferObject>();
    public List<TransformTransferObject> newTrails = new List<TransformTransferObject>();
}

public static class Client
{
    static IPAddress IP;
    static IPEndPoint EP;
    static Socket S;

    static Vector3 PlayerPosition = new Vector3();
    static Vector3 PlayerScale = new Vector3();
    static Vector3 PlayerRotation = new Vector3();
    static string PlayerID = "";

    public static List<Player> Players = new List<Player>();
    public static List<Cube> bulletTraces = new List<Cube>();
    public static List<Cube> bulletTrails = new List<Cube>();
    public static List<Cube> newTraces = new List<Cube>();
    public static List<Cube> newTrails = new List<Cube>();

    static bool ReceivedMyID = false;
    static bool ReceivedOtherIDs = false;

    public static void SetPlayerTransform(Vector3 Position, Vector3 Scale, Vector3 Rotation)
    {
        PlayerPosition = Position;
        PlayerScale = Scale;
        PlayerRotation = Rotation;
    }

    public static async Task ConnectToServer()
    {
        string str = File.ReadAllText("connection.json");
        Connection c = JsonConvert.DeserializeObject<Connection>(str);

        IP = IPAddress.Parse(c.ip);
        EP = new IPEndPoint(IP, c.port);
        S = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

        try
        {
            S.Connect(EP);

            if (S.Connected)
            {
                Console.WriteLine($"{DateTime.Now.ToString()}: Connected");

                Task send = Task.Run(SendData);
                Task receive = Task.Run(ReceiveData);
            }
            else
            {
                Console.WriteLine("Error");
            }
        }
        catch (SocketException ex)
        {
            Console.WriteLine(ex.Message);
        }

        while (true) { }
    }

    private static async Task SendData()
    {
        while (ReceivedMyID == false || ReceivedOtherIDs == false) { }

        Console.WriteLine("start sending");

        await S.SendAsync(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(new Message { type = "newplayer" })));

        while (true)
        {
            UpdatePackage u = new UpdatePackage();
            u.player.SetTransform(PlayerPosition, PlayerScale, PlayerRotation);

            for (int i = 0; i < newTraces.Count; i++)
            {
                u.newTraces.Add(new TransformTransferObject(newTraces[i].Position, newTraces[i].Scale, newTraces[i].Rotation));
            }

            for (int i = 0; i < newTrails.Count; i++)
            {
                u.newTrails.Add(new TransformTransferObject(newTrails[i].Position, newTrails[i].Scale, newTrails[i].Rotation));
            }

            string str = JsonConvert.SerializeObject(u);
            Message msg = new Message { type = "update", message = str };
            string json = JsonConvert.SerializeObject(msg);
            byte[] bytes = Encoding.ASCII.GetBytes(json);

            await S.SendAsync(bytes);

            newTraces.Clear();
            newTrails.Clear();

            Task.Delay(20).Wait();
        }
    }

    private static async Task ReceiveData()
    {
        while (true)
        {
            try
            {
                byte[] buffer = new byte[1024];
                int receivedBytesCount = await S.ReceiveAsync(buffer);
                string str = Encoding.ASCII.GetString(buffer, 0, receivedBytesCount);
                Message msg = JsonConvert.DeserializeObject<Message>(str);

                HandleMessage(msg);
            }
            catch
            {
                continue;
            }
        }
    }

    static void HandleMessage(Message msg)
    {
        switch (msg.type)
        {
            case "update":

                UpdatePackage u = JsonConvert.DeserializeObject<UpdatePackage>(msg.message);
                Player p = Players.Find(x => x.ID == msg.sender);

                (p.Position, p.Scale, p.Rotation) = u.player.GetTransform();

                for (int i = 0; i < u.newTraces.Count; i++)
                {
                    Cube trace = new Cube();
                    (trace.Position, trace.Scale, trace.Rotation) = u.newTraces[i].GetTransform();
                    bulletTraces.Add(trace);
                }

                for (int i = 0; i < u.newTrails.Count; i++)
                {
                    Cube trail = new Cube();
                    (trail.Position, trail.Scale, trail.Rotation) = u.newTrails[i].GetTransform();
                    bulletTraces.Add(trail);
                }

                break;

            case "ping":

                Console.WriteLine("ping\n");

                break;

            case "newplayer":

                Players.Add(new Player { ID = msg.sender });

                Console.WriteLine($"new player: {msg.sender}");

                break;

            case "playerleft":

                Players.Remove(Players.Find(x => x.ID == msg.sender));

                Console.WriteLine($"player left: {msg.sender}");

                break;

            case "ids":

                List<string> ids = JsonConvert.DeserializeObject<List<string>>(msg.message);

                Console.WriteLine("there are players online: ");

                for (int i = 0; i < ids.Count - 1; i++)
                {
                    Players.Add(new Player { ID = ids[i] });
                    Console.WriteLine(ids[i]);
                }

                PlayerID = ids[ids.Count - 1];

                Console.WriteLine($"my id: {PlayerID}");

                ReceivedMyID = true;
                ReceivedOtherIDs = true;

                break;
        }
    }
}