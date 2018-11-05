using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPuzzleController : MonoBehaviour {

    static public bool mustShowForCommandChoices = false;
    static public bool mustShowIfConditionsChoices = false;
    static public bool mustShowIfCommandChoices = false;

    public GameObject commands;
    public GameObject forCommandChoices;
    public GameObject ifCommandChoices;
    public GameObject ifConditionsChoices;
    public GameObject backButton;
    public GameObject confirmButton;


    // Use this for initialization
    void Start () {
        this.setDefaultValues();
    }
	
	// Update is called once per frame
	void Update () {       
        if (mustShowForCommandChoices)
        {
            commands.SetActive(false);
            forCommandChoices.SetActive(true);
            backButton.SetActive(true);
            confirmButton.SetActive(true);
            return;
        }

        if (mustShowIfConditionsChoices)
        {
            commands.SetActive(false);
            ifConditionsChoices.SetActive(true);
            backButton.SetActive(true);
            return;
        }

        if (mustShowIfCommandChoices)
        {
            commands.SetActive(false);
            ifConditionsChoices.SetActive(false);
            ifCommandChoices.SetActive(true);
            backButton.SetActive(true);
            return;
        }

        this.setDefaultValues();
    }

    private void setDefaultValues()
    {
        commands.SetActive(true);
        forCommandChoices.SetActive(false);
        ifCommandChoices.SetActive(false);
        ifConditionsChoices.SetActive(false);
        backButton.SetActive(false);
        confirmButton.SetActive(false);
    }
}
