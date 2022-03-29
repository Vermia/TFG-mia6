
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class BehBoard : MonoBehaviour
{
    public static bool gameActive;

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

    GameObject StateText;

    public static GameObject player1{get; private set;}
    public static GameObject player2{get; private set;}

    // Start is called before the first frame update
    void Awake(){
        gameActive=false;
        turnText = GameObject.Find("Turn").GetComponent<Text>();
        squareWH = 1.28f; // 64 * 2 / 100

        newTurn=true;
        turnTimer=0f;
        
        generateMap();
    }

    // Update is called once per frame
    void Update(){
        //startGame();
        if(gameActive){
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

            if(player1.GetComponent<BehCharacter>().currHP <= 0){
                defeat();
            }
            if(player2.GetComponent<BehCharacter>().currHP <= 0){
                victory();
            }
            if(Input.GetKeyDown(KeyCode.X)){
                victory();
            }
        }
    }

    public static void startGame(){
        if(board==null){
            GameObject.Find("Board").GetComponent<BehBoard>().generateMap();
            gameActive = false;
        }
        else{
            gameActive = true;
        }
        
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


    //OBJECT CREATION
    GameObject createObject(int i, int j){
        things.Add(Instantiate(character));
        int last = things.Count-1;
        board[i,j].SendMessage("setOccupant", things[last]);
        things[last].transform.position = new Vector2( i*squareWH, -j*squareWH );
        things[last].GetComponent<BehCharacter>().currentSquare = board[i,j];
        things[last].GetComponent<BehCharacter>().targetSquare  = board[i,j];
        return things[last];
    }

    public GameObject createWall(int i, int j){
        GameObject newObj = createObject(i, j);

        newObj.GetComponent<SpriteRenderer>().sprite  = wallSprite;
        newObj.GetComponent<BehCharacter>().objectType = Objects.wall;
        newObj.GetComponent<BehCharacter>().nombre = "Muro";
        newObj.GetComponent<BoxCollider2D>().size = new Vector2(1.28f, 1.28f);

        return newObj;
    }

    public GameObject createBullet(int i, int j, seeDirections dir, GameObject source){
        GameObject newObj = createObject(i, j);

        Action act = new Action(HardActions.doNothing);
        switch(dir){
            case seeDirections.right:
                act.hardAction = HardActions.moveRight;
            break;
            case seeDirections.up:
                act.hardAction = HardActions.moveUp;
            break;
            case seeDirections.left:
                act.hardAction = HardActions.moveLeft;
            break;
            case seeDirections.down:
                act.hardAction = HardActions.moveDown;
            break;
        }
        Rule rule = new Rule();
        rule.setAction(act);
        newObj.GetComponent<BehCharacter>().addRule(rule);

        newObj.GetComponent<BehCharacter>().objectType = Objects.projectile;

        newObj.GetComponent<BehCharacter>().decideNextTurn();

        newObj.GetComponent<BehCharacter>().currHP = 15; //Bullet HP = Damage dealt
        newObj.GetComponent<BehCharacter>().source = source;

        return newObj;
    }

    public GameObject createGunPickup(int i, int j, int charges){
        GameObject newObj = createObject(i, j);

        newObj.GetComponent<BehCharacter>().obtainItem(ItemTypes.gun, 5);
        newObj.GetComponent<BehCharacter>().objectType = Objects.pickup;

        return newObj;
    }


    ////////////////////////////////////////////////

    public static  void destroyThing(BehCharacter chara){
        for(int i = 0 ; i < things.Count ; ++i){
            if(things[i] == chara.gameObject){
                Destroy(chara.gameObject);
                things.RemoveAt(i);
                break;
            }
        }

    }

    void forceEnd(){
        //decide winner
            
        //clean up
        gameActive=false;

        //go to menu (?)
    }



    void generateMap(){
        widthInSquares=10;
        heightInSquares=7;
        board = new GameObject[widthInSquares, heightInSquares];
        things = new List<GameObject>();

        Destroy(StateText);
        StateText=null;

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
                    BehCharacter behlast = things[last].GetComponent<BehCharacter>();
                    behlast.currentSquare = board[i,j];
                    behlast.targetSquare  = board[i,j];

                    behlast.objectType=Objects.player;
                    behlast.nombre="Jugador1";

                    things[last].GetComponent<SpriteRenderer>().color = new Color(0,1,1,1);

                    player1 = things[last];
                    GameObject.Find("UICanvasImageEdit").GetComponent<BehUIEdit>().player = player1.GetComponent<BehCharacter>();
                    GameObject.Find("UICanvasImageEdit").GetComponent<BehUIEdit>().updateRuleInfo();


                }
                if(i==1 && j==3){
                    things.Add(Instantiate(character));
                    int last = things.Count-1;
                    things[last].transform.position = new Vector2( i*squareWH, -j*squareWH );
                    board[i,j].SendMessage("setOccupant", things[last]);

                    BehCharacter behlast = things[last].GetComponent<BehCharacter>();
                    behlast.currentSquare = board[i,j];
                    behlast.targetSquare  = board[i,j];

                    behlast.obtainItem(ItemTypes.gun, 5);
                    
                    Rule r2 = new Rule();
                    Action action = new Action(HardActions.moveRight);
                    action.addSoftAction(SoftActions.incVariable, Variables.A);
                    action.affectedItem=ItemTypes.gun;
                    r2.setAction( action );
                    behlast.addRule(r2);
                    behlast.objectType=Objects.player;

                    things[last].GetComponent<SpriteRenderer>().color = new Color(1,0,0,1);
                    behlast.nombre="Jugador2";
                    player2 = things[last];
                }

                

                if(i==0 || j==0 || i==board.GetLength(0)-1 || j == board.GetLength(1)-1){
                    createWall(i,j);
                }
                
            }
        }
        createWall(7,2); createWall(7,3); createWall(3,1);
        createGunPickup(4,5,   5);
    }


    //END
    void defeat(){
        gameActive=false;
        Debug.Log("Has perdido");
    }

    void victory(){
        gameActive=false;
        StateText = BehUIRule.createText(GameObject.Find("CanvasRules"), new Vector2(0,0), "¡Victoria!", new Color(1.0f,1.0f,1.0f,1.0f), 45);
        cleanEverything();
        Debug.Log("Has ganado");
    }

    public void cleanEverything(){
        for(int i = 0 ; i<things.Count ; ++i){
            Destroy(things[i]);
        }
        things=null;
        player1=null;
        player2=null;
        for(int i=0; i<board.GetLength(0) ; ++i){
            for(int j=0 ; j<board.GetLength(1) ; ++j){
                Destroy(board[i,j]);
            }
        }
        board=null;
        GameObject.Find("UICanvasImageEdit").GetComponent<BehUIEdit>().reset();
    }

}
