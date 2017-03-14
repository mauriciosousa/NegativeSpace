using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

using UnityEngine;

public class HandheldMessage
{
    private bool _click;
    public bool Click { get { return _click; } }

    private Quaternion _rotation;
    public Quaternion Rotation { get { return _rotation; } }

    public HandheldMessage()
    {
        _click = false;
        _rotation = Quaternion.identity;
    }

    public void Update(string udpMessage)
    {
        string[] statements = udpMessage.Split('/');
        Vector3 eulerRotation = new Vector3();
        foreach (string s in statements)
        {
            string [] tokens = s.Split('=');
            if (tokens[0] == "click") _click = bool.Parse(tokens[1]);
            else if (tokens[0] == "r.x") eulerRotation.x = float.Parse(tokens[1].Replace(',', '.'));
            else if (tokens[0] == "r.y") eulerRotation.y = float.Parse(tokens[1].Replace(',', '.'));
            else if (tokens[0] == "r.z") eulerRotation.z = float.Parse(tokens[1].Replace(',', '.'));
        }
        _rotation = Quaternion.Euler(eulerRotation);
    }
}

public class UDPHandheldListener : MonoBehaviour
{
    private UdpClient _client;
    private int _port;

    public HandheldMessage Message;
    private string _decryptKey;

    private bool _receiving;
    public bool Receiving { get { return _receiving; } }

    public UDPHandheldListener(int port, string decryptKey)
    {
        _receiving = false;
        Message = new HandheldMessage();
        _decryptKey = decryptKey;
        _port = port;
        _client = new UdpClient(new IPEndPoint(IPAddress.Any, port));
        Debug.Log("[UDPHandheldListener] Listening at port " + port);

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

        _receiving = true;
        Message.Update(DataEncryptor.Decrypt(Encoding.UTF8.GetString(received), _decryptKey));

        _client.BeginReceive(new AsyncCallback(recv), null);

    }
}

