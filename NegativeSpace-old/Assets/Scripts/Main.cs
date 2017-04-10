using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{


    public SurfaceRectangle surfaceRectangle;

    private NegativeSpace _negativeSpace;
    private NSProperties _properties;
    private RPCNetwork _NSNetwork;
    private PerspectiveProjection _projection;

    public bool showUI = false;


    public GUIStyle _titleStyle;
    public GUIStyle _normalStyle;

    

    void Awake()
    {
        Application.runInBackground = true;
        surfaceRectangle = null;
    }

	void Start ()
    {
        _properties = GetComponent<NSProperties>();


        //_negativeSpace = GameObject.Find("NegativeSpace").GetComponent<NegativeSpace>();

        //_NSNetwork = GameObject.Find("NegativeSpace").GetComponent<RPCNetwork>();
        //_projection = Camera.main.GetComponent<PerspectiveProjection>();
    }

    void Update () {

        if (Input.GetKeyDown(KeyCode.F1))
        {
            showUI = !showUI;
        }


        if (Input.GetKeyDown(KeyCode.R))
        {

            GameObject.Find("AVESTRUZ").transform.RotateAround(Camera.main.GetComponent<PerspectiveProjection>().getSurfaceBaryCenter(), Vector3.up, 180);
        }

	}

    void OnGUI()
    {
        if (showUI)
        {
            GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");

            int top = 10;
            int left = 10;
            int newLine = 20;
            int tabShift = 120;

            GUI.Label(new Rect(left, top, 200, 30), "Configured Location:", _titleStyle);
            GUI.Label(new Rect(left + tabShift, top, 200, 30), _negativeSpace.location.ToString(), _normalStyle);
            top += newLine;

            GUI.Label(new Rect(left, top, 200, 30), "Network Status:", _titleStyle);
            GUI.Label(new Rect(left + tabShift, top, 200, 30), _NSNetwork.ConnectionStatus, _normalStyle);
            top += newLine;

            GUI.Label(new Rect(left, top, 200, 30), "Screen Orientation:", _titleStyle);
            GUI.Label(new Rect(left + tabShift, top, 200, 30), _projection.screenOrientation.ToString(), _normalStyle);
            top += newLine;

            if (_negativeSpace.NSObsjects.Count > 0)
            {
                GUI.Label(new Rect(left, top, 200, 30), "NSObjects:", _titleStyle);
                string nso = "";
                foreach (GameObject o in _negativeSpace.NSObsjects.Values)
                {
                    nso += o.name + "(" + o.GetComponent<NSObject>().lockStatus.ToString() + "), ";
                }
                GUI.Label(new Rect(left + tabShift, top, Screen.width, 500), nso, _normalStyle);

            }
        }
    }

    internal void receiveSurface(string stringToParse)
    {
        string[] s = stringToParse.Split(MessageSeparators.L1);
        string name = s[0];
        Vector3 bl = convertRemoteStringToVector3(s[1]);
        Vector3 br = convertRemoteStringToVector3(s[2]);
        Vector3 tl = convertRemoteStringToVector3(s[3]);
        Vector3 tr = convertRemoteStringToVector3(s[4]);

        surfaceRectangle = new SurfaceRectangle(bl, br, tl, tr);
    }

    internal static Vector3 convertRemoteStringToVector3(string v)
    {
        string[] p = v.Split(MessageSeparators.L3);
        return new Vector3(float.Parse(p[0].Replace(',', '.')), float.Parse(p[1].Replace(',', '.')), float.Parse(p[2].Replace(',', '.')));
    }
}
