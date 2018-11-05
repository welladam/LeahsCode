﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleCrystalCollider : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		

	}

    private void OnTriggerEnter(Collider collision)
    {
        PuzzleManipulate.objectWithCrystalSamePosition = collision.gameObject;
    }

    private void OnTriggerExit(Collider collision)
    {
        PuzzleManipulate.objectWithCrystalSamePosition = null;
    }

    private void OnCollisionEnter(Collision collision)
    {
        PuzzleManipulate.objectWithCrystalSamePosition = collision.gameObject;
    }

    private void onCollisionExit(Collision collision)
    {
        PuzzleManipulate.objectWithCrystalSamePosition = null;
    }
}
