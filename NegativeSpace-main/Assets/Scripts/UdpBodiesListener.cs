﻿using UnityEngine;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Text;

public class UdpBodiesListener : MonoBehaviour
{

    private bool _canDoStuff = false;

    public static string NoneMessage = "0";

    private int _port;

    private UdpClient _udpClient = null;
    private IPEndPoint _anyIP;
    private List<string> _stringsToParse;

    public void startListening(int port)
    {
        _port = port;
        GameObject.Find("Main").GetComponent<VisualLog>().WriteLine(this, "Listening to Bodies in port " + port);
        udpRestart();
        _canDoStuff = true;
    }

    public void udpRestart()
    {
        if (_udpClient != null)
        {
            _udpClient.Close();
        }

        _stringsToParse = new List<string>();

        _anyIP = new IPEndPoint(IPAddress.Any, _port);

        _udpClient = new UdpClient(_anyIP);

        _udpClient.BeginReceive(new AsyncCallback(this.ReceiveCallback), null);
    }

    public void ReceiveCallback(IAsyncResult ar)
    {
        Byte[] receiveBytes = _udpClient.EndReceive(ar, ref _anyIP);
        _stringsToParse.Add(Encoding.ASCII.GetString(receiveBytes));

        _udpClient.BeginReceive(new AsyncCallback(this.ReceiveCallback), null);
    }

    void Update()
    {
        if (_canDoStuff)
        {
            while (_stringsToParse != null && _stringsToParse.Count > 0)
            {
                string stringToParse = _stringsToParse.First();
                _stringsToParse.RemoveAt(0);

                List<Body> bodies = new List<Body>();

                if (stringToParse.Length != 1)
                {
                    foreach (string b in stringToParse.Split(MessageSeparators.L1))
                    {

                        if (b != NoneMessage)
                        {
                            bodies.Add(new Body(b));
                        }
                    }
                    gameObject.GetComponent<BodiesManager>().setNewFrame(bodies.ToArray());
                }
            }
        }
    }

    void OnApplicationQuit()
    {
        if (_udpClient != null) _udpClient.Close();
    }

    void OnQuit()
    {
        OnApplicationQuit();
    }
}
