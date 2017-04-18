using System;
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
    private SurfaceRequestListener _surfaceRequestListener;
    private NegativeSpace _negativeSpace;
    private UdpBodiesListener _bodiesListener;
    private BodiesManager _bodies;
    private PerspectiveProjection _prespectiveProjection;
    private Tracker _tracker;
    private TcpKinectListener _tcpKinectListener;
    private RPCWorkspace _workspace;

    private bool ConfigLoaded = false;
    private bool _localSurfaceReceived = false;
    public bool LocalSurfaceReceived { get { return _localSurfaceReceived; } }
    private bool _remoteSurfaceReceived = false;
    public bool RemoteSurfaceReceived { get { return _remoteSurfaceReceived; } }


    private SurfaceRectangle _localSurface;
    private SurfaceRectangle _remoteSurface;

    private bool Configured = false;

    void Awake()
    {
        Application.runInBackground = true;
        _surfaceRequestListener = GetComponent<SurfaceRequestListener>();
        _properties = GetComponent<Properties>();
        _log = GetComponent<VisualLog>();
        _negativeSpace = GetComponent<NegativeSpace>();
        _bodiesListener = GameObject.Find("BodiesManager").GetComponent<UdpBodiesListener>();
        _bodies = GameObject.Find("BodiesManager").GetComponent<BodiesManager>();
        _prespectiveProjection = Camera.main.GetComponent<PerspectiveProjection>();
        _tracker = GameObject.Find("RavatarManager").GetComponent<Tracker>();
        _tcpKinectListener = GameObject.Find("RavatarManager").GetComponent<TcpKinectListener>();
        _workspace = GetComponent<RPCWorkspace>();

        _properties.load();

        ConfigLoaded = _properties.ConfigLoaded;

        if (!ConfigLoaded)
        {
            _log.Show = true;
        }
        else
        {
            _log.WriteLine(this, "Config Loaded");
            _surfaceRequestListener.StartReceive();
        }

        if (location == Location.A)
        {
            _workspace.InitServer();
        }
        else
        {
            _workspace.InitClient();
        }
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

        bool ready = ConfigLoaded & _localSurfaceReceived & _remoteSurfaceReceived & _workspace.Connected;
        if (ready)
        {
            if (!Configured)
            { // space setup here

                _bodiesListener.startListening(int.Parse(_properties.localSetupInfo.trackerBroadcastPort));

                GameObject localOrigin = new GameObject("LocalOrigin");
                localOrigin.transform.rotation = Quaternion.identity;
                localOrigin.transform.position = Vector3.zero;

                GameObject remoteOrigin = new GameObject("RemoteOrigin");
                remoteOrigin.transform.rotation = Quaternion.identity;
                remoteOrigin.transform.position = Vector3.zero;

                GameObject localScreenCenter = new GameObject("localScreenCenter");
                localScreenCenter.transform.position = _localSurface.Center;
                localScreenCenter.transform.rotation = _localSurface.Perpendicular;

                Vector3 BLp = _calculateRemoteProxy(_localSurface.SurfaceBottomLeft, localScreenCenter, _properties.negativeSpaceLength);
                Vector3 BRp = _calculateRemoteProxy(_localSurface.SurfaceBottomRight, localScreenCenter, _properties.negativeSpaceLength);
                Vector3 TRp = _calculateRemoteProxy(_localSurface.SurfaceTopRight, localScreenCenter, _properties.negativeSpaceLength);
                Vector3 TLp = _calculateRemoteProxy(_localSurface.SurfaceTopLeft, localScreenCenter, _properties.negativeSpaceLength);

                SurfaceRectangle remoteSurfaceProxy = new SurfaceRectangle(BLp, BRp, TLp, TRp);

                GameObject remoteScreenCenter = new GameObject("remoteScreenCenter");
                remoteScreenCenter.transform.position = _remoteSurface.Center;
                remoteScreenCenter.transform.rotation = _remoteSurface.Perpendicular;

                localOrigin.transform.parent = localScreenCenter.transform;
                remoteOrigin.transform.parent = remoteScreenCenter.transform;
                remoteScreenCenter.transform.position = localScreenCenter.transform.position;
                remoteScreenCenter.transform.rotation = Quaternion.LookRotation(-localScreenCenter.transform.forward, localScreenCenter.transform.up);

                remoteScreenCenter.transform.position = remoteSurfaceProxy.Center;

                _localSurface.CenterGameObject = localScreenCenter;
                _remoteSurface.CenterGameObject = remoteScreenCenter;

                _negativeSpace.create(location, _localSurface, remoteSurfaceProxy, _properties.negativeSpaceLength);

                centerCamera();

                _prespectiveProjection.init(_localSurface);
                _tcpKinectListener.init();
                _tracker.init();

                Configured = true;
                _log.Show = false;
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (_prespectiveProjection.Running && _prespectiveProjection.Active)
                    {
                        _prespectiveProjection.Active = false;
                        centerCamera();
                    }
                    else if (_prespectiveProjection.Running && !_prespectiveProjection.Active)
                    {
                        _prespectiveProjection.Active = true;
                    }
                }
            }
        }
    }

    private void centerCamera()
    {
        Camera.main.transform.position = _localSurface.CenterGameObject.transform.position - 0.5f * _localSurface.CenterGameObject.transform.forward;
        Camera.main.transform.rotation = Quaternion.LookRotation(_localSurface.CenterGameObject.transform.forward, _localSurface.CenterGameObject.transform.up);
    }

    private Vector3 _calculateRemoteProxy(Vector3 point, GameObject localScreenCenter, float negativeSpaceLength)
    {
        return point + negativeSpaceLength * localScreenCenter.transform.forward;
    }

    public void setRemoteSurface(SurfaceRectangle remoteSurface)
    {
        Debug.Log("REMOTE: " + remoteSurface.ToString());
        _remoteSurface = remoteSurface;
        _remoteSurfaceReceived = true;
    }

    public void setLocalSurface(SurfaceRectangle localSurface)
    {
        Debug.Log("LOCAL: " + localSurface.ToString());
        _localSurface = localSurface;
        _localSurfaceReceived = true;
    }
}
