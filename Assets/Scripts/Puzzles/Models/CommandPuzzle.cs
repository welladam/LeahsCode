using System.Collections.Generic;

public class CommandPuzzle  {
    public int id;
    public string command;
    public string commandType;
    public List<string> commandsFor = new List<string>();

    public int countLoop = 1;
    public string condition = string.Empty;
}
