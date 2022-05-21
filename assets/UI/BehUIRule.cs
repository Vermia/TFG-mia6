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

    
    static public Color textColor{get; private set;}
   

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
        
        

        textColor = new Color(0,1,0,1);
    }

     // Start is called before the first frame update
    void Start(){
        newRuleInfo(BehBoard.player2.GetComponent<BehCharacter>());
    }

    // Update is called once per frame
    void Update(){
        RectTransform rtrans = (RectTransform)GetComponentInParent(typeof(RectTransform));
        float scrW = Application.isEditor ? 2035f :  Screen.width;  //Segun si estamos en el editor de Unity o en ejecutable
        float LPos=693.36f;
        float RPos=1227.5f;

        switch(movingState){
            case UIMovingState.standbyL:
                if(Input.GetKey(KeyCode.Space)){
                    movingState=UIMovingState.movingR;
                }
                GetComponent<RectTransform>().anchoredPosition = new Vector2(LPos, -108f); 
            break;
            case UIMovingState.standbyR:
                if(Input.GetKey(KeyCode.Space)){
                    movingState=UIMovingState.movingL;
                }
            break;
            case UIMovingState.movingR:
                if(GetComponent<RectTransform>().anchoredPosition.x >= RPos){
                    movingState = UIMovingState.standbyR;
                    break;
                } 
                GetComponent<RectTransform>().anchoredPosition += new Vector2(500 / wholeSeconds * Time.deltaTime, 0f); 
                    
            break;
            
            case UIMovingState.movingL:
                if(GetComponent<RectTransform>().anchoredPosition.x <= LPos){
                    movingState = UIMovingState.standbyL;
                    break;
                } 
                GetComponent<RectTransform>().anchoredPosition -= new Vector2(500 / wholeSeconds * Time.deltaTime, 0f); 
                
            break;
        }

        if(BehBoard.newTurn){
            updateValues();
            updateRuleInfo();
            updateInventory();
        }
    }

    public void newRuleInfo(BehCharacter pnewSource){
        infoSource=pnewSource;
        updateValues();
        updateStatementInfo();
        updateInventory();
    }

    public void updateStatementInfo(){
        var content = GameObject.Find("RuleContent");
        var auxList = new List<GameObject>();
        foreach(Transform child in content.transform){
            auxList.Add(child.gameObject);
        }
        auxList.ForEach(child => Destroy(child));

        
        for(int i=0 ; i<infoSource.statements.Count ; ++i){
            Statement stat = infoSource.statements[i];
            //fondo
            GameObject newImg = new GameObject();
            newImg.AddComponent(typeof(Image));
            newImg.transform.SetParent(content.transform);
            newImg.transform.localScale = new Vector3(1,1,0);
            newImg.GetComponent<RectTransform>().sizeDelta = new Vector2(500,200);
            newImg.GetComponent<Image>().color = new Color(0,0,0,1);

            if(stat is Instruction){
                Debug.Log("es una instruccion");
                newImg.GetComponent<RectTransform>().sizeDelta = new Vector2(500,100);

                Instruction instr=stat as Instruction;
                
                string actText = calculateActionText(instr.action);
                GameObject actTextGO = createText(newImg, new Vector2(20,0), actText, textColor, 20);
                actTextGO.GetComponent<Text>().horizontalOverflow=HorizontalWrapMode.Wrap;
                actTextGO.GetComponent<RectTransform>().anchorMin = new Vector2(0,1);
                actTextGO.GetComponent<RectTransform>().anchorMax = new Vector2(0,1);
                actTextGO.GetComponent<RectTransform>().pivot = new Vector2(0,1);
                actTextGO.GetComponent<RectTransform>().sizeDelta = new Vector2(500,110);
            }
            else if(stat is Conditional){
                Debug.Log("es un condicional");

                Conditional condal=stat as Conditional;

                string condText = calculateCondText(condal.cond);
                string actText  = calculateActionText(condal.action);
                string elseText = calculateActionText(condal.elseAction);
                string fullText = "Si: " + condText + ", entonces: " + actText;
                if(condal.elseAction!=null) fullText+= ", y en caso contrario: "+elseText;
                GameObject condTextGO = createText(newImg, new Vector2(20,-5), fullText, textColor, 30);
                condTextGO.GetComponent<Text>().horizontalOverflow=HorizontalWrapMode.Wrap;
                condTextGO.GetComponent<RectTransform>().anchorMin = new Vector2(0,1);
                condTextGO.GetComponent<RectTransform>().anchorMax = new Vector2(0,1);
                condTextGO.GetComponent<RectTransform>().pivot = new Vector2(0,1);
                condTextGO.GetComponent<RectTransform>().sizeDelta = new Vector2(500,200);         
            }
            else if(stat is Loop){
                Debug.Log("es un bucle");
                Loop loop = stat as Loop;

                string fulltext = "En los siguientes " + loop.times + " turnos, " + calculateActionText(loop.action);
                GameObject condTextGO = createText(newImg, new Vector2(20,-5), fulltext, textColor, 30);
                condTextGO.GetComponent<Text>().horizontalOverflow=HorizontalWrapMode.Wrap;
                condTextGO.GetComponent<RectTransform>().anchorMin = new Vector2(0,1);
                condTextGO.GetComponent<RectTransform>().anchorMax = new Vector2(0,1);
                condTextGO.GetComponent<RectTransform>().pivot = new Vector2(0,1);
                condTextGO.GetComponent<RectTransform>().sizeDelta = new Vector2(500,200);
            }
        }
    }

    public void updateRuleInfo(){
        ////Limpiar reglas
        //var content = GameObject.Find("RuleContent");
        //var auxList = new List<GameObject>();
        //foreach(Transform child in content.transform){
        //    auxList.Add(child.gameObject);
        //}
        //auxList.ForEach(child => Destroy(child));
//
        ////Nuevas reglas
        //foreach(Rule rule in infoSource.rules){
        //    //Imagen de fondo de cada regla
        //    GameObject newImg = new GameObject();
        //    newImg.AddComponent(typeof(Image));
        //    newImg.transform.SetParent(content.transform);
        //    newImg.transform.localScale = new Vector3(1,1,0);
        //    newImg.GetComponent<RectTransform>().sizeDelta = new Vector2(500,200);
        //    newImg.GetComponent<Image>().color = new Color(0,0,0,1);
//
        //    //Texto común a todas las reglas
        //    createText(newImg, new Vector2(-140,40), "Si:", textColor, 45);
        //    createText(newImg, new Vector2(60,40), "Entonces:", textColor, 45);
//
        //    //Condiciones
        //    string condText="";
        //    for(int i = 0 ; i < rule.conds.Count ; ++i){
        //        //CONJUNCION
        //        if(i>0){
        //            condText += rule.condAND ? " Y " :  " O ";
        //        }
//
        //        condText += calculateCondText(rule.conds[i]);
        //    }
//
        //    //if(rule.conds.Count!=0)
        //    //    condText = calculateCondText(rule.conds[0]);
        //    //else condText = "";
//
//
        //    GameObject condTextGO = createText(newImg, new Vector2(10,-70), condText, textColor, 30);
        //    condTextGO.GetComponent<Text>().horizontalOverflow=HorizontalWrapMode.Wrap;
        //    condTextGO.GetComponent<RectTransform>().anchorMin = new Vector2(0,1);
        //    condTextGO.GetComponent<RectTransform>().anchorMax = new Vector2(0,1);
        //    condTextGO.GetComponent<RectTransform>().pivot = new Vector2(0,1);
        //    condTextGO.GetComponent<RectTransform>().sizeDelta = new Vector2(190,110);
//
        //    //Acciones
        //    string actText = calculateActionText(rule.action);
        //    GameObject actTextGO = createText(newImg, new Vector2(270,-70), actText, textColor, 20);
        //    actTextGO.GetComponent<Text>().horizontalOverflow=HorizontalWrapMode.Wrap;
        //    actTextGO.GetComponent<RectTransform>().anchorMin = new Vector2(0,1);
        //    actTextGO.GetComponent<RectTransform>().anchorMax = new Vector2(0,1);
        //    actTextGO.GetComponent<RectTransform>().pivot = new Vector2(0,1);
        //    actTextGO.GetComponent<RectTransform>().sizeDelta = new Vector2(210,110);
        //}
    }

    public void updateInventory(){
        //Limpiar reglas
        var content = GameObject.Find("InventoryContent");
        var auxList = new List<GameObject>();
        foreach(Transform child in content.transform){
            auxList.Add(child.gameObject);
        }
        auxList.ForEach(child => Destroy(child));

        //Nuevas reglas
        foreach(Item item in infoSource.inventory){
            //Imagen de fondo de cada regla
            GameObject newImg = new GameObject();
            newImg.AddComponent(typeof(Image));
            newImg.transform.SetParent(content.transform);
            newImg.transform.localScale = new Vector3(1,1,0);
            newImg.GetComponent<RectTransform>().sizeDelta = new Vector2(265,100);
            newImg.GetComponent<Image>().color = new Color(0,0,0,1);

            //Item
            string typeText="";
            switch(item.type){
                case ItemTypes.gun:
                    typeText="Pistola";
                break;
                case ItemTypes.stars:
                    typeText="Estrellas";
                break;
            }

            string chargesText = item.charges.ToString();


            GameObject condTextGO = createText(newImg, new Vector2(10,0), typeText, textColor, 30);
            condTextGO.GetComponent<Text>().horizontalOverflow=HorizontalWrapMode.Wrap;
            condTextGO.GetComponent<RectTransform>().anchorMin = new Vector2(0,1);
            condTextGO.GetComponent<RectTransform>().anchorMax = new Vector2(0,1);
            condTextGO.GetComponent<RectTransform>().pivot = new Vector2(0,1);
            condTextGO.GetComponent<RectTransform>().sizeDelta = new Vector2(190,110);

            //Acciones
            GameObject actTextGO = createText(newImg, new Vector2(190,0), chargesText, textColor, 30);
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
        txtConditionComp.resizeTextForBestFit=true;
        
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
                case Objects.player:
                    res+= "Jugador ";
                break;
                case Objects.wall:
                    res+= "Muro ";
                break;
                case Objects.pickup:
                    res+= "Recogible ";
                break;
                case Objects.projectile:
                    res+= "Bala ";
                break;
            }

            switch(cond.affectedNumber){
                case 0:  res+="a mi derecha";                   break;
                case 1:  res+="arriba a la derecha";            break;
                case 2:  res+="arriba de mí";                   break;
                case 3:  res+="arriba a la izquierda";          break;
                case 4:  res+="a mi izquierda";                 break;
                case 5:  res+="abajo a la izquierda";           break;
                case 6:  res+="debajo de mí";                   break;
                case 7:  res+="abajo a la derecha";             break;
                case 8:  res+="dos casillas a mi derecha";      break;
                case 9:  res+="dos casillas arriba de mí";      break;
                case 10: res+="dos casillas a mi izquierda";    break;
                case 11: res+="dos casillas debajo de mi";      break;
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
            case HardActions.move:      res += "Me muevo "; break;
            case HardActions.shoot:     res += "Disparo "; break;
            case HardActions.pickup:    res += "Recojo un objeto "; break;
            default: break;
        }
        switch(action.affectedDirection){
            case Dir4.right:      res += "a mi derecha"; break;
            case Dir4.up:     res += "hacia arriba"; break;
            case Dir4.left:    res += "a mi izquierda"; break;
            case Dir4.down:    res += "hacia abajo"; break;
            default: break;
        }

        if(action.hardAction==HardActions.doNothing) res="No hago nada";
        return res;
    }
}
