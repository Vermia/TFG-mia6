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

    Text softActionCountText;

    Rule newRuleShowcase;

    Dropdown condDropdown;
    Dropdown condNumDropdown;
    Dropdown condObjDropdown;
    Dropdown condVarDropdown;
    Dropdown actDropdown;
    Dropdown condDirDropdown;
    Dropdown actDirDropdown;
    Toggle condPositiveToggle;

    List<SoftAction> softActions;
    int selectedSoftAction;



    // Start is called before the first frame update
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
        
        GameObject.Find("RuleEditPreviousSoftActionButton").GetComponent<Button>().onClick.AddListener( ()=>{previousSoftAction();} );
        GameObject.Find("RuleEditNewSoftActionButton").GetComponent<Button>().onClick.AddListener( ()=>{newSoftAction();} );
        GameObject.Find("RuleEditNextSoftActionButton").GetComponent<Button>().onClick.AddListener( ()=>{nextSoftAction();} );

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

        softActions = new List<SoftAction>();
        selectedSoftAction=-1; //Will have -1 as long as there is no soft action.

        softActionCountText = GameObject.Find("RuleEditSoftActionCount").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update(){

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
        foreach(var s_act in softActions){
            act.addSoftAction(s_act);
        }
        
        //HardAction
        if(actDropdown.value == 0) newRuleShowcase.action.hardAction = HardActions.doNothing;
        if(actDropdown.value==1){
            switch(actDirDropdown.value){
                case 0: newRuleShowcase.action.hardAction = HardActions.moveRight; break;
                case 1: newRuleShowcase.action.hardAction = HardActions.moveLeft;  break;
                case 2: newRuleShowcase.action.hardAction = HardActions.moveUp;    break;
                case 3: newRuleShowcase.action.hardAction = HardActions.moveDown;  break;
            }
        }
        //newRuleShowcase.setAction(new Action( (HardActions)actDropdown ));

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

    void addNewSoftActionPanel(){
        /*GameObject parent = new GameObject();
        parent.AddComponent( typeof(HorizontalLayoutGroup) );
        parent.transform.SetParent( GameObject.Find("RuleEditSoftActionContent").transform );


        GameObject typeDropdownGO = new GameObject();
        typeDropdownGO.transform.SetParent( parent.transform );
        Dropdown typeDropdown = (Dropdown) typeDropdownGO.AddComponent(typeof(Dropdown));
        List<Dropdown.OptionData> types = new List<Dropdown.OptionData>();
        types.Add( new Dropdown.OptionData("Dar valor"));
        types.Add( new Dropdown.OptionData("Incrementar"));
        types.Add( new Dropdown.OptionData("Decrementar"));
        typeDropdown.AddOptions(types);

        GameObject VarDropdownGO = new GameObject();
        VarDropdownGO.transform.SetParent( parent.transform );
        Dropdown VarDropdown = (Dropdown) VarDropdownGO.AddComponent(typeof(Dropdown));
        List<Dropdown.OptionData> vars = new List<Dropdown.OptionData>();
        vars.Add( new Dropdown.OptionData("A"));
        vars.Add( new Dropdown.OptionData("B"));
        vars.Add( new Dropdown.OptionData("C"));
        vars.Add( new Dropdown.OptionData("D"));
        vars.Add( new Dropdown.OptionData("E"));
        vars.Add( new Dropdown.OptionData("F"));
        typeDropdown.AddOptions(vars);

        GameObject NumDropdownGO = new GameObject();
        NumDropdownGO.transform.SetParent( parent.transform );
        Dropdown NumDropdown = (Dropdown) NumDropdownGO.AddComponent(typeof(Dropdown));
        List<Dropdown.OptionData> nums = new List<Dropdown.OptionData>();
        for(int i=0 ; i<=9 ; i++){
            nums.Add( new Dropdown.OptionData(i.ToString()));
        }

        softActions.Add( new SoftActionPanel(typeDropdown, VarDropdown, NumDropdown) );*/
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
    }

    void newSoftAction(){
        softActions.Add(new SoftAction(SoftActions.setVariable, Variables.A, 0));
        selectedSoftAction = softActions.Count-1;
    }

    void nextSoftAction(){
        if(softActions.Count > 1){
            selectedSoftAction++;
            if(selectedSoftAction >= softActions.Count){
                selectedSoftAction = 0;
            }
        }
    }

    //get soft action data from code and load it into the dropdowns
    void loadDataIntoSoftActionUI(int index){
        
    }

    //Take the data from the dropdowns to update softActions
    void takeDataFromSoftActionUI(int index){

    }

}
