using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NegativeSpaceCursor : MonoBehaviour {

    public string address;
    public int port;
    public string EncriptKey;

    private UDPUnicast _udp = null;

    public bool Click = false;

    public Texture texture;
    public int TextureSize;

    public bool ShowConfig;
    public Texture Network_ON;
    public Texture Network_OFF;

    private GUIStyle _titleStyle;
    private GUIStyle _normalText;
    private GUIStyle _yText;
    private GUIStyle _buttonText;


    private GUIStyle _red;
    private GUIStyle _green;
    private GUIStyle _blue;


    private string newAddress = "";
    private string newPort = "";

    private Vector3 _accel;

    void Start ()
    {
        Application.runInBackground = true;
        _udp = new UDPUnicast(address, port);

        _accel = Vector3.zero;

        newAddress = address;
        newPort = "" + port;

        _titleStyle = new GUIStyle();
        _titleStyle.fontStyle = FontStyle.Bold;
        _titleStyle.normal.textColor = Color.white;
        _titleStyle.fontSize = 70;

        _normalText = new GUIStyle();
        _normalText.fontSize = 70;
        _normalText.normal.textColor = Color.white;

        _buttonText = new GUIStyle();
        _buttonText.fontSize = 70;
        _buttonText.normal.textColor = Color.yellow;

        _yText = new GUIStyle();
        _yText.fontSize = 70;
        _yText.normal.textColor = Color.yellow;

        _red = new GUIStyle();
        _red.fontSize = 60;
        _red.normal.textColor = Color.red;

        _green = new GUIStyle();
        _green.fontSize = 60;
        _green.normal.textColor = Color.green;

        _blue = new GUIStyle();
        _blue.fontSize = 60;
        _blue.normal.textColor = Color.blue;
    }
	
	void Update ()
    {
        Click = Input.GetMouseButton(0);
        _accel = Input.acceleration;

        string toSend = 
            "click=" + Click + "/"
            + "r.x=" + _accel.x + "/"
            + "r.y=" + _accel.y + "/"
            + "r.z=" + _accel.z;
        toSend.Replace(",", ".");
        //Debug.Log(toSend);
        toSend = DataEncryptor.Encrypt(toSend, EncriptKey);
        _udp.send(toSend);
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(0, Screen.height - 250, 250, 250), ShowConfig ? Network_ON : Network_OFF, GUIStyle.none))
        {
            ShowConfig = !ShowConfig;
        }

        if (ShowConfig)
        {

            int top = 50;
            int left = 100;

            GUI.Label(new Rect(left, top, 500, 100), "x = " + _accel.x, _red);
            top += 70;
            GUI.Label(new Rect(left, top, 500, 100), "y = " + _accel.y, _green);
            top += 70;
            GUI.Label(new Rect(left, top, 500, 100), "z = " + _accel.z, _blue);
            top += 70;

            top += 350;
            top += 250;
            GUI.Label(new Rect(left, top, 2000, 300), "Network Settings:", _titleStyle);
            top += 250;
            left += 100;
            GUI.Label(new Rect(left, top, 2000, 300), "UDP Address:", _normalText);
            newAddress = GUI.TextField(new Rect(left + 500, top, 300, 200), newAddress, _yText);
            top += 150;
            GUI.Label(new Rect(left, top, 2000, 300), "UDP Port:", _normalText);
            newPort = GUI.TextField(new Rect(left + 500, top, 300, 200), newPort, _yText);
            top += 250;


            if (GUI.Button(new Rect(left, top, 500, 250), "Reset", _buttonText))
            {
                GUI.Box(new Rect(left, top, 500, 250), "");


                int p;
                if (int.TryParse(newPort, out p))
                {
                    port = p;
                    _udp = new UDPUnicast(address, port);
                }
            }

        }
        else if (Click)
        {
            Vector3 e = Input.mousePosition;
            GUI.DrawTexture(new Rect(e.x - TextureSize / 2, Screen.height - e.y - TextureSize / 2, TextureSize, TextureSize), texture);
        }
    }
}
