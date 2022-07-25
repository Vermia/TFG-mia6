using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BehUIScore : MonoBehaviour{

    Text levelPointsOneLevelText;
    Text levelPointsTotalText;
    Text starPointsAmountText;
    Text starPointsTotalText;
    Text actionsPointsAmountText;
    Text actionsPointsTotalText;
    Text totalPointsTotalText;

    int pointsOneLevel;
    int pointsOneStar;
    int pointsOneAction;

    int levelPointsTotal;
    int starPointsAmount;
    int starPointsTotal;
    int actionsPointsAmount;
    int actionsPointsTotal;
    int totalPointsTotal;
    

    void Awake(){
        levelPointsOneLevelText=GameObject.Find("LevelPointsPerOne").GetComponent<Text>();
        levelPointsTotalText=GameObject.Find("LevelPointsTotal").GetComponent<Text>();
        starPointsAmountText=GameObject.Find("StarPointsAmount").GetComponent<Text>();
        starPointsTotalText=GameObject.Find("StarPointsTotal").GetComponent<Text>();
        actionsPointsAmountText=GameObject.Find("ActionsPointsAmount").GetComponent<Text>();
        actionsPointsTotalText=GameObject.Find("ActionsPointsTotal").GetComponent<Text>();
        totalPointsTotalText=GameObject.Find("TotalPointsTotal").GetComponent<Text>();

        pointsOneLevel=40;
        pointsOneStar=100;
        pointsOneAction=30;

        levelPointsTotal=40;
        starPointsAmount=0;
        starPointsTotal=0;
        actionsPointsAmount=0;
        actionsPointsTotal=0;
        totalPointsTotal=40;
    }
    
    void Start(){
        //comeHere();
    }

    void Update(){
        //if(Input.GetKeyDown(KeyCode.B)){
        //    goAway();
        //} else if(Input.GetKeyDown(KeyCode.V)){
        //    comeHere();
        //}
    }

    //disappear from the screen
    public void goAway(){
        transform.localPosition = new Vector3(10000, 10000, 0);
    }

    //appear on the screen
    public void comeHere(){
        transform.localPosition = new Vector3(161, -107, 0);
    }

    public int loadValues(int levelWorth, int starAmount, int actionsAmount){
        levelPointsTotal = pointsOneLevel = levelWorth;

        starPointsAmount = starAmount;
        actionsPointsAmount = actionsAmount;
        
        starPointsTotal = starPointsAmount*pointsOneStar;
        actionsPointsTotal = actionsPointsAmount*pointsOneAction;

        totalPointsTotal = levelPointsTotal + actionsPointsTotal + starPointsTotal;

        showValuesInText();
        return totalPointsTotal;
    }

    public void showValuesInText(){
        levelPointsTotalText.text = levelPointsOneLevelText.text = pointsOneLevel.ToString();

        starPointsAmountText.text = "x" + starPointsAmount;
        starPointsTotalText.text = starPointsTotal.ToString();

        actionsPointsAmountText.text = "x" + actionsPointsAmount;
        actionsPointsTotalText.text = actionsPointsTotal.ToString();

        totalPointsTotalText.text = totalPointsTotal.ToString();
    }
}
