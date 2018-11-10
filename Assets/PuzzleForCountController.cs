using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PuzzleForCountController : MonoBehaviour
{

    public TextMeshProUGUI counter;

    // Use this for initialization
    void Start()
    {
        ButtonDownScript.currentNumberLoopForCommand = 1;
    }

    // Update is called once per frame
    void Update()
    {
        counter.text = ButtonDownScript.currentNumberLoopForCommand.ToString();
    }

    public void IncreaseCounter()
    {
        int currentCount = int.Parse(counter.text);
        currentCount++;

        if (currentCount == 10)
            return;

        ButtonDownScript.currentNumberLoopForCommand = currentCount;
    }

    public void DecreaseCounter()
    {
        int currentCount = int.Parse(counter.text);
        currentCount--;

        if (currentCount == 0)
            return;

        ButtonDownScript.currentNumberLoopForCommand = currentCount;
    }
}