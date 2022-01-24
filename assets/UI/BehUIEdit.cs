using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BehUIEdit : MonoBehaviour
{

    public UIMovingState movingState;
    float wholeSeconds;
    float UIWidth;
    BehCharacter player;

    Text A_value;
    Text B_value;
    Text C_value;
    Text D_value;
    Text E_value;
    Text F_value;

    Rule newRuleShowcase;


    // Start is called before the first frame update
    void Start(){
        movingState=UIMovingState.movingR;
        UIWidth = GetComponent<RectTransform>().sizeDelta.x;
        wholeSeconds = 1f;
        newRuleShowcase = new Rule();

        player = BehBoard.player1.GetComponent<BehCharacter>();
        updateRuleInfo();
        reformulateNewRule();

        A_value = GameObject.Find("EditA_value").GetComponent<Text>();
        B_value = GameObject.Find("EditB_value").GetComponent<Text>();
        C_value = GameObject.Find("EditC_value").GetComponent<Text>();
        D_value = GameObject.Find("EditD_value").GetComponent<Text>();
        E_value = GameObject.Find("EditE_value").GetComponent<Text>();
        F_value = GameObject.Find("EditF_value").GetComponent<Text>();

        GameObject.Find("EditA_up").GetComponent<Button>().onClick.AddListener( ()=>{player.incVariable(Variables.A);} );
        GameObject.Find("EditB_up").GetComponent<Button>().onClick.AddListener( ()=>{player.incVariable(Variables.B);} );
        GameObject.Find("EditC_up").GetComponent<Button>().onClick.AddListener( ()=>{player.incVariable(Variables.C);} );
        GameObject.Find("EditD_up").GetComponent<Button>().onClick.AddListener( ()=>{player.incVariable(Variables.D);} );
        GameObject.Find("EditE_up").GetComponent<Button>().onClick.AddListener( ()=>{player.incVariable(Variables.E);} );
        GameObject.Find("EditF_up").GetComponent<Button>().onClick.AddListener( ()=>{player.incVariable(Variables.F);} );

        GameObject.Find("EditA_down").GetComponent<Button>().onClick.AddListener( ()=>{player.decVariable(Variables.A);} );
        GameObject.Find("EditB_down").GetComponent<Button>().onClick.AddListener( ()=>{player.decVariable(Variables.B);} );
        GameObject.Find("EditC_down").GetComponent<Button>().onClick.AddListener( ()=>{player.decVariable(Variables.C);} );
        GameObject.Find("EditD_down").GetComponent<Button>().onClick.AddListener( ()=>{player.decVariable(Variables.D);} );
        GameObject.Find("EditE_down").GetComponent<Button>().onClick.AddListener( ()=>{player.decVariable(Variables.E);} );
        GameObject.Find("EditF_down").GetComponent<Button>().onClick.AddListener( ()=>{player.decVariable(Variables.F);} );

    }

    // Update is called once per frame
    void Update(){
        RectTransform rtrans = (RectTransform)GetComponentInParent(typeof(RectTransform));
        float scrW = Application.isEditor ? 2035f :  Screen.width;  //Segun si estamos en el editor de Unity o en ejecutable
        float rightBorder = -390f;
        float leftBorder = -1407f;

        A_value.text = player.variables[(int)Variables.A].ToString();
        B_value.text = player.variables[(int)Variables.B].ToString();
        C_value.text = player.variables[(int)Variables.C].ToString();
        D_value.text = player.variables[(int)Variables.D].ToString();
        E_value.text = player.variables[(int)Variables.E].ToString();
        F_value.text = player.variables[(int)Variables.F].ToString();


        switch(movingState){
            case UIMovingState.standbyL:
                GetComponent<RectTransform>().anchoredPosition = new Vector2 (leftBorder, GetComponent<RectTransform>().anchoredPosition.y);
            break;
            case UIMovingState.standbyR:
                GetComponent<RectTransform>().anchoredPosition = new Vector2 (rightBorder, GetComponent<RectTransform>().anchoredPosition.y);
            break;
            case UIMovingState.movingR:
                if(GetComponent<RectTransform>().anchoredPosition.x >= rightBorder){
                    movingState = UIMovingState.standbyR;
                    break;
                } 
                 GetComponent<RectTransform>().anchoredPosition += new Vector2 ((UIWidth) / wholeSeconds * Time.deltaTime, 0f);
            break;
            
            case UIMovingState.movingL:
                if(GetComponent<RectTransform>().anchoredPosition.x <= leftBorder){
                    movingState = UIMovingState.standbyL;
                    break;
                } 
                GetComponent<RectTransform>().anchoredPosition -= new Vector2 ((UIWidth) / wholeSeconds * Time.deltaTime, 0f);
                 
            break;
        }
    }

    public void retractOrExpand(){
        if(movingState==UIMovingState.standbyL){
            movingState=UIMovingState.movingR;
        }
        else if(movingState==UIMovingState.standbyR){
            movingState=UIMovingState.movingL;
        }
    }

    void updateRuleInfo(){
        
        //name_value.text = ;

        //Limpiar reglas
        var content = GameObject.Find("EditRuleContent");
        var auxList = new List<GameObject>();
        foreach(Transform child in content.transform){
            auxList.Add(child.gameObject);
        }
        auxList.ForEach(child => Destroy(child));

        //Nuevas reglas
        foreach(Rule rule in player.rules){
            //Imagen de fondo de cada regla
            GameObject newImg = new GameObject();
            newImg.AddComponent(typeof(Image));
            newImg.transform.SetParent(content.transform);
            newImg.transform.localScale = new Vector3(1,1,0);
            newImg.GetComponent<RectTransform>().sizeDelta = new Vector2(500,200);

            //Texto común a todas las reglas
            BehUIRule.createText(newImg, new Vector2(-140+70,30), "Si:", new Color(0,0,1,1), 45);
            BehUIRule.createText(newImg, new Vector2(60,30), "Entonces:", new Color(0,0,1,1), 45);

            //Condiciones
            string condText;
            if(rule.conds.Count!=0)
                condText = BehUIRule.calculateCondText(rule.conds[0]);
            else condText = "";


                GameObject condTextGO = BehUIRule.createText(newImg, new Vector2(10+70,-70), condText, new Color(0,0,1,1), 30);
                condTextGO.GetComponent<Text>().horizontalOverflow=HorizontalWrapMode.Wrap;
                condTextGO.GetComponent<RectTransform>().anchorMin = new Vector2(0,1);
                condTextGO.GetComponent<RectTransform>().anchorMax = new Vector2(0,1);
                condTextGO.GetComponent<RectTransform>().pivot = new Vector2(0,1);
                condTextGO.GetComponent<RectTransform>().sizeDelta = new Vector2(190,110);

            string actText = BehUIRule.calculateActionText(rule.action);
            GameObject actTextGO = BehUIRule.createText(newImg, new Vector2(270,-70), actText, new Color(0,0,1,1), 20);
            actTextGO.GetComponent<Text>().horizontalOverflow=HorizontalWrapMode.Wrap;
            actTextGO.GetComponent<RectTransform>().anchorMin = new Vector2(0,1);
            actTextGO.GetComponent<RectTransform>().anchorMax = new Vector2(0,1);
            actTextGO.GetComponent<RectTransform>().pivot = new Vector2(0,1);
            actTextGO.GetComponent<RectTransform>().sizeDelta = new Vector2(210,110);

            //Opciones de edicion
            
            int index=0;
            for(int i=0 ; i<player.rules.GetLength(0);++i){
                if(player.rules[i] == rule){
                    index=i;
                    break;
                }
            }

            if(index!=0){
                GameObject moveUpGO = createButton(newImg, new Vector2(30,-30), new Vector2(30,30), new Color(1, 0.5f, 0, 1), ()=>{ Debug.Log("Index = " + index);player.moveRuleUp(index); updateRuleInfo(); } );
            }

            GameObject removeGO = createButton(newImg, new Vector2(30,-80), new Vector2(30,30), new Color(1, 0, 0, 1), ()=>{Debug.Log("Index = " + index);player.removeRule(index); updateRuleInfo(); } );

            if(index!= player.rules.GetLength(0)-1 ){
                GameObject moveDownGO = createButton(newImg, new Vector2(30,-130), new Vector2(30,30), new Color(1, 0.5f, 0, 1), ()=>{Debug.Log("Index = " + index);player.moveRuleDown(index); updateRuleInfo(); } );
            }

        }
    }

    public static GameObject createButton(GameObject parent, Vector2 pos, Vector2 size, Color color, UnityAction action ){
        GameObject creatingGO = new GameObject();
        creatingGO.transform.SetParent(parent.transform);
        creatingGO.transform.localScale = new Vector3(1,1,0);
        creatingGO.AddComponent(typeof(Image));
        creatingGO.AddComponent(typeof(Button));
        Image creatingImg = creatingGO.GetComponent<Image>();
        RectTransform creatingRT = creatingGO.GetComponent<RectTransform>();
        Button creatingButton = creatingGO.GetComponent<Button>();

        creatingImg.color= color;

        creatingRT.anchorMin = new Vector2(0,1);
        creatingRT.anchorMax = new Vector2(0,1);
        creatingRT.pivot = new Vector2(0,1);
        creatingRT.sizeDelta = size;
        creatingRT.anchoredPosition = pos;

        creatingButton.onClick.AddListener(action);

        return creatingGO;
    }

    void reformulateNewRule(){
        GameObject content = GameObject.Find("UICanvasImageEdit");
        GameObject newRuleCanvas = GameObject.Find("RuleEditNewRuleShowcase");
        newRuleCanvas.transform.SetParent(content.transform);

        BehUIRule.createText(newRuleCanvas, new Vector2(-140+25,30), "Si:" , new Color(0,0,1,1), 45);
        BehUIRule.createText(newRuleCanvas, new Vector2(30,30), "Entonces:", new Color(0,0,1,1), 45);

        string condText;
        if(newRuleShowcase.conds.Count!=0)
            condText = BehUIRule.calculateCondText(newRuleShowcase.conds[0]);
        else condText = "";

        GameObject condTextGO = BehUIRule.createText(newRuleCanvas, new Vector2(50,-70), condText, new Color(0,0,1,1), 30);
                condTextGO.GetComponent<Text>().horizontalOverflow=HorizontalWrapMode.Wrap;
                condTextGO.GetComponent<RectTransform>().anchorMin = new Vector2(0,1);
                condTextGO.GetComponent<RectTransform>().anchorMax = new Vector2(0,1);
                condTextGO.GetComponent<RectTransform>().pivot = new Vector2(0,1);
                condTextGO.GetComponent<RectTransform>().sizeDelta = new Vector2(190,110);

        string actText = BehUIRule.calculateActionText(newRuleShowcase.action);
                GameObject actTextGO = BehUIRule.createText(newRuleCanvas, new Vector2(210,-70), actText, new Color(0,0,1,1), 20);
                actTextGO.GetComponent<Text>().horizontalOverflow=HorizontalWrapMode.Wrap;
                actTextGO.GetComponent<RectTransform>().anchorMin = new Vector2(0,1);
                actTextGO.GetComponent<RectTransform>().anchorMax = new Vector2(0,1);
                actTextGO.GetComponent<RectTransform>().pivot = new Vector2(0,1);
                actTextGO.GetComponent<RectTransform>().sizeDelta = new Vector2(210,110);
    }
}
