using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPuzzleController : MonoBehaviour {

    static public bool mustShowForCommandChoices = false;
    public GameObject commands;
    public GameObject forCommandChoices;
    public GameObject buttons;

    // Use this for initialization
    void Start () {
        forCommandChoices.SetActive(false);
        buttons.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
        commands.SetActive(!mustShowForCommandChoices);
        forCommandChoices.SetActive(mustShowForCommandChoices);
        buttons.SetActive(mustShowForCommandChoices);
    }
}
