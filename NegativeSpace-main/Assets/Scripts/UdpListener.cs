using UnityEngine;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Text;


public class UdpListener : MonoBehaviour {

    private UdpClient _udpClient = null;
    private IPEndPoint _anyIP;
    private List<byte[]> _stringsToParse; // TMA: Store the bytes from the socket instead of converting to strings. Saves time.
    private byte[] _receivedBytes;
    private int number = 0;
    CloudMessage message;

    private Properties _properties;

    private bool _startConfig = false;
    private bool _start = false;

    void Start()
    {
        message = new CloudMessage();
    }

    private void udpRestart()
    {
        _properties = GameObject.Find("Main").GetComponent<Properties>();

        if (_udpClient != null)
        {
            _udpClient.Close();
        }

        _stringsToParse = new List<byte[]>();
        
		_anyIP = new IPEndPoint(IPAddress.Any, int.Parse(_properties.localSetupInfo.ravatarListenPort));
        
        _udpClient = new UdpClient(_anyIP);

        _udpClient.BeginReceive(new AsyncCallback(this.ReceiveCallback), null);

		Debug.Log("[UDPListener] Receiving in port: " + _properties.localSetupInfo.ravatarListenPort);
        _start = true;
    }

    public void init()
    {
        udpRestart();

        _startConfig = true;
    }
    
    public void ReceiveCallback(IAsyncResult ar)
    {
        Byte[] receiveBytes = _udpClient.EndReceive(ar, ref _anyIP);
        _udpClient.BeginReceive(new AsyncCallback(this.ReceiveCallback), null);
        _stringsToParse.Add(receiveBytes);
    }

    void Update()
    {
        if (_startConfig)
        {
            //udpRestart();
            _startConfig = false;
        }

        if (_start)
        {
            while (_stringsToParse.Count > 0)
            {
                try
                {
                    byte[] toProcess = _stringsToParse.First();
                    if (toProcess != null)
                    {
                        // TMA: THe first char distinguishes between a BodyMessage and a CloudMessage
                        if (Convert.ToChar(toProcess[0]) == 'C')
                        {
                            string stringToParse = Encoding.ASCII.GetString(toProcess);
                            string[] splitmsg = stringToParse.Split(MessageSeparators.L0);
                            message.set(splitmsg[1], toProcess, splitmsg[0].Length);
                            gameObject.GetComponent<Tracker>().setNewCloud(message);
                        }
                        else if (Convert.ToChar(toProcess[0]) == 'A')
                        {
                            Debug.Log("Got Calibration Message! ");
                            string stringToParse = Encoding.ASCII.GetString(toProcess);
                            string[] splitmsg = stringToParse.Split(MessageSeparators.L0);
                            AvatarMessage av = new AvatarMessage(splitmsg[1], toProcess);
                            gameObject.GetComponent<Tracker>().processAvatarMessage(av);
                        }
                    }
                    _stringsToParse.RemoveAt(0);
                }
                catch (Exception exc) { _stringsToParse.RemoveAt(0); }
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
