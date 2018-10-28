﻿using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit3D;

[RequireComponent(typeof(Collider))]
public class InteractPuzzleCrystalBox : MonoBehaviour {

    public static bool forceInteractClose = false;

    public LayerMask layers;
    public TextMesh enterTextButton;

    public Camera cameraOriginal;
    public Camera cameraFocus;

    public GameObject uiPuzzle;

    private bool hasPlayerInArea;

	// Use this for initialization
	void Start () {
        this.enterTextButton.gameObject.SetActive(false);
        this.hasPlayerInArea = false;
        this.uiPuzzle.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetButton("Interact") && hasPlayerInArea)
        {              
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            PlayerInput.Instance.ReleaseControl();
            cameraOriginal.enabled = false;
            cameraFocus.enabled = true;
            this.uiPuzzle.SetActive(true);
            enterTextButton.gameObject.SetActive(false);
        }

        if (forceInteractClose || (Input.GetButton("Interact Close") && hasPlayerInArea))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            PlayerInput.Instance.GainControl();
            cameraOriginal.enabled = true;
            cameraFocus.enabled = false;
            this.uiPuzzle.SetActive(false);
            enterTextButton.gameObject.SetActive(true);
            forceInteractClose = false;
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (0 != (layers.value & 1 << other.gameObject.layer))
        {
            this.enterTextButton.gameObject.SetActive(true);
            this.hasPlayerInArea = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (0 != (layers.value & 1 << other.gameObject.layer))
        {
            this.enterTextButton.gameObject.SetActive(false);
            this.hasPlayerInArea = false;
        }
    }
}
