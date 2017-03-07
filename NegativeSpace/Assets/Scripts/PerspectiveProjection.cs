using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ScreenOrientation
{
    Portrait,
    Landscape
}

//[ExecuteInEditMode]
public class PerspectiveProjection : MonoBehaviour {

    public ScreenOrientation screenOrientation;

    public GameObject projectionScreen;
    public bool estimateViewFrustum = true;
    public bool setNearClipPlane = false;
    public float nearClipDistanceOffset = -0.01f;

    private bool _DoWeHaveASurface = false;

    public bool SurfaceCalibrated { get { return _DoWeHaveASurface; } }
    private Camera cameraComponent;

    private Vector3 _surfaceBaryCenter = Vector3.zero;

    private SurfaceRectangle surfaceRect;
    private List<GameObject> _surfaceVertices;

    public Transform SurfaceCenter;

    void Start () {
        surfaceRect = new SurfaceRectangle();
        _DoWeHaveASurface = surfaceRect.load();
        _surfaceVertices = new List<GameObject>();

        GameObject screenCenterGO = new GameObject();
        screenCenterGO.name = "ScreenCenter";
        Transform newTrans = screenCenterGO.transform;
        newTrans.position = (surfaceRect.SurfaceBottomLeft + surfaceRect.SurfaceTopRight) * 0.5f;
        Vector3 upwards = (surfaceRect.SurfaceTopLeft - surfaceRect.SurfaceBottomLeft).normalized;
        Vector3 right = (surfaceRect.SurfaceTopRight - surfaceRect.SurfaceTopLeft).normalized;
        newTrans.rotation = Quaternion.LookRotation(Vector3.Cross(upwards, right), upwards);

        SurfaceCenter = newTrans;

        if (_DoWeHaveASurface)
        {
            Debug.Log("We HAVE SURFACE");
            _instantiateSphere(surfaceRect.SurfaceBottomLeft, "BL", SurfaceCenter);
            _instantiateSphere(surfaceRect.SurfaceBottomRight, "BR", SurfaceCenter);
            _instantiateSphere(surfaceRect.SurfaceTopLeft, "TL", SurfaceCenter);
            _instantiateSphere(surfaceRect.SurfaceTopRight, "TR", SurfaceCenter);
        }

        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);//(GameObject)Instantiate(Resources.Load("Prefabs/yoda-go"), Vector3.zero, Quaternion.identity);
        cube.transform.parent = screenCenterGO.transform;
        cube.transform.localPosition = Vector3.zero;// new Vector3(0f, -0.05f, 0f);
        cube.transform.localRotation = Quaternion.identity;
        cube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        cube.GetComponent<Renderer>().enabled = false;
    }

    private void _instantiateSphere(Vector3 position, string name, Transform parent)
    {
        GameObject sphere = GameObject.Find(name);

        if (sphere == null)
        {
            sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.name = name;
        }
        sphere.transform.position = position;
        sphere.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
        sphere.transform.localRotation = Quaternion.identity;
        sphere.GetComponent<MeshRenderer>().enabled = false;
        _surfaceVertices.Add(sphere);

        sphere.transform.parent = parent;
    }

    void Update () {

        if (Input.GetKeyDown(KeyCode.Q))
        {
            foreach (GameObject go in _surfaceVertices)
            {
                go.GetComponent<MeshRenderer>().enabled = !go.GetComponent<MeshRenderer>().enabled;
            }
        }
	}

    internal Vector3 getSurfaceBaryCenter()
    {
        if (_DoWeHaveASurface)
        {
            return (surfaceRect.SurfaceBottomLeft + surfaceRect.SurfaceTopRight) * 0.5f;
        }
        return Vector3.zero;
    }

    void LateUpdate()
    {
        cameraComponent = Camera.main;

        if (null != cameraComponent && _DoWeHaveASurface)
        {

            Debug.DrawLine(surfaceRect.SurfaceTopLeft, surfaceRect.SurfaceTopRight, Color.red);
            Debug.DrawLine(surfaceRect.SurfaceTopLeft, surfaceRect.SurfaceBottomLeft, Color.red);
            Debug.DrawLine(surfaceRect.SurfaceBottomRight, surfaceRect.SurfaceTopRight, Color.red);
            Debug.DrawLine(surfaceRect.SurfaceBottomRight, surfaceRect.SurfaceBottomLeft, Color.red);



            Vector3 pa; // lower left corner in world coordinates
            Vector3 pb; // lower right corner
            Vector3 pc; // upper left corner
            Vector3 pe; // eye position

            if (screenOrientation == ScreenOrientation.Landscape)
            {
                pa = surfaceRect.SurfaceBottomRight;
                pb = surfaceRect.SurfaceBottomLeft;
                pc = surfaceRect.SurfaceTopRight;
            }
            else
            {
                pa = surfaceRect.SurfaceTopRight;
                pb = surfaceRect.SurfaceBottomRight;
                pc = surfaceRect.SurfaceTopLeft;
            }

            pe = transform.position;

            float n = cameraComponent.nearClipPlane;
            float f = cameraComponent.farClipPlane;

            Vector3 va; // from pe to pa
            Vector3 vb; // from pe to pb
            Vector3 vc; // from pe to pc
            Vector3 vr; // right axis of screen
            Vector3 vu; // up axis of screen
            Vector3 vn; // normal vector of screen

            float l; // distance to left screen edge
            float r; // distance to right screen edge
            float b; // distance to bottom screen edge
            float t; // distance to top screen edge
            float d; // distance from eye to screen 

            vr = pb - pa;
            vu = pc - pa;
            va = pa - pe;
            vb = pb - pe;
            vc = pc - pe;

            // are we looking at the backface of the plane object?
            if (Vector3.Dot(-Vector3.Cross(va, vc), vb) < 0.0f)
            {
                // mirror points along the z axis (most users 
                // probably expect the x axis to stay fixed)
                vu = -vu;
                pa = pc;
                pb = pa + vr;
                pc = pa + vu;
                va = pa - pe;
                vb = pb - pe;
                vc = pc - pe;
            }

            vr.Normalize();
            vu.Normalize();
            vn = -Vector3.Cross(vr, vu);
            // we need the minus sign because Unity 
            // uses a left-handed coordinate system
            vn.Normalize();

            d = -Vector3.Dot(va, vn);
            if (setNearClipPlane)
            {
                n = d + nearClipDistanceOffset;
                cameraComponent.nearClipPlane = n;
            }
            l = Vector3.Dot(vr, va) * n / d;
            r = Vector3.Dot(vr, vb) * n / d;
            b = Vector3.Dot(vu, va) * n / d;
            t = Vector3.Dot(vu, vc) * n / d;

            Matrix4x4 p = new Matrix4x4();
            p[0, 0] = 2.0f * n / (r - l);
            p[0, 1] = 0.0f;
            p[0, 2] = (r + l) / (r - l);
            p[0, 3] = 0.0f;

            p[1, 0] = 0.0f;
            p[1, 1] = 2.0f * n / (t - b);
            p[1, 2] = (t + b) / (t - b);
            p[1, 3] = 0.0f;

            p[2, 0] = 0.0f;
            p[2, 1] = 0.0f;
            p[2, 2] = (f + n) / (n - f);
            p[2, 3] = 2.0f * f * n / (n - f);

            p[3, 0] = 0.0f;
            p[3, 1] = 0.0f;
            p[3, 2] = -1.0f;
            p[3, 3] = 0.0f;

            Matrix4x4 rm = new Matrix4x4(); // rotation matrix;
            rm[0, 0] = vr.x;
            rm[0, 1] = vr.y;
            rm[0, 2] = vr.z;
            rm[0, 3] = 0.0f;

            rm[1, 0] = vu.x;
            rm[1, 1] = vu.y;
            rm[1, 2] = vu.z;
            rm[1, 3] = 0.0f;

            rm[2, 0] = vn.x;
            rm[2, 1] = vn.y;
            rm[2, 2] = vn.z;
            rm[2, 3] = 0.0f;

            rm[3, 0] = 0.0f;
            rm[3, 1] = 0.0f;
            rm[3, 2] = 0.0f;
            rm[3, 3] = 1.0f;

            Matrix4x4 tm = new Matrix4x4(); // translation matrix;
            tm[0, 0] = 1.0f;
            tm[0, 1] = 0.0f;
            tm[0, 2] = 0.0f;
            tm[0, 3] = -pe.x;

            tm[1, 0] = 0.0f;
            tm[1, 1] = 1.0f;
            tm[1, 2] = 0.0f;
            tm[1, 3] = -pe.y;

            tm[2, 0] = 0.0f;
            tm[2, 1] = 0.0f;
            tm[2, 2] = 1.0f;
            tm[2, 3] = -pe.z;

            tm[3, 0] = 0.0f;
            tm[3, 1] = 0.0f;
            tm[3, 2] = 0.0f;
            tm[3, 3] = 1.0f;

            // set matrices
            cameraComponent.projectionMatrix = p;
            cameraComponent.worldToCameraMatrix = rm * tm;
            // The original paper puts everything into the projection 
            // matrix (i.e. sets it to p * rm * tm and the other 
            // matrix to the identity), but this doesn't appear to 
            // work with Unity's shadow maps

            if (estimateViewFrustum)
            {
                // rotate camera to screen for culling to work
                Quaternion q = new Quaternion();
                q.SetLookRotation((0.5f * (pb + pc) - pe), vu);
                // look at center of screen
                cameraComponent.transform.rotation = q;

                // set fieldOfView to a conservative estimate 
                // to make frustum tall enough
                if (cameraComponent.aspect >= 1.0)
                {
                    cameraComponent.fieldOfView = Mathf.Rad2Deg *
                       Mathf.Atan(((pb - pa).magnitude + (pc - pa).magnitude)
                       / va.magnitude);
                }
                else
                {
                    // take the camera aspect into account to 
                    // make the frustum wide enough 
                    cameraComponent.fieldOfView =
                       Mathf.Rad2Deg / cameraComponent.aspect *
                       Mathf.Atan(((pb - pa).magnitude + (pc - pa).magnitude)
                       / va.magnitude);
                }
            }
        }
    }

}
