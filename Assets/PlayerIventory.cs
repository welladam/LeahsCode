using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerIventory : MonoBehaviour {

    public static int goldCoinsCount = 0;

    public TextMeshProUGUI goldCoinText;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        goldCoinText.text = goldCoinsCount.ToString();
    }

    public void AddGoldCoins(int count)
    {
        goldCoinsCount += count;
    }

    public void RemoveGoldCoins(int count)
    {
        goldCoinsCount -= count;
    }
}
