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

    public HandType handType;

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
        handType = HandType.Unknown;
    }

    void Update ()
    {
    }

    internal void updateValues(Vector3 head, Vector3 hand, Vector3 nsSize, Quaternion handheldRotation, bool Click)
    {
        if (surface == null) return;

        transform.position = hand;

        if (pointingTechnique == PointingTechnique.HeadHandVectorRotation)
        {
            Vector3 pointingDir = (hand - head).normalized;
            ProjectorPointerGO.transform.forward = pointingDir;

            Debug.DrawRay(head, pointingDir);
        }
        else if (pointingTechnique == PointingTechnique.HandheldRotation)
        {
            ProjectorPointerGO.transform.rotation = handheldRotation;
        }

        if (Click)
        {
            Debug.Log("Hand " + handType + " " + Click);
        }
    }

}
