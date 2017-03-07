using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NSProperties : MonoBehaviour {

    private NameServerClient _nameServer;
    public string NameServerAddress;
    public int NameServerPort;


    void Awake()
    {
        _nameServer = new NameServerClient(NameServerAddress, NameServerPort);
        

    }

	void Start () {
        
    }
	
	void Update () {
		
	}

    void OnGUI()
    {
        //if (GUI.Button(new Rect(10, 10, 100, 100), ""))
        //{
        //    Debug.Log(_nameServer.Request("tester"));

        //}
    }
}
