using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrackerProperties : MonoBehaviour {

    private static TrackerProperties _singleton;

    public int listenPort = 22222;
    public int trackerPort = 53803;

    public string configFilename = "configSettings.txt";

    private TrackerProperties()
    {
        _singleton = this;
    }

    public static TrackerProperties Instance
    {
        get
        {
            return _singleton;
        }
    }

    void Start()
    {
    //_singleton = this;
    }
}
