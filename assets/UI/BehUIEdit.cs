using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Linq;

public enum Dir4{
    right, up, left, down
}

public class BehUIEdit : MonoBehaviour{

    public UIMovingState movingState;
    float wholeSeconds;
    float UIWidth;
    public BehCharacter player;

    Button newRuleButton;
    Dir4 currDir;

    GameObject upArrow;
    GameObject leftArrow;
    GameObject downArrow;
    GameObject rightArrow;
    GameObject arrowSelector;

    GameObject moveButtonGO;
    GameObject shootButtonGO;
    GameObject pickupButtonGO;
    GameObject actionSelector;

    public int selectedStatement;


    public Sprite[] imgs;

    void Start(){
        movingState=UIMovingState.movingR;
        UIWidth = GetComponent<RectTransform>().sizeDelta.x;
        wholeSeconds = 1f;
//        newRuleButton = GameObject.Find("RuleEditNewRuleButton").GetComponent<Button>();
        selectedStatement = -1;

        player = BehBoard.player1.GetComponent<BehCharacter>();
        reformulateNewRule();

        upArrow=GameObject.Find("moveup");
        rightArrow=GameObject.Find("moveright");
        downArrow=GameObject.Find("movedown");
        leftArrow=GameObject.Find("moveleft");
        arrowSelector=GameObject.Find("arrowSelect");

        moveButtonGO=GameObject.Find("moveButton");
        shootButtonGO=GameObject.Find("shootButton");
        pickupButtonGO=GameObject.Find("pickupButton");
        actionSelector=GameObject.Find("actionSelect");

        rightArrow.GetComponent<Button>().onClick.AddListener( ()=>{clickOnArrow(0);} );
        upArrow.GetComponent<Button>().onClick.AddListener(    ()=>{clickOnArrow(1);} );
        leftArrow.GetComponent<Button>().onClick.AddListener(  ()=>{clickOnArrow(2);} );
        downArrow.GetComponent<Button>().onClick.AddListener(  ()=>{clickOnArrow(3);} );

        moveButtonGO.GetComponent<Button>().onClick.AddListener(    ()=>{clickOnAction(0);} );
        shootButtonGO.GetComponent<Button>().onClick.AddListener(   ()=>{clickOnAction(1);} );
        pickupButtonGO.GetComponent<Button>().onClick.AddListener(  ()=>{clickOnAction(2);} );

//        newRuleButton.onClick.AddListener( addNewRule );
        updateRuleInfo();
    }

    void Awake(){
        updateRuleInfo();
    }

    // Update is called once per frame
    void Update(){
        

        RectTransform rtrans = (RectTransform)GetComponentInParent(typeof(RectTransform));
        float scrW = Application.isEditor ? 2035f :  Screen.width;  //Segun si estamos en el editor de Unity o en ejecutable
        float rightBorder = -306.25f;
        float leftBorder = -1483f;


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
                 updateRuleInfo();
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

    //All rules for the player (left side of the menu)
    public void updateRuleInfo(){
        Color textColor= new Color(0,1,0,1);
        var content = GameObject.Find("EditRuleContent");
        var auxList = new List<GameObject>();
        foreach(Transform child in content.transform){
            auxList.Add(child.gameObject);
        }
        auxList.ForEach(child => Destroy(child));

        for(int i=0 ; i<player.statements.Count ; ++i){
            Statement stat = player.statements[i];
            

            //fondo
            GameObject newImg = new GameObject();
            newImg.AddComponent(typeof(Image));
            newImg.transform.SetParent(content.transform);
            newImg.transform.localScale = new Vector3(1,1,0);
            newImg.GetComponent<RectTransform>().sizeDelta = new Vector2(500,200);
            newImg.GetComponent<Image>().color = new Color(0,0,0,1);

            if(i==selectedStatement){
                //GameObject selectedImg = new GameObject();
                //selectedImg.AddComponent(typeof(Image));
                //selectedImg.transform.SetParent(newImg.transform);
                //selectedImg.transform.localScale = new Vector3(1,1,0);
                //selectedImg.GetComponent<RectTransform>().sizeDelta = new Vector2(550,250);
                //selectedImg.GetComponent<Image>().color = new Color(1,0.5f,0,1);
                newImg.GetComponent<Image>().color = new Color(0,0,0.25f,1);
            }

            if(stat is Instruction){
                newImg.GetComponent<RectTransform>().sizeDelta = new Vector2(500,100);

                Instruction instr=stat as Instruction;
                
                string actText = BehUIRule.calculateActionText(instr.action);
                GameObject actTextGO = BehUIRule.createText(newImg, new Vector2(20,0), actText, textColor, 20);
                actTextGO.GetComponent<Text>().horizontalOverflow=HorizontalWrapMode.Wrap;
                actTextGO.GetComponent<RectTransform>().anchorMin = new Vector2(0,1);
                actTextGO.GetComponent<RectTransform>().anchorMax = new Vector2(0,1);
                actTextGO.GetComponent<RectTransform>().pivot = new Vector2(0,1);
                actTextGO.GetComponent<RectTransform>().sizeDelta = new Vector2(500,110);
            }
            else if(stat is Conditional){

                Conditional condal=stat as Conditional;

                string condText = BehUIRule.calculateCondText(condal.cond);
                string actText  = BehUIRule.calculateActionText(condal.action);
                string elseText = BehUIRule.calculateActionText(condal.elseAction);
                string fullText = "Si: " + condText + ", entonces: " + actText;
                if(condal.elseAction!=null) fullText+= ", y en caso contrario: "+elseText;
                GameObject condTextGO = BehUIRule.createText(newImg, new Vector2(20,-5), fullText, textColor, 30);
                condTextGO.GetComponent<Text>().horizontalOverflow=HorizontalWrapMode.Wrap;
                condTextGO.GetComponent<RectTransform>().anchorMin = new Vector2(0,1);
                condTextGO.GetComponent<RectTransform>().anchorMax = new Vector2(0,1);
                condTextGO.GetComponent<RectTransform>().pivot = new Vector2(0,1);
                condTextGO.GetComponent<RectTransform>().sizeDelta = new Vector2(500,200);         
            }
            else if(stat is Loop){
                Loop loop = stat as Loop;

                string fulltext = "En los siguientes " + loop.times + " turnos, " + BehUIRule.calculateActionText(loop.action);
                GameObject condTextGO = BehUIRule.createText(newImg, new Vector2(20,-5), fulltext, textColor, 30);
                condTextGO.GetComponent<Text>().horizontalOverflow=HorizontalWrapMode.Wrap;
                condTextGO.GetComponent<RectTransform>().anchorMin = new Vector2(0,1);
                condTextGO.GetComponent<RectTransform>().anchorMax = new Vector2(0,1);
                condTextGO.GetComponent<RectTransform>().pivot = new Vector2(0,1);
                condTextGO.GetComponent<RectTransform>().sizeDelta = new Vector2(500,200);
            }

            Button selectOneStatement = newImg.AddComponent(typeof(Button)) as Button;
            int a=i;  //magia oscura
            selectOneStatement.onClick.AddListener( ()=>{
                selectedStatement=a; 
                updateRuleInfo();
                loadLeftToRight();
            });

        }

        GameObject newRuleButtonAtLeftMenu = createButton(content, new Vector2(0,0), new Vector2(400,100), new Color(0,1,0,1), addNewRule);
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

    //Only takes the rule from newRuleShowcase. Must use obtainNewRuleValues() to change newRuleShowcase and actually change what is displayed.
    void reformulateNewRule(){
        
    }

    void obtainNewRuleValues(){
        

    }

    public void refreshRule(int value){ //needs int param to be used as dropdown listener
        obtainNewRuleValues();
        reformulateNewRule();
    }


    public void reset(){

    }


    void clickOnArrow(int dir){
        if(selectedStatement==-1) return;
        arrowSelector.GetComponent<Image>().color=new Color(0, 1, 0, 0.1f);
        switch(dir){
            case 0:
                currDir=Dir4.right;
                if(player.statements[selectedStatement] is Instruction)  (player.statements[selectedStatement] as Instruction).action.affectedDirection=Dir4.right;
                arrowSelector.transform.SetParent(rightArrow.transform, false);
            break;
            case 1:
                currDir=Dir4.up;
                if(player.statements[selectedStatement] is Instruction)  (player.statements[selectedStatement] as Instruction).action.affectedDirection=Dir4.up;
                arrowSelector.transform.SetParent(upArrow.transform, false);
            break;
            case 2:
                currDir=Dir4.left;
                if(player.statements[selectedStatement] is Instruction)  (player.statements[selectedStatement] as Instruction).action.affectedDirection=Dir4.left;
                arrowSelector.transform.SetParent(leftArrow.transform, false);
            break;
            case 3:
                currDir=Dir4.down;
                if(player.statements[selectedStatement] is Instruction)  (player.statements[selectedStatement] as Instruction).action.affectedDirection=Dir4.down;
                arrowSelector.transform.SetParent(downArrow.transform, false);
            break;
            default:break;
        }

        updateRuleInfo();
    }

    void clickOnAction(int act){ //0: move, 1:shoot, 2: pickup
        actionSelector.GetComponent<Image>().color=new Color(0, 1, 0, 0.1f);

        Action action=null;
        if(player.statements[selectedStatement] is Instruction){
            action=(player.statements[selectedStatement] as Instruction).action;
        }else if(player.statements[selectedStatement] is Conditional){
            action=(player.statements[selectedStatement] as Conditional).action;
        }else if(player.statements[selectedStatement] is Loop){
            action=(player.statements[selectedStatement] as Loop).action;
        }
        
        rightArrow.GetComponent<Image>().sprite = imgs[act];
        upArrow.GetComponent<Image>().sprite = imgs[act];
        leftArrow.GetComponent<Image>().sprite = imgs[act];
        downArrow.GetComponent<Image>().sprite = imgs[act];
        switch(act){
            case 0:
                actionSelector.transform.SetParent(moveButtonGO.transform, false);
                action.hardAction=HardActions.move;
            break;

            case 1:
                actionSelector.transform.SetParent(shootButtonGO.transform, false);
                action.hardAction=HardActions.shoot;
            break;

            case 2:
                actionSelector.transform.SetParent(pickupButtonGO.transform, false);
                action.hardAction=HardActions.pickup;
            break;

            default:break;
        }

        updateRuleInfo();
    }

    void addNewRule(){
        HardActions act=HardActions.move;
        Dir4 dir=currDir;

        Instruction instr = new Instruction(new Action(act, dir));
        player.addStatement(instr);

        selectedStatement = player.statements.Count - 1;
        updateRuleInfo();
    }

    void loadLeftToRight(){
        Statement stat = player.statements[selectedStatement];

        if(stat is Instruction){
            Instruction instr = stat as Instruction;
            switch(instr.action.hardAction){
                case HardActions.move: clickOnAction(0); break;
                case HardActions.shoot: clickOnAction(1); break;
                case HardActions.pickup: clickOnAction(2); break;
                default:break;
            }
            switch(instr.action.affectedDirection){
                case Dir4.right: clickOnArrow(0); break;
                case Dir4.up:    clickOnArrow(1); break;
                case Dir4.left:  clickOnArrow(2); break;
                case Dir4.down:  clickOnArrow(3); break;
                default:break;
            }
        }
        else if (stat is Conditional){
            Conditional condal = stat as Conditional;
            switch(condal.action.hardAction){
                case HardActions.move: clickOnAction(0); break;
                case HardActions.shoot: clickOnAction(1); break;
                case HardActions.pickup: clickOnAction(2); break;
                default:break;
            }
            switch(condal.action.affectedDirection){
                case Dir4.right: clickOnArrow(0); break;
                case Dir4.up:    clickOnArrow(1); break;
                case Dir4.left:  clickOnArrow(2); break;
                case Dir4.down:  clickOnArrow(3); break;
                default:break;
            }
        }
        else if(stat is Loop){
            Loop loop = stat as Loop;
            switch(loop.action.hardAction){
                case HardActions.move: clickOnAction(0); break;
                case HardActions.shoot: clickOnAction(1); break;
                case HardActions.pickup: clickOnAction(2); break;
                default:break;
            }
            switch(loop.action.affectedDirection){
                case Dir4.right: clickOnArrow(0); break;
                case Dir4.up:    clickOnArrow(1); break;
                case Dir4.left:  clickOnArrow(2); break;
                case Dir4.down:  clickOnArrow(3); break;
                default:break;
            }
        }
    }
    
}
