using Gamekit3D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManipulate : MonoBehaviour
{

    public GameObject puzzleBox1;
    public GameObject puzzleBox2;
    public GameObject puzzleCrystal;
    public GameObject puzzleCrystalSimulate;

    static public List<CommandPuzzle> listCommands = new List<CommandPuzzle>();
    static public bool startPuzzleControl = false;
    static public bool showNextStepTutorial = false;

    private List<GameObject> generatedObjects = new List<GameObject>();

    private float defaultMovePositionCrystal = 0.12f;
    private float defaultMovePositionCrystalSimulate = 0.06f;

    // Use this for initialization
    void Start()
    {
        this.puzzleCrystalSimulate.SetActive(false);
        Color color = puzzleCrystalSimulate.GetComponent<Renderer>().material.color;
        puzzleCrystalSimulate.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 1);
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(StartPuzzle());
        CommandSelected();
    }

    public void CommandSelected()
    {
        if (showNextStepTutorial && listCommands.Count > 0)
        {
            for (int i = 0; i < generatedObjects.Count; i++)
            {
                Destroy(generatedObjects[i]);
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
                            string command = commandPuzzleMain.commandsFor[j];
                            Vector3 crystalPosition = this.puzzleCrystalSimulate.transform.position;
                            Vector3 newPosition = this.puzzleCrystalSimulate.transform.position;

                            Vector3 arrowPosition = new Vector3();
                            string nameArrow = string.Empty;

                            if (command == "moveUp")
                            {
                                newPosition = new Vector3(crystalPosition.x, crystalPosition.y + defaultMovePositionCrystal, crystalPosition.z);
                                nameArrow = "arrowUp";
                                arrowPosition = new Vector3(crystalPosition.x, crystalPosition.y + defaultMovePositionCrystalSimulate, crystalPosition.z);
                            }

                            if (command == "moveDown")
                            {
                                newPosition = new Vector3(crystalPosition.x, crystalPosition.y - defaultMovePositionCrystal, crystalPosition.z);
                                nameArrow = "arrowDown";
                                arrowPosition = new Vector3(crystalPosition.x, crystalPosition.y - defaultMovePositionCrystalSimulate, crystalPosition.z);
                            }

                            if (command == "turnRight")
                            {
                                newPosition = new Vector3(crystalPosition.x + defaultMovePositionCrystal, crystalPosition.y, crystalPosition.z);
                                nameArrow = "arrowRight";
                                arrowPosition = new Vector3(crystalPosition.x + defaultMovePositionCrystalSimulate, crystalPosition.y, crystalPosition.z);
                            }

                            if (command == "turnLeft")
                            {
                                newPosition = new Vector3(crystalPosition.x - defaultMovePositionCrystal, crystalPosition.y, crystalPosition.z);
                                nameArrow = "arrowLeft";
                                arrowPosition = new Vector3(crystalPosition.x - defaultMovePositionCrystalSimulate, crystalPosition.y, crystalPosition.z);
                            }

                            if (newPosition != crystalPosition)
                            {
                                if ((newPosition.x <= -1.0 || newPosition.x >= -0.6) || (newPosition.y <= 1.3 || newPosition.y >= 1.7))
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
                if (commandPuzzleMain.commandType == "for")
                {
                    for (int i = 0; i < commandPuzzleMain.countLoop; i++)
                    {
                        for (int j = 0; j < commandPuzzleMain.commandsFor.Count; j++)
                        {
                            yield return new WaitForSeconds(1);
                            this.puzzleCrystal.transform.position = this.getNewPositionCrystal(commandPuzzleMain.commandsFor[j]);
                        }
                    }
                }
            }

            listCommands.Clear();
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

        if (command == "moveRight")
        {
            newPosition = new Vector3(crystalPosition.x + defaultMovePositionCrystal, crystalPosition.y, crystalPosition.z);
        }

        if (command == "moveLeft")
        {
            newPosition = new Vector3(crystalPosition.x - defaultMovePositionCrystal, crystalPosition.y, crystalPosition.z);
        }

        if (command == "attack")
        {
            //Damageable d = puzzleBox1.GetComponentInChildren<Damageable>();
            //if (d != null)
            //{
            //    Damageable.DamageMessage damage = new Damageable.DamageMessage()
            //    {
            //        amount = 1,
            //        damager = this,
            //        direction = Vector3.up,
            //    };

            //    d.ApplyDamage(damage);
            //}
        }

        return newPosition;
    }
}
