using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogPersonController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ChangeState()
    {
        GetComponent<Animator>().SetBool("DialogFinished", true);
    }
}
