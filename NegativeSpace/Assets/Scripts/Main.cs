using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour {


    public bool init = false;
    

    private Sensors _sensors;
    private int _pov;

    void Awake()
    {
        Application.runInBackground = true;

        _pov = 0;
        _sensors = this.GetComponent<Sensors>();
    }

	void Start () {

    }

    void Update () {

        if (false && Input.GetKeyDown(KeyCode.P))
        {
            if (_sensors.sensorsList.Count > 0)
            {
                if (_pov < _sensors.sensorsList.Count - 1)
                {
                    _pov++;
                }
                else
                {
                    _pov = 0;
                }
                Camera.main.transform.position = _sensors.sensorsList[_pov].transform.position;
                Vector3 r = _sensors.sensorsList[_pov].transform.rotation.eulerAngles;
                r.y = r.y - 180;
                Camera.main.transform.rotation = Quaternion.Euler(r); ;

            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {

            GameObject.Find("AVESTRUZ").transform.RotateAround(Camera.main.GetComponent<PerspectiveProjection>().getSurfaceBaryCenter(), Vector3.up, 180);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            init = true;
        }
	}

    void OnGUI()
    {

    }
}
