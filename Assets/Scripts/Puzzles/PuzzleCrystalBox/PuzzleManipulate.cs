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

    private int? currentId = null;
    private List<GameObject> generatedObjects = new List<GameObject>();

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
            int lastIndex = listCommands.Count - 1;
            CommandPuzzle lastCommand = listCommands[lastIndex];

            if (currentId != lastCommand.id)
            {
                currentId = lastCommand.id;
            }

            List<CommandPuzzle> commands = listCommands.FindAll(command => command.id == currentId);

            for (int i = 0; i < generatedObjects.Count; i++)
            {
                Destroy(generatedObjects[i]);
            }

            this.puzzleCrystalSimulate.transform.position = this.puzzleCrystal.transform.position;

            for (int i = 0; i < lastCommand.countLoop; i++)
            {
                foreach (CommandPuzzle commandPuzzle in commands)
                {
                    Vector3 crystalPosition = this.puzzleCrystalSimulate.transform.position;
                    Vector3 newPosition = this.puzzleCrystalSimulate.transform.position;

                    Vector3 arrowPosition = new Vector3();
                    string nameArrow = string.Empty;

                    string command = commandPuzzle.command;

                    if (command == "moveUp")
                    {
                        newPosition = new Vector3(crystalPosition.x, crystalPosition.y + 0.4f, crystalPosition.z);
                        nameArrow = "arrowUp";
                        arrowPosition = new Vector3(crystalPosition.x, crystalPosition.y + 0.2f, crystalPosition.z);
                    }

                    if (command == "moveDown")
                    {
                        newPosition = new Vector3(crystalPosition.x, crystalPosition.y - 0.4f, crystalPosition.z);
                        nameArrow = "arrowDown";
                        arrowPosition = new Vector3(crystalPosition.x, crystalPosition.y - 0.2f, crystalPosition.z);
                    }

                    if (command == "moveRight")
                    {
                        newPosition = new Vector3(crystalPosition.x + 0.4f, crystalPosition.y, crystalPosition.z);
                        nameArrow = "arrowRight";
                        arrowPosition = new Vector3(crystalPosition.x + 0.2f, crystalPosition.y, crystalPosition.z);
                    }

                    if (command == "moveLeft")
                    {
                        newPosition = new Vector3(crystalPosition.x - 0.4f, crystalPosition.y, crystalPosition.z);
                        nameArrow = "arrowLeft";
                        arrowPosition = new Vector3(crystalPosition.x - 0.2f, crystalPosition.y, crystalPosition.z);
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

                    if (newPosition != crystalPosition)
                    {
                        if ((newPosition.x <= -1.3 || newPosition.x >= 0.1) || (newPosition.y <= 1.3 || newPosition.y >= 2.5))
                        {
                            listCommands.RemoveAt(listCommands.Count - 1);
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

            foreach (CommandPuzzle command in listCommands)
            {
                if(command.commandType == "for")
                {
                    for(int i = 0; i < command.countLoop; i++)
                    {
                        yield return new WaitForSeconds(1);
                        this.puzzleCrystal.transform.position = this.getNewPositionCrystal(command);
                    }
                }
            }

            listCommands.Clear();
        }
    }

    private Vector3 getNewPositionCrystal(CommandPuzzle command)
    {
        Vector3 crystalPosition = this.puzzleCrystal.transform.position;
        Vector3 newPosition = crystalPosition;

        if (command.command == "moveUp")
        {
            newPosition = new Vector3(crystalPosition.x, crystalPosition.y + 0.4f, crystalPosition.z);
        }

        if (command.command == "moveDown")
        {
            newPosition = new Vector3(crystalPosition.x, crystalPosition.y - 0.4f, crystalPosition.z);
        }

        if (command.command == "moveRight")
        {
            newPosition = new Vector3(crystalPosition.x + 0.4f, crystalPosition.y, crystalPosition.z);
        }

        if (command.command == "moveLeft")
        {
            newPosition = new Vector3(crystalPosition.x - 0.4f, crystalPosition.y, crystalPosition.z);
        }

        if (command.command == "attack")
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
