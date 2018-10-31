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
    public GameCommandReceiver door;

    static public int currentId = 0;
    static public string currentCommand = string.Empty;
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
        if(button.name == "exitButton")
        {
            InteractPuzzleCrystalBox.forceInteractClose = true;
        }

        if (button.name == "startButton")
        {
            //InteractPuzzleCrystalBox.forceInteractClose = true;
            //SendGameCommand sendGameCommand = new SendGameCommand();

            //sendGameCommand.interactionType = GameCommandType.Open;
            //sendGameCommand.interactiveObject = door;
            //sendGameCommand.coolDown = 1;
            //sendGameCommand.oneShot = true;
            //sendGameCommand.Send();

            PuzzleManipulate.startPuzzleControl = true;
            return;
        }

        if (button.name == "forCommandButton")
        {
            UIPuzzleController.mustShowForCommandChoices = true;

            currentCommand = "for";
            currentId++;
            return;
        }

        if (button.name == "backButton")
        {
            foreach (CommandPuzzle command in PuzzleManipulate.listCommands)
            {
                if (command.id == currentId)
                {
                    PuzzleManipulate.listCommands.Remove(command);
                }
            }

            UIPuzzleController.mustShowForCommandChoices = false;
            currentCommand = string.Empty;
            return;
        }


        if (button.name == "confirmButton")
        {
            if (PuzzleManipulate.listCommands.Count > 0)
            {
                foreach (CommandPuzzle commandPuzzle in PuzzleManipulate.listCommands)
                {
                    Destroy(commandPuzzle.panelResult);
                    commandPuzzle.panelResult = null;
                }

                this.createResultItems();

                UIPuzzleController.mustShowForCommandChoices = false;
                currentCommand = string.Empty;
            }

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


        if (currentCommand == "if")
        {
            CommandPuzzle commandPuzzle = new CommandPuzzle();
            commandPuzzle.id = currentId;
            commandPuzzle.command = button.name;
            commandPuzzle.commandType = currentCommand;
            commandPuzzle.commandTypeName = "CONDIÇÃO";
            commandPuzzle.countLoop = currentNumberLoopForCommand;

            TextMeshProUGUI textMesh = button.GetComponentInChildren<TextMeshProUGUI>();
            commandPuzzle.commandName = textMesh.text;

            PuzzleManipulate.listCommands.Add(commandPuzzle);
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
}
