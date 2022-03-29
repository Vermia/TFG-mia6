using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Linq;

public class BehUIEdit : MonoBehaviour{

    public UIMovingState movingState;
    float wholeSeconds;
    float UIWidth;
    public BehCharacter player;

    Text A_value;
    Text B_value;
    Text C_value;
    Text D_value;
    Text E_value;
    Text F_value;

    Text softActionCountText;
    Text conditionCountText;

    Rule newRuleShowcase;

    Dropdown condDropdown;
    Dropdown condNumDropdown;
    Dropdown condObjDropdown;
    Dropdown condVarDropdown;
    Dropdown actDropdown;
    Dropdown condDirDropdown;
    Dropdown actDirDropdown;

    Dropdown softActTypeDropdown;
    Dropdown softActVarDropdown;
    Dropdown softActNumDropdown;
    Toggle condPositiveToggle;

    List<SoftAction> softActions;
    int selectedSoftAction;

    List<Condition> conds;
    int selectedCondition;


    void Start(){
        condDropdown = GameObject.Find("RuleEditConditionDropdown").GetComponent<Dropdown>();
        condDropdown.onValueChanged.AddListener( condDropdownListener );

        actDropdown  = GameObject.Find("RuleEditHardActionDropdown").GetComponent<Dropdown>();
        actDropdown.onValueChanged.AddListener( actDropdownListener );


        condVarDropdown = GameObject.Find("RuleEditConditionVarDropdown").GetComponent<Dropdown>();
        condNumDropdown = GameObject.Find("RuleEditConditionNumDropdown").GetComponent<Dropdown>();
        condObjDropdown = GameObject.Find("RuleEditConditionObjDropdown").GetComponent<Dropdown>();
        condDirDropdown = GameObject.Find("RuleEditConditionDirDropdown").GetComponent<Dropdown>();
        actDirDropdown  = GameObject.Find("RuleEditActionDirDropdown").GetComponent<Dropdown>();
        condPositiveToggle = GameObject.Find("RuleEditPositiveRule").GetComponent<Toggle>();

        softActTypeDropdown = GameObject.Find("RuleEditSoftActionTypeDropdown").GetComponent<Dropdown>();
        softActVarDropdown  = GameObject.Find("RuleEditSoftActionVarDropdown").GetComponent<Dropdown>();
        softActNumDropdown  = GameObject.Find("RuleEditSoftActionNumDropdown").GetComponent<Dropdown>();


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

        GameObject.Find("RuleEditPreviousConditionButton").GetComponent<Button>().onClick.AddListener( ()=>{previousCondition();} );
        GameObject.Find("RuleEditNewConditionButton").GetComponent<Button>().onClick.AddListener( ()=>{newCondition();} );
        GameObject.Find("RuleEditNextConditionButton").GetComponent<Button>().onClick.AddListener( ()=>{nextCondition();} );
        GameObject.Find("RuleEditDeleteConditionButton").GetComponent<Button>().onClick.AddListener( ()=>{
            conds.RemoveAt(selectedCondition);
            if(selectedCondition >= conds.Count){
                selectedCondition--;
            }
            if(conds.Count==0){
                selectedCondition=-1;
            }
        } );
        
        GameObject.Find("RuleEditPreviousSoftActionButton").GetComponent<Button>().onClick.AddListener( ()=>{previousSoftAction();} );
        GameObject.Find("RuleEditNewSoftActionButton").GetComponent<Button>().onClick.AddListener( ()=>{newSoftAction();} );
        GameObject.Find("RuleEditNextSoftActionButton").GetComponent<Button>().onClick.AddListener( ()=>{nextSoftAction();} );
        GameObject.Find("RuleEditDeleteSoftActionButton").GetComponent<Button>().onClick.AddListener( ()=>{
            softActions.RemoveAt(selectedSoftAction);
            if(selectedSoftAction >= softActions.Count){
                selectedSoftAction--;
            }
            if(softActions.Count==0){
                selectedSoftAction=-1;
            }
        } );

        GameObject.Find("RuleEditNewRuleButton").GetComponent<Button>().onClick.AddListener( addCurrentNewRule );
        //GameObject.Find("RuleEditAddSoftActionButton").GetComponent<Button>().onClick.AddListener( addNewSoftActionPanel );
        condPositiveToggle.GetComponent<Toggle>().onValueChanged.AddListener( togglePositiveRule );

        condNumDropdown.onValueChanged.AddListener( refreshRule );
        condVarDropdown.onValueChanged.AddListener( refreshRule );
        condObjDropdown.onValueChanged.AddListener( refreshRule );
        condDirDropdown.onValueChanged.AddListener( refreshRule );
        actDirDropdown.onValueChanged.AddListener( refreshRule );

        condNumDropdown.gameObject.SetActive(false);
        condVarDropdown.gameObject.SetActive(false);
        condObjDropdown.gameObject.SetActive(false);
        condDirDropdown.gameObject.SetActive(false);
        actDirDropdown.gameObject.SetActive(false);

        softActTypeDropdown.onValueChanged.AddListener( softTypeValueChanged );
        softActNumDropdown.onValueChanged.AddListener( refreshRule );
        softActVarDropdown.onValueChanged.AddListener( refreshRule );

        softActions = new List<SoftAction>();
        selectedSoftAction = -1; //Will have -1 as long as there is no soft action.

        conds = new List<Condition>();
        selectedCondition = -1;

        softActionCountText = GameObject.Find("RuleEditSoftActionCount").GetComponent<Text>();
        conditionCountText = GameObject.Find("RuleEditConditionCount").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update(){
        takeDataFromSoftActionUI();
        refreshRule(0);

        RectTransform rtrans = (RectTransform)GetComponentInParent(typeof(RectTransform));
        float scrW = Application.isEditor ? 2035f :  Screen.width;  //Segun si estamos en el editor de Unity o en ejecutable
        float rightBorder = -350f;
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

        softActionCountText.text = (selectedSoftAction+1).ToString() + "/" + softActions.Count; //arrays DON'T start at zero. Sometimes.
        conditionCountText.text = (selectedCondition+1).ToString() + "/" + conds.Count;

        if(softActions.Count==0){
            softActNumDropdown.gameObject.SetActive(false);
            softActVarDropdown.gameObject.SetActive(false);
            softActTypeDropdown.gameObject.SetActive(false);
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

    public void updateRuleInfo(){
        
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
            for(int i=0 ; i<player.rules.Count;++i){
                if(player.rules[i] == rule){
                    index=i;
                    break;
                }
            }

            if(index!=0){
                GameObject moveUpGO = createButton(newImg, new Vector2(30,-30), new Vector2(30,30), new Color(1, 0.5f, 0, 1), ()=>{ Debug.Log("Index = " + index);player.moveRuleUp(index); updateRuleInfo(); } );
            }

            GameObject removeGO = createButton(newImg, new Vector2(30,-80), new Vector2(30,30), new Color(1, 0, 0, 1), ()=>{ Debug.Log("Index = " + index);player.removeRule(index); updateRuleInfo(); } );

            if(index!= player.rules.Count-1 ){
                GameObject moveDownGO = createButton(newImg, new Vector2(30,-130), new Vector2(30,30), new Color(1, 0.5f, 0, 1), ()=>{Debug.Log("Index = " + index);player.moveRuleDown(index); updateRuleInfo(); } );
            }

        }

        //??????
        //BehUIRule infoRight = GameObject.Find("UICanvasImageRight").GetComponent<BehUIRule>();
        //infoRight.updateValues();
        //infoRight.updateRuleInfo();
        //infoRight.updateInventory();
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
        GameObject content = GameObject.Find("UICanvasImageEdit");
        GameObject newRuleCanvas = GameObject.Find("RuleEditNewRuleShowcase");
        foreach(Transform child in newRuleCanvas.transform){
            Destroy(child.gameObject);
        }
        newRuleCanvas.transform.SetParent(content.transform);

        BehUIRule.createText(newRuleCanvas, new Vector2(-140+25,30), "Si:" , new Color(0,0,1,1), 45);
        BehUIRule.createText(newRuleCanvas, new Vector2(30,30), "Entonces:", new Color(0,0,1,1), 45);

        string condText;
        if(newRuleShowcase.conds.Count!=0)
            condText = BehUIRule.calculateCondText(newRuleShowcase.conds[0]);
        else condText = "";

        GameObject condTextGO = BehUIRule.createText(newRuleCanvas, new Vector2(10,-70), condText, new Color(0,0,1,1), 30);
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

    void condDropdownListener(int value){
        GameObject childgroup = GameObject.Find("RuleEditConditionChildGroup");
        //foreach(Transform child in childgroup.transform){
        //    Destroy(child.gameObject);
        //}

        switch(value){
            case 0:
                condNumDropdown.gameObject.SetActive(false);
                condVarDropdown.gameObject.SetActive(false);
                condObjDropdown.gameObject.SetActive(false);
                condDirDropdown.gameObject.SetActive(false);
            break;
            case 1: //See
                condNumDropdown.gameObject.SetActive(false);
                condVarDropdown.gameObject.SetActive(false);
                condObjDropdown.gameObject.SetActive(true);
                condDirDropdown.gameObject.SetActive(true);
            break;
            case 2: case 3: case 4: //NumberEqualTo, NumberLessThan, NumberMoreThan
                condNumDropdown.gameObject.SetActive(true);
                condVarDropdown.gameObject.SetActive(true);
                condObjDropdown.gameObject.SetActive(false);
                condDirDropdown.gameObject.SetActive(false);
            break;
        }

        obtainNewRuleValues();
        reformulateNewRule();
    }

    void actDropdownListener(int value){
        switch(value){
            case 0:
                actDirDropdown.gameObject.SetActive(false);
            break;
            case 1:
                actDirDropdown.gameObject.SetActive(true);
            break;
            case 2:
                actDirDropdown.gameObject.SetActive(true);
            break;
        }
        obtainNewRuleValues();
        reformulateNewRule();
    }

    void obtainNewRuleValues(){
        newRuleShowcase = new Rule();
        Action act = new Action(HardActions.doNothing);

        //Conds
        Condition newcond = obtainCondFromValues();
        if(newcond!=null) newRuleShowcase.addCondition(newcond);

        //SoftAction
        List<SoftAction> aux = new List<SoftAction>(softActions);
        foreach(var i in aux){
            act.addSoftAction(i.copy());
        }
        
        //HardAction
        if(actDropdown.value == 0) act.hardAction = HardActions.doNothing;
        if(actDropdown.value==1){
            switch(actDirDropdown.value){
                case 0: act.hardAction = HardActions.moveRight; break;
                case 1: act.hardAction = HardActions.moveLeft;  break;
                case 2: act.hardAction = HardActions.moveUp;    break;
                case 3: act.hardAction = HardActions.moveDown;  break;
            }
        }
        if(actDropdown.value==2){
            switch(actDirDropdown.value){
                case 0: act.hardAction = HardActions.shootRight; break;
                case 1: act.hardAction = HardActions.shootLeft;  break;
                case 2: act.hardAction = HardActions.shootUp;    break;
                case 3: act.hardAction = HardActions.shootDown;  break;
            }
        }
        newRuleShowcase.setAction(act);

    }

    public void refreshRule(int value){ //needs int param to be used as dropdown listener
        obtainNewRuleValues();
        reformulateNewRule();
    }

    Condition obtainCondFromValues(){
        Conditions type = Conditions.numberEqualTo;
        Objects obj = Objects.player;
        int number=0;
        Variables vari=Variables.A;
        bool pos;
        
        switch(condDropdown.value){
            case 0: return null;
            case 1: //SEE
                type = Conditions.see;
                number = condDirDropdown.value;
                obj = (Objects) condObjDropdown.value;
            break;  
            case 2: case 3: case 4:
                if(condDropdown.value == 2) type=Conditions.numberMoreThan; 
                else if(condDropdown.value == 3) type=Conditions.numberLessThan; 
                else if(condDropdown.value == 4) type=Conditions.numberEqualTo;
                number = condNumDropdown.value;
                vari = (Variables) condVarDropdown.value;
            break;
        }

        if(condPositiveToggle.isOn){
            pos=true;
        } else pos=false;

        Condition res = new Condition(type, obj, number, vari, pos);
        return res;
    }

    void addCurrentNewRule(){
        player.addRule(newRuleShowcase);
        updateRuleInfo();
    }

    void togglePositiveRule(bool pos){
        refreshRule(0);
    }


    void previousSoftAction(){
        if(softActions.Count > 1){
            selectedSoftAction--;
            if(selectedSoftAction < 0){
                selectedSoftAction = softActions.Count - 1;
            }
        }

        loadDataIntoSoftActionUI();
    }

    void newSoftAction(){
        softActions.Add(new SoftAction(SoftActions.setVariable, Variables.A, 0));
        selectedSoftAction = softActions.Count-1;

        loadDataIntoSoftActionUI();
    }

    void nextSoftAction(){
        if(softActions.Count > 1){
            selectedSoftAction++;
            if(selectedSoftAction >= softActions.Count){
                selectedSoftAction = 0;
            }
        }

        loadDataIntoSoftActionUI(); 
    }

    //get soft action data from the selected action and load it into the dropdowns
    void loadDataIntoSoftActionUI(){
        //Type
        switch(softActions[selectedSoftAction].softAction){
            case SoftActions.setVariable:
                softActTypeDropdown.value = 0;
                softActNumDropdown.gameObject.SetActive(true);
                softActTypeDropdown.gameObject.SetActive(true);
                softActVarDropdown.gameObject.SetActive(true);
            break;
            case SoftActions.incVariable:
                softActTypeDropdown.value = 1;
                softActNumDropdown.gameObject.SetActive(false);
                softActTypeDropdown.gameObject.SetActive(true);
                softActVarDropdown.gameObject.SetActive(true);
            break;
            case SoftActions.decVariable:
                softActTypeDropdown.value = 2;
                softActNumDropdown.gameObject.SetActive(false);
                softActTypeDropdown.gameObject.SetActive(true);
                softActVarDropdown.gameObject.SetActive(true);
            break;
        }

        //Variable
        softActVarDropdown.value = (int) softActions[selectedSoftAction].affectedVariable;

        //Number
        softActNumDropdown.value = softActions[selectedSoftAction].affectedNumber;
           
    }

    void loadDataIntoConditionUI(){
        //Type
        switch(conds[selectedCondition].type){
            case Conditions.see:
                condDropdown.value = 0;
                condDirDropdown.gameObject.SetActive(true);
                condObjDropdown.gameObject.SetActive(true);
                condVarDropdown.gameObject.SetActive(false);
                condNumDropdown.gameObject.SetActive(false);
            break;
            case Conditions.numberEqualTo:
                condDropdown.value = 1;
                condDirDropdown.gameObject.SetActive(false);
                condObjDropdown.gameObject.SetActive(false);
                condVarDropdown.gameObject.SetActive(true);
                condNumDropdown.gameObject.SetActive(true);
            break;
            case Conditions.numberMoreThan:
                condDropdown.value = 2;
                condDirDropdown.gameObject.SetActive(false);
                condObjDropdown.gameObject.SetActive(false);
                condVarDropdown.gameObject.SetActive(true);
                condNumDropdown.gameObject.SetActive(true);
            break;
            case Conditions.numberLessThan:
                condDropdown.value = 3;
                condDirDropdown.gameObject.SetActive(false);
                condObjDropdown.gameObject.SetActive(false);
                condVarDropdown.gameObject.SetActive(true);
                condNumDropdown.gameObject.SetActive(true);
            break;
        }

        //Variable
        condVarDropdown.value = (int) conds[selectedCondition].affectedVariable;

        //Number
        condNumDropdown.value = conds[selectedCondition].affectedNumber;

        //Object
        condObjDropdown.value = (int) conds[selectedCondition].affectedObject;
        
        //Positive?
        condPositiveToggle.isOn = conds[selectedCondition].positive;
    }

    //Take the data from the dropdowns to update softActions
    void takeDataFromSoftActionUI(){
        if(selectedSoftAction!=-1){
            softActions[selectedSoftAction].softAction = (SoftActions) softActTypeDropdown.value;
            softActions[selectedSoftAction].affectedVariable = (Variables) softActVarDropdown.value;
            softActions[selectedSoftAction].affectedNumber = softActNumDropdown.value;
        }
        
    }
    void softTypeValueChanged(int num){
        if(num == 0){ // SET
            softActNumDropdown.gameObject.SetActive(true);
        } else{ //INC Y DEC
            softActNumDropdown.gameObject.SetActive(false);
        }
        refreshRule(0);
    }

    public void reset(){
        condDropdown.value=0;
        condNumDropdown.value=0;
        condObjDropdown.value=0;
        condVarDropdown.value=0;
        actDropdown.value=0;
        condDirDropdown.value=0;
        actDirDropdown.value=0;

        softActions.Clear();
        var content = GameObject.Find("EditRuleContent");


    }

    void previousCondition(){
        if(conds.Count > 1){
            selectedCondition--;
            if(selectedCondition < 0){
                selectedCondition = softActions.Count - 1;
            }
        }

        loadDataIntoConditionUI();
    }

    void newCondition(){
        conds.Add(new Condition(Conditions.see, Objects.player, 0, Variables.A));
        selectedCondition = softActions.Count-1;

        loadDataIntoConditionUI();
    }

    void nextCondition(){
        if(conds.Count > 1){
            selectedCondition++;
            if(selectedCondition >= conds.Count){
                selectedCondition = 0;
            }
        }

        loadDataIntoConditionUI(); 
    }
    
}

