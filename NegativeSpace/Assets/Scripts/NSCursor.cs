using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NSCursor : MonoBehaviour {

    public GameObject cursor;

    public NegativespaceSurface surface = null;

    private float _origin;

    [Range(0.1f, 1f)]
    public float scaleFactor = 1f;

    private Vector3 _lastHandPosition;

    void Start () {
        //cursor.GetComponent<Renderer>().enabled = false;
        _origin = 0f;
        _lastHandPosition = Vector3.one * float.NegativeInfinity;
    }

    void Update () {
		
	}

    internal void updateValues(Vector3 head, Vector3 hand, Vector3 nsSize)
    {
        if (surface == null) return;

        if(_lastHandPosition.x != float.NegativeInfinity)
        {
            gameObject.transform.localPosition += gameObject.transform.worldToLocalMatrix.MultiplyVector(hand - _lastHandPosition);

            gameObject.transform.localPosition = new Vector3(
                Mathf.Clamp(gameObject.transform.localPosition.x, -nsSize.x / 2.0f, nsSize.x / 2.0f), 
                Mathf.Clamp(gameObject.transform.localPosition.y, -nsSize.y / 2.0f, nsSize.y / 2.0f), 
                Mathf.Clamp(gameObject.transform.localPosition.z, 0, nsSize.z)
                );
        }

        _lastHandPosition = hand;

        /*if (hand.z > surface.BR.z && hand.z < surface.BL.z
            && hand.y > surface.BR.y && hand.y < surface.TR.y)
        {
            if (!cursor.GetComponent<Renderer>().enabled)
            {
                // Surface Enter
                _origin = hand.x;
            }

            float midSpaceOrigin = ((surface.BL + surface.sBL) * 0.5f).x;

            //cursor.transform.position = new Vector3(midSpaceOrigin - scaleFactor*(_origin - hand.x), hand.y, hand.z);
            cursor.transform.position = hand;


            cursor.GetComponent<Renderer>().enabled = true;
        }
        else
        {
            if (cursor.GetComponent<Renderer>().enabled)
            {
                // Surface Exit
            }

            cursor.transform.position = hand;

            cursor.GetComponent<Renderer>().enabled = false;
        }*/
    }
}
