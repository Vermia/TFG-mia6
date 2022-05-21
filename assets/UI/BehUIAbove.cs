using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BehUIAbove : MonoBehaviour
{
    Text hp1;
    Text points1;
    BehCharacter player1;
    BehCharacter player2;

    // Start is called before the first frame update
    void Start()
    {
        player1 = BehBoard.player1.GetComponent<BehCharacter>();
        player2 = BehBoard.player2.GetComponent<BehCharacter>();
        hp1 = GameObject.Find("UIAboveHP1").GetComponent<Text>();
        points1 = GameObject.Find("UIAbovePoints1").GetComponent<Text>();

        updateValues();
    }

    // Update is called once per frame
    void Update(){
        if(BehBoard.gameActive){
            updateValues();
        }
    }

    void updateValues(){
        hp1.text = player1.currHP.ToString() + "/" + player1.maxHP.ToString();
        points1.text = player1.currStars.ToString() + "/" + player1.maxStars.ToString();
    }
}
