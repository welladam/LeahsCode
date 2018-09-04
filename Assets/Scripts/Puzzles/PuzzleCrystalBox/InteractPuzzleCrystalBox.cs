using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit3D;

[RequireComponent(typeof(Collider))]
public class InteractPuzzleCrystalBox : MonoBehaviour {

    public LayerMask layers;
    public TextMesh textButton;
    public Transform objectOnFocus;
    public CameraSettings cameraRig;

    private bool hasPlayerInArea;

	// Use this for initialization
	void Start () {
        this.textButton.gameObject.SetActive(false);
        this.hasPlayerInArea = false;
    }

    private void Update()
    {
        if (Input.GetButton("Interact") && hasPlayerInArea)
        {
            cameraRig.follow = objectOnFocus;
            cameraRig.lookAt = objectOnFocus;
            cameraRig.keyboardAndMouseCamera.Priority = 0;
            cameraRig.controllerCamera.Priority = 1;
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (0 != (layers.value & 1 << other.gameObject.layer))
        {
            this.textButton.gameObject.SetActive(true);
            this.hasPlayerInArea = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (0 != (layers.value & 1 << other.gameObject.layer))
        {
            this.textButton.gameObject.SetActive(false);
            this.hasPlayerInArea = false;
        }
    }
}
