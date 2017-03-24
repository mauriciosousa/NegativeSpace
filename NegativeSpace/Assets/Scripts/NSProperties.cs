using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;


public class NSProperties : MonoBehaviour {

    public NegativeSpace negativeSpace;

    private Location myLocation;
    private Location remoteLocation;


    public string configFilename;

    public string remote_NegativeSpaceMachine_Address;
    public int RPC_Port;
    public int handheld_Port;

    


    void Awake()
    {
        myLocation = negativeSpace.location;
        remoteLocation = myLocation == Location.A ? Location.B : Location.A;

        remote_NegativeSpaceMachine_Address = getProperty(remoteLocation, "machine.address");
        RPC_Port = getPropertyInt(myLocation, "rpc.port");
        handheld_Port = getPropertyInt(myLocation, "rcv.handheld.port");

    }

    public string getProperty(Location location, string property)
    {
        string val = "" + location.ToString() + "." + property;
        Debug.Log("[CONFIG] Retrieving property: " + val);
        return ConfigProperties.load(configFilename, val);
    }

    public int getPropertyInt(Location location, string property)
    {
        return int.Parse(getProperty(location, property));
    }

}
