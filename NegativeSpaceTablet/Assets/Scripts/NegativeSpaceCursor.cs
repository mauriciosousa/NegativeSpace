using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DominantHandType
{
    Left,
    Right
}

public class NegativeSpaceCursor : MonoBehaviour {

    public string address;
    public int port;
    public string EncriptKey;

    private UDPUnicast _udp = null;
    public int sendRate = 1000;

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

    public DominantHandType dominantHand = DominantHandType.Right;

    public Texture buttonTexture;



    void Start ()
    {
        Application.runInBackground = true;
        _udp = new UDPUnicast(address, port, sendRate);

        newAddress = address;
        newPort = "" + port;

        _titleStyle = new GUIStyle();
        _titleStyle.fontStyle = FontStyle.Bold;
        _titleStyle.normal.textColor = Color.white;
        _titleStyle.fontSize = 60;

        _normalText = new GUIStyle();
        _normalText.fontSize = 60;
        _normalText.normal.textColor = Color.white;

        _buttonText = new GUIStyle();
        _buttonText.fontSize = 70;
        _buttonText.normal.textColor = Color.yellow;
        _buttonText.alignment = TextAnchor.UpperRight;

        _yText = new GUIStyle();
        _yText.fontSize = 60;
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

        if (!Input.gyro.enabled) return;

        Click = Input.GetMouseButton(0);
        
        string toSend =
            "hand=" + (dominantHand == DominantHandType.Right ? "Right" : "Left") + "/"
            + "click=" + Click + "/"
            + "r.x=" + gameObject.transform.rotation.x + "/"
            + "r.y=" + gameObject.transform.rotation.y + "/"
            + "r.z=" + gameObject.transform.rotation.z + "/"
            + "r.w=" + gameObject.transform.rotation.w;

        toSend.Replace(",", ".");
        toSend = DataEncryptor.Encrypt(toSend, EncriptKey);

        if (!ShowConfig)
        {
            _udp.send(toSend);
        }
    }

    private static Quaternion ConvertRotation(Quaternion q)
    {
        return new Quaternion(q.x, q.y, -q.z, -q.w);
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(0, Screen.height - 250, 250, 250), ShowConfig ? Network_ON : Network_OFF, GUIStyle.none))
        {
            ShowConfig = !ShowConfig;
        }

        if (ShowConfig)
        {
            int newLine = 110;
            int width = 1500;
            int height = newLine;
            int bheight = height + ((int)0.5f) * height;


            int top = 5;
            int left = 100;

            GUI.Label(new Rect(left, top, width, height), "Sending to: " + address + ":" + port, _normalText); top += newLine;
            GUI.Label(new Rect(left, top, width, height), "Gyro: " + Input.gyro.enabled, Input.gyro.enabled ? _normalText : _red); top += newLine;
            if (Input.gyro.enabled) GUI.Label(new Rect(left, top, width, height), "Attitude: " + Input.gyro.attitude, _normalText); top += newLine;
            if (Input.gyro.enabled) GUI.Label(new Rect(left, top, width, height), "Rotation: " + gameObject.transform.rotation.ToString(), _normalText); top += newLine;

            GUI.Label(new Rect(left, top, width, height), "x = " + gameObject.transform.rotation.eulerAngles.x, _red); top += newLine;
            GUI.Label(new Rect(left, top, width, height), "y = " + gameObject.transform.rotation.eulerAngles.y, _green); top += newLine;
            GUI.Label(new Rect(left, top, width, height), "z = " + gameObject.transform.rotation.eulerAngles.z, _blue); top += newLine;

            top += newLine;

            GUI.Label(new Rect(left, top, width, height), "Network Settings:", _titleStyle);
            top += newLine; top += newLine;
            GUI.Label(new Rect(left + 100, top, width, height), "UDP Address:", _normalText);
            newAddress = GUI.TextField(new Rect(Screen.width - 450, top, 400, bheight), newAddress, _buttonText);
            top += newLine; top += newLine;
            GUI.Label(new Rect(left + 100, top, width, height), "UDP Port:", _normalText);
            newPort = GUI.TextField(new Rect(Screen.width - 450, top, 400, bheight), newPort, _buttonText);

            top += newLine; top += newLine;
            if (GUI.Button(new Rect(Screen.width - 550, top, 500, bheight), "Reset", _buttonText))
            {

                int p;
                if (int.TryParse(newPort, out p))
                {
                    port = p;
                    address = newAddress;
                    _udp = new UDPUnicast(address, port, sendRate);
                }
            }

            top += newLine; top += newLine;

            GUI.Label(new Rect(left, top, width, height), "Dominant Hand:", _titleStyle);
            if (GUI.Button(new Rect(Screen.width - 550, top, 500, bheight), dominantHand.ToString(), _buttonText))
            {
                if (dominantHand == DominantHandType.Right)
                {
                    dominantHand = DominantHandType.Left;
                }
                else dominantHand = DominantHandType.Right;
            }

            top += newLine; top += newLine;
            GUI.Label(new Rect(left, top, width, height), "Reset Gyro Calibration:", _titleStyle);
            if (GUI.Button(new Rect(Screen.width - 550, top, 500, bheight), "Reset", _buttonText))
            {
                //gameObject.GetComponent<NSGyroController>().AttachGyro();
                gameObject.GetComponent<NSGyroController>().UpdateCalibration(true);
            }

        }
        else if (Click)
        {
            Vector3 e = Input.mousePosition;
            GUI.DrawTexture(new Rect(e.x - TextureSize / 2, Screen.height - e.y - TextureSize / 2, TextureSize, TextureSize), texture);
        }
    }
}
