﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LockType
{
    Local, Remote, NotLocked
}

public class NSObject : MonoBehaviour
{

    public LockType lockStatus;

    void Start()
    {
        lockStatus = LockType.NotLocked;
    }

    void Update()
    {

    }
}
