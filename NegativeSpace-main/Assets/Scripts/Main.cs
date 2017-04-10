using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Location
{
    A, B
}

public class Main : MonoBehaviour {

    public Location location;
    private Properties _properties;
    private VisualLog _log;

    private bool ConfigLoaded = false;
    private bool LocalSurfaceReceived = false;
    private bool RemoteSurfaceReceived = false;

    private bool Configured = false;

    void Awake()
    {
        Application.runInBackground = true;
        _properties = GetComponent<Properties>();
        _log = GetComponent<VisualLog>();
        _properties.load();

        ConfigLoaded = _properties.ConfigLoaded;

        if (!ConfigLoaded)
        {
            _log.Show = true;
        }
        else _log.WriteLine(this, "Config Loaded");
    }

	void Start ()
    {
        GetComponent<SurfaceRequest>().request();
    }
	
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            _log.Show = !_log.Show;
        }

        bool ready = ConfigLoaded & LocalSurfaceReceived & RemoteSurfaceReceived;
        if (ready)
        {
            if (!Configured)
            {
                // create the Negative Space
                Configured = true;
            }
            else
            {
                // Business
            }
        }
	}
}
