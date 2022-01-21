
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class BehBoard : MonoBehaviour
{
    public GameObject squareEmpty;
    public GameObject character;
    public Sprite wallSprite;

    static public GameObject[,] board{get; private set;}
    static public List<GameObject> things{get; private set;}
    static public bool newTurn{get; private set;}
    static public int widthInSquares {get; private set;} 
    static public int heightInSquares {get; private set;} 

    private float turnTimer;
    private float squareWH = 1.28f; // 64 * 2 / 100
    private int currentTurn;
    Text turnText;

    public static GameObject player1{get; private set;}
    public static GameObject player2{get; private set;}

    // Start is called before the first frame update
    void Start(){
        turnText = GameObject.Find("Turn").GetComponent<Text>();
        squareWH = 1.28f; // 64 * 2 / 100

        newTurn=true;
        turnTimer=5f;
        widthInSquares=10;
        heightInSquares=7;

        board = new GameObject[widthInSquares, heightInSquares];
        things = new List<GameObject>();

        for(int i=0 ; i<widthInSquares ; i++){
            for(int j=0 ; j<heightInSquares ; j++){
                board[i,j] = Instantiate(squareEmpty);
                board[i,j].transform.position = new Vector2( i*squareWH, -j*squareWH );
                board[i,j].SendMessage("setPosition", new Vector2i(i,j));
                if(i==2 && j==5){
                    things.Add(Instantiate(character));
                    int last = things.Count-1;
                    things[last].transform.position = new Vector2( i*squareWH, -j*squareWH );
                    board[i,j].SendMessage("setOccupant", things[last]);
                    things[last].GetComponent<BehCharacter>().currentSquare = board[i,j];
                    things[last].GetComponent<BehCharacter>().targetSquare  = board[i,j];

                    things[last].GetComponent<BehCharacter>().variables[(int)Variables.B]=2;
                    things[last].GetComponent<BehCharacter>().variables[(int)Variables.C]=2;

                    things[last].GetComponent<BehCharacter>().rules = new Rule[4];
                    things[last].GetComponent<BehCharacter>().rules[0] = new Rule();
                    things[last].GetComponent<BehCharacter>().rules[1] = new Rule();
                    things[last].GetComponent<BehCharacter>().rules[2] = new Rule();
                    things[last].GetComponent<BehCharacter>().rules[3] = new Rule();

                    things[last].GetComponent<BehCharacter>().rules[0].addCondition(Conditions.numberEqualTo, Objects.player, 0, Variables.B);
                    Action act0 = new Action(HardActions.moveLeft);
                    act0.addSoftAction(SoftActions.setVariable, Variables.A, 0);
                    act0.addSoftAction(SoftActions.setVariable, Variables.B, 2);
                    things[last].GetComponent<BehCharacter>().rules[0].setAction(act0);

                    things[last].GetComponent<BehCharacter>().rules[1].addCondition(Conditions.numberEqualTo, Objects.player, 0, Variables.C);
                    Action act1 = new Action(HardActions.moveRight);
                    act1.addSoftAction(SoftActions.setVariable, Variables.A, 1);
                    act1.addSoftAction(SoftActions.setVariable, Variables.C, 2);
                    things[last].GetComponent<BehCharacter>().rules[1].setAction( act1 );

                    things[last].GetComponent<BehCharacter>().rules[2].addCondition(Conditions.numberEqualTo, Objects.player, 1, Variables.A);
                    Action act2 = new Action(HardActions.moveRight);
                    act2.addSoftAction(SoftActions.decVariable, Variables.B);
                    things[last].GetComponent<BehCharacter>().rules[2].setAction( act2 );

                    things[last].GetComponent<BehCharacter>().rules[3].addCondition(Conditions.numberEqualTo, Objects.player, 0, Variables.A);
                    Action act3 = new Action(HardActions.moveLeft);
                    act3.addSoftAction(SoftActions.decVariable, Variables.C);
                    things[last].GetComponent<BehCharacter>().rules[3].setAction( act3 );


                    things[last].GetComponent<BehCharacter>().objectType=Objects.player;
                    things[last].GetComponent<BehCharacter>().nombre="Jugador1";

                    things[last].GetComponent<SpriteRenderer>().color = new Color(0,1,1,1);

                    player1 = things[last];
                }
                if(i==1 && j==4){
                    things.Add(Instantiate(character));
                    int last = things.Count-1;
                    things[last].transform.position = new Vector2( i*squareWH, -j*squareWH );
                    board[i,j].SendMessage("setOccupant", things[last]);
                    things[last].GetComponent<BehCharacter>().currentSquare = board[i,j];
                    things[last].GetComponent<BehCharacter>().targetSquare  = board[i,j];

                    things[last].GetComponent<BehCharacter>().rules = new Rule[2];
                    things[last].GetComponent<BehCharacter>().rules[0] = new Rule();
                    things[last].GetComponent<BehCharacter>().rules[0].addCondition(Conditions.see, Objects.wall, 0, Variables.A, false);
                    things[last].GetComponent<BehCharacter>().rules[0].setAction( new Action(HardActions.moveRight) );
                    things[last].GetComponent<BehCharacter>().rules[1] = new Rule();
                    //things[last].GetComponent<BehCharacter>().rules[1].addCondition(Conditions.numberEqualTo, Objects.player, 1,Variables.A, true);
                    Action action = new Action(HardActions.moveUp);
                    action.addSoftAction(SoftActions.incVariable, Variables.A);
                    things[last].GetComponent<BehCharacter>().rules[1].setAction( action );
                    things[last].GetComponent<BehCharacter>().objectType=Objects.player;

                    things[last].GetComponent<SpriteRenderer>().color = new Color(1,0,0,1);
                    things[last].GetComponent<BehCharacter>().nombre="Jugador2";
                    player2 = things[last];
                }

                

                if(i==0 || j==0 || i==board.GetLength(0)-1 || j == board.GetLength(1)-1){
                    createWall(i,j);
                }
            }
        }
        createWall(4,4); createWall(7,3); createWall(3,1);
    }

    // Update is called once per frame
    void Update(){
        
        if(turnTimer <= 0){
            newTurn=true;
            if(currentTurn<40) currentTurn++;
            turnTimer=1f;
        } else{
            newTurn=false;
            turnTimer -= Time.deltaTime;
        }

        if(currentTurn >= 40){
            forceEnd();
        }
        turnText.text = currentTurn.ToString();
    }

    public static GameObject getObjectInSquare(int pi, int pj){
        GameObject res = null;

        foreach (GameObject item in things) {   
            if(item.GetComponent<BehCharacter>().currentSquare.GetComponent<BehSquare>().i == pi && item.GetComponent<BehCharacter>().currentSquare.GetComponent<BehSquare>().j == pj){
                res = item;
            }            
        }

        return res;
    }

    public GameObject createWall(int i, int j){
        things.Add(Instantiate(character));
        int last = things.Count-1;
        things[last].transform.position = new Vector2( i*squareWH, -j*squareWH );
        board[i,j].SendMessage("setOccupant", things[last]);
        things[last].GetComponent<BehCharacter>().currentSquare = board[i,j];
        things[last].GetComponent<BehCharacter>().targetSquare  = board[i,j];
        things[last].GetComponent<SpriteRenderer>().sprite  = wallSprite;
        things[last].GetComponent<BehCharacter>().objectType = Objects.wall;
        return things[last];
    }

    void forceEnd(){
        //decide winner
            
        //clean up

        //go to menu (?)
    }
}
