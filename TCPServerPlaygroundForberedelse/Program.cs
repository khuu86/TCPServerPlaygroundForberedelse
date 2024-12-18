// See https://aka.ms/new-console-template for more information
using System.Net.Sockets;
using System.Net;
using System.Text.Json;

Console.WriteLine("TCP Server Playground");

TcpListener listener = new TcpListener(IPAddress.Any, 7531);

listener.Start();
while (true)
{
    TcpClient socket = listener.AcceptTcpClient();
    IPEndPoint clientEndPoint = socket.Client.RemoteEndPoint as IPEndPoint;
    Console.WriteLine("Client connected: " + clientEndPoint.Address);

    Task.Run(() => HandleClient(socket));
}

// listener.Stop();

// Method to handle client connections
void HandleClient(TcpClient socket)
{
    NetworkStream ns = socket.GetStream();
    StreamReader reader = new StreamReader(ns);
    StreamWriter writer = new StreamWriter(ns);

    // Liste med legepladser
    var playGrounds = new List<PlayGround>
    {
        new PlayGround(1, "MillPark", 10, 5),
        new PlayGround(2, "Secret Playground", 12, 4),
        new PlayGround(3, "Library", 8, 3),
        new PlayGround(4, "School", 15, 7)
    };

    while (socket.Connected)
    {
        string? message = reader.ReadLine()?.Trim(); // Modtager alder fra klient

        if (int.TryParse(message, out int age))
        {
            // Filtrér legepladser baseret på alder
            var filteredPlayGrounds = playGrounds
                .Where(pg => pg.MinChildAge <= age)
                .ToList();

            if (filteredPlayGrounds.Any())
            {
                // Serialiser listen til JSON og send som svar
                string jsonResponse = JsonSerializer.Serialize(filteredPlayGrounds);
                writer.WriteLine(jsonResponse);
            }
            else
            {
                writer.WriteLine("Ingen legepladser fundet for den angivne alder.");
            }
        }
        else
        {
            writer.WriteLine("Ugyldigt input. Send venligst en gyldig alder.");
        }

        writer.Flush(); // Sikrer at beskeden sendes
    }
}

// PlayGround class
public class PlayGround
{
    public int ID { get; set; }
    public string Name { get; set; }
    public int MaxChildren { get; set; }
    public int MinChildAge { get; set; }

    public PlayGround(int id, string name, int maxChildren, int minChildAge)
    {
        ID = id;
        Name = name;
        MaxChildren = maxChildren;
        MinChildAge = minChildAge;
    }
}
