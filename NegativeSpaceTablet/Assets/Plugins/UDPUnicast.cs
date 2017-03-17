using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

using UnityEngine;

public class UDPUnicast
{
    private string _address;
    private int _port;

    private IPEndPoint _remoteEndPoint;
    private UdpClient _udp;
    private int _sendRate;

    private DateTime _lastSent;

    private bool _streaming = false;

    public UDPUnicast(string address, int port, int sendRate = 100)
    {
        _lastSent = DateTime.Now;
        reset(address, port, sendRate);
    }

    public void reset(string address, int port, int sendRate = 100)
    {
        _sendRate = sendRate;
        try
        {
            _port = port;
            _address = address;

            _remoteEndPoint = new IPEndPoint(IPAddress.Parse(_address), _port);
            _udp = new UdpClient();
            _streaming = true;
            Debug.Log("[UDPUnicast] sending to " + _address + ":" + _port);
        }
        catch (Exception e)
        {
        }
    }

    public void send(string line)
    {
        if (_streaming)
        {
            try
            {
                if (DateTime.Now > _lastSent.AddMilliseconds(_sendRate))
                {
                    byte[] data = Encoding.UTF8.GetBytes(line);
                    _udp.Send(data, data.Length, _remoteEndPoint);
                    _lastSent = DateTime.Now;

                    //Debug.Log(line);
                }
            }
            catch (Exception e)
            {
            }

        }
    }
}
