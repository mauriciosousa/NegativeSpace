using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensors : MonoBehaviour {

    public List<GameObject> sensorsList;
    public GameObject sensorModels;

    void Awake()
    {
        sensorsList = new List<GameObject>();
        sensorModels = new GameObject();
        sensorModels.name = "Sensor Models";
        sensorModels.transform.parent = gameObject.transform;
    }

    void Start()
    {
        
    }

    void Update()
    {

    }

    public void addSensor(GameObject sensor)
    {
        sensorsList.Add(sensor);
    }

    internal void addSensor(string id, Vector3 vector3, Quaternion quaternion)
    {
        Vector3 r = quaternion.eulerAngles;
        r.y = r.y - 180;

        GameObject go = (GameObject)Instantiate(Resources.Load("Prefabs/kinectModel"), vector3, Quaternion.Euler(r));
        go.name = "sensormodel " + id;
        go.transform.parent = sensorModels.transform;
        addSensor(go);
    }
}
