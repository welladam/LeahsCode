using Gamekit3D.GameCommands;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ButtonDownScript : MonoBehaviour
{
    public Button button;
    public GameObject contentResultContainer;

    static public int currentId = 0;
    static public string currentCommand = string.Empty;
    static public string currentCondition = string.Empty;
    static public string currentConditionName = string.Empty;
    static public int currentNumberLoopForCommand;

    // Use this for initialization
    void Start()
    {
        currentNumberLoopForCommand = 1;
        button.onClick.AddListener(onClickButton);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void onClickButton()
    {
        if (button.name == "exitButton")
        {
            InteractPuzzleCrystalBox.forceInteractClose = true;
            PuzzleManipulate.mustRestartPuzzle = true;
        }

        if (button.name == "startButton")
        {
            PuzzleManipulate.startPuzzleControl = true;
            return;
        }

        if (button.name == "backButton")
        {
            if (PuzzleManipulate.listCommands.Count > 0)
            {
                CommandPuzzle lastCommand = PuzzleManipulate.listCommands[PuzzleManipulate.listCommands.Count - 1];

                if (lastCommand.id == currentId)
                {
                    PuzzleManipulate.listCommands.Remove(lastCommand);
                }
            }

            PuzzleManipulate.showNextStepTutorial = true;

            UIPuzzleController.mustShowForCommandChoices = false;
            UIPuzzleController.mustShowIfCommandChoices = false;
            UIPuzzleController.mustShowIfConditionsChoices = false;       

            currentCondition = string.Empty;
            currentConditionName = string.Empty;
            currentCommand = string.Empty;
            return;
        }

        if (button.name == "confirmButton")
        {
            if (PuzzleManipulate.listCommands.Count > 0)
            {
                this.ClearAndCreateResultContent();

                UIPuzzleController.mustShowForCommandChoices = false;
                currentCommand = string.Empty;
            }

            return;
        }

        this.ifCommands();

        this.forCommands();
    }

    private void ifCommands()
    {
        if (button.name == "ifCommandButton")
        {
            UIPuzzleController.mustShowIfConditionsChoices = true;

            currentCommand = "if";
            currentId++;
            return;
        }

        // Conditions 

        if (button.name == "foundBox")
        {
            UIPuzzleController.mustShowIfConditionsChoices = false;
            UIPuzzleController.mustShowIfCommandChoices = true;

            currentCondition = button.name;
            TextMeshProUGUI textMesh = button.GetComponentInChildren<TextMeshProUGUI>();
            currentConditionName = textMesh.text;

            return;
        }

        // Actions

        if (button.name == "destroyBox")
        {
            UIPuzzleController.mustShowIfCommandChoices = false;

            CommandPuzzle commandPuzzle = new CommandPuzzle();
            commandPuzzle.id = currentId;
            commandPuzzle.command = button.name;
            commandPuzzle.commandType = currentCommand;
            commandPuzzle.commandTypeName = "CONDIÇÃO";
            commandPuzzle.commandCondition = currentCondition;
            commandPuzzle.commandConditionName = currentConditionName;

            TextMeshProUGUI textMesh = button.GetComponentInChildren<TextMeshProUGUI>();
            commandPuzzle.commandName = textMesh.text;

            PuzzleManipulate.listCommands.Add(commandPuzzle);
            this.ClearAndCreateResultContent();
        }
    }

    private void forCommands()
    {
        if (button.name == "forCommandButton")
        {
            UIPuzzleController.mustShowForCommandChoices = true;

            currentCommand = "for";
            currentId++;
            return;
        }

        if (currentCommand == "for")
        {
            CommandPuzzle lastCommand = PuzzleManipulate.listCommands.Count > 0
                                                        ? PuzzleManipulate.listCommands[PuzzleManipulate.listCommands.Count - 1]
                                                        : new CommandPuzzle();

            if (lastCommand.id != currentId)
            {
                CommandPuzzle commandPuzzle = new CommandPuzzle();
                commandPuzzle.id = currentId;
                commandPuzzle.commandType = currentCommand;
                commandPuzzle.commandTypeName = "REPETIÇÃO";
                commandPuzzle.countLoop = currentNumberLoopForCommand;

                PuzzleManipulate.listCommands.Add(commandPuzzle);
                lastCommand = PuzzleManipulate.listCommands[PuzzleManipulate.listCommands.Count - 1];
            }

            CommandFor commandFor = new CommandFor();
            TextMeshProUGUI textMesh = button.GetComponentInChildren<TextMeshProUGUI>();
            commandFor.commandName = textMesh.text;
            commandFor.command = button.name;

            lastCommand.commandsFor.Add(commandFor);
            PuzzleManipulate.showNextStepTutorial = true;
            return;
        }
    }

    private void createResultItems()
    {
        foreach (CommandPuzzle commandPuzzle in PuzzleManipulate.listCommands)
        {
            GameObject panel = new GameObject("Panel");
            panel.AddComponent<CanvasRenderer>();

            RectTransform panelRectTransform = panel.AddComponent<RectTransform>();
            panelRectTransform.sizeDelta = new Vector2(520f, 100f);

            Image panelImageBackground = panel.AddComponent<Image>();
            panelImageBackground.sprite = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Fantasy Wooden GUI  Free/normal_ui_set A/UI board Large  stone.png", typeof(Sprite));

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
                commandTextVerticalLayoutGroup.padding.left = 60;
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

            panel.transform.SetParent(contentResultContainer.transform, false);

            commandPuzzle.panelResult = panel;
        }
    }

    private void ClearAndCreateResultContent()
    {
        foreach (CommandPuzzle commandPuzzle in PuzzleManipulate.listCommands)
        {
            Destroy(commandPuzzle.panelResult);
            commandPuzzle.panelResult = null;
        }

        this.createResultItems();
    }
}
