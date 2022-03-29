using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BehUIAbove : MonoBehaviour
{
    Text hp1;
    Text hp2;
    Text points1;
    Text points2;

    // Start is called before the first frame update
    void Start()
    {
        hp1 = GameObject.Find("HP1").GetComponent<Text>();
        hp2 = GameObject.Find("HP2").GetComponent<Text>();
        points1 = GameObject.Find("points1").GetComponent<Text>();
        points2 = GameObject.Find("points2").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update(){
        if(BehBoard.gameActive){
            BehCharacter player1 = BehBoard.player1.GetComponent<BehCharacter>();
            BehCharacter player2 = BehBoard.player2.GetComponent<BehCharacter>();
            hp1.text = player1.currHP.ToString() + "/" + player1.maxHP.ToString();
            hp2.text = player2.currHP.ToString() + "/" + player2.maxHP.ToString();
            points1.text = player1.currStars.ToString() + "/" + player1.maxStars.ToString();
            points2.text = player2.currStars.ToString() + "/" + player2.maxStars.ToString();
        }
    }
}
