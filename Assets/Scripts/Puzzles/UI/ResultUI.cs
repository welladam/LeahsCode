using System.Collections;
using System.Collections.Generic;
using Gamekit3D;
using UnityEngine;
using UnityEngine.Playables;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gamekit3D
{
    public class ResultUI : MonoBehaviour
    {
        public GameObject resultCanvas;

        protected bool m_InPause;
        protected PlayableDirector[] m_Directors;

        void Start()
        {
            m_Directors = FindObjectsOfType<PlayableDirector>();
        }

        public void BackLevel()
        {
            m_InPause = true;
            SwitchPauseState();
            PuzzleManipulate.mustRestartPuzzle = true;
            InteractPuzzleCrystalBox.forceInteractClose = true;
        }

        public void ContinueLevel()
        {
            m_InPause = true;
            SwitchPauseState();
            PuzzleManipulate.mustFinishLevel = true;
        }

        public void RestartLevel()
        {
            m_InPause = true;
            SwitchPauseState();
            PuzzleManipulate.mustRestartPuzzle = true;
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
            if (!m_InPause && ScreenFader.IsFading)
                return;

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

            Time.timeScale = m_InPause ? 1 : 0;

            if (resultCanvas)
                resultCanvas.SetActive(false);

            m_InPause = !m_InPause;
        }
    }
}
