using Gamekit3D;
using Gamekit3D.GameCommands;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PuzzleManipulate : MonoBehaviour
{
    public GameObject puzzleBox1;
    public GameObject puzzleBox2;
    public GameObject puzzleCrystal;
    public GameObject puzzleCrystalSimulate;
    public GameObject puzzleMain;

    public GameObject uIPuzzleWin;
    public GameObject uIPuzzleFail;
    public GameCommandReceiver door;

    public float defaultMovePositionCrystal = 0.12f;
    public float defaultMovePositionCrystalSimulate = 0.06f;
    public float defaultMinValueX = -1.0f;
    public float defaultMaxValueX = -0.6f;
    public float defaultMinValueY = 1.3f;
    public float defaultMaxValueY = 1.7f;

    static public List<CommandPuzzle> listCommands = new List<CommandPuzzle>();
    static public bool startPuzzleControl = false;
    static public bool showNextStepTutorial = false;
    static public bool mustRestartPuzzle = false;
    static public bool mustFinishLevel = false;

    static public GameObject objectWithCrystalSamePosition;

    static private GameObject puzzleBox1Source;
    static private GameObject puzzleBox2Source;
    static private GameObject puzzleCrystalSource;
    static private GameObject puzzleCrystalSimulateSource;

    private List<GameObject> generatedObjects = new List<GameObject>();
    private TextMeshProUGUI currentTextSelected;
    private int delayTime = 1;

    // Mission Objectives
    private static int boxesDesroyed = 2;


    // Use this for initialization
    void Start()
    {
        this.puzzleCrystalSimulate.SetActive(false);
        Color color = puzzleCrystalSimulate.GetComponent<Renderer>().material.color;
        puzzleCrystalSimulate.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 1);

        puzzleBox1Source = Object.Instantiate(puzzleBox1, puzzleMain.transform);
        puzzleBox2Source = Object.Instantiate(puzzleBox2, puzzleMain.transform);
        puzzleCrystalSource = Object.Instantiate(puzzleCrystal, puzzleMain.transform);
        puzzleCrystalSimulateSource = Object.Instantiate(puzzleCrystalSimulate, puzzleMain.transform);

        puzzleBox1Source.SetActive(false);
        puzzleBox2Source.SetActive(false);
        puzzleCrystalSource.SetActive(false);
        puzzleCrystalSimulateSource.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(StartPuzzle());
        CommandSelected();
        RestartLevel();
        FinishLevel();
    }

    private void FinishLevel()
    {
        if (mustFinishLevel)
        {
            InteractPuzzleCrystalBox.alreadyCompletedPuzzle = true;
            InteractPuzzleCrystalBox.forceInteractClose = true;
            SendGameCommand sendGameCommand = new SendGameCommand();

            sendGameCommand.interactionType = GameCommandType.Open;
            sendGameCommand.interactiveObject = door;
            sendGameCommand.coolDown = 1;
            sendGameCommand.oneShot = true;
            sendGameCommand.Send();
            mustFinishLevel = false;
        }
    }

    private void RestartLevel()
    {
        if (mustRestartPuzzle)
        {
            foreach (CommandPuzzle commandPuzzle in listCommands)
            {
                Destroy(commandPuzzle.panelResult);
                commandPuzzle.panelResult = null;
            }

            listCommands.Clear();

            for (int i = 0; i < generatedObjects.Count; i++)
            {
                Destroy(generatedObjects[i]);
            }

            Destroy(puzzleBox1);
            Destroy(puzzleBox2);
            Destroy(puzzleCrystal);
            Destroy(puzzleCrystalSimulate);

            puzzleBox1 = Object.Instantiate(puzzleBox1Source, puzzleMain.transform);
            puzzleBox2 = Object.Instantiate(puzzleBox2Source, puzzleMain.transform);
            puzzleCrystal = Object.Instantiate(puzzleCrystalSource, puzzleMain.transform);
            puzzleCrystalSimulate = Object.Instantiate(puzzleCrystalSimulateSource, puzzleMain.transform);

            puzzleBox1.SetActive(true);
            puzzleBox2.SetActive(true);
            puzzleCrystal.SetActive(true);
            puzzleCrystalSimulate.SetActive(true);

            mustRestartPuzzle = false;
        }
    }

    public void CommandSelected()
    {
        if (showNextStepTutorial)
        {
            foreach (GameObject gameObject in generatedObjects)
            {
                Destroy(gameObject);
            }

            this.puzzleCrystalSimulate.transform.position = this.puzzleCrystal.transform.position;

            foreach (CommandPuzzle commandPuzzleMain in listCommands)
            {
                if (commandPuzzleMain.commandType == "for")
                {
                    for (int i = 0; i < commandPuzzleMain.countLoop; i++)
                    {
                        for (int j = 0; j < commandPuzzleMain.commandsFor.Count; j++)
                        {
                            CommandFor commandFor = commandPuzzleMain.commandsFor[j];
                            Vector3 crystalPosition = this.puzzleCrystalSimulate.transform.position;
                            Vector3 newPosition = this.puzzleCrystalSimulate.transform.position;

                            Vector3 arrowPosition = new Vector3();
                            string nameArrow = string.Empty;

                            if (commandFor.command == "moveUp")
                            {
                                newPosition = new Vector3(crystalPosition.x, crystalPosition.y + defaultMovePositionCrystal, crystalPosition.z);
                                nameArrow = "arrowUp";
                                arrowPosition = new Vector3(crystalPosition.x, crystalPosition.y + defaultMovePositionCrystalSimulate, crystalPosition.z);
                            }

                            if (commandFor.command == "moveDown")
                            {
                                newPosition = new Vector3(crystalPosition.x, crystalPosition.y - defaultMovePositionCrystal, crystalPosition.z);
                                nameArrow = "arrowDown";
                                arrowPosition = new Vector3(crystalPosition.x, crystalPosition.y - defaultMovePositionCrystalSimulate, crystalPosition.z);
                            }

                            if (commandFor.command == "turnRight")
                            {
                                newPosition = new Vector3(crystalPosition.x + defaultMovePositionCrystal, crystalPosition.y, crystalPosition.z);
                                nameArrow = "arrowRight";
                                arrowPosition = new Vector3(crystalPosition.x + defaultMovePositionCrystalSimulate, crystalPosition.y, crystalPosition.z);
                            }

                            if (commandFor.command == "turnLeft")
                            {
                                newPosition = new Vector3(crystalPosition.x - defaultMovePositionCrystal, crystalPosition.y, crystalPosition.z);
                                nameArrow = "arrowLeft";
                                arrowPosition = new Vector3(crystalPosition.x - defaultMovePositionCrystalSimulate, crystalPosition.y, crystalPosition.z);
                            }

                            if (newPosition != crystalPosition)
                            {
                                if ((newPosition.x <= defaultMinValueX || newPosition.x >= defaultMaxValueX) || (newPosition.y <= defaultMinValueY || newPosition.y >= defaultMaxValueY))
                                {
                                    commandPuzzleMain.commandsFor.RemoveAt(j);
                                    return;
                                }

                                this.puzzleCrystalSimulate.SetActive(true);
                                this.puzzleCrystalSimulate.transform.position = newPosition;

                                GameObject arrow = Resources.Load("Prefabs/" + nameArrow, typeof(GameObject)) as GameObject;
                                var prefab = Instantiate(arrow, arrowPosition, Quaternion.identity);
                                generatedObjects.Add(prefab);
                            }
                        }
                    }
                }
            }
        }

        showNextStepTutorial = false;
    }


    public IEnumerator StartPuzzle()
    {
        if (startPuzzleControl)
        {
            startPuzzleControl = false;

            this.puzzleCrystalSimulate.SetActive(false);
            foreach (GameObject prefab in generatedObjects)
            {
                Destroy(prefab);
            }

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

                            currentTextSelected = commandPuzzleMain.panelResult.GetComponentsInChildren<TextMeshProUGUI>()[j + 1];
                            currentTextSelected.fontSize = 30;
                            currentTextSelected.color = new Color32(103, 255, 93, 255);
                            currentTextSelected.fontStyle = FontStyles.Bold;
                            this.puzzleCrystal.transform.position = this.getNewPositionCrystal(commandPuzzleMain.commandsFor[j].command);

                            yield return new WaitForSeconds(delayTime);
                        }
                    }
                }
            }

            yield return new WaitForSeconds(delayTime);
            this.ValidateFinishPuzzle();
        }
    }

    private void ConditionAction(CommandPuzzle commandPuzzleMain)
    {
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
            }catch(Exception ex) { }
        }
    }

    private void ValidateFinishPuzzle()
    {
        if (boxesDesroyed == 0)
        {
            uIPuzzleWin.SetActive(true);
        }
        else
        {
            uIPuzzleFail.SetActive(true);
        }
    }

    private Vector3 getNewPositionCrystal(string command)
    {
        Vector3 crystalPosition = this.puzzleCrystal.transform.position;
        Vector3 newPosition = crystalPosition;

        if (command == "moveUp")
        {
            newPosition = new Vector3(crystalPosition.x, crystalPosition.y + defaultMovePositionCrystal, crystalPosition.z);
        }

        if (command == "moveDown")
        {
            newPosition = new Vector3(crystalPosition.x, crystalPosition.y - defaultMovePositionCrystal, crystalPosition.z);
        }

        if (command == "turnRight")
        {
            newPosition = new Vector3(crystalPosition.x + defaultMovePositionCrystal, crystalPosition.y, crystalPosition.z);
        }

        if (command == "turnLeft")
        {
            newPosition = new Vector3(crystalPosition.x - defaultMovePositionCrystal, crystalPosition.y, crystalPosition.z);
        }

        return newPosition;
    }
}
