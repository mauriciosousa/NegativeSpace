﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NegativespaceSurface
{
    public Vector3 BL;
    public Vector3 BR;
    public Vector3 TL;
    public Vector3 TR;
    public Vector3 sBL;
    public Vector3 sBR;
    public Vector3 sTL;
    public Vector3 sTR;
}

public enum Location
{
    A,
    B
}

public class NegativeSpace : MonoBehaviour
{


    private Transform origin;

    private PerspectiveProjection _perspectiveProjection;
    private bool _spaceCreated = false;
    public bool Created { get { return _spaceCreated; } }

    private BodiesManager _bodiesManager;

    public float NegativeSpaceDepth = 0.1f;
    public Material NegativeSpaceMaterial;


    public GameObject leftCursor;
    private NSCursor _leftCursorScript;

    public GameObject rightCursor;
    private NSCursor _rightCursorScript;

    public GameObject leftCursorRemote;
    public GameObject rightCursorRemote;

    private UDPHandheldListener _handheldListener;
    private int handheldListenPort;
    public string DecryptKey;



    private NegativespaceSurface _surface = null;

    public Vector3 NegativeSpaceSize;

    public bool Handheld_CLICK = false;


    public Location location = Location.A;
    private RPCNetwork _NSNetwork;
    private Dictionary<string, GameObject> _NSObjects;
    public Dictionary<string, GameObject> NSObsjects { get { return _NSObjects; } }

    void Awake()
    {
        _perspectiveProjection = Camera.main.GetComponent<PerspectiveProjection>();
        _leftCursorScript = leftCursor.GetComponent<NSCursor>();
        _rightCursorScript = rightCursor.GetComponent<NSCursor>();

        _NSObjects = new Dictionary<string, GameObject>();

        _NSNetwork = this.gameObject.GetComponent<RPCNetwork>();
    }

    void Start ()
    {

        NSProperties p = GameObject.Find("Main").GetComponent<NSProperties>();
        handheldListenPort = p.handheld_Port;

        _handheldListener = new UDPHandheldListener(handheldListenPort, DecryptKey);
    }

    void Update ()
    {
        if (_perspectiveProjection.SurfaceCalibrated)
        {
            if (!_spaceCreated)
            {
                _createNegativeSpace();
                origin = GameObject.Find("ScreenCenter").transform;
                _bodiesManager = GameObject.Find("BodiesManagerGO").GetComponent<BodiesManager>();
                _surface = new NegativespaceSurface();


                //Debug.Log("waiting");
            }

            //_syncNSObjects();
            //_updateCursors();
            
            //_NSNetwork.lockObject(o.name);
            //_NSNetwork.unlockObject(o.name);

            //_DEBUG_NEGATIVESPACECONNECTIONS();
        }
    }

    private void _DEBUG_NEGATIVESPACECONNECTIONS()
    {
        if (Input.GetKeyDown(KeyCode.F2))
        {
            _createLocalObject("helmet");
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            _createLocalObject("cube");
        }

        if (Input.GetMouseButtonDown(0))
        { // if left button pressed...
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Debug.DrawRay(ray.origin, ray.direction);
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.transform.gameObject.name);
                NSObject o = hit.transform.gameObject.GetComponent<NSObject>();
                if (o != null)
                {
                    if (o.lockStatus == LockType.NotLocked)
                    {
                        o.lockStatus = LockType.Local;
                        _NSNetwork.lockObject(o.name);
                    }
                    else if (o.lockStatus == LockType.Local)
                    {
                        o.lockStatus = LockType.NotLocked;
                        _NSNetwork.unlockObject(o.name);
                    }
                }
                else
                { Debug.Log("null object"); }
            }
        }
    }

    private void _createNegativeSpace()
    {
        gameObject.transform.parent = _perspectiveProjection.SurfaceCenter;
        gameObject.transform.localPosition = Vector3.zero;
        gameObject.transform.localRotation = Quaternion.identity;

        GameObject BL = null;
        GameObject BR = null;
        GameObject TR = null;
        GameObject TL = null;

        if (_perspectiveProjection.screenOrientation == ScreenOrientation.Landscape) // TODO: Move to the PerspectiveProjection.cs
        {
            BL = GameObject.Find("BL");
            BR = GameObject.Find("BR");
            TR = GameObject.Find("TR");
            TL = GameObject.Find("TL");
        }
        else if (_perspectiveProjection.screenOrientation == ScreenOrientation.Portrait)
        {
            BL = GameObject.Find("BR");
            BR = GameObject.Find("TR");
            TR = GameObject.Find("TL");
            TL = GameObject.Find("BL");
        }


        GameObject sBL = _getShiftedObject("SBL", BL, Vector3.forward, _perspectiveProjection.SurfaceCenter);
        GameObject sBR = _getShiftedObject("SBR", BR, Vector3.forward, _perspectiveProjection.SurfaceCenter);
        GameObject sTR = _getShiftedObject("STR", TR, Vector3.forward, _perspectiveProjection.SurfaceCenter);
        GameObject sTL = _getShiftedObject("STL", TL, Vector3.forward, _perspectiveProjection.SurfaceCenter);

        _surface.BL = BL.transform.localPosition;
        _surface.BR = BR.transform.localPosition;
        _surface.TL = TL.transform.localPosition;
        _surface.TR = TR.transform.localPosition;
        _surface.sBL = sBL.transform.localPosition;
        _surface.sBR = sBR.transform.localPosition;
        _surface.sTL = sTL.transform.localPosition;
        _surface.sTR = sTR.transform.localPosition;

        NegativeSpaceSize = new Vector3(Vector3.Distance(_surface.BL, _surface.BR), Vector3.Distance(_surface.BL, _surface.TL), Vector3.Distance(_surface.BL, _surface.sBL));

        _leftCursorScript.surface = _surface;
        _leftCursorScript.handType = HandType.Left;

        _rightCursorScript.surface = _surface;
        _rightCursorScript.handType = HandType.Right;

        MeshFilter meshFilter = (MeshFilter)gameObject.AddComponent(typeof(MeshFilter));
        Mesh m = new Mesh();
        m.name = "NegSpaceMesh";
        m.vertices = new Vector3[]
        {
                    BL.transform.localPosition, BR.transform.localPosition, TR.transform.localPosition, TL.transform.localPosition,
                    sBL.transform.localPosition, sBR.transform.localPosition, sTR.transform.localPosition, sTL.transform.localPosition
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
        renderer.material = NegativeSpaceMaterial;

        MeshCollider collider = gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;



        /* Create Remote Screen Center */
        Vector3 worldCenter = (BL.transform.position + sTR.transform.position) / 2;

        GameObject localSpace = new GameObject();
        localSpace.name = "LocalSpace";
        localSpace.transform.position = worldCenter;
        localSpace.transform.rotation = _perspectiveProjection.ScreenCenter.transform.localRotation;

        _perspectiveProjection.ScreenCenter.transform.parent = localSpace.transform;
        //_perspectiveProjection.ScreenCenter.transform.localPosition = Vector3.zero;

        GameObject remoteSpace = new GameObject();
        remoteSpace.name = "RemoteSpace";
        


        GameObject RavatarManager = GameObject.Find("RavatarManager");
        Tracker tracker = RavatarManager.GetComponent<Tracker>();
        foreach (GameObject o in tracker.cloudGameObjects)
        {
         //   o.transform.parent = remoteSpace.transform;
            
        }

        remoteSpace.transform.position = localSpace.transform.position;
        //remoteSpace.transform.forward = -localSpace.transform.forward;
        //remoteSpace.transform.up = localSpace.transform.up;


        /*   */
        _spaceCreated = true;
        Debug.Log("Negative Space Created");
    }

    private void _updateCursors()
    {
        if (_bodiesManager.human != null)
        {
            Vector3 head = _bodiesManager.human.body.Joints[BodyJointType.head];
            Vector3 leftHand = _bodiesManager.human.body.Joints[BodyJointType.leftHandTip];
            Vector3 rightHand = _bodiesManager.human.body.Joints[BodyJointType.rightHandTip];


            //_leftCursorScript.updateValues(head, leftHand, NegativeSpaceSize, _handheldListener);
            //_rightCursorScript.updateValues(head, rightHand, NegativeSpaceSize, _handheldListener);


            if (_handheldListener.Receiving && _handheldListener.Message.Hand != HandType.Unknown)
            {
                if (_handheldListener.Message.Hand == HandType.Left)
                {
                    _leftCursorScript.updateValues(head, leftHand, NegativeSpaceSize, _handheldListener.Message.Rotation, _handheldListener.Message.Click);
                    _rightCursorScript.updateValues(head, rightHand, NegativeSpaceSize, Quaternion.identity, false);
                }
                else
                {
                    _leftCursorScript.updateValues(head, leftHand, NegativeSpaceSize, Quaternion.identity, false);
                    _rightCursorScript.updateValues(head, rightHand, NegativeSpaceSize, _handheldListener.Message.Rotation, _handheldListener.Message.Click);
                }
            }
            else
            {
                _leftCursorScript.updateValues(head, leftHand, NegativeSpaceSize, Quaternion.identity, false);
                _rightCursorScript.updateValues(head, rightHand, NegativeSpaceSize, Quaternion.identity, false);
            }

            _NSNetwork.updateNSCursors(_leftCursorScript.gameObject.transform.position, _leftCursorScript.gameObject.transform.rotation,
                                       _rightCursorScript.gameObject.transform.position, _rightCursorScript.gameObject.transform.rotation);
        }
    }

    internal void updateRemoteCursors(Vector3 leftPosition, Quaternion leftRotation, Vector3 rightPosition, Quaternion rightRotation)
    {
        if (leftCursorRemote != null && rightCursorRemote != null)
        {
            leftCursorRemote.transform.localPosition = leftPosition;
            leftCursorRemote.transform.localRotation = leftRotation;
            rightCursorRemote.transform.localPosition = rightPosition;
            rightCursorRemote.transform.localRotation = rightRotation;
        }
    }

    private void _syncNSObjects()
    {
        foreach (GameObject o in _NSObjects.Values)
        {
            NSObject nso = o.GetComponent<NSObject>();
            if (nso.lockStatus == LockType.Local)
            {
                _NSNetwork.updateNSObjectSend(nso.name, o.transform.localPosition, o.transform.localRotation);
            }
        }
    }

    private Quaternion convert(Quaternion r)
    {
        return Quaternion.Inverse(r);
    }

    public Vector3 convertToNSCoordinates(Vector3 v)
    {
        return new Vector3(
                Mathf.Clamp(v.x, -NegativeSpaceSize.x / 2.0f, NegativeSpaceSize.x / 2.0f),
                Mathf.Clamp(v.y, -NegativeSpaceSize.y / 2.0f, NegativeSpaceSize.y / 2.0f),
                Mathf.Clamp(v.z, 0, NegativeSpaceSize.z)
                );
    }

    private GameObject _getShiftedObject(string name, GameObject g, Vector3 normalized, Transform parent)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        go.transform.parent = parent;
        go.transform.localRotation = Quaternion.identity;
        go.name = name;
        go.transform.localPosition = g.transform.localPosition + NegativeSpaceDepth * (normalized);
        go.GetComponent<MeshRenderer>().enabled = false;
        return go;
    }

    private Vector3 _calcNormal(Vector3 a, Vector3 b, Vector3 c)
    {
        return Vector3.Cross((b - a),(c - a));
    }

    #region workspace Sync
    private int ObjectInitCounter = 0;
    public string generateNSObjectID()
    {
        return "" + location + ObjectInitCounter++;
    }

    internal void updateObject(string uid, Vector3 position, Quaternion rotation)
    {
        if (_NSObjects.ContainsKey(uid))
        {
            GameObject o = _NSObjects[uid];
            o.transform.localPosition = position;
            o.transform.localRotation = rotation;
        }
    }

    internal void lockObject(string uid)
    {
        if (_NSObjects.ContainsKey(uid))
        {
            _NSObjects[uid].GetComponent<NSObject>().lockStatus = LockType.Remote;
        }
    }

    internal void unlockObject(string uid)
    {
        if (_NSObjects.ContainsKey(uid))
        {
            _NSObjects[uid].GetComponent<NSObject>().lockStatus = LockType.NotLocked;
        }
    }

    private void _createLocalObject(string description)
    {
        string uid = generateNSObjectID();
        GameObject o = _createObject(description, uid);
        _NSNetwork.instantiateObject(description, uid);
    }

    public void createRemoteObject(string description, string uid)
    {
        GameObject o = _createObject(description, uid);
    }

    private GameObject _createObject(string description, string uid)
    {
        GameObject o;
        if (description == "cube")
        {
            o = GameObject.CreatePrimitive(PrimitiveType.Cube);
            o.transform.parent = origin;
            o.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            o.transform.localPosition = Vector3.zero;
            o.transform.localRotation = Quaternion.identity;

        }
        else if (description == "helmet")
        {
            o = (GameObject) Instantiate(Resources.Load("prefabs/Helmet"), Vector3.zero, Quaternion.identity);
            o.transform.parent = origin;
            o.transform.localPosition = Vector3.zero;
            o.transform.localRotation = Quaternion.Euler(-90, 180, 0);
            o.name = "Helmet";
        }
        else
        {
            o = new GameObject();
        }

        
        o.AddComponent<NSObject>();
        NSObject ns = o.GetComponent<NSObject>();
        ns.name = uid;
        _NSObjects.Add(uid, o);
        return o;
    }
    #endregion
}
