using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NegativeSpaceCursor : MonoBehaviour {

    public int port;
    public string EncriptKey;

    private UDPBroadcast _udpBroadcast = null;

    public bool Click = false;

    public Texture texture;
    public int TextureSize;

    public bool ShowConfig;
    public Texture Network_ON;
    public Texture Network_OFF;

    private GUIStyle _titleStyle;
    private GUIStyle _normalText;
    private GUIStyle _buttonText;

    private string newPort = "";

    void Start ()
    {
        Application.runInBackground = true;
        _udpBroadcast = new UDPBroadcast(port);
        newPort = "" + port;

        _titleStyle = new GUIStyle();
        _titleStyle.fontStyle = FontStyle.Bold;
        _titleStyle.normal.textColor = Color.white;
        _titleStyle.fontSize = 100;

        _normalText = new GUIStyle();
        _normalText.fontSize = 100;
        _normalText.normal.textColor = Color.white;

        _buttonText = new GUIStyle();
        _buttonText.fontSize = 100;
        _buttonText.normal.textColor = Color.blue;



    }
	
	void Update ()
    {
        Click = Input.GetMouseButton(0);
        Vector3 rotation = Input.acceleration;

        string toSend = 
            "click=" + Click + "/"
            + "r.x=" + rotation.x + "/"
            + "r.y=" + rotation.y + "/"
            + "r.z=" + rotation.z;
        toSend.Replace(",", ".");
        //Debug.Log(toSend);
        toSend = DataEncryptor.Encrypt(toSend, EncriptKey);
        _udpBroadcast.send(toSend);
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(0, Screen.height - 250, 250, 250), ShowConfig ? Network_ON : Network_OFF, GUIStyle.none))
        {
            ShowConfig = !ShowConfig;
        }

        if (ShowConfig)
        {
            int top = 100;
            int left = 100;


            GUI.Label(new Rect(left, top, 2000, 250), "sensing to port " + port, _normalText);
            top += 250;
            top += 250;
            GUI.Label(new Rect(left, top, 2000, 300), "Network Settings:", _titleStyle);
            top += 250;
            left += 100;
            GUI.Label(new Rect(left, top, 2000, 300), "UDP Port:", _normalText);
            left += 500;
            newPort = GUI.TextField(new Rect(left, top, 300, 200), newPort, _normalText);
            top += 250;


            if (GUI.Button(new Rect(left, top, 500, 250), "Reset", _buttonText))
            {
                GUI.Box(new Rect(left, top, 500, 250), "");


                int p;
                if (int.TryParse(newPort, out p))
                {
                    port = p;
                    _udpBroadcast = new UDPBroadcast(port);
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
