using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPCWorkspace : MonoBehaviour {

    public bool Connected { get { return Network.peerType == NetworkPeerType.Client || Network.peerType == NetworkPeerType.Server; } }

    private Main _main;
    private NetworkView _networkView;
    private NegativeSpace _negativeSpace;
    private Properties _properties;
    private VisualLog _log;


    public int connectionDelay = 2000;
    private DateTime _connectionTime;

    void _init ()
    {
        _main = GetComponent<Main>();
        _networkView = GetComponent<NetworkView>();
        _negativeSpace = GetComponent<NegativeSpace>();
        _properties = GetComponent<Properties>();
        _log = GetComponent<VisualLog>();
	}

    internal void InitServer()
    {
        _init();
        Network.InitializeServer(2, int.Parse(_properties.localSetupInfo.rpcPort), false);
    }

    internal void InitClient()
    {
        _init();
        Network.Connect(_properties.remoteSetupInfo.machineAddress, int.Parse(_properties.remoteSetupInfo.rpcPort));
        _connectionTime = DateTime.Now;
    }

    void Update()
    {
        Debug.Log(Network.peerType.ToString());

        if (_main.location == Location.B 
            && Network.peerType == NetworkPeerType.Disconnected
            && DateTime.Now > _connectionTime.AddMilliseconds(connectionDelay))
        {
            InitClient();
            connectionDelay += 100;
        }
    }

    void OnConnectedToServer()
    {
        _log.WriteLine("[RPC] Connection established");
    }

    void OnFailedToConnect()
    {
        _log.WriteLine("[RPC] Connection failed");
        Debug.Log("Is client? " + Network.isClient);
        Debug.Log("not connected");
    }
}
