using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PointingTechnique
{
    HandheldRotation,
    HeadHandVectorRotation
}


public class NSCursor : MonoBehaviour {


    public PointingTechnique pointingTechnique;

    public GameObject ProjectorPointerGO;

    public NegativespaceSurface surface = null;

    private float _origin;

    //[Range(0.1f, 1f)]
    //public float scaleFactor = 1f;

    public GameObject SelectedObject
    {
        get
        {
            return ProjectorPointerGO.GetComponent<ProjectionClipper>().HitObject;
        }
    }

    void Start ()
    {
        _origin = 0f;
    }

    void Update ()
    {
    }
    
    internal void updateValues(Vector3 head, Vector3 hand, Vector3 nsSize, UDPHandheldListener handheldListener)
    {
        if (surface == null) return;

        transform.position = hand;

        if (pointingTechnique == PointingTechnique.HeadHandVectorRotation)
        {
            Vector3 pointingDir = (hand - head).normalized;
            ProjectorPointerGO.transform.forward = pointingDir;
        }
        else if (pointingTechnique == PointingTechnique.HandheldRotation)
        {
            if (handheldListener.Receiving)
            {
                ProjectorPointerGO.transform.forward = handheldListener.Message.Rotation.eulerAngles;
            }
        }


        /**
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
    */



    }

}
