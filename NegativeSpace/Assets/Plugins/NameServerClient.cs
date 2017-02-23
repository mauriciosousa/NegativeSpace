using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class NameServerHost
{
    private string _id;
    public string ID { get { return _id; } }

    private string _address;
    public string Address { get { return _address; } }

    private int _port;
    public int Port { get { return _port; } }

    public NameServerHost(string id, string address, int port)
    {
        _id = id; _address = address; _port = port;
    }

    public override string ToString()
    {
        return "HostName: {" + ID + ", " + Address + "," + Port + "}";
    }
}

public class NameServerClient
{
    private int BUFFER = 1024;

    private bool _connected;
    public bool Connected { get { return _connected; } }

    private TcpClient _client;
    private Stream _stream;

    private string _address;
    private int _port;

    private ASCIIEncoding _encoder;

    public NameServerClient()
    {
        _connected = false;
    }

    public void connect(string address, int port)
    {
        _address = address;
        _port = port;

        _encoder = new ASCIIEncoding();

        _client = new TcpClient();
        try
        {
            _client.Connect(address, port);

            _stream = _client.GetStream();

            _connected = true;
            
        }
        catch (Exception e)
        {
            _connected = false;
            Console.WriteLine("Unable to connect");
        }
    }

    public NameServerHost request(string id)
    {
        byte[] idB = _encoder.GetBytes("get/" + id + "/");
        if (Connected)
        {
            try
            {
                _stream.Write(idB, 0, idB.Length);

                byte[] ret = new byte[BUFFER];
                int bytesRead = _stream.Read(ret, 0, ret.Length);
                if (bytesRead > 0) return _parse(_encoder.GetString(ret));
            }
            catch
            {
                close();
                _connected = false;
            }
        }
        return null;
    }

    private NameServerHost _parse(string v)
    {
        if (v != "none")
        {
            string[] s = v.Split('/');
            if (s.Length == 4)
            {
                int port;
                if (int.TryParse(s[2], out port))
                {
                    return new NameServerHost(s[0], s[1], port);
                }
            }
        }

        return null;
    }

    public void close()
    {
        _client.Close();
    }

}
