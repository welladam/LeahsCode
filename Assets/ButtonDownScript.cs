using Gamekit3D.GameCommands;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonDownScript : MonoBehaviour
{
    public GameObject button;
    public GameObject contentResultContainer;
    public GameCommandReceiver door;

    static public int currentId = 0;
    static public string currentCommnand = string.Empty;
    static public int currentNumberLoopForCommand;

    // Use this for initialization
    void Start()
    {
        currentNumberLoopForCommand = 1;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnMouseUpAsButton()
    {
        if (button.name == "startButton")
        {
            InteractPuzzleCrystalBox.forceInteractClose = true;
            SendGameCommand sendGameCommand = new SendGameCommand();

            sendGameCommand.interactionType = GameCommandType.Open;
            sendGameCommand.interactiveObject = door;
            sendGameCommand.coolDown = 1;
            sendGameCommand.oneShot = true;
            sendGameCommand.Send();

            //PuzzleManipulate.startPuzzleControl = true;
            return;
        }

        if (button.name == "forCommand")
        {
            UIPuzzleController.mustShowForCommandChoices = true;

            currentCommnand = "for";
            currentId++;
            return;
        }

        if (button.name == "backButton")
        {
            foreach(CommandPuzzle command in PuzzleManipulate.listCommands)
            {
                if(command.id == currentId)
                {
                    PuzzleManipulate.listCommands.Remove(command);
                }
            }

            UIPuzzleController.mustShowForCommandChoices = false;
            currentCommnand = string.Empty;
            return;
        }


        if (button.name == "confirmButton")
        {
            //Vector2 originalSize = contentResultContainer.GetComponent<RectTransform>().sizeDelta;
            //contentResultContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(originalSize.x, originalSize.y + 2);
            UIPuzzleController.mustShowForCommandChoices = false;
            currentCommnand = string.Empty;
            return;
        }

        CommandPuzzle commandPuzzle = new CommandPuzzle();
        commandPuzzle.id = currentId;
        commandPuzzle.command = button.name;
        commandPuzzle.commandType = currentCommnand;
        commandPuzzle.countLoop = currentNumberLoopForCommand;

        PuzzleManipulate.listCommands.Add(commandPuzzle);
        PuzzleManipulate.showNextStepTutorial = true;
    }
}
