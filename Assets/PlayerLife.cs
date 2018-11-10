using Gamekit3D;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerLife : MonoBehaviour {

    public Damageable representedDamageable;

    public TextMeshProUGUI heartsText;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        heartsText.text = representedDamageable.currentHitPoints.ToString();
    }
}
