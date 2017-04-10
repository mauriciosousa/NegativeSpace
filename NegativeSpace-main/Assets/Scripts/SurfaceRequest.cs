using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class SurfaceRectangle
{
    private Vector3 _bl;
    public Vector3 SurfaceBottomLeft { get { return _bl; } }
    private Vector3 _br;
    public Vector3 SurfaceBottomRight { get { return _br; } }
    private Vector3 _tl;
    public Vector3 SurfaceTopLeft { get { return _tl; } }
    private Vector3 _tr;
    public Vector3 SurfaceTopRight { get { return _tr; } }

    public SurfaceRectangle(Vector3 BL, Vector3 BR, Vector3 TL, Vector3 TR)
    {
        _bl = BL;
        _br = BR;
        _tl = TL;
        _tr = TR;
    }
}

public class SurfaceMessage
{
    public static string createRequestMessage(int port)
    {
        return "SurfaceMessage" + MessageSeparators.L0 + Network.player.ipAddress + MessageSeparators.L1 + port;
    }
}

public class SurfaceRequest : MonoBehaviour
{
    public SurfaceRectangle localSurface = null;
    public SurfaceRectangle remoteSurface = null;

    private Properties _properties;
    private Main _main;

    private VisualLog _log;

    void Awake()
    {
        _properties = GetComponent<Properties>();
        _main = GetComponent<Main>();
        _log = GetComponent<VisualLog>();
    }

    public void request()
    {
        _request(_properties.localSetupInfo.trackerListenPort, _properties.localSetupInfo.localSurfaceListen);
        _request(_properties.remoteSetupInfo.trackerListenPort, _properties.localSetupInfo.remoteSurfaceListen);
    }

    private void _request(string trackerPort, string receivePort)
    {
        _log.WriteLine(this, "Requesting surface to " + trackerPort + " to receive in " + receivePort);
        UdpClient udp = new UdpClient();
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Broadcast, int.Parse(trackerPort));
        string message = SurfaceMessage.createRequestMessage(int.Parse(receivePort));
        byte[] data = Encoding.UTF8.GetBytes(message);
        udp.Send(data, data.Length, remoteEndPoint);
    }
}
