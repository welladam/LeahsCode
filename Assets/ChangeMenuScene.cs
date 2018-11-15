using Gamekit3D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMenuScene : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void StartGame()
    {
        TransitionPoint transitionPoint = new TransitionPoint();
        transitionPoint.newSceneName = "Level01";
        transitionPoint.transitionDestinationTag = SceneTransitionDestination.DestinationTag.A;
        transitionPoint.transitionType = TransitionPoint.TransitionType.DifferentZone;

        SceneController.TransitionToScene(transitionPoint);
    }
}
