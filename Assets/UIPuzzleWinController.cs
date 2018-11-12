using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIPuzzleWinController : MonoBehaviour {

    public UnityEvent OnGetPuzzleFinishedStars;
    public Image star1;
    public Image star2;
    public Image star3;
    public TextMeshProUGUI starDescription;

    public static bool mustUpdateWinMenu = false;

    // Use this for initialization
    void Start () {
        OnGetPuzzleFinishedStars.Invoke();
    }
	
	// Update is called once per frame
	void Update () {
        if (mustUpdateWinMenu)
        {
            mustUpdateWinMenu = false;
            int numStars = PuzzleManipulate.starsPuzzleFinished;
            Sprite[] imagesStar = Resources.LoadAll<Sprite>("Images/AtiAi");

            star1.sprite = numStars >= 1 ? imagesStar[1] : imagesStar[0];
            star2.sprite = numStars >= 2 ? imagesStar[1] : imagesStar[0];
            star3.sprite = numStars >= 3 ? imagesStar[1] : imagesStar[0];

            starDescription.text = numStars >= 3 ? "Perfeito!" : numStars >= 2 ? "Ótimo!" : "Legal!";
        }
    }
}
