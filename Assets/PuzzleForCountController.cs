using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleForCountController : MonoBehaviour
{

    public GameObject button;
    public TextMesh counter;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnMouseUpAsButton()
    {
        if (button.name == "nextCountLoop")
        {
            int currentCount = int.Parse(counter.text);
            currentCount++;

            if (currentCount == 10)
                return;

            ButtonDownScript.currentNumberLoopForCommand = currentCount;
            counter.text = currentCount.ToString();
        }

        if (button.name == "previousCountLoop")
        {
            int currentCount = int.Parse(counter.text);
            currentCount--;

            if (currentCount == 0)
                return;

            ButtonDownScript.currentNumberLoopForCommand = currentCount;
            counter.text = currentCount.ToString();
        }
    }
}