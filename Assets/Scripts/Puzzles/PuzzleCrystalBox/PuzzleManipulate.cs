using Gamekit3D;
using Gamekit3D.GameCommands;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PuzzleManipulate : MonoBehaviour
{
    public GameObject puzzleBox1;
    public GameObject puzzleBox2;
    public GameObject puzzleBox3;
    public GameObject healthBox;
    public GameCommandReceiver door;

    public GameObject puzzleMain;
    public GameObject uIPuzzleWin;
    public GameObject uIPuzzleFail;
    public GameObject panelGameObject;

    public bool skipNumMaxActionsAllowedValidation = false;
    public bool mustStartIfTutorial = false;
    public int countBoxMustDetroy;
    public bool mustFoundKey = false;
    public List<GameObject> listFloorsCantPass;

    public int sizeXMatrix = 3;
    public int sizeYMatrix = 3;
    public List<GameObject> listGameObjectFloors;

    public int maxCommands3Stars;
    public int maxCommands2Stars;

    public UnityEvent OnPuzzleCompleted;
    public UnityEvent OnPuzzleClosed;
    public UnityEvent OnValidateNumMaxActionsAllowed;

    static public List<CommandPuzzle> listCommands = new List<CommandPuzzle>();
    static public bool puzzleInProgress = false;
    static public bool startPuzzleControl = false;
    static public bool showNextStepTutorial = false;
    static public bool mustRestartPuzzle = false;
    static public bool mustFinishLevel = false;
    static public bool refreshResultContentPanel = false;
    static public int starsPuzzleFinished = 0;

    static public GameObject objectWithCrystalSamePosition;

    private bool hasPlayerInArea = false;

    private GameObject puzzleBox1Source = null;
    private GameObject puzzleBox2Source = null;
    private GameObject puzzleBox3Source = null;

    private List<GameObject> generatedObjects = new List<GameObject>();
    private List<GameObject> generatedPanelsObjects = new List<GameObject>();
    private TextMeshProUGUI currentTextSelected;
    private int delayTime = 1;

    private int currentFloorX = 0;
    private int currentFloorY = 0;
    private GameObject[,] matrixFloors;

    // Mission Objectives
    private int boxesDesroyed = 0;
    private bool keyFounded = false;


    // Use this for initialization
    void Start()
    {
        boxesDesroyed = countBoxMustDetroy;
        matrixFloors = new GameObject[sizeXMatrix, sizeYMatrix];

        int countListFloors = 0;

        for (int i = 0; i < sizeYMatrix; i++)
        {
            for (int j = 0; j < sizeXMatrix; j++)
            {
                matrixFloors[j, i] = listGameObjectFloors[countListFloors];
                countListFloors++;
            }
        }

        if (puzzleBox1 != null)
        {
            puzzleBox1Source = Object.Instantiate(puzzleBox1, puzzleMain.transform);
            puzzleBox1Source.SetActive(false);
        }

        if (puzzleBox2 != null)
        {
            puzzleBox2Source = Object.Instantiate(puzzleBox2, puzzleMain.transform);
            puzzleBox2Source.SetActive(false);
        }

        if (puzzleBox3 != null)
        {
            puzzleBox3Source = Object.Instantiate(puzzleBox3, puzzleMain.transform);
            puzzleBox3Source.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(StartPuzzle());
        CommandSelected();
        RestartLevel();
        FinishLevel();
        ControlCrystal();

        if (refreshResultContentPanel && hasPlayerInArea)
        {
            ClearAndCreateResultContent();
            refreshResultContentPanel = false;
        }
    }

    public void SetHasPlayerInArea()
    {
        hasPlayerInArea = !hasPlayerInArea;
    }

    private void ControlCrystal()
    {
        if (mustStartIfTutorial && InteractPuzzleCrystalBox.isInPuzzleGame && hasPlayerInArea)
        {
            bool mustChangePosition = false;

            GameObject currentCrystal = GetCurrentCrystal();
            int oldFloorY = currentFloorY;
            int oldFloorX = currentFloorX;

            if (Input.GetButtonDown("Vertical") && Input.GetAxisRaw("Vertical") > 0)
            {
                currentFloorY++;
                mustChangePosition = true;
            }

            if (Input.GetButtonDown("Vertical") && Input.GetAxisRaw("Vertical") < 0)
            {
                currentFloorY--;
                mustChangePosition = true;
            }

            if (Input.GetButtonDown("Horizontal") && Input.GetAxisRaw("Horizontal") > 0)
            {
                currentFloorX++;
                mustChangePosition = true;
            }

            if (Input.GetButtonDown("Horizontal") && Input.GetAxisRaw("Horizontal") < 0)
            {
                currentFloorX--;
                mustChangePosition = true;
            }

            if ((currentFloorX > 2 || currentFloorX < 0) || (currentFloorY > 2 || currentFloorY < 0))
            {
                currentFloorY = oldFloorY;
                currentFloorX = oldFloorX;
                return;
            }

            if (mustChangePosition)
            {
                currentCrystal.SetActive(false);

                GameObject newCrystal = GetCurrentCrystal();
                newCrystal.SetActive(true);
            }
        }
    }

    private void FinishLevel()
    {
        if (mustFinishLevel && hasPlayerInArea)
        {
            OnPuzzleCompleted.Invoke();
            OnPuzzleClosed.Invoke();

            if (door != null)
            {
                SendGameCommand sendGameCommand = new SendGameCommand();

                sendGameCommand.interactionType = GameCommandType.Open;
                sendGameCommand.interactiveObject = door;
                sendGameCommand.coolDown = 1;
                sendGameCommand.oneShot = true;
                sendGameCommand.Send();
            }

            if (healthBox != null)
            {
                healthBox.GetComponent<InteractHealthBox>().OnInteractHealthBox();
            }

            mustFinishLevel = false;
        }
    }

    private void RestartLevel()
    {
        if (mustRestartPuzzle && hasPlayerInArea)
        {
            ClearAllCrystals();
            ClearResultContent();

            listCommands.Clear();

            currentFloorX = 0;
            currentFloorY = 0;

            GetCurrentCrystal().SetActive(true);

            for (int i = 0; i < generatedObjects.Count; i++)
            {
                Destroy(generatedObjects[i]);
            }

            if (puzzleBox1Source != null)
            {
                Destroy(puzzleBox1);
                puzzleBox1 = Object.Instantiate(puzzleBox1Source, puzzleMain.transform);
                puzzleBox1.SetActive(true);
            }

            if (puzzleBox2Source != null)
            {
                Destroy(puzzleBox2);
                puzzleBox2 = Object.Instantiate(puzzleBox2Source, puzzleMain.transform);
                puzzleBox2.SetActive(true);
            }

            if (puzzleBox3Source != null)
            {
                Destroy(puzzleBox3);
                puzzleBox3 = Object.Instantiate(puzzleBox3Source, puzzleMain.transform);
                puzzleBox3.SetActive(true);
            }

            mustRestartPuzzle = false;
        }
    }

    public void CommandSelected()
    {
        if (showNextStepTutorial && hasPlayerInArea)
        {
            showNextStepTutorial = false;

            foreach (GameObject gameObject in generatedObjects)
            {
                Destroy(gameObject);
            }

            GetCurrentCrystalSimulator().SetActive(false);

            currentFloorX = 0;
            currentFloorY = 0;

            foreach (CommandPuzzle commandPuzzleMain in listCommands)
            {
                if (commandPuzzleMain.commandType == "for")
                {
                    for (int i = 0; i < commandPuzzleMain.countLoop; i++)
                    {
                        for (int j = 0; j < commandPuzzleMain.commandsFor.Count; j++)
                        {
                            CommandFor commandFor = commandPuzzleMain.commandsFor[j];
                            GameObject currentCrystalSimulator = GetCurrentCrystalSimulator();

                            if (commandFor.command == "moveUp")
                            {
                                currentFloorY++;
                            }

                            if (commandFor.command == "moveDown")
                            {
                                currentFloorY--;
                            }

                            if (commandFor.command == "turnRight")
                            {
                                currentFloorX++;
                            }

                            if (commandFor.command == "turnLeft")
                            {
                                currentFloorX--;
                            }

                            if ((currentFloorX >= sizeXMatrix || currentFloorX < 0) || (currentFloorY >= sizeYMatrix || currentFloorY < 0) || !ValidateMustAddCommand(j))
                            {
                                commandPuzzleMain.commandsFor.RemoveAt(j);
                                ClearOnlyCrystalSimulators();
                                currentFloorX = 0;
                                currentFloorY = 0;
                                VerifyMustDeleteCommand(commandPuzzleMain);
                                return;
                            }

                            currentCrystalSimulator.SetActive(false);

                            GameObject newCrystalSimulator = GetCurrentCrystalSimulator();
                            Color color = newCrystalSimulator.GetComponent<Renderer>().material.color;
                            newCrystalSimulator.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 1);

                            newCrystalSimulator.SetActive(true);
                            showNextStepTutorial = false;
                        }
                    }
                }
            }

            this.ClearAndCreateResultContent();
        }
    }

    public IEnumerator StartPuzzle()
    {
        if (startPuzzleControl && hasPlayerInArea)
        {
            puzzleInProgress = true;
            startPuzzleControl = false;

            foreach (GameObject prefab in generatedObjects)
            {
                Destroy(prefab);
            }

            GetCurrentCrystalSimulator().SetActive(false);

            currentFloorX = 0;
            currentFloorY = 0;

            foreach (CommandPuzzle commandPuzzleMain in listCommands)
            {
                if (commandPuzzleMain.commandType == "if")
                {
                    if (currentTextSelected != null)
                    {
                        currentTextSelected.fontSize = 25;
                        currentTextSelected.color = Color.white;
                        currentTextSelected.fontStyle = FontStyles.Normal;
                    }

                    currentTextSelected = commandPuzzleMain.panelResult.GetComponentsInChildren<TextMeshProUGUI>()[2];
                    currentTextSelected.fontSize = 30;
                    currentTextSelected.color = new Color32(103, 255, 93, 255);
                    currentTextSelected.fontStyle = FontStyles.Bold;

                    yield return new WaitForSeconds(delayTime);

                    this.ConditionAction(commandPuzzleMain);

                    if (currentTextSelected != null)
                    {
                        currentTextSelected.fontSize = 25;
                        currentTextSelected.color = Color.white;
                        currentTextSelected.fontStyle = FontStyles.Normal;
                    }

                    currentTextSelected = commandPuzzleMain.panelResult.GetComponentsInChildren<TextMeshProUGUI>()[4];
                    currentTextSelected.fontSize = 30;
                    currentTextSelected.color = new Color32(103, 255, 93, 255);
                    currentTextSelected.fontStyle = FontStyles.Bold;

                    yield return new WaitForSeconds(delayTime);
                }

                if (commandPuzzleMain.commandType == "for")
                {
                    for (int i = 0; i < commandPuzzleMain.countLoop; i++)
                    {
                        for (int j = 0; j < commandPuzzleMain.commandsFor.Count; j++)
                        {
                            if (currentTextSelected != null)
                            {
                                currentTextSelected.fontSize = 25;
                                currentTextSelected.color = Color.white;
                                currentTextSelected.fontStyle = FontStyles.Normal;
                            }

                            currentTextSelected = commandPuzzleMain.panelResult.GetComponentsInChildren<TextMeshProUGUI>()[j + 2];
                            currentTextSelected.fontSize = 30;
                            currentTextSelected.color = new Color32(103, 255, 93, 255);
                            currentTextSelected.fontStyle = FontStyles.Bold;

                            GetCurrentCrystal().SetActive(false);

                            string command = commandPuzzleMain.commandsFor[j].command;
                            if (command == "moveUp")
                            {
                                currentFloorY++;
                            }

                            if (command == "moveDown")
                            {
                                currentFloorY--;
                            }

                            if (command == "turnRight")
                            {
                                currentFloorX++;
                            }

                            if (command == "turnLeft")
                            {
                                currentFloorX--;
                            }

                            GetCurrentCrystal().SetActive(true);

                            yield return new WaitForSeconds(delayTime);
                        }
                    }
                }
            }

            yield return new WaitForSeconds(delayTime);
            this.ValidateFinishPuzzle();
            puzzleInProgress = false;
        }
    }


    private GameObject GetCurrentCrystal(int? valueX = null, int? valueY = null)
    {
        int currentX = valueX != null ? (int)valueX : currentFloorX;
        int currentY = valueY != null ? (int)valueY : currentFloorY;

        return matrixFloors[currentX, currentY].transform.Find("PuzzleCrystal").gameObject;
    }

    private GameObject GetCurrentCrystalSimulator(int? valueX = null, int? valueY = null)
    {
        int currentX = valueX != null ? (int)valueX : currentFloorX;
        int currentY = valueY != null ? (int)valueY : currentFloorY;

        return matrixFloors[currentX, currentY].transform.Find("PuzzleCrystalSimulator").gameObject;
    }

    private void ConditionAction(CommandPuzzle commandPuzzleMain)
    {
        if (commandPuzzleMain.commandCondition == "foundKey" && commandPuzzleMain.command == "getKey")
        {
            if (objectWithCrystalSamePosition != null && objectWithCrystalSamePosition.tag == "Key")
            {
                Destroy(objectWithCrystalSamePosition);
                keyFounded = true;
            }
            return;
        }

        if (commandPuzzleMain.commandCondition == "foundBox" && commandPuzzleMain.command == "destroyBox")
        {
            try
            {
                Damageable d = objectWithCrystalSamePosition.GetComponentInChildren<Damageable>() == null ?
                    objectWithCrystalSamePosition.GetComponentInParent<Damageable>() : objectWithCrystalSamePosition.GetComponentInChildren<Damageable>();

                if (d == null)
                {
                    d = objectWithCrystalSamePosition.GetComponent<Damageable>();
                }

                if (d != null)
                {
                    Damageable.DamageMessage damage = new Damageable.DamageMessage()
                    {
                        amount = 999,
                        damager = this,
                        direction = Vector3.up,
                    };

                    d.ApplyDamage(damage);
                    boxesDesroyed--;
                }
            }
            catch (System.Exception ex) { }
        }
    }

    private bool ValidateMustAddCommand(int indexCommandFor)
    {
        if (listFloorsCantPass.Count > 0)
        {
            GameObject floor = matrixFloors[currentFloorX, currentFloorY];
            if (listFloorsCantPass.Contains(floor))
            {
                return false;
            }
        }

        if (!skipNumMaxActionsAllowedValidation && listCommands.Count > 0)
        {
            int countCommands = listCommands[listCommands.Count - 1].commandsFor.Count;

            if (countCommands > 3 && countCommands - 1 == indexCommandFor)
            {
                OnValidateNumMaxActionsAllowed.Invoke();
                return false;
            }

        }

        return true;
    }

    private void ValidateFinishPuzzle()
    {
        bool objectivesFinished = false;

        if (countBoxMustDetroy > 0 && boxesDesroyed == 0)
        {
            objectivesFinished = true;
        }

        if (mustFoundKey && keyFounded)
        {
            objectivesFinished = true;
        }

        if (objectivesFinished)
        {
            uIPuzzleWin.SetActive(true);
        }
        else
        {
            uIPuzzleFail.SetActive(true);
        }
    }

    private void ClearOnlyCrystalSimulators()
    {
        for (int i = 0; i < sizeYMatrix; i++)
        {
            for (int j = 0; j < sizeXMatrix; j++)
            {
                GetCurrentCrystalSimulator(j, i).SetActive(false);
            }
        }
    }

    private void ClearAllCrystals()
    {
        for (int i = 0; i < sizeYMatrix; i++)
        {
            for (int j = 0; j < sizeXMatrix; j++)
            {
                GetCurrentCrystal(j, i).SetActive(false);
                GetCurrentCrystalSimulator(j, i).SetActive(false);
            }
        }
    }

    private void createResultItems()
    {
        foreach (CommandPuzzle commandPuzzle in listCommands)
        {
            GameObject panel = new GameObject("Panel");
            panel.AddComponent<CanvasRenderer>();

            RectTransform panelRectTransform = panel.AddComponent<RectTransform>();
            panelRectTransform.sizeDelta = new Vector2(520f, 100f);

            Image panelImageBackground = panel.AddComponent<Image>();
            panelImageBackground.sprite = (Sprite)Resources.Load("Images/BoardResult", typeof(Sprite));

            VerticalLayoutGroup panelVerticalLayoutGroup = panel.AddComponent<VerticalLayoutGroup>();
            panelVerticalLayoutGroup.padding.top = 30;
            panelVerticalLayoutGroup.padding.bottom = 30;
            panelVerticalLayoutGroup.padding.left = 30;
            panelVerticalLayoutGroup.padding.right = 30;

            ContentSizeFitter panelContentSizeFitter = panel.AddComponent<ContentSizeFitter>();
            panelContentSizeFitter.verticalFit = ContentSizeFitter.FitMode.MinSize;

            GameObject textTitle = new GameObject("TextMeshProUGUI");
            textTitle.AddComponent<CanvasRenderer>();
            textTitle.AddComponent<RectTransform>();

            VerticalLayoutGroup textTitleVerticalLayoutGroup = textTitle.AddComponent<VerticalLayoutGroup>();
            textTitleVerticalLayoutGroup.padding.top = 10;
            textTitleVerticalLayoutGroup.padding.bottom = 60;
            textTitleVerticalLayoutGroup.padding.left = 10;
            textTitleVerticalLayoutGroup.padding.right = 10;

            ContentSizeFitter textTitleContentSizeFitter = textTitle.AddComponent<ContentSizeFitter>();
            textTitleContentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            TextMeshProUGUI textTitleMeshPro = textTitle.AddComponent<TextMeshProUGUI>();
            textTitleMeshPro.text = commandPuzzle.commandTypeName;
            textTitleMeshPro.fontStyle = FontStyles.Bold;
            textTitleMeshPro.color = Color.red;
            textTitleMeshPro.fontSize = 40;

            textTitle.transform.SetParent(panel.transform, false);

            if (commandPuzzle.commandType == "if")
            {
                // Title
                GameObject titleConditionText = new GameObject("TextMeshProUGUI");
                titleConditionText.AddComponent<CanvasRenderer>();
                titleConditionText.AddComponent<RectTransform>();

                VerticalLayoutGroup titleConditionTextVerticalLayoutGroup = titleConditionText.AddComponent<VerticalLayoutGroup>();
                titleConditionTextVerticalLayoutGroup.padding.top = 10;
                titleConditionTextVerticalLayoutGroup.padding.bottom = 20;
                titleConditionTextVerticalLayoutGroup.padding.left = 10;
                titleConditionTextVerticalLayoutGroup.padding.right = 10;

                ContentSizeFitter titleConditionTextContentSizeFitter = titleConditionText.AddComponent<ContentSizeFitter>();
                titleConditionTextContentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

                TextMeshProUGUI titleConditionTextMeshPro = titleConditionText.AddComponent<TextMeshProUGUI>();
                titleConditionTextMeshPro.text = "Quando";
                titleConditionTextMeshPro.fontSize = 24;
                titleConditionTextMeshPro.color = new Color32(255, 147, 0, 255);

                titleConditionText.transform.SetParent(panel.transform, false);

                // Condition
                GameObject conditionText = new GameObject("TextMeshProUGUI");
                conditionText.AddComponent<CanvasRenderer>();
                conditionText.AddComponent<RectTransform>();

                VerticalLayoutGroup conditionTextVerticalLayoutGroup = conditionText.AddComponent<VerticalLayoutGroup>();
                conditionTextVerticalLayoutGroup.padding.top = 10;
                conditionTextVerticalLayoutGroup.padding.bottom = 40;
                conditionTextVerticalLayoutGroup.padding.left = 60;
                conditionTextVerticalLayoutGroup.padding.right = 10;

                ContentSizeFitter conditionTextContentSizeFitter = conditionText.AddComponent<ContentSizeFitter>();
                conditionTextContentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

                TextMeshProUGUI conditionTextMeshPro = conditionText.AddComponent<TextMeshProUGUI>();
                conditionTextMeshPro.text = commandPuzzle.commandConditionName;
                conditionTextMeshPro.fontSize = 25;

                conditionText.transform.SetParent(panel.transform, false);

                // Title
                GameObject actionTitleText = new GameObject("TextMeshProUGUI");
                actionTitleText.AddComponent<CanvasRenderer>();
                actionTitleText.AddComponent<RectTransform>();

                VerticalLayoutGroup actionTitleTextVerticalLayoutGroup = actionTitleText.AddComponent<VerticalLayoutGroup>();
                actionTitleTextVerticalLayoutGroup.padding.top = 10;
                actionTitleTextVerticalLayoutGroup.padding.bottom = 20;
                actionTitleTextVerticalLayoutGroup.padding.left = 10;
                actionTitleTextVerticalLayoutGroup.padding.right = 10;

                ContentSizeFitter actionTitleTextContentSizeFitter = actionTitleText.AddComponent<ContentSizeFitter>();
                actionTitleTextContentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

                TextMeshProUGUI actionTitleTextMeshPro = actionTitleText.AddComponent<TextMeshProUGUI>();
                actionTitleTextMeshPro.text = "Faça";
                actionTitleTextMeshPro.fontSize = 24;
                actionTitleTextMeshPro.color = new Color32(91, 255, 83, 255);

                actionTitleText.transform.SetParent(panel.transform, false);

                // Command
                GameObject commandText = new GameObject("TextMeshProUGUI");
                commandText.AddComponent<CanvasRenderer>();
                commandText.AddComponent<RectTransform>();

                VerticalLayoutGroup commandTextVerticalLayoutGroup = commandText.AddComponent<VerticalLayoutGroup>();
                commandTextVerticalLayoutGroup.padding.top = 10;
                commandTextVerticalLayoutGroup.padding.bottom = 20;
                commandTextVerticalLayoutGroup.padding.left = 20;
                commandTextVerticalLayoutGroup.padding.right = 10;

                ContentSizeFitter commandTextContentSizeFitter = commandText.AddComponent<ContentSizeFitter>();
                commandTextContentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

                TextMeshProUGUI commandTextMeshPro = commandText.AddComponent<TextMeshProUGUI>();
                commandTextMeshPro.text = commandPuzzle.commandName;
                commandTextMeshPro.fontSize = 25;

                commandText.transform.SetParent(panel.transform, false);
            }
            else if (commandPuzzle.commandType == "for")
            {
                GameObject textSubTitle = new GameObject("TextMeshProUGUI");
                textSubTitle.AddComponent<CanvasRenderer>();
                textSubTitle.AddComponent<RectTransform>();

                VerticalLayoutGroup textSubTitleVerticalLayoutGroup = textSubTitle.AddComponent<VerticalLayoutGroup>();
                textSubTitleVerticalLayoutGroup.padding.top = 0;
                textSubTitleVerticalLayoutGroup.padding.bottom = 40;
                textSubTitleVerticalLayoutGroup.padding.left = 10;
                textSubTitleVerticalLayoutGroup.padding.right = 10;

                ContentSizeFitter textSubTitleContentSizeFitter = textSubTitle.AddComponent<ContentSizeFitter>();
                textSubTitleContentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

                TextMeshProUGUI textCounterMeshPro = textSubTitle.AddComponent<TextMeshProUGUI>();
                textCounterMeshPro.text = "Contador: " + commandPuzzle.countLoop;
                textCounterMeshPro.fontStyle = FontStyles.Bold;
                textCounterMeshPro.color = Color.red;
                textCounterMeshPro.fontSize = 30;

                textCounterMeshPro.transform.SetParent(panel.transform, false);

                foreach (CommandFor commandFor in commandPuzzle.commandsFor)
                {
                    GameObject commandText = new GameObject("TextMeshProUGUI");
                    commandText.AddComponent<CanvasRenderer>();
                    commandText.AddComponent<RectTransform>();

                    VerticalLayoutGroup commandTextVerticalLayoutGroup = commandText.AddComponent<VerticalLayoutGroup>();
                    commandTextVerticalLayoutGroup.padding.top = 10;
                    commandTextVerticalLayoutGroup.padding.bottom = 20;
                    commandTextVerticalLayoutGroup.padding.left = 10;
                    commandTextVerticalLayoutGroup.padding.right = 10;

                    ContentSizeFitter commandTextContentSizeFitter = commandText.AddComponent<ContentSizeFitter>();
                    commandTextContentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

                    TextMeshProUGUI commandTexteMeshPro = commandText.AddComponent<TextMeshProUGUI>();
                    commandTexteMeshPro.text = commandFor.commandName;
                    commandTexteMeshPro.fontSize = 25;

                    commandText.transform.SetParent(panel.transform, false);
                }
            }

            panel.transform.SetParent(panelGameObject.transform, false);

            commandPuzzle.panelResult = panel;
            generatedPanelsObjects.Add(panel);
        }
    }

    public void ClearAndCreateResultContent()
    {
        this.ClearResultContent();
        this.createResultItems();
    }

    public void ClearResultContent()
    {
        foreach (GameObject panel in generatedPanelsObjects)
        {
            Destroy(panel);
        }
    }

    private void VerifyMustDeleteCommand(CommandPuzzle commandPuzzle)
    {
        if (commandPuzzle.commandsFor.Count <= 0)
        {
            listCommands.Remove(commandPuzzle);
        }

        showNextStepTutorial = true;
    }

    public void GetQtdStarsPuzzleFinished()
    {
        if (hasPlayerInArea)
        {
            int numCommands = listCommands.Count;

            if (numCommands <= maxCommands3Stars)
            {
                starsPuzzleFinished = 3;
            }
            else if (numCommands <= maxCommands2Stars)
            {
                starsPuzzleFinished = 2;
            }
            else
            {
                starsPuzzleFinished = 1;
            }

            UIPuzzleWinController.mustUpdateWinMenu = true;
        }
    }
}
