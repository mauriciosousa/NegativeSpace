using System;
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

public class NegativeSpace : MonoBehaviour {

    private Transform origin;

    private PerspectiveProjection _perspectiveProjection;
    private bool _spaceCreated = false;

    private BodiesManager _bodiesManager;

    public float NegativeSpaceDepth = 0.1f;
    public Material NegativeSpaceMaterial;


    public GameObject leftCursor;
    private NSCursor _leftCursorScript;

    private UDPHandheldListener _handheldListener;
    public int handheldListenPort;
    public string DecryptKey;

    public GameObject rightCursor;
    private NSCursor _rightCursorScript;

    private NegativespaceSurface _surface = null;

    public Vector3 NegativeSpaceSize;

    public bool Handheld_CLICK = false;

    void Awake()
    {
        _perspectiveProjection = Camera.main.GetComponent<PerspectiveProjection>();
        _leftCursorScript = leftCursor.GetComponent<NSCursor>();
        _rightCursorScript = rightCursor.GetComponent<NSCursor>();

        _handheldListener = new UDPHandheldListener(handheldListenPort, DecryptKey);
    }

	void Start ()
    {
        origin = GameObject.Find("ScreenCenter").transform;
        _bodiesManager = GameObject.Find("BodiesManagerGO").GetComponent<BodiesManager>();
        _surface = new NegativespaceSurface();
    }

    void Update ()
    {
        if (_perspectiveProjection.SurfaceCalibrated)
        {
            if (!_spaceCreated)
            {
                gameObject.transform.parent = _perspectiveProjection.SurfaceCenter;
                gameObject.transform.localPosition = Vector3.zero;
                gameObject.transform.localRotation = Quaternion.identity;

                GameObject BL = GameObject.Find("BL");
                GameObject BR = GameObject.Find("BR");
                GameObject TR = GameObject.Find("TR");
                GameObject TL = GameObject.Find("TL");

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
                _rightCursorScript.surface = _surface;

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
                

                _spaceCreated = true;
                Debug.Log("Negative Space Created");

                _createCube(Vector3.zero);
                _createCube(new Vector3(0,0, 0.1f));
                _createCube(new Vector3(0, 0.1f, 0));
                _createCube(new Vector3(0.1f, 0, 0));
            }

            // Negative Space stuff
            if (_bodiesManager.human != null)
            {
                Vector3 head = _bodiesManager.human.body.Joints[BodyJointType.head];
                Vector3 leftHand = _bodiesManager.human.body.Joints[BodyJointType.leftHandTip];
                Vector3 rightHand = _bodiesManager.human.body.Joints[BodyJointType.rightHandTip];


                _leftCursorScript.updateValues(head, leftHand, NegativeSpaceSize, _handheldListener);
                _rightCursorScript.updateValues(head, rightHand, NegativeSpaceSize, _handheldListener);


                if (_handheldListener.Receiving)
                {
                    if (_handheldListener.Message.Click)
                    {
                        Handheld_CLICK = true;
                    }
                    else
                    {
                        Handheld_CLICK = false;
                    }
                }
            }
        }
    }

    private void _createCube(Vector3 vector3)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.parent = origin;
        cube.transform.localPosition = convertToNSCoordinates(vector3);
        cube.transform.localRotation = Quaternion.identity;
        cube.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        //cube.GetComponent<Renderer>().enabled = false;
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
}
