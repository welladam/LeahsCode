using System.Collections.Generic;
using UnityEngine;

public class CommandPuzzle  {
    public int id;
    public string command;
    public string commandName;
    public string commandType;
    public string commandTypeName;
    public List<CommandFor> commandsFor = new List<CommandFor>();
    public GameObject panelResult;

    public int countLoop = 1;
}
