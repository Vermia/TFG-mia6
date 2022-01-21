using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class BehCharacter : MonoBehaviour
{
    public GameObject currentSquare;
    public GameObject targetSquare;
    public HardActions currentTurn;

    public Rule[] rules;

    private float speedUnit = 1.28f;
    public Objects objectType;


    //Atributos del elemento
    public int maxHP;
    public int currHP;
    public int maxStars;
    public int currStars;
    public int A;
    public int B;
    public int C;
    public int D;
    public int E;
    public int F;
    public string nombre;

    // Start is called before the first frame update
    void Start(){
        
    }

    void Awake(){
        rules = new Rule[1];
        rules[0] = new Rule();
        rules[0].setAction(new Action(HardActions.doNothing));
        A=1;
        B=1;
        C=1;
        D=1;
        E=1;
        F=1;
        maxHP = 60;
        currHP = 60;
        maxStars=100;
        currStars=0;
    }

    // Update is called once per frame
    void Update(){
        if(BehBoard.newTurn){
            decideNextTurn();
        }else{
            continueTurn();
        }

         if (Input.GetMouseButtonDown(0)){
             RaycastHit raycastHit;
             Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
             if (Physics.Raycast(ray, out raycastHit, 100f)){
                 if (raycastHit.transform != null){
                    BehUIRule ui = GameObject.Find("UICanvasImageRight").GetComponent<BehUIRule>();
                    ui.expand();
                    ui.newRuleInfo(this);
                    Debug.Log("Hit");
                 }
             }
         }
    }

    void OnMouseOver(){
        if(Input.GetMouseButtonDown(0)){
            BehUIRule ui = GameObject.Find("UICanvasImageRight").GetComponent<BehUIRule>();
            ui.expand();
            ui.newRuleInfo(this);
        }
    }

    void continueTurn(){
        //Debug.Log("Action: " + currentTurn);
        if(currentTurn == HardActions.moveUp){
            gameObject.transform.position = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + speedUnit*Time.deltaTime);
        } else if(currentTurn == HardActions.moveLeft){
            gameObject.transform.position = new Vector2(gameObject.transform.position.x - speedUnit*Time.deltaTime, gameObject.transform.position.y);
        } else if(currentTurn == HardActions.moveDown){
            gameObject.transform.position = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - speedUnit*Time.deltaTime);
        } else if(currentTurn == HardActions.moveRight){
            gameObject.transform.position = new Vector2(gameObject.transform.position.x + speedUnit*Time.deltaTime, gameObject.transform.position.y);
        } else{
            
        }
    }

    void decideNextTurn(){
        occupy(targetSquare);
        BehSquare mySquareBehavior = currentSquare.GetComponent<BehSquare>();

        currentTurn=HardActions.doNothing;
        for(int i=0 ; i<rules.Length ; ++i){
            if(rules[i].run(this)){
                break;
            }
        }


        //Debug.Log("My square: [" + mySquareBehavior.i + ", " + mySquareBehavior.j + "]");
        //if(mySquareBehavior.i != 0){
        //    currentTurn = HardActions.moveLeft; targetSquare = BehBoard.board[ mySquareBehavior.i - 1, mySquareBehavior.j ];
        //} else if(mySquareBehavior.j != 0){
        //    currentTurn = HardActions.moveUp; targetSquare = BehBoard.board[ mySquareBehavior.i, mySquareBehavior.j - 1 ];
        //} else {
        //    currentTurn = HardActions.doNothing;
        //}
    }

    void occupy(GameObject newSquare){
        currentSquare = newSquare;
        transform.position = new Vector2(newSquare.transform.position.x, newSquare.transform.position.y);
        //if(BehBoard.newTurn){
        //    BehBoard.newTurn=false;
        //}
    }

}
