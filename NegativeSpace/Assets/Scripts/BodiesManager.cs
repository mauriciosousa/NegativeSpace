using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodiesManager : MonoBehaviour {

    private Dictionary<string, Human> _humans;
    private PerspectiveProjection _projectionScript;

    private bool _humanLocked = false;
    public Human human = null;

    void Start()
    {
        _humans = new Dictionary<string, Human>();
        _projectionScript = Camera.main.GetComponent<PerspectiveProjection>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            _humanLocked = !_humanLocked;
        }

        if (_humans.Count > 0)
        {
            if (human != null &&_humanLocked && _humans.ContainsKey(human.id))
            {
                human = _humans[human.id];
            }
            else
            {
                _humanLocked = false;
                Vector3 surface = _projectionScript.getSurfaceBaryCenter();
                Human newHuman = null;
                foreach (Human h in _humans.Values)
                {
                    if (newHuman == null)
                    {
                        newHuman = h;
                    }
                    else
                    {
                        if (Vector3.Distance(h.body.Joints[BodyJointType.head], surface) < Vector3.Distance(newHuman.body.Joints[BodyJointType.head], surface))
                        {
                            newHuman = h;
                        }
                    }
                }
                human = newHuman;
            }
            Camera.main.transform.position = human.body.Joints[BodyJointType.head];
        }

        // finally
        _cleanDeadHumans();
    }

    public void setNewFrame(Body[] bodies)
    {
        foreach (Body b in bodies)
        {
            try
            {
                string bodyID = b.Properties[BodyPropertiesType.UID];
                if (!_humans.ContainsKey(bodyID))
                {
                    _humans.Add(bodyID, new Human());
                }
                _humans[bodyID].Update(b);
            }
            catch (Exception e) { }
        }
    }

    void _cleanDeadHumans()
    {
        List<Human> deadhumans = new List<Human>();

        foreach (Human h in _humans.Values)
        {
            if (DateTime.Now > h.lastUpdated.AddMilliseconds(1000))
                deadhumans.Add(h);
        }

        foreach (Human h in deadhumans)
        {
            _humans.Remove(h.id);
        }
    }
}
