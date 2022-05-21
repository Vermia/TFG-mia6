
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
    public Sprite RedPlayer;
    public Sprite BluePlayer;
    public Sprite Pickup;
    public Sprite StarSprite;
    public Sprite bulletSprite;
    public Sprite breakableSprite;

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
        
        generateMap(1);
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

            if(currentTurn >= 60){
                forceEnd();
            }
            turnText.text = currentTurn.ToString();

            if(player1.GetComponent<BehCharacter>().currHP <= 0){
                defeat();
            }
            if(player2.GetComponent<BehCharacter>().currHP <= 0){
                victory();
            }
            if(player1.GetComponent<BehCharacter>().currStars >= player1.GetComponent<BehCharacter>().maxStars){
                victory();
            }
            if(player2.GetComponent<BehCharacter>().currStars >= player2.GetComponent<BehCharacter>().maxStars){
                defeat();
            }
            if(Input.GetKeyDown(KeyCode.X)){
                victory();
            }
        }
    }

    public static void startGame(){
        if(board==null){
            GameObject.Find("Board").GetComponent<BehBoard>().generateMap(0);
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

    public GameObject createBullet(int i, int j, Dir4 dir, GameObject source){
        GameObject newObj = createObject(i, j);

        Action act = new Action(HardActions.move, dir);
       
        //newObj.GetComponent<BehCharacter>().

        newObj.GetComponent<BehCharacter>().objectType = Objects.projectile;

        newObj.GetComponent<BehCharacter>().decideNextTurn();

        newObj.GetComponent<BehCharacter>().currHP = 15; //Bullet HP = Damage dealt
        newObj.GetComponent<BehCharacter>().source = source;
        newObj.GetComponent<SpriteRenderer>().sprite = bulletSprite;

        return newObj;
    }

    public GameObject createGunPickup(int i, int j, int charges){
        GameObject newObj = createObject(i, j);

        newObj.GetComponent<BehCharacter>().obtainItem(ItemTypes.gun, 5);
        newObj.GetComponent<BehCharacter>().objectType = Objects.pickup;
        newObj.GetComponent<SpriteRenderer>().sprite = Pickup;
        

        return newObj;
    }

    public GameObject createStarsPickup(int i, int j, int amount){
        GameObject newObj = createObject(i, j);

        newObj.GetComponent<BehCharacter>().obtainItem(ItemTypes.stars, amount);
        newObj.GetComponent<BehCharacter>().objectType = Objects.pickup;
        newObj.GetComponent<SpriteRenderer>().sprite = StarSprite;

        return newObj;
    }

    public GameObject createBreakableWall(int i, int j, int HP){
        GameObject newObj = createObject(i, j);

        
        newObj.GetComponent<BehCharacter>().objectType = Objects.breakablewall;
        newObj.GetComponent<BehCharacter>().maxHP = HP;
        newObj.GetComponent<BehCharacter>().currHP = HP;
        newObj.GetComponent<SpriteRenderer>().sprite = breakableSprite;
        newObj.name= "Muro rompible";


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



    void generateMap(int seed){
        if(seed==0){
            map1();
        }
        else if(seed==1){
            map2();
        }
        
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

    ////MAPS

    void map1(){
        prepMap(10,7);

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

                    things[last].GetComponent<SpriteRenderer>().sprite = BluePlayer;

                    player1 = things[last];
                    GameObject.Find("UICanvasImageEdit").GetComponent<BehUIEdit>().player = player1.GetComponent<BehCharacter>();
                    GameObject.Find("UICanvasImageEdit").GetComponent<BehUIEdit>().updateRuleInfo();


                }
                

                

                if(i==0 || j==0 || i==board.GetLength(0)-1 || j == board.GetLength(1)-1){
                    createWall(i,j);
                }
                
            }
        }
        createWall(7,2); createWall(7,3); createWall(3,1);
        createGunPickup(4,5,   5);
        createStarsPickup(1,1,100);
    }

    void map2(){
        prepMap(17,9);
        GameObject.Find("Main Camera").transform.position = new Vector2(9.50f, -4f);

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

                    things[last].GetComponent<SpriteRenderer>().sprite = BluePlayer;

                    player1 = things[last];
                    GameObject.Find("UICanvasImageEdit").GetComponent<BehUIEdit>().player = player1.GetComponent<BehCharacter>();
                    GameObject.Find("UICanvasImageEdit").GetComponent<BehUIEdit>().updateRuleInfo();


                }
                if(i==14 && j==6){
                    things.Add(Instantiate(character));
                    int last = things.Count-1;
                    things[last].transform.position = new Vector2( i*squareWH, -j*squareWH );
                    board[i,j].SendMessage("setOccupant", things[last]);

                    BehCharacter behlast = things[last].GetComponent<BehCharacter>();
                    behlast.currentSquare = board[i,j];
                    behlast.targetSquare  = board[i,j];

                    behlast.obtainItem(ItemTypes.gun, 5);
                    behlast.addStatement(new Loop(1,new Action(HardActions.move, Dir4.up)));
                    behlast.addStatement(new Loop(4,new Action(HardActions.move, Dir4.up)));
                    behlast.addStatement(new Loop(3,new Action(HardActions.move, Dir4.up)));

                    things[last].GetComponent<SpriteRenderer>().sprite = RedPlayer;
                    behlast.nombre="Jugador2";
                    player2 = things[last];
                }

                

                if(i==0 || j==0 || i==board.GetLength(0)-1 || j == board.GetLength(1)-1){
                    createWall(i,j);
                }
                
            }
        }
        createWall(7,2); createWall(7,3); createWall(3,1); createWall(6,4); createWall(9,4); createWall(7,5); createWall(9,3); createWall(9,2); createWall(8,5);
        createWall(11,3); createWall(12,3);createWall(2,2);createWall(1,6);createWall(2,6);createWall(13,2);createWall(14,2);createWall(14,1);
        createGunPickup(4,5,   5);  createGunPickup(12,6,   5); 
        createStarsPickup(1,1,25); createStarsPickup(10,5,25); createStarsPickup(6,7,25); createStarsPickup(15,7,25);createStarsPickup(7,4,75);
        createStarsPickup(1,7,25);createStarsPickup(15,1,25);
        createBreakableWall(8,3,40);

    }

    void prepMap(int width, int height){
        widthInSquares=width;
        heightInSquares=height;
        board = new GameObject[widthInSquares, heightInSquares];
        things = new List<GameObject>();

        Destroy(StateText);
        StateText=null;
    }

}
