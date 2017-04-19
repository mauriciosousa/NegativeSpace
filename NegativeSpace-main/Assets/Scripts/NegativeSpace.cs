using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NegativeSpace : MonoBehaviour {

    private Main _main;
    private Properties _properties;
    private VisualLog _log;
    private BodiesManager _bodiesManager;
    private RPCWorkspace _rpc;

    private bool _spaceCreated = false;
    private Location _location;
    private SurfaceRectangle _localSurface;
    private SurfaceRectangle _remoteSurfaceProxy;
    private float _negativeSpaceLength;

    public Material negativeSpaceMaterial;

    private UDPHandheldListener _handheldListener;

    private GameObject NegativeSpaceCenter = null;

    private Dictionary<string, GameObject> _negativeSpaceObjects;
    private Dictionary<string, GameObject> negativeSpaceObjects { get { return _negativeSpaceObjects; } }

    void Awake()
    {
        _negativeSpaceObjects = new Dictionary<string, GameObject>();
    }

    void Start ()
    {
        _main = GetComponent<Main>();
        _properties = GetComponent<Properties>();
        _log = GetComponent<VisualLog>();
        _bodiesManager = GameObject.Find("BodiesManager").GetComponent<BodiesManager>();
        _rpc = GetComponent<RPCWorkspace>();
	}

    internal void create(Location location, SurfaceRectangle localSurface, SurfaceRectangle remoteSurfaceProxy, float length)
    {
        _handheldListener = new UDPHandheldListener(int.Parse(_properties.localSetupInfo.receiveHandheldPort), "negativespace");

        _location = location;
        _localSurface = localSurface;
        _remoteSurfaceProxy = remoteSurfaceProxy;
        _negativeSpaceLength = length;

        _createNegativeSpaceMesh();

        NegativeSpaceCenter = new GameObject("NegativeSpaceCenter");
        NegativeSpaceCenter.transform.position = (_localSurface.SurfaceBottomLeft + _remoteSurfaceProxy.SurfaceTopRight) * 0.5f;
        NegativeSpaceCenter.transform.rotation = GameObject.Find("localScreenCenter").transform.rotation;

        _log.WriteLine(this, "Waiting for handheld at " + _properties.localSetupInfo.receiveHandheldPort);
        _log.WriteLine(this, "Negative Space Created for Location " + _location + " with length " + _negativeSpaceLength);
        _spaceCreated = true;
    }

    private void _createNegativeSpaceMesh()
    {
        MeshFilter meshFilter = (MeshFilter)gameObject.AddComponent(typeof(MeshFilter));
        Mesh m = new Mesh();
        m.name = "NegativeSpaceMesh";
        m.vertices = new Vector3[]
            {
                _localSurface.SurfaceBottomLeft, _localSurface.SurfaceBottomRight, _localSurface.SurfaceTopRight, _localSurface.SurfaceTopLeft,
                _remoteSurfaceProxy.SurfaceBottomLeft, _remoteSurfaceProxy.SurfaceBottomRight, _remoteSurfaceProxy.SurfaceTopRight, _remoteSurfaceProxy.SurfaceTopLeft
            };

        m.triangles = new int[]
            {
                    0, 4, 3,
                    0, 1, 4,
                    1, 5, 4,
                    1, 2, 5,
                    2, 6, 5,
                    2, 7, 6,
                    3, 7, 2,
                    3, 4, 7
            };
        m.RecalculateNormals();
        m.RecalculateBounds();

        meshFilter.mesh = m;
        MeshRenderer renderer = gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        renderer.material = negativeSpaceMaterial;
        MeshCollider collider = gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
    }

    void Update()
    {
        if (_spaceCreated)
        {
            CommonUtils.drawSurface(_localSurface.SurfaceBottomLeft, _localSurface.SurfaceBottomRight, _localSurface.SurfaceTopRight, _localSurface.SurfaceTopLeft, Color.red);
            CommonUtils.drawSurface(_remoteSurfaceProxy.SurfaceBottomLeft, _remoteSurfaceProxy.SurfaceBottomRight, _remoteSurfaceProxy.SurfaceTopRight, _remoteSurfaceProxy.SurfaceTopLeft, Color.green);

            if (_bodiesManager.human != null)
            {
                Vector3 head = _bodiesManager.human.body.Joints[BodyJointType.head];
                Vector3 leftHand = _bodiesManager.human.body.Joints[BodyJointType.leftHandTip];
                Vector3 rightHand = _bodiesManager.human.body.Joints[BodyJointType.rightHandTip];

                // todo
            }

            _syncNegativeSpaceObjects();
        }
    }

    private GameObject _instantiateObject(string primitive, string uid)
    {
        if (NegativeSpaceCenter == null) NegativeSpaceCenter = GameObject.Find("NegativeSpaceCenter");

        GameObject o;
        if (primitive == "cube") o = GameObject.CreatePrimitive(PrimitiveType.Cube);
        else if (primitive == "sphere") o = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        else o = new GameObject();

        o.transform.parent = NegativeSpaceCenter.transform;
        o.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        o.transform.localPosition = Vector3.zero;
        o.transform.localRotation = Quaternion.identity;
        o.name = uid;

        NegativeSpaceObject ns = o.AddComponent<NegativeSpaceObject>();
        ns.name = uid;
        _negativeSpaceObjects.Add(uid, o);
        return o;
    }

    internal void instantiateLocalObject(string primitive)
    {
        string uid = generateNSObjectID();
        GameObject o = _instantiateObject(primitive, uid);
        _rpc.instantiateObject(primitive, uid);
    }

    internal void instantiateRemoteObject(string primitive, string uid)
    {
        GameObject o = _instantiateObject(primitive, uid);
    }

    private int ObjectInitCounter = 0;
    public string generateNSObjectID()
    {
        return "" + _main.location + ObjectInitCounter++;
    }

    internal void updateObject(string uid, Vector3 position, Quaternion rotation)
    {
        if (_negativeSpaceObjects.ContainsKey(uid))
        {
            GameObject o = _negativeSpaceObjects[uid];
            o.transform.localPosition = position;
            o.transform.localRotation = rotation;
        }
    }

    internal void unlockObject(string uid)
    {
        if (_negativeSpaceObjects.ContainsKey(uid))
        {
            _negativeSpaceObjects[uid].GetComponent<NegativeSpaceObject>().lockStatus = LockType.Remote;
        }
    }

    internal void lockObject(string uid)
    {
        if (_negativeSpaceObjects.ContainsKey(uid))
        {
            _negativeSpaceObjects[uid].GetComponent<NegativeSpaceObject>().lockStatus = LockType.NotLocked;
        }
    }

    private void _syncNegativeSpaceObjects()
    {
        foreach (GameObject o in _negativeSpaceObjects.Values)
        {
            NegativeSpaceObject nso = o.GetComponent<NegativeSpaceObject>();
            if (nso.lockStatus == LockType.Local)
            {
                _rpc.updateNegativeSpaceObject(nso.name, o.transform.localPosition, o.transform.localRotation);
            }
        }
    }
}
