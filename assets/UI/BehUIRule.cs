using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum UIMovingState{
    standbyL, standbyR, movingL, movingR
}



public class BehUIRule : MonoBehaviour
{
    public UIMovingState movingState;
    float wholeSeconds;
    float UIWidth;
    BehCharacter infoSource;

    Text HP_value;
    Text Stars_value;
    Text A_value;
    Text B_value;
    Text C_value;
    Text D_value;
    Text E_value;
    Text F_value;
    Text name_value;
    

   

    void Awake(){
        movingState=UIMovingState.standbyR;
        wholeSeconds = 1f;
        UIWidth = GetComponent<RectTransform>().sizeDelta.x;
        //ruleArrayInfo = BehBoard.things[0].GetComponent<BehCharacter>().rules;
        A_value = GameObject.Find("A_value").GetComponent<Text>();
        B_value = GameObject.Find("B_value").GetComponent<Text>();
        C_value = GameObject.Find("C_value").GetComponent<Text>();
        D_value = GameObject.Find("D_value").GetComponent<Text>();
        E_value = GameObject.Find("E_value").GetComponent<Text>();
        F_value = GameObject.Find("F_value").GetComponent<Text>();
        HP_value = GameObject.Find("HP_value").GetComponent<Text>();
        Stars_value = GameObject.Find("Stars_value").GetComponent<Text>();
        name_value = GameObject.Find("Name_value").GetComponent<Text>();
        
        
    }

     // Start is called before the first frame update
    void Start(){
        newRuleInfo(BehBoard.player2.GetComponent<BehCharacter>());
    }

    // Update is called once per frame
    void Update(){
        RectTransform rtrans = (RectTransform)GetComponentInParent(typeof(RectTransform));
        float scrW = Application.isEditor ? 2035f :  Screen.width;  //Segun si estamos en el editor de Unity o en ejecutable


        switch(movingState){
            case UIMovingState.standbyL:
                if(Input.GetKey(KeyCode.Space)){
                    movingState=UIMovingState.movingR;
                }
                GetComponent<RectTransform>().anchoredPosition = new Vector2(scrW/2 - UIWidth/2, -108f); 
            break;
            case UIMovingState.standbyR:
                if(Input.GetKey(KeyCode.Space)){
                    movingState=UIMovingState.movingL;
                }
            break;
            case UIMovingState.movingR:
                if(GetComponent<RectTransform>().anchoredPosition.x >= scrW/2+UIWidth/2){
                    movingState = UIMovingState.standbyR;
                    break;
                } 
                GetComponent<RectTransform>().anchoredPosition += new Vector2((UIWidth) / wholeSeconds * Time.deltaTime, 0f); 
                    
            break;
            
            case UIMovingState.movingL:
                if(GetComponent<RectTransform>().anchoredPosition.x <= scrW/2-UIWidth/2){
                    movingState = UIMovingState.standbyL;
                    break;
                } 
                GetComponent<RectTransform>().anchoredPosition -= new Vector2((UIWidth) / wholeSeconds * Time.deltaTime, 0f); 
                
            break;
        }

        updateValues();
    }

    public void newRuleInfo(BehCharacter pnewSource){
        infoSource=pnewSource;
        updateValues();
        updateRuleInfo();
    }

    void updateRuleInfo(){
        
        //name_value.text = ;

        //Limpiar reglas
        var content = GameObject.Find("RuleContent");
        var auxList = new List<GameObject>();
        foreach(Transform child in content.transform){
            auxList.Add(child.gameObject);
        }
        auxList.ForEach(child => Destroy(child));

        //Nuevas reglas
        foreach(Rule rule in infoSource.rules){
            //Imagen de fondo de cada regla
            GameObject newImg = new GameObject();
            newImg.AddComponent(typeof(Image));
            newImg.transform.SetParent(content.transform);
            newImg.transform.localScale = new Vector3(1,1,0);
            newImg.GetComponent<RectTransform>().sizeDelta = new Vector2(500,200);

            //Texto común a todas las reglas
            createText(newImg, new Vector2(-140,40), "Si:", new Color(0,0,1,1), 45);
            createText(newImg, new Vector2(60,40), "Entonces:", new Color(0,0,1,1), 45);

            //Condiciones
            string condText;
            if(rule.conds.Count!=0)
                condText = calculateCondText(rule.conds[0]);
            else condText = "";


                GameObject condTextGO = createText(newImg, new Vector2(10,-70), condText, new Color(0,0,1,1), 30);
                condTextGO.GetComponent<Text>().horizontalOverflow=HorizontalWrapMode.Wrap;
                condTextGO.GetComponent<RectTransform>().anchorMin = new Vector2(0,1);
                condTextGO.GetComponent<RectTransform>().anchorMax = new Vector2(0,1);
                condTextGO.GetComponent<RectTransform>().pivot = new Vector2(0,1);
                condTextGO.GetComponent<RectTransform>().sizeDelta = new Vector2(190,110);

            string actText = calculateActionText(rule.action);
            GameObject actTextGO = createText(newImg, new Vector2(270,-70), actText, new Color(0,0,1,1), 20);
            actTextGO.GetComponent<Text>().horizontalOverflow=HorizontalWrapMode.Wrap;
            actTextGO.GetComponent<RectTransform>().anchorMin = new Vector2(0,1);
            actTextGO.GetComponent<RectTransform>().anchorMax = new Vector2(0,1);
            actTextGO.GetComponent<RectTransform>().pivot = new Vector2(0,1);
            actTextGO.GetComponent<RectTransform>().sizeDelta = new Vector2(210,110);
        }
    }

    public void updateValues(){
//Valores texto inmediatos
        A_value.text = infoSource.variables[(int)Variables.A].ToString();
        B_value.text = infoSource.variables[(int)Variables.B].ToString();
        C_value.text = infoSource.variables[(int)Variables.C].ToString();
        D_value.text = infoSource.variables[(int)Variables.D].ToString();
        E_value.text = infoSource.variables[(int)Variables.E].ToString();
        F_value.text = infoSource.variables[(int)Variables.F].ToString();
        name_value.text = infoSource.nombre.ToString();
        HP_value.text = infoSource.currHP.ToString() + "/" + infoSource.maxHP.ToString();
        Stars_value.text = infoSource.currStars.ToString() + "/" + infoSource.maxStars.ToString();
    }


    public void expand(){
        if(movingState == UIMovingState.standbyR)
            movingState=UIMovingState.movingL;
    }

    public void retract(){
        movingState=UIMovingState.movingR;
    }

    public static GameObject createText(GameObject parent, Vector2 pos, string textContent, Color color, int size){
        GameObject txtCondition = new GameObject();

        txtCondition.AddComponent(typeof(Text));
        Text txtConditionComp = txtCondition.GetComponent<Text>();
        txtConditionComp.text = textContent;
        txtConditionComp.color= color;
        Font font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
        txtConditionComp.font = font;
        txtConditionComp.fontSize = size;
        txtCondition.transform.SetParent(parent.transform);
        txtCondition.transform.localScale = new Vector2(1,1); //????????
        txtCondition.GetComponent<RectTransform>().localPosition = new Vector2(pos.x, pos.y);
        txtConditionComp.horizontalOverflow = HorizontalWrapMode.Overflow;
        
        return txtCondition;
    }

    public static string calculateCondText(Condition cond){
        string res = "";
        if(cond==null) return "";

        switch(cond.type){
            case Conditions.see:
                res+= cond.positive ? "Veo " : "No veo ";
            break;
            case Conditions.numberEqualTo: case Conditions.numberLessThan: case Conditions.numberMoreThan:
                res+= "Mi variable ";
                switch(cond.affectedVariable){
                    case Variables.A: res += "A "; break;
                    case Variables.B: res += "B "; break;
                    case Variables.C: res += "C "; break;
                    case Variables.D: res += "D "; break;
                    case Variables.E: res += "E "; break;
                    case Variables.F: res += "F "; break;
                }
                if(!cond.positive) res +="no ";
                res += "es ";

                if(cond.type == Conditions.numberEqualTo) res+="igual a ";
                 else if(cond.type == Conditions.numberLessThan) res+="menor que ";
                 else if(cond.type == Conditions.numberMoreThan) res+="mayor que ";

                res+= cond.affectedNumber;
            break;

            default: break;
        }

        

        if(cond.type == Conditions.see){
            switch(cond.affectedObject){
                case Objects.wall:
                    res+= "Muro ";
                break;
            }

            switch(cond.affectedNumber){
                case 0: res+="a mi derecha";   break;
                case 2: res+="arriba de mí";   break;
                case 4: res+="a mi izquierda"; break;
                case 6: res+="debajo de mí";   break;
            }
        }

        

        return res;
    }

    public static string calculateActionText(Action action){
        string res="";

        foreach(SoftAction act in action.softActions){
            string vari="";
            switch(act.affectedVariable){
                case Variables.A: vari= "A "; break;
                case Variables.B: vari= "B "; break;
                case Variables.C: vari= "C "; break;
                case Variables.D: vari= "D "; break;
                case Variables.E: vari= "E "; break;
                case Variables.F: vari= "F "; break;
            }

            switch(act.softAction){
                case SoftActions.setVariable:
                    res+= "Le doy el valor " + act.affectedNumber.ToString() + " a mi variable " + vari;
                break;
                case SoftActions.incVariable:
                    res+= "Sumo 1 a mi variable " + vari;
                break;
                case SoftActions.decVariable:
                    res+= "Resto 1 a mi variable " + vari;
                break;
            }
            res+="\n";
        }

        switch(action.hardAction){
            case HardActions.moveRight: res += "Me muevo a mi derecha"; break;
            case HardActions.moveUp:    res += "Me muevo hacia arriba"; break;
            case HardActions.moveLeft:  res += "Me muevo a mi izquierda"; break;
            case HardActions.moveDown:  res += "Me muevo hacia abajo"; break;
            case HardActions.doNothing: res += "No hago nada"; break;

            default: break;
        }

        return res;
    }
}
