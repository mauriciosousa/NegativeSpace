using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

using UnityEngine;

public class UDPListener
{
    private UdpClient _client;
    private int _port;

    public string stringToParse = null;

    public UDPListener(int port)
    {
        _port = port;
        _client = new UdpClient(new IPEndPoint(IPAddress.Any, port));

        try
        {
            _client.BeginReceive(new AsyncCallback(recv), null);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    public void recv(IAsyncResult res)
    {
        IPEndPoint ep = new IPEndPoint(IPAddress.Any, _port);
        byte[] received = _client.EndReceive(res, ref ep);

        stringToParse = Encoding.UTF8.GetString(received);
        
        _client.BeginReceive(new AsyncCallback(recv), null);

    }
}
