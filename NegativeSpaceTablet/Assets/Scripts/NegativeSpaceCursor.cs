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

	void Start ()
    {
        Application.runInBackground = true;
        _udpBroadcast = new UDPBroadcast(port);
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
        if (Click)
        {
            Vector3 e = Input.mousePosition;
            GUI.DrawTexture(new Rect(e.x - TextureSize / 2, Screen.height - e.y - TextureSize / 2, TextureSize, TextureSize), texture);
        }
    }
}
