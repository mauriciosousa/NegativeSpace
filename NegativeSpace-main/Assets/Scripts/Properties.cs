using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class NSInfo
{
    internal string localSurfaceListen;
    internal Location location;
    internal string machineAddress;
    internal string receiveHandheldPort;
    internal string remoteSurfaceListen;
    internal string rpcPort;
    internal string trackerBroadcastPort;
    internal string trackerListenPort;
}

public class Properties : MonoBehaviour {

    private VisualLog _log;

    private bool _configRead = false;
    public bool ConfigLoaded { get { return _configRead; } }

    public string configFilename;
    private string _filename;

    public NSInfo localSetupInfo = null;
    public NSInfo remoteSetupInfo = null;

    private Location _location;

    void Awake()
    {
        _log = GetComponent<VisualLog>();
        _location = GetComponent<Main>().location;
    }

    public void load()
    {
        _filename = Application.dataPath + "/" + configFilename;
        if (File.Exists(_filename))
        {
            _log.WriteLine(this, "Config file found!");

            try
            {
                localSetupInfo = _retrieveInfo(_location);
                remoteSetupInfo = _retrieveInfo(_location == Location.A ? Location.B : Location.A);


                _configRead = true;
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                Debug.Log(e.StackTrace);
                _configRead = false;
            }
        }
        else
        {
            _log.WriteLine(this, "Cannot find the config file");
        }
    }

    private NSInfo _retrieveInfo(Location _location)
    {
        NSInfo info = new NSInfo();

        info.location = _location;
        info.machineAddress = load(_location.ToString() + ".machine.address");
        info.rpcPort = load(_location.ToString() + ".rpc.port");
        info.receiveHandheldPort = load(_location.ToString() + ".rcv.handheld.port");
        info.trackerBroadcastPort = load(_location.ToString() + ".tracker.broadcast.port");
        info.trackerListenPort = load(_location.ToString() + ".tracker.listen.port");
        info.trackerListenPort = load(_location.ToString() + ".ravatar.listen.port");
        info.localSurfaceListen = load(_location.ToString() + ".local.surface.listen");
        info.remoteSurfaceListen = load(_location.ToString() + ".remote.surface.listen");

        return info;
    }

    private string load(string property)
    {
        if (File.Exists(_filename))
        {
            List<string> lines = new List<string>(File.ReadAllLines(_filename));
            foreach (string line in lines)
            {
                if (line.Split('=')[0] == property)
                {
                    //_log.WriteLine(this, line.Split('=')[0] + ": " + line.Split('=')[1]);
                    return line.Split('=')[1];
                }
            }
            throw new System.InvalidOperationException("Not Found");
        }
        else
            throw new System.InvalidOperationException("Not Found");
    }
}
