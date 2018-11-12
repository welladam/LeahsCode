using Gamekit3D;
using Gamekit3D.GameCommands;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogsController : MonoBehaviour
{
    public static bool isPlayerDialoging = false;

    public Animator animator;
    public GameObject dialogTextCanvas;
    public TextMeshProUGUI textMeshProUGUI;
    public Camera cameraOriginal;
    public Camera cameraKidJonas;
    public Camera cameraKidCaio;
    public Camera cameraKidJulia;
    public GameObject moneyCanvas;
    public GameCommandReceiver doorStepLevel01Step01;
    public List<GameObject> listMonster;

    public bool isWarningMessage = false;

    protected Coroutine m_DeactivationCoroutine;
    
    private static string currentText = string.Empty;
    private static int currentLevel = 0;
    private static int currentStep = 0;
    private static int currentIndexTextStep = 0;

    protected readonly int m_HashActivePara = Animator.StringToHash("Active");

    private List<string> listTextLevel01Step01 = new List<string>();
    private List<string> listTextLevel01Step02 = new List<string>();
    private List<string> listTextLevel01Step03 = new List<string>();

    private bool isPlayerWarning = false;
    private bool mustForceClose = false;

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
    
        listTextLevel01Step03.Add("Level01_Step3_INFO1");
        listTextLevel01Step03.Add("Level01_Step3_INFO2");
        listTextLevel01Step03.Add("Level01_Step3_INFO3");
        listTextLevel01Step03.Add("Level01_Step3_INFO4");
        listTextLevel01Step03.Add("Level01_Step3_INFO5");
    }

    private void Update()
    {
        if (!isWarningMessage && isPlayerDialoging && Input.GetButtonDown("Interact"))
        {
            audioData.Play();
            ManipulateNextLevel();
        }

        if (mustForceClose && isPlayerWarning && isWarningMessage)
        {
            DeactivateCanvasWithDelay(5f);
            isPlayerWarning = false;
            mustForceClose = false;
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

                ActivateCanvasWithTranslatedText(listTextLevel01Step01[currentIndexTextStep]);
                return;
            }

            if (currentStep == 2)
            {
                if (currentIndexTextStep > 5)
                {
                    DesactiveDialogLevel01();
                    return;
                }

                ActivateCanvasWithTranslatedText(listTextLevel01Step02[currentIndexTextStep]);
                return;
            }

            if (currentStep == 3)
            {
                if (currentIndexTextStep > 4)
                {
                    DesactiveDialogLevel01();
                    return;
                }

                ActivateCanvasWithTranslatedText(listTextLevel01Step03[currentIndexTextStep]);
                return;
            }
        }

        DeactivateCanvasWithDelay(0f);
    }

    IEnumerator SetAnimatorParameterWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        animator.SetBool(m_HashActivePara, false);
    }

    public void ActiveDialogMaxActionsAllowed()
    {
        mustForceClose = true;
        ActivateCanvasWithTranslatedText("NumMaxActionsAllowed_INFO");
    }

    public void ActiveDialogLevel01(string phraseKey)
    {
        if (!cameraKidJonas.enabled)
        {
            currentLevel = 1;
            currentStep = 1;
            cameraKidJonas.enabled = true;
        }

        ActivateCanvasWithTranslatedText(phraseKey);
    }

    public void ActiveDialogLevel01Step2(string phraseKey)
    {
        bool canGoToNextStep = true;

        listMonster.ForEach(monster => { if (monster != null) canGoToNextStep = false; });

        if (canGoToNextStep)
        {
            if (!cameraKidCaio.enabled)
            {
                currentLevel = 1;
                currentStep = 2;
                cameraKidCaio.enabled = true;

                ActivateCanvasWithTranslatedText(phraseKey);
            }
        }
    }

    public void ActiveDialogLevel01Step3(string phraseKey)
    {
        if (!cameraKidJulia.enabled)
        {
            currentLevel = 1;
            currentStep = 3;
            cameraKidJulia.enabled = true;
        }

        ActivateCanvasWithTranslatedText(phraseKey);
    }

    public void ActiveTutorialIFPuzzleCrystalBoxLevel01Step02(GameObject tutorialIFPuzzleCrystalBox)
    {
        bool canActiveThePuzzle = true;

        listMonster.ForEach(monster => { if (monster != null) canActiveThePuzzle = false; });

        if(canActiveThePuzzle)
        {
            tutorialIFPuzzleCrystalBox.SetActive(true);
        }
    }


    public void ActiveDialogTutorial(string phraseKey)
    {
        ActivateCanvasWithTranslatedText(phraseKey);
    }

    public void ActivateCanvasWithTranslatedText(string phraseKey)
    {
        currentText = phraseKey;      

        if (isWarningMessage)
        {
            isPlayerWarning = true;
        }
        else
        {
            isPlayerDialoging = true;
        }

        if (m_DeactivationCoroutine != null)
        {
            StopCoroutine(m_DeactivationCoroutine);
            m_DeactivationCoroutine = null;
        }

        gameObject.SetActive(true);
        animator.SetBool(m_HashActivePara, true);
        textMeshProUGUI.text = Translator.Instance[phraseKey];

        if (!isWarningMessage)
        {
            dialogTextCanvas.SetActive(true);
            PlayerInput.Instance.ReleaseControl();
            cameraOriginal.enabled = false;
        }
    }

    public void DesactivateDialogLevel01Step02(GameObject infoZone)
    {
        bool canDesativateStep = true;

        listMonster.ForEach(monster => { if (monster != null) canDesativateStep = false; });

        if(canDesativateStep)
        {
            infoZone.SetActive(false);
        }
    }

    public void DesactiveDialogLevel01(float delay = 0f)
    {
        cameraKidJonas.enabled = false;
        cameraKidCaio.enabled = false;
        cameraKidJulia.enabled = false;
        DeactivateCanvasWithDelay(delay);
    }

    public void DeactivateCanvasWithDelay(float delay)
    {
        m_DeactivationCoroutine = StartCoroutine(SetAnimatorParameterWithDelay(delay));

        currentLevel = 0;
        currentStep = 0;
        currentIndexTextStep = 0;

        if (dialogTextCanvas != null)
        {
            dialogTextCanvas.SetActive(false);
        }

        if (!isWarningMessage)
        {
            PlayerInput.Instance.GainControl();
            cameraOriginal.enabled = true;
        }

        isPlayerDialoging = false;
    }

}
