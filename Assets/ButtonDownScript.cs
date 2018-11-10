using Gamekit3D.GameCommands;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ButtonDownScript : MonoBehaviour
{
    public Button button;
    public GameObject panelGameObject;

    public UnityEvent OnPuzzleClosed;

    static public int currentId = 0;
    static public string currentCommand = string.Empty;
    static public string currentCondition = string.Empty;
    static public string currentConditionName = string.Empty;
    static public int currentNumberLoopForCommand = 1;

    static public bool forceOpenIfTutorial = false;

    // Use this for initialization
    void Start()
    {
        button.onClick.AddListener(onClickButton);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void onClickButton()
    {
        if (PuzzleManipulate.puzzleInProgress)
        {
            return;
        }

        if (forceOpenIfTutorial || (button.name == "helpButton" || button.name == "doneButton"))
        {
            if (panelGameObject != null)
            {
                panelGameObject.SetActive(!panelGameObject.activeSelf);
            }

            forceOpenIfTutorial = false;
            return;
        }

        if (button.name == "exitButton")
        {
            OnPuzzleClosed.Invoke();
            return;
        }

        if (button.name == "startButton")
        {
            PuzzleManipulate.startPuzzleControl = true;
            return;
        }

        if (button.name == "backButton")
        {
            PuzzleManipulate.refreshResultContentPanel = true;

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

        if (button.name == "foundBox" || button.name == "foundKey")
        {
            UIPuzzleController.mustShowIfConditionsChoices = false;
            UIPuzzleController.mustShowIfCommandChoices = true;

            currentCondition = button.name;
            TextMeshProUGUI textMesh = button.GetComponentInChildren<TextMeshProUGUI>();
            currentConditionName = textMesh.text;

            return;
        }

        // Actions

        if (button.name == "destroyBox" || button.name == "getKey")
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
            PuzzleManipulate.refreshResultContentPanel = true;
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
}
