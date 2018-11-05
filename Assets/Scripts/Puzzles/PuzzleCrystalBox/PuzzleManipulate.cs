using Gamekit3D;
using Gamekit3D.GameCommands;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PuzzleManipulate : MonoBehaviour
{
    public GameObject puzzleBox1;
    public GameObject puzzleBox2;
    public GameObject puzzleMain;

    public GameObject uIPuzzleWin;
    public GameObject uIPuzzleFail;
    public GameCommandReceiver door;

    public int sizeMatrix = 3;
    public List<GameObject> listGameObjectFloors;

    static public List<CommandPuzzle> listCommands = new List<CommandPuzzle>();
    static public bool startPuzzleControl = false;
    static public bool showNextStepTutorial = false;
    static public bool mustRestartPuzzle = false;
    static public bool mustFinishLevel = false;

    static public GameObject objectWithCrystalSamePosition;

    static private GameObject puzzleBox1Source;
    static private GameObject puzzleBox2Source;

    private List<GameObject> generatedObjects = new List<GameObject>();
    private TextMeshProUGUI currentTextSelected;
    private int delayTime = 1;

    private int currentFloorX = 0;
    private int currentFloorY = 0;
    private GameObject[,] matrixFloors = new GameObject[3, 3];

    // Mission Objectives
    private static int boxesDesroyed = 2;


    // Use this for initialization
    void Start()
    {
        int countListFloors = 0;

        for(int i = 0; i < sizeMatrix; i++)
        {
            for(int j = 0; j < sizeMatrix; j++)
            {
                matrixFloors[j, i] = listGameObjectFloors[countListFloors];
                countListFloors++;
            }
        }

        //Color color = puzzleCrystalSimulate.GetComponent<Renderer>().material.color;
        //puzzleCrystalSimulate.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 1);

        puzzleBox1Source = Object.Instantiate(puzzleBox1, puzzleMain.transform);
        puzzleBox2Source = Object.Instantiate(puzzleBox2, puzzleMain.transform);

        puzzleBox1Source.SetActive(false);
        puzzleBox2Source.SetActive(false);
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
            GetCurrentCrystal().SetActive(false);
            GetCurrentCrystalSimulator().SetActive(false);

            foreach (CommandPuzzle commandPuzzle in listCommands)
            {
                Destroy(commandPuzzle.panelResult);
                commandPuzzle.panelResult = null;
            }

            listCommands.Clear();

            currentFloorX = 0;
            currentFloorY = 0;

            GetCurrentCrystal().SetActive(true);

            for (int i = 0; i < generatedObjects.Count; i++)
            {
                Destroy(generatedObjects[i]);
            }

            Destroy(puzzleBox1);
            Destroy(puzzleBox2);

            puzzleBox1 = Object.Instantiate(puzzleBox1Source, puzzleMain.transform);
            puzzleBox2 = Object.Instantiate(puzzleBox2Source, puzzleMain.transform);

            puzzleBox1.SetActive(true);
            puzzleBox2.SetActive(true);

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

                            if ((currentFloorX > 2 || currentFloorX < 0) || (currentFloorY > 2 || currentFloorY < 0))
                            {
                                commandPuzzleMain.commandsFor.RemoveAt(j);
                                return;
                            }

                            currentCrystalSimulator.SetActive(false);

                            GameObject newCrystalSimulator = GetCurrentCrystalSimulator();
                            Color color = newCrystalSimulator.GetComponent<Renderer>().material.color;
                            newCrystalSimulator.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 1);

                            newCrystalSimulator.SetActive(true);
                            //GameObject arrow = Resources.Load("Prefabs/" + nameArrow, typeof(GameObject)) as GameObject;
                            //var prefab = Instantiate(arrow, arrowPosition, Quaternion.identity);
                            //if (defaultRotateY > 0)
                            //{
                            //    prefab.transform.Rotate(Vector3.up, defaultRotateY);
                            //}
                            //generatedObjects.Add(prefab);

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

                            currentTextSelected = commandPuzzleMain.panelResult.GetComponentsInChildren<TextMeshProUGUI>()[j + 1];
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
        }
    }


    private GameObject GetCurrentCrystal()
    {
        return matrixFloors[currentFloorX, currentFloorY].transform.Find("PuzzleCrystal").gameObject;
    }

    private GameObject GetCurrentCrystalSimulator()
    {
        return matrixFloors[currentFloorX, currentFloorY].transform.Find("PuzzleCrystalSimulator").gameObject;
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
            }
            catch (System.Exception ex) { }
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
}
