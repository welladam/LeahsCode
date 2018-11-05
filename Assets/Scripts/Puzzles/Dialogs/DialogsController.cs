using Gamekit3D;
using Gamekit3D.GameCommands;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogsController : MonoBehaviour
{
    public Animator animator;
    public GameObject dialogTextCanvas;
    public TextMeshProUGUI textMeshProUGUI;
    public Camera cameraOriginal;
    public Camera cameraKidJonas;
    public Camera cameraKidCaio;

    public GameObject moneyCanvas;

    public GameCommandReceiver doorStepLevel01Step01;

    protected Coroutine m_DeactivationCoroutine;

    private static bool isPlayerDialoging = false;
    private static string currentText = string.Empty;
    private static int currentLevel = 0;
    private static int currentStep = 0;
    private static int currentIndexTextStep = 0;

    protected readonly int m_HashActivePara = Animator.StringToHash("Active");

    private List<string> listTextLevel01Step01 = new List<string>();
    private List<string> listTextLevel01Step02 = new List<string>();

    private AudioSource audioData;

    private void Start()
    {
        audioData = GetComponent<AudioSource>();

        listTextLevel01Step01.Add("Level01_Step1_INFO1");
        listTextLevel01Step01.Add("Level01_Step1_INFO2");
        listTextLevel01Step01.Add("Level01_Step1_INFO3");
        listTextLevel01Step01.Add("Level01_Step1_INFO4");
        listTextLevel01Step01.Add("Level01_Step1_INFO5");
        listTextLevel01Step01.Add("Level01_Step1_INFO6");

        listTextLevel01Step02.Add("Level01_Step2_INFO1");
        listTextLevel01Step02.Add("Level01_Step2_INFO2");
        listTextLevel01Step02.Add("Level01_Step2_INFO3");
        listTextLevel01Step02.Add("Level01_Step2_INFO4");
        listTextLevel01Step02.Add("Level01_Step2_INFO5");
        listTextLevel01Step02.Add("Level01_Step2_INFO6");

        listTextLevel01Step02.Add("Level01_Step2_INFO6");
    }

    private void Update()
    {
        if (isPlayerDialoging && Input.GetButtonDown("Interact"))
        {
            audioData.Play();
            ManipulateNextLevel();
        }
    }

    private void ManipulateNextLevel()
    {
        if (currentLevel == 1)
        {
            currentIndexTextStep++;

            if (currentStep == 1)
            {
                if (currentIndexTextStep > 5)
                {
                    DesactiveDialogLevel01();
                    SendGameCommand sendGameCommand = new SendGameCommand();

                    sendGameCommand.interactionType = GameCommandType.Open;
                    sendGameCommand.interactiveObject = doorStepLevel01Step01;
                    sendGameCommand.coolDown = 1;
                    sendGameCommand.oneShot = true;
                    sendGameCommand.Send();

                    moneyCanvas.SetActive(true);
                    return;
                }

                ActiveDialogLevel01(listTextLevel01Step01[currentIndexTextStep]);
                return;
            }

            if (currentStep == 2)
            {
                if (currentIndexTextStep > 5)
                {
                    DesactiveDialogLevel01();
                    return;
                }

                ActiveDialogLevel01Step2(listTextLevel01Step02[currentIndexTextStep]);
                return;
            }
        }
    }

    IEnumerator SetAnimatorParameterWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        animator.SetBool(m_HashActivePara, false);
    }

    public void ActiveDialogLevel01(string phraseKey)
    {
        currentLevel = 1;
        currentStep = 1;
        cameraKidJonas.enabled = true;

        ActivateCanvasWithTranslatedText(phraseKey);
    }

    public void ActiveDialogLevel01Step2(string phraseKey)
    {
        currentLevel = 1;
        currentStep = 2;
        cameraKidCaio.enabled = true;

        ActivateCanvasWithTranslatedText(phraseKey);
    }

    public void ActiveDialogLevel01Step3(string phraseKey)
    {
        currentLevel = 1;
        currentStep = 3;

        ActivateCanvasWithTranslatedText(phraseKey);
    }

    private void ActivateCanvasWithTranslatedText(string phraseKey)
    {
        currentText = phraseKey;
        isPlayerDialoging = true;

        if (m_DeactivationCoroutine != null)
        {
            StopCoroutine(m_DeactivationCoroutine);
            m_DeactivationCoroutine = null;
        }

        gameObject.SetActive(true);
        animator.SetBool(m_HashActivePara, true);
        textMeshProUGUI.text = Translator.Instance[phraseKey];
        dialogTextCanvas.SetActive(true);
        PlayerInput.Instance.ReleaseControl();
        cameraOriginal.enabled = false;
    }

    public void DesactiveDialogLevel01(float delay = 0f)
    {
        cameraKidJonas.enabled = false;
        cameraKidCaio.enabled = false;
        DeactivateCanvasWithDelay(delay);
    }

    private void DeactivateCanvasWithDelay(float delay)
    {
        m_DeactivationCoroutine = StartCoroutine(SetAnimatorParameterWithDelay(delay));

        currentLevel = 0;
        currentStep = 0;
        currentIndexTextStep = 0;

        dialogTextCanvas.SetActive(false);
        PlayerInput.Instance.GainControl();
        cameraOriginal.enabled = true;
        isPlayerDialoging = false;
    }
}
