using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPCNetwork : MonoBehaviour
{


    private string _address;
    private int _port;

    private NegativeSpace workspace;

    private NetworkView networkView;


    public int connectionDelay = 2000;
    private DateTime _startTime;

    public string ConnectionStatus
    {
        get
        {
            return Network.peerType.ToString() + (Network.peerType == NetworkPeerType.Server ?  "( " + Network.connections.Length + " clients)" : "");
        }
    }


    void Start()
    {
        workspace = this.gameObject.GetComponent<NegativeSpace>() as NegativeSpace;
        networkView = GetComponent<NetworkView>();

        NSProperties p = GameObject.Find("Main").GetComponent<NSProperties>();
        _address = p.remote_NegativeSpaceMachine_Address;
        _port = p.RPC_Port;
    }


    void Update()
    {
        if (Network.peerType == NetworkPeerType.Disconnected)
        {
            if (workspace.location == Location.A)
            {
                Network.InitializeServer(5, _port, false);
            }

            if (workspace.location == Location.B && DateTime.Now > _startTime.AddMilliseconds(connectionDelay))
            {
                Network.Connect(_address, _port);
                _startTime = DateTime.Now;
                connectionDelay += 100;
            }
        }
    }

    [RPC]
    void updateNSObjectSend_Remote(string uid, Vector3 position, Quaternion rotation)
    {
        workspace.updateObject(uid, position, rotation);
    }

    internal void updateNSObjectSend(string uid, Vector3 position, Quaternion rotation)
    {
        if (Network.peerType != NetworkPeerType.Disconnected)
        {
            networkView.RPC("updateNSObjectSend_Remote", RPCMode.Others, uid, position, rotation);
        }
    }

    [RPC]
    void instantiateObject_Remote(string description, string uid)
    {
        workspace.createRemoteObject(description, uid);
    }

    internal void instantiateObject(string description, string uid)
    {
        if (Network.peerType != NetworkPeerType.Disconnected)
        {
            networkView.RPC("instantiateObject_Remote", RPCMode.Others, description, uid);
        }
    }

    [RPC]
    void lockObject_Remote(string uid)
    {
        workspace.lockObject(uid);
    }

    internal void lockObject(string uid)
    {
        if (Network.peerType != NetworkPeerType.Disconnected)
        {
            networkView.RPC("lockObject_Remote", RPCMode.Others, uid);
        }
    }

    [RPC]
    void unlockObject_Remote(string uid)
    {
        workspace.unlockObject(uid);
    }

    internal void unlockObject(string uid)
    {
        if (Network.peerType != NetworkPeerType.Disconnected)
        {
            networkView.RPC("unlockObject_Remote", RPCMode.Others, uid);
        }
    }

    [RPC]
    internal void updateNSCursors_Remote(Vector3 leftPosition, Quaternion leftRotation, Vector3 rightPosition, Quaternion rightRotation)
    {
        workspace.updateRemoteCursors(leftPosition, leftRotation, rightPosition, rightRotation);
    }

    internal void updateNSCursors(Vector3 leftPosition, Quaternion leftRotation, Vector3 rightPosition, Quaternion rightRotation)
    {
        if (Network.peerType != NetworkPeerType.Disconnected)
        {
            networkView.RPC("updateNSCursors_Remote", RPCMode.Others, leftPosition, leftRotation, rightPosition, rightRotation);
        }
    }

    void OnConnectedToServer()
    {

    }

    void OnFailedToConnect()
    {
        Debug.Log("Is client? " + Network.isClient);
        Debug.Log("not connected");
    }


}
