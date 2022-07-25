using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Linq;

public enum Dir4{
    right, up, left, down, center
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

    GameObject actGroup;

    GameObject newInstructionButton;
    GameObject newConditionalButton;
    GameObject newLoopButton;

    GameObject changeToInstructionButton;
    GameObject changeToConditionalButton;
    GameObject changeToLoopButton;
    GameObject statSelector;
    GameObject changeGroup;

    GameObject editVeces;
    GameObject editVeces_value;
    Button editVeces_up;
    Button editVeces_down;

    GameObject condGroup;

    GameObject seeRight;
    GameObject seeUp;
    GameObject seeLeft;
    GameObject seeDown;
    GameObject seeDirSelector;
    GameObject seeWall;
    GameObject seeBreakable;
    GameObject seeBullet;
    GameObject seePickup;
    GameObject seeObjSelector;

    public int ruleCounter;
    Text ruleCounterText;


    public int selectedStatement;


    public Sprite[] imgs;

    void Awake(){
        movingState=UIMovingState.movingR;
        UIWidth = GetComponent<RectTransform>().sizeDelta.x;
        wholeSeconds = 1f;
//        newRuleButton = GameObject.Find("RuleEditNewRuleButton").GetComponent<Button>();
        selectedStatement = -1;
        ruleCounterText=GameObject.Find("RuleCounter").GetComponent<Text>();

        

        ruleCounter=5;

        upArrow=GameObject.Find("moveup");
        rightArrow=GameObject.Find("moveright");
        downArrow=GameObject.Find("movedown");
        leftArrow=GameObject.Find("moveleft");
        arrowSelector=GameObject.Find("arrowSelect");

        moveButtonGO=GameObject.Find("moveButton");
        shootButtonGO=GameObject.Find("shootButton");
        pickupButtonGO=GameObject.Find("pickupButton");
        actionSelector=GameObject.Find("actionSelect");
        actGroup=GameObject.Find("UIEditActionGroup");

        newInstructionButton=GameObject.Find("newInstructionButton");
        newConditionalButton=GameObject.Find("newConditionalButton");
        newLoopButton=GameObject.Find("newLoopButton");

        changeToInstructionButton=GameObject.Find("changeToInstructionButton");
        changeToConditionalButton=GameObject.Find("changeToConditionalButton");
        changeToLoopButton=GameObject.Find("changeToLoopButton");
        statSelector=GameObject.Find("statSelect");
        changeGroup=GameObject.Find("UIEditChangeGroup");

        editVeces=GameObject.Find("EditVeces");
        editVeces_up=GameObject.Find("EditVeces_up").GetComponent<Button>();
        editVeces_down=GameObject.Find("EditVeces_down").GetComponent<Button>();
        editVeces_value=GameObject.Find("EditVeces_value");

        seeRight=GameObject.Find("seeRight");
        seeUp=GameObject.Find("seeUp");
        seeLeft=GameObject.Find("seeLeft");
        seeDown=GameObject.Find("seeDown");
        seeDirSelector=GameObject.Find("seeDirSelect");
        seeWall=GameObject.Find("seeWall");
        seeBreakable=GameObject.Find("seeBreakable");
        seeBullet=GameObject.Find("seeBullet");
        seePickup=GameObject.Find("seePickup");
        seeObjSelector=GameObject.Find("seeObjSelect");
        condGroup = GameObject.Find("UIEditCondGroup");

        rightArrow.GetComponent<Button>().onClick.AddListener( ()=>{clickOnArrow(0);} );
        upArrow.GetComponent<Button>().onClick.AddListener(    ()=>{clickOnArrow(1);} );
        leftArrow.GetComponent<Button>().onClick.AddListener(  ()=>{clickOnArrow(2);} );
        downArrow.GetComponent<Button>().onClick.AddListener(  ()=>{clickOnArrow(3);} );

        moveButtonGO.GetComponent<Button>().onClick.AddListener(    ()=>{clickOnAction(0);} );
        shootButtonGO.GetComponent<Button>().onClick.AddListener(   ()=>{clickOnAction(1);} );
        pickupButtonGO.GetComponent<Button>().onClick.AddListener(  ()=>{clickOnAction(2);} );

        newInstructionButton.GetComponent<Button>().onClick.AddListener( ()=>{addNewRule();} );
        //newConditionalButton.GetComponent<Button>().onClick.AddListener( ()=>{addNewCond();} );
        //newLoopButton.GetComponent<Button>().onClick.AddListener( ()=>{addNewLoop();} );

        changeToInstructionButton.GetComponent<Button>().onClick.AddListener( ()=>{changeStatementToInstruction();} );
        changeToConditionalButton.GetComponent<Button>().onClick.AddListener( ()=>{changeStatementToConditional();} );
        changeToLoopButton.GetComponent<Button>().onClick.AddListener( ()=>{changeStatementToLoop();} );

        editVeces_up.onClick.AddListener(   ()=>{incVeces();} );
        editVeces_down.onClick.AddListener( ()=>{decVeces();} );
        
//        newRuleButton.onClick.AddListener( addNewRule );

        seeRight.GetComponent<Button>().onClick.AddListener( ()=>{pickSeeDir(seeDirections.right);seeDirSelector.transform.SetParent(seeRight.transform, false);} );
        seeUp.GetComponent<Button>().onClick.AddListener( ()=>{pickSeeDir(seeDirections.up);seeDirSelector.transform.SetParent(seeUp.transform, false);} );
        seeLeft.GetComponent<Button>().onClick.AddListener( ()=>{pickSeeDir(seeDirections.left);seeDirSelector.transform.SetParent(seeLeft.transform, false);} );
        seeDown.GetComponent<Button>().onClick.AddListener( ()=>{pickSeeDir(seeDirections.down);seeDirSelector.transform.SetParent(seeDown.transform, false);} );

        seeWall.GetComponent<Button>().onClick.AddListener( ()=>{pickSeeObject(Objects.wall); seeObjSelector.transform.SetParent(seeWall.transform, false);} );
        seeBreakable.GetComponent<Button>().onClick.AddListener( ()=>{pickSeeObject(Objects.breakablewall);seeObjSelector.transform.SetParent(seeBreakable.transform, false);} );
        seeBullet.GetComponent<Button>().onClick.AddListener( ()=>{pickSeeObject(Objects.projectile);seeObjSelector.transform.SetParent(seeBullet.transform, false);} );
        seePickup.GetComponent<Button>().onClick.AddListener( ()=>{pickSeeObject(Objects.pickup);seeObjSelector.transform.SetParent(seePickup.transform, false);} );

        editVeces.SetActive(false);
        actGroup.SetActive(false);
        condGroup.SetActive(false);
        changeGroup.SetActive(false);
    }

    void Start(){
        updateRuleInfo();
        player = BehBoard.player1.GetComponent<BehCharacter>();
        reformulateNewRule();
    }

    // Update is called once per frame
    void Update(){
        

        RectTransform rtrans = (RectTransform)GetComponentInParent(typeof(RectTransform));
        float scrW = Application.isEditor ? 2035f :  Screen.width;  //Segun si estamos en el editor de Unity o en ejecutable
        //float rightBorder = -306.25f;
        //float leftBorder = -1483f;


        //switch(movingState){
        //    case UIMovingState.standbyL:
        //        GetComponent<RectTransform>().anchoredPosition = new Vector2 (leftBorder, GetComponent<RectTransform>().anchoredPosition.y);
        //    break;
        //    case UIMovingState.standbyR:
        //        GetComponent<RectTransform>().anchoredPosition = new Vector2 (rightBorder, GetComponent<RectTransform>().anchoredPosition.y);
        //    break;
        //    case UIMovingState.movingR:
        //        if(GetComponent<RectTransform>().anchoredPosition.x >= rightBorder){
        //            movingState = UIMovingState.standbyR;
        //            break;
        //        } 
        //         GetComponent<RectTransform>().anchoredPosition += new Vector2 ((UIWidth) / wholeSeconds * Time.deltaTime, 0f);
        //         updateRuleInfo();
        //    break;
        //    
        //    case UIMovingState.movingL:
        //        if(GetComponent<RectTransform>().anchoredPosition.x <= leftBorder){
        //            movingState = UIMovingState.standbyL;
        //            break;
        //        } 
        //        GetComponent<RectTransform>().anchoredPosition -= new Vector2 ((UIWidth) / wholeSeconds * Time.deltaTime, 0f);
        //    break;
        //}

        
        
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
        if(player==null) return;
        Color textColor= new Color(0,1,0,1);
        var content = GameObject.Find("EditRuleContent");
        var auxList = new List<GameObject>();
        foreach(Transform child in content.transform){
            auxList.Add(child.gameObject);
        }
        auxList.ForEach(child => Destroy(child));

        if(ruleCounterText) ruleCounterText.text = ruleCounter.ToString();


        for(int i=0 ; i<player.statements.Count ; ++i){
            Statement stat = player.statements[i];
            

            //fondo
            float width=275;
            float textWidthOffset=20;
            float height=100;
            int textSize=25;
            GameObject newImg = new GameObject();
            newImg.AddComponent(typeof(Image));
            newImg.transform.SetParent(content.transform, false);
            newImg.transform.localScale = new Vector3(1,1,0);
            newImg.GetComponent<RectTransform>().sizeDelta = new Vector2(width,height);
            newImg.GetComponent<Image>().color = new Color(0,0,0,1);

            GameObject delButtonGO = new GameObject();
            RectTransform delButtonRTrans=delButtonGO.AddComponent<RectTransform>();
            delButtonGO.transform.SetParent(newImg.transform, false);
            delButtonGO.GetComponent<RectTransform>().localPosition=new Vector2(121,43);
            delButtonGO.GetComponent<RectTransform>().sizeDelta=new Vector2(20,20);
            Button delButton = delButtonGO.AddComponent<Button>();
            Image delButtonImg = delButtonGO.AddComponent<Image>();
            delButtonImg.sprite = imgs[7];
            int a=i; //magia oscura
            delButton.onClick.AddListener( ()=>{
                player.deleteStatement(a);
                ruleCounter++;
                selectedStatement=0;
                updateRuleInfo();

                if(player.statements.Count==0){
                    selectedStatement=-1;
                    actGroup.SetActive(false);
                    condGroup.SetActive(false);
                    editVeces.SetActive(false);
                    changeGroup.SetActive(false);
                }
            } );

            if(i==selectedStatement){
                newImg.GetComponent<Image>().color = new Color(0,0,0.25f,1);
            }

            if(stat is Instruction){
                newImg.GetComponent<RectTransform>().sizeDelta = new Vector2(width,height);

                Instruction instr=stat as Instruction;
                
                string actText = BehUIRule.calculateActionText(instr.action);
                GameObject actTextGO = BehUIRule.createText(newImg, new Vector2(textWidthOffset,-5), actText, textColor, textSize);
                actTextGO.GetComponent<Text>().horizontalOverflow=HorizontalWrapMode.Wrap;
                actTextGO.GetComponent<RectTransform>().anchorMin = new Vector2(0,1);
                actTextGO.GetComponent<RectTransform>().anchorMax = new Vector2(0,1);
                actTextGO.GetComponent<RectTransform>().pivot = new Vector2(0,1);
                actTextGO.GetComponent<RectTransform>().sizeDelta = new Vector2(width-2*textWidthOffset,height);
            }
            else if(stat is Conditional){

                Conditional condal=stat as Conditional;

                string condText = BehUIRule.calculateCondText(condal.cond);
                string actText  = BehUIRule.calculateActionText(condal.action);
                string elseText = BehUIRule.calculateActionText(condal.elseAction);
                string fullText = "Si: " + condText + ", entonces: " + actText;
                if(condal.elseAction!=null) fullText+= ", y en caso contrario: "+elseText;
                GameObject condTextGO = BehUIRule.createText(newImg, new Vector2(textWidthOffset,-5), fullText, textColor, textSize);
                condTextGO.GetComponent<Text>().horizontalOverflow=HorizontalWrapMode.Wrap;
                condTextGO.GetComponent<RectTransform>().anchorMin = new Vector2(0,1);
                condTextGO.GetComponent<RectTransform>().anchorMax = new Vector2(0,1);
                condTextGO.GetComponent<RectTransform>().pivot = new Vector2(0,1);
                condTextGO.GetComponent<RectTransform>().sizeDelta = new Vector2(width-2*textWidthOffset,height);         
            }
            else if(stat is Loop){
                Loop loop = stat as Loop;

                string fulltext = "En los siguientes " + loop.times + " turnos, " + BehUIRule.calculateActionText(loop.action);
                GameObject condTextGO = BehUIRule.createText(newImg, new Vector2(textWidthOffset,-5), fulltext, textColor, textSize);
                condTextGO.GetComponent<Text>().horizontalOverflow=HorizontalWrapMode.Wrap;
                condTextGO.GetComponent<RectTransform>().anchorMin = new Vector2(0,1);
                condTextGO.GetComponent<RectTransform>().anchorMax = new Vector2(0,1);
                condTextGO.GetComponent<RectTransform>().pivot = new Vector2(0,1);
                condTextGO.GetComponent<RectTransform>().sizeDelta = new Vector2(width-2*textWidthOffset,height);
            }

            Button selectOneStatement = newImg.AddComponent(typeof(Button)) as Button;
            selectOneStatement.onClick.AddListener( ()=>{
                selectedStatement=a; 
                updateRuleInfo();
                loadLeftToRight();
            });

        }

        GameObject newRuleButtonContainerGO = new GameObject();
        HorizontalLayoutGroup newRuleButtonContainer = newRuleButtonContainerGO.AddComponent<HorizontalLayoutGroup>();
        GameObject newInstructionButtonAtLeftMenu = createButton(content, new Vector2(0,0), new Vector2(64,64), new Color(0,1,0,1), addNewRule);
        newInstructionButtonAtLeftMenu.transform.SetParent(newRuleButtonContainerGO.transform, false);
        GameObject newConditionalButtonAtLeftMenu = createButton(content, new Vector2(0,0), new Vector2(64,64), new Color(0,1,1,1), addNewRule);
        newConditionalButtonAtLeftMenu.transform.SetParent(newRuleButtonContainerGO.transform, false);
        GameObject newLoopButtonAtLeftMenu = createButton(content, new Vector2(0,0), new Vector2(64,64), new Color(0,0,1,1), addNewRule);
        newLoopButtonAtLeftMenu.transform.SetParent(newRuleButtonContainerGO.transform, false);
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
        if(editVeces!=null && editVeces.activeSelf)editVeces.SetActive(false);
        if(actGroup!=null && actGroup.activeSelf)actGroup.SetActive(false);
        if(condGroup!=null && condGroup.activeSelf)condGroup.SetActive(false);
        if(changeGroup!=null && changeGroup.activeSelf)changeGroup.SetActive(false);
        updateRuleInfo();
    }


    void clickOnArrow(int dir){
        if(selectedStatement==-1) return;
        arrowSelector.GetComponent<Image>().color=new Color(0, 1, 0, 0.1f);
        if(player.statements[selectedStatement] is Instruction && (player.statements[selectedStatement] as Instruction).action.hardAction==HardActions.doNothing){
            clickOnAction(0);
        }
        else if(player.statements[selectedStatement] is Conditional && (player.statements[selectedStatement] as Conditional).action.hardAction==HardActions.doNothing){
            clickOnAction(0);
        }
        else if(player.statements[selectedStatement] is Loop && (player.statements[selectedStatement] as Loop).action.hardAction==HardActions.doNothing){
            clickOnAction(0);
        }
        switch(dir){
            case 0:
                currDir=Dir4.right;
                if(player.statements[selectedStatement] is Instruction)  (player.statements[selectedStatement] as Instruction).action.affectedDirection=Dir4.right;
                if(player.statements[selectedStatement] is Conditional)  (player.statements[selectedStatement] as Conditional).action.affectedDirection=Dir4.right;
                if(player.statements[selectedStatement] is Loop)  (player.statements[selectedStatement] as Loop).action.affectedDirection=Dir4.right;
                arrowSelector.transform.SetParent(rightArrow.transform, false);
            break;
            case 1:
                currDir=Dir4.up;
                if(player.statements[selectedStatement] is Instruction)  (player.statements[selectedStatement] as Instruction).action.affectedDirection=Dir4.up;
                if(player.statements[selectedStatement] is Conditional)  (player.statements[selectedStatement] as Conditional).action.affectedDirection=Dir4.up;
                if(player.statements[selectedStatement] is Loop)  (player.statements[selectedStatement] as Loop).action.affectedDirection=Dir4.up;
                arrowSelector.transform.SetParent(upArrow.transform, false);
            break;
            case 2:
                currDir=Dir4.left;
                if(player.statements[selectedStatement] is Instruction)  (player.statements[selectedStatement] as Instruction).action.affectedDirection=Dir4.left;
                if(player.statements[selectedStatement] is Conditional)  (player.statements[selectedStatement] as Conditional).action.affectedDirection=Dir4.left;
                if(player.statements[selectedStatement] is Loop)  (player.statements[selectedStatement] as Loop).action.affectedDirection=Dir4.left;
                arrowSelector.transform.SetParent(leftArrow.transform, false);
            break;
            case 3:
                currDir=Dir4.down;
                if(player.statements[selectedStatement] is Instruction)  (player.statements[selectedStatement] as Instruction).action.affectedDirection=Dir4.down;
                if(player.statements[selectedStatement] is Conditional)  (player.statements[selectedStatement] as Conditional).action.affectedDirection=Dir4.down;
                if(player.statements[selectedStatement] is Loop)  (player.statements[selectedStatement] as Loop).action.affectedDirection=Dir4.down;
                arrowSelector.transform.SetParent(downArrow.transform, false);
            break;
            default:break;
        }

        updateRuleInfo();
    }

    void clickOnAction(int act){ //0: move, 1:shoot, 2: pickup
        if(selectedStatement==-1) return;
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
        if(ruleCounter<=0){
            ruleCounter=0;
            return;
        }
        actGroup.SetActive(true);

        HardActions act=HardActions.doNothing;
        //if(actionSelector.transform.parent==shootButtonGO.transform){
        //    act=HardActions.shoot;
        //} else if(actionSelector.transform.parent==pickupButtonGO.transform){
        //    act=HardActions.pickup;
        //}
        

        Dir4 dir=Dir4.center;

        Instruction instr = new Instruction(new Action(act, dir));
        player.addStatement(instr);

        selectedStatement = player.statements.Count - 1;

        changeGroup.SetActive(true);
        ruleCounter--;
        updateRuleInfo();
        loadLeftToRight();
    }

    void addNewCond(){
        HardActions act=HardActions.move;
        if(actionSelector.transform.parent==shootButtonGO.transform){
            act=HardActions.shoot;
        } else if(actionSelector.transform.parent==pickupButtonGO.transform){
            act=HardActions.pickup;
        }
        
        Dir4 dir=currDir;

        Conditional condal = new Conditional(new Condition(Conditions.see, Objects.wall, 0,Variables.A), new Action(act, dir));
        player.addStatement(condal);

        selectedStatement = player.statements.Count - 1;
        updateRuleInfo();
    }

    void addNewLoop(){
        HardActions act=HardActions.move;
        if(actionSelector.transform.parent==shootButtonGO.transform){
            act=HardActions.shoot;
        } else if(actionSelector.transform.parent==pickupButtonGO.transform){
            act=HardActions.pickup;
        }
        
        Dir4 dir=currDir;

        Loop loop = new Loop(3,  new Action(act, dir));
        player.addStatement(loop);

        selectedStatement = player.statements.Count - 1;
        updateRuleInfo();
    }

    void loadLeftToRight(){
        Statement stat = player.statements[selectedStatement];

        if(stat is Instruction){
            Instruction instr = stat as Instruction;
            switch(instr.action.affectedDirection){
                case Dir4.right: clickOnArrow(0); break;
                case Dir4.up:    clickOnArrow(1); break;
                case Dir4.left:  clickOnArrow(2); break;
                case Dir4.down:  clickOnArrow(3); break;
                default:break;
            }
            switch(instr.action.hardAction){
                case HardActions.doNothing: actionSelector.GetComponent<Image>().color=new Color(0,0,0,0); arrowSelector.GetComponent<Image>().color=new Color(0,0,0,0);  break;
                case HardActions.move: clickOnAction(0); break;
                case HardActions.shoot: clickOnAction(1); break;
                case HardActions.pickup: clickOnAction(2); break;
                default: break;
            }
            
            
            statSelector.transform.SetParent(changeToInstructionButton.transform, false);
            editVeces.SetActive(false);
            condGroup.SetActive(false);
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
            statSelector.transform.SetParent(changeToConditionalButton.transform, false);
            editVeces.SetActive(false);
            condGroup.SetActive(true);

            switch(condal.cond.affectedNumber){
                case 0: //derecha
                    seeDirSelector.transform.SetParent( seeRight.transform, false );
                break;
                case 2: //arriba
                    seeDirSelector.transform.SetParent( seeUp.transform, false );
                break;
                case 4: //izquierda
                    seeDirSelector.transform.SetParent( seeLeft.transform, false );
                break;
                case 6: //abajo
                    seeDirSelector.transform.SetParent( seeDown.transform, false );
                break;
            }

            switch(condal.cond.affectedObject){
                case Objects.wall:
                    seeObjSelector.transform.SetParent( seeWall.transform, false );
                    seeDirSelector.GetComponent<Image>().sprite = imgs[3];
                break;
                case Objects.breakablewall:
                    seeObjSelector.transform.SetParent( seeBreakable.transform, false );
                    seeDirSelector.GetComponent<Image>().sprite = imgs[4];
                break;
                case Objects.projectile:
                    seeObjSelector.transform.SetParent( seeBullet.transform, false );
                    seeDirSelector.GetComponent<Image>().sprite = imgs[5];
                break;
                case Objects.pickup:
                    seeObjSelector.transform.SetParent( seePickup.transform, false );
                    seeDirSelector.GetComponent<Image>().sprite = imgs[6];
                break;
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
            editVeces.SetActive(true);
            condGroup.SetActive(false);

            statSelector.transform.SetParent(changeToLoopButton.transform, false);
            editVeces_value.GetComponent<Text>().text = loop.times.ToString();
        }
    }

    void changeStatementToInstruction(){
        Action origAction=null;
        Statement origStat = player.statements[selectedStatement];
        if(origStat is Conditional){
            Conditional origCondal = origStat as Conditional;
            origAction=origCondal.action;
        }
        if(origStat is Loop){
            Loop origCondal = origStat as Loop;
            origAction=origCondal.action;
        }
        if(origAction==null) return;
        Instruction instr = new Instruction(new Action(origAction.hardAction, origAction.affectedDirection));
        player.statements[selectedStatement]=instr;

        statSelector.transform.SetParent(changeToInstructionButton.transform, false);

        editVeces.SetActive(false);
        condGroup.SetActive(false);

        updateRuleInfo();
        loadLeftToRight();
    }

    void changeStatementToConditional(){
        Action origAction=null;
        Statement origStat = player.statements[selectedStatement];
        if(origStat is Instruction){
            Instruction origInstr = origStat as Instruction;
            origAction=origInstr.action;
        }
        if(origStat is Loop){
            Loop origCondal = origStat as Loop;
            origAction=origCondal.action;
        }
        
        Conditional condal = new Conditional(new Condition(Conditions.see, Objects.wall, 0,Variables.A), new Action(origAction.hardAction, origAction.affectedDirection));
        player.statements[selectedStatement]=condal;
        statSelector.transform.SetParent(changeToConditionalButton.transform, false);
        editVeces.SetActive(false);
        condGroup.SetActive(true);
        updateRuleInfo();
        loadLeftToRight();
    }

    void changeStatementToLoop(){
        Action origAction=null;
        Statement origStat = player.statements[selectedStatement];
        if(origStat is Instruction){
            Instruction origInstr = origStat as Instruction;
            origAction=origInstr.action;
        }
        if(origStat is Conditional){
            Conditional origLoop = origStat as Conditional;
            origAction=origLoop.action;
        }
        
        Loop loop = new Loop(3, new Action(origAction.hardAction, origAction.affectedDirection));
        player.statements[selectedStatement]=loop;
        statSelector.transform.SetParent(changeToLoopButton.transform, false);
        editVeces.SetActive(true);
        condGroup.SetActive(false);
        updateRuleInfo();
        loadLeftToRight();
    }

    void incVeces(){
        if(player.statements[selectedStatement] is Loop){
            Loop loop = (player.statements[selectedStatement] as Loop);
            loop.times++;
            if(loop.times>9) loop.times--;
            editVeces_value.GetComponent<Text>().text=loop.times.ToString();
        }
    
        updateRuleInfo();
    }

    void decVeces(){
        if(player.statements[selectedStatement] is Loop){
            Loop loop = (player.statements[selectedStatement] as Loop);
            loop.times--;
            if(loop.times<2) loop.times++;
            editVeces_value.GetComponent<Text>().text=loop.times.ToString();
        }

        updateRuleInfo();
    }

    void pickSeeObject(Objects pobj){
        if(player.statements[selectedStatement] is Instruction) return;
        if(player.statements[selectedStatement] is Loop) return;

        Conditional condal = player.statements[selectedStatement] as Conditional;
        condal.cond.affectedObject=pobj;

        switch(pobj){
            case Objects.wall:
                seeDirSelector.GetComponent<Image>().sprite=imgs[3];
            break;
            case Objects.breakablewall:
                seeDirSelector.GetComponent<Image>().sprite=imgs[4];
            break;
            case Objects.projectile:
                seeDirSelector.GetComponent<Image>().sprite=imgs[5];
            break;
            case Objects.pickup:
                seeDirSelector.GetComponent<Image>().sprite=imgs[6];
            break;
        }

        updateRuleInfo();
    }

    void pickSeeDir(seeDirections pdir){
        if(player.statements[selectedStatement] is Instruction) return;
        if(player.statements[selectedStatement] is Loop) return;

        Conditional condal = player.statements[selectedStatement] as Conditional;
        condal.cond.affectedNumber=(int)pdir;

        updateRuleInfo();
    }
    
}
