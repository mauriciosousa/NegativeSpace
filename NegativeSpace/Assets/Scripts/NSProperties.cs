using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;


public class NSProperties : MonoBehaviour {

    public NegativeSpace negativeSpace;
    private Location location;

    public string configFilename;

    public string RpcNS_Address;
    public int RpcNS_Port;
    public int HandheldPort;



    void Awake()
    {
        location = negativeSpace.location;

        RpcNS_Address = getProperty("machine.address");
        RpcNS_Port = getPropertyInt("rpc.port");

        HandheldPort = getPropertyInt("rcv.handheld.port");



    }

    void Start ()
    {
    }

    public string getProperty(string property)
    {
        string val = "" + location.ToString() + "." + property;
        Debug.Log("[CONFIG] Retrieving property: " + val);
        return ConfigProperties.load(configFilename, val);
    }

    public int getPropertyInt(string property)
    {
        return int.Parse(getProperty(property));
    }

}
