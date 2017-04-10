using UnityEngine;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Text;

public class SurfaceRequestListener : MonoBehaviour
{
    private Properties _properties;

    void Awake()
    {
        _properties = GetComponent<Properties>();
    }
	
	void Update ()
    {
		
	}
}
