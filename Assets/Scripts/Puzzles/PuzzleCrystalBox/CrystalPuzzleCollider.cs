using Gamekit3D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalPuzzleCollider : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        Damageable d = other.GetComponentInChildren<Damageable>();
        Damageable.DamageMessage damage = new Damageable.DamageMessage()
        {
            amount = 1,
            damager = this,
            direction = Vector3.up,
        };

        if(d != null) { 
        d.ApplyDamage(damage);
            }
    }
}
