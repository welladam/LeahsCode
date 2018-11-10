using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleCrystalCollider : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {


    }

    private void OnDisable()
    {
        PuzzleManipulate.objectWithCrystalSamePosition = null;
    }

    private void OnTriggerEnter(Collider collision)
    {
        PuzzleManipulate.objectWithCrystalSamePosition = collision.gameObject.activeSelf ? collision.gameObject : null;
    }
}
