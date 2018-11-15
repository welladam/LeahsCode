using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ExitGameScreenFader : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(GameOverController.finishGame)
        {
            if(Input.GetButtonDown("Cancel"))
            {
#if UNITY_EDITOR
                EditorApplication.isPlaying = false;
#else
		    Application.Quit();
#endif
            }
        }
	}
}
