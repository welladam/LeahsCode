using Gamekit3D.GameCommands;
using System.Collections;
using System.Collections.Generic;
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
            foreach(CommandPuzzle command in PuzzleManipulate.listCommands)
            {
                if(command.id == currentId)
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
            //Vector2 originalSize = contentResultContainer.GetComponent<RectTransform>().sizeDelta;
            //contentResultContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(originalSize.x, originalSize.y + 2);

            if(currentCommand == "for") {
                
            }

            UIPuzzleController.mustShowForCommandChoices = false;
            currentCommand = string.Empty;
            return;
        }

        if(currentCommand == "for") {
            CommandPuzzle lastCommand = PuzzleManipulate.listCommands.Count > 0 
                                                        ? PuzzleManipulate.listCommands[PuzzleManipulate.listCommands.Count - 1] 
                                                        : new CommandPuzzle();
            
            if(lastCommand.id != currentId) {
                CommandPuzzle commandPuzzle = new CommandPuzzle();
                commandPuzzle.id = currentId;
                commandPuzzle.commandType = currentCommand;
                commandPuzzle.countLoop = currentNumberLoopForCommand;
                PuzzleManipulate.listCommands.Add(commandPuzzle);
                lastCommand = PuzzleManipulate.listCommands[PuzzleManipulate.listCommands.Count - 1];
            }

            lastCommand.commandsFor.Add(button.name);
            PuzzleManipulate.showNextStepTutorial = true;
            return;
        }


        if (currentCommand == "if")
        {
            CommandPuzzle commandPuzzle = new CommandPuzzle();
            commandPuzzle.id = currentId;
            commandPuzzle.command = button.name;
            commandPuzzle.commandType = currentCommand;
            commandPuzzle.countLoop = currentNumberLoopForCommand;
            PuzzleManipulate.listCommands.Add(commandPuzzle);
        }
    }
}
