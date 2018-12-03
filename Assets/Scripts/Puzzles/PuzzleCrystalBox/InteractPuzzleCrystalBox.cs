using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit3D;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class InteractPuzzleCrystalBox : MonoBehaviour
{

    public static bool isInPuzzleGame = false;

    public LayerMask layers;
    public TextMesh enterTextButton;
    public GameObject ellen;
    public Camera cameraOriginal;
    public Camera cameraFocus;
    public GameObject uiPuzzle;
    public bool mustOpenTutorialWhenStart = false;
    public bool alreadyCompletedPuzzle = false;

    public UnityEvent OnPlayerInArea;

    private bool hasPlayerInArea;
    private bool mustHideEnterText = false;



    // Use this for initialization
    void Start()
    {
        this.enterTextButton.gameObject.SetActive(false);
        this.hasPlayerInArea = false;
        this.uiPuzzle.SetActive(false);
    }

    private void Update()
    {
        if (alreadyCompletedPuzzle)
        {
            enterTextButton.text = "Você já completou esse enigma!";
        }

        if (!isInPuzzleGame && !alreadyCompletedPuzzle && Input.GetButton("Interact") && hasPlayerInArea)
        {
            ellen.transform.localScale = new Vector3(0, 1, 0);
            //ellen.transform.position = new Vector3(ellen.transform.position.x + 1f, ellen.transform.position.y + 1f, ellen.transform.position.z + 1f);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            PlayerInput.Instance.ReleaseControl();
            cameraOriginal.enabled = false;
            cameraFocus.enabled = true;
            this.uiPuzzle.SetActive(true);
            enterTextButton.gameObject.SetActive(false);

            mustHideEnterText = true;
            isInPuzzleGame = true;

            if (mustOpenTutorialWhenStart)
            {
                ButtonDownScript.forceOpenIfTutorial = true;
            }

            OnPlayerInArea.Invoke();
            PuzzleManipulate.mustRestartPuzzle = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (0 != (layers.value & 1 << other.gameObject.layer))
        {
            this.enterTextButton.gameObject.SetActive(!mustHideEnterText);
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

    public void AlreadyCompletedPuzzle()
    {
        alreadyCompletedPuzzle = true;
    }

    public void ClosePuzzle()
    {
        ellen.transform.localScale = new Vector3(1, 1, 1);
        //ellen.transform.position = new Vector3(ellen.transform.position.x, ellen.transform.position.y + 1f, ellen.transform.position.z);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        PlayerInput.Instance.GainControl();
        cameraOriginal.enabled = true;
        cameraFocus.enabled = false;
        this.uiPuzzle.SetActive(false);
        enterTextButton.gameObject.SetActive(true);
        mustHideEnterText = false;
        isInPuzzleGame = false;
        PuzzleManipulate.mustRestartPuzzle = true;
        OnPlayerInArea.Invoke();
    }
}
