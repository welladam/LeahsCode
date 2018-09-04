using Gamekit3D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManipulate : MonoBehaviour {

    public GameObject puzzleBox1;
    public GameObject puzzleBox2;
    public GameObject puzzleCrystal;
    
    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Vertical") && Input.GetAxisRaw("Vertical") > 0) { 
            Vector3 crystalPosition = this.puzzleCrystal.transform.position;
            this.puzzleCrystal.transform.position = new Vector3(crystalPosition.x, crystalPosition.y + 0.4f, crystalPosition.z);
    }

        if (Input.GetButtonDown("Vertical") && Input.GetAxisRaw("Vertical") < 0) 
        {
            Vector3 crystalPosition = this.puzzleCrystal.transform.position;
            this.puzzleCrystal.transform.position = new Vector3(crystalPosition.x, crystalPosition.y - 0.4f, crystalPosition.z);
        }

        if (Input.GetButtonDown("Horizontal") && Input.GetAxisRaw("Horizontal") > 0)
        {
            Vector3 crystalPosition = this.puzzleCrystal.transform.position;
            this.puzzleCrystal.transform.position = new Vector3(crystalPosition.x + 0.4f, crystalPosition.y, crystalPosition.z);
        }

        if (Input.GetButtonDown("Horizontal") && Input.GetAxisRaw("Horizontal") < 0)
        {
            Vector3 crystalPosition = this.puzzleCrystal.transform.position;
            this.puzzleCrystal.transform.position = new Vector3(crystalPosition.x - 0.4f, crystalPosition.y, crystalPosition.z);
        }

        if (Input.GetButtonDown("Fire1"))
        {
            Damageable d = puzzleBox1.GetComponentInChildren<Damageable>();
            Damageable.DamageMessage damage = new Damageable.DamageMessage()
            {
                amount = 1,
                damager = this,
                direction = Vector3.up,
            };

            d.ApplyDamage(damage);
        }
    }
}
