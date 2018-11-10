using System.Collections;
using System.Collections.Generic;
using Gamekit3D;
using UnityEngine;
using UnityEngine.Playables;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MenuCanvasUI : MonoBehaviour
{

    public GameObject pauseCanvas;
    public GameObject optionsCanvas;
    public GameObject audioCanvas;

    protected bool m_InPause;
    protected PlayableDirector[] m_Directors;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        m_Directors = FindObjectsOfType<PlayableDirector>();
    }

    public void Quit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
		    Application.Quit();
#endif
    }

    public void ExitPause()
    {
        m_InPause = true;
        SwitchPauseState();
    }

    void Update()
    {
        if (PlayerInput.Instance != null && PlayerInput.Instance.Pause)
        {
            SwitchPauseState();
        }
    }

    protected void SwitchPauseState()
    {
        Cursor.lockState = m_InPause ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !m_InPause;

        for (int i = 0; i < m_Directors.Length; i++)
        {
            if (m_Directors[i].state == PlayState.Playing && !m_InPause)
            {
                m_Directors[i].Pause();
            }
            else if (m_Directors[i].state == PlayState.Paused && m_InPause)
            {
                m_Directors[i].Resume();
            }
        }

        if (!m_InPause)
            CameraShake.Stop();

        if (m_InPause)
            PlayerInput.Instance.GainControl();
        else
            PlayerInput.Instance.ReleaseControl();

        Time.timeScale = m_InPause ? 1 : 0;

        if (pauseCanvas)
            pauseCanvas.SetActive(!m_InPause);

        if (optionsCanvas)
            optionsCanvas.SetActive(false);

        if (audioCanvas)
            audioCanvas.SetActive(false);


        if (m_InPause && InteractPuzzleCrystalBox.isInPuzzleGame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            PlayerInput.Instance.ReleaseControl();
        }

        if(m_InPause && DialogsController.isPlayerDialoging)
        {
            Cursor.lockState = CursorLockMode.None;
            PlayerInput.Instance.ReleaseControl();
        }

        m_InPause = !m_InPause;
    }
}
