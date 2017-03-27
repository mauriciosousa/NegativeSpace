using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{


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
    }

	void Start ()
    {
        _negativeSpace = GameObject.Find("NegativeSpace").GetComponent<NegativeSpace>();
        _properties = GetComponent<NSProperties>();
        _NSNetwork = GameObject.Find("NegativeSpace").GetComponent<RPCNetwork>();
        _projection = Camera.main.GetComponent<PerspectiveProjection>();
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
            int newLine = 25;



            GUI.Label(new Rect(left, top, 200, 30), "Network Status:", _titleStyle);
            GUI.Label(new Rect(left + 100, top, 200, 30), _NSNetwork.ConnectionStatus, _normalStyle);
            top += newLine;

            GUI.Label(new Rect(left, top, 200, 30), "Screen Orientation:", _titleStyle);
            GUI.Label(new Rect(left + 100, top, 200, 30), _projection.screenOrientation.ToString(), _normalStyle);
            top += newLine;

            if (_negativeSpace.NSObsjects.Count > 0)
            {
                GUI.Label(new Rect(left, top, 200, 30), "NSObjects:", _titleStyle);
                string nso = "";
                foreach (GameObject o in _negativeSpace.NSObsjects.Values)
                {
                    nso += o.name + "(" + o.GetComponent<NSObject>().lockStatus.ToString() + "), ";
                }
                GUI.Label(new Rect(left + 100, top, Screen.width, 500), nso, _normalStyle);

            }
        }
    }
}
