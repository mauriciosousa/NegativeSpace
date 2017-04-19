using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPCWorkspace : MonoBehaviour {

    private bool _running = false;

    public bool Connected { get { return Network.peerType == NetworkPeerType.Client || Network.peerType == NetworkPeerType.Server; } }

    private Main _main;
    private NetworkView _networkView;
    private NegativeSpace _negativeSpace;
    private Properties _properties;
    private VisualLog _log;

    public int connectionDelay = 2000;
    private DateTime _connectionTime;

    private GameObject _negativeSpaceCenter = null;

    void _init ()
    {
        _main = GetComponent<Main>();
        _networkView = GetComponent<NetworkView>();
        _negativeSpace = GetComponent<NegativeSpace>();
        _properties = GetComponent<Properties>();
        _log = GetComponent<VisualLog>();
        _running = true;
	}

    internal void InitServer()
    {
        if (!_running) _init();
        Network.InitializeServer(2, int.Parse(_properties.localSetupInfo.rpcPort), false);
    }

    internal void InitClient()
    {
        if (!_running) _init();
        if (_properties.ConfigLoaded)
        {
            Debug.Log("Trying to connect... " + _properties.remoteSetupInfo.machineAddress);
            Network.Connect(_properties.remoteSetupInfo.machineAddress, int.Parse(_properties.remoteSetupInfo.rpcPort));
            _connectionTime = DateTime.Now;
        }
    }

    void Update()
    {
        //Debug.Log(Network.peerType.ToString());

        if (_running && _main.location == Location.B 
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

    [RPC]
    void updateNSObjectSend_Remote(string uid, Vector3 position, Quaternion rotation)
    {
        _negativeSpace.updateObject(uid, position, rotation);
    }

    internal void updateNegativeSpaceObject(string uid, Vector3 position, Quaternion rotation)
    {
        if (Network.peerType != NetworkPeerType.Disconnected)
        {
            _networkView.RPC("updateNSObjectSend_Remote", RPCMode.Others, uid, position, rotation);
        }
    }

    [RPC]
    void instantiateObject_Remote(string description, string uid)
    {
        _negativeSpace.instantiateRemoteObject(description, uid);
    }

    internal void instantiateObject(string description, string uid)
    {
        if (Network.peerType != NetworkPeerType.Disconnected)
        {
            _networkView.RPC("instantiateObject_Remote", RPCMode.Others, description, uid);
        }
    }

    [RPC]
    void lockObject_Remote(string uid)
    {
        _negativeSpace.lockObject(uid);
    }

    internal void lockObject(string uid)
    {
        if (Network.peerType != NetworkPeerType.Disconnected)
        {
            _networkView.RPC("lockObject_Remote", RPCMode.Others, uid);
        }
    }

    [RPC]
    void unlockObject_Remote(string uid)
    {
        _negativeSpace.unlockObject(uid);
    }

    internal void unlockObject(string uid)
    {
        if (Network.peerType != NetworkPeerType.Disconnected)
        {
            _networkView.RPC("unlockObject_Remote", RPCMode.Others, uid);
        }
    }
}
