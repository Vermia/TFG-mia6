
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class BehBoard : MonoBehaviour
{
    public static bool gameActive;
    
    public GameObject restartButtongGO;

    public GameObject squareEmpty;
    public GameObject character;
    public Sprite wallSprite;
    public Sprite RedPlayer;
    public Sprite BluePlayer;
    public Sprite Pickup;
    public Sprite StarSprite;
    public Sprite bulletSprite;
    public Sprite breakableSprite;
    public Sprite goalSprite;

    public GameObject victoryText;

    static public GameObject[,] board{get; private set;}
    static public List<GameObject> things{get; private set;}
    static public bool newTurn{get; private set;}
    static public int widthInSquares {get; private set;} 
    static public int heightInSquares {get; private set;} 

    private float turnTimer;
    private float squareWH = 1.28f; // 64 * 2 / 100
    private int currentTurn;
    private int currentLevel;
    Text turnText;

    GameObject StateText;
    GameObject scoreScreenGO;
    public Button nextLevelButton;

    public static GameObject player1{get; private set;}
    public static GameObject player2{get; private set;}
    public GameObject goal{get; private set;}

    public int totalStarsThisLevel;
    int overallPoints;
    int deltaPoints;
    Text overallPointsText;

    BehRetractable ruleBlockerRetractable;

    // Start is called before the first frame update
    void Awake(){
        scoreScreenGO = GameObject.Find("ScoreScreen");
        nextLevelButton = GameObject.Find("nextLevelButton").GetComponent<Button>();
        nextLevelButton.onClick.AddListener( ()=>{ generateMap(++currentLevel); scoreScreenGO.GetComponent<BehUIScore>().goAway(); addPoints(deltaPoints); } );

        ruleBlockerRetractable = GameObject.Find("UICanvasEditCovering").GetComponent<BehRetractable>();
        ruleBlockerRetractable.goAway();

        deltaPoints=0;
        overallPoints = 0;
        overallPointsText = GameObject.Find("TotalPoints").GetComponent<Text>();

        currentLevel=0;
        gameActive=false;
        turnText = GameObject.Find("Turn").GetComponent<Text>();
        squareWH = 1.28f; // 64 * 2 / 100

        victoryText=GameObject.Find("VictoryText");
        victoryText.SetActive(false);

        restartButtongGO=GameObject.Find("RestartButton");
        restartButtongGO.GetComponent<Button>().onClick.AddListener( ()=>{
            reset();
        } );

        newTurn=true;
        turnTimer=0f;
    }

    void Start(){
        generateMap(0);
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

            if(player1.GetComponent<BehCharacter>().currHP <= 0){
                defeat();
            }
            if(BehCharacter.distanceTwoPoints(player1.transform.position.x, player1.transform.position.y, goal.transform.position.x, goal.transform.position.y) <0.01){
                victory();
            }
            if(Input.GetKeyDown(KeyCode.X)){
                victory();
            }

        } 
        turnText.text = currentTurn.ToString();
    }

    public void startGame(){
        if(board==null){
            //GameObject.Find("Board").GetComponent<BehBoard>().generateMap(0);
            //gameActive = false;
        }
        else{
            gameActive = true;
            ruleBlockerRetractable.comeHere();
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

    public void addPoints(int amount){
        overallPoints += amount;
        overallPointsText.text = overallPoints.ToString();
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

    GameObject createPlayer(int i, int j){
        GameObject newObj = createObject(i, j);
        BehCharacter behlast = newObj.GetComponent<BehCharacter>();

        behlast.objectType=Objects.player;
        behlast.nombre="Robito";
        behlast.maxHP=behlast.currHP=15;

        newObj.GetComponent<SpriteRenderer>().sprite = BluePlayer;
        newObj.GetComponent<SpriteRenderer>().sortingOrder = 6;

        player1 = newObj;
        GameObject.Find("UICanvasImageEdit").GetComponent<BehUIEdit>().player = player1.GetComponent<BehCharacter>();
        
        return newObj;
    }

    public GameObject createWall(int i, int j){
        GameObject newObj = createObject(i, j);

        newObj.GetComponent<SpriteRenderer>().sprite  = wallSprite;
        newObj.GetComponent<BehCharacter>().objectType = Objects.wall;
        newObj.GetComponent<BehCharacter>().nombre = "Muro";
        newObj.GetComponent<BehCharacter>().maxHP=newObj.GetComponent<BehCharacter>().currHP= 2000;
        newObj.GetComponent<BoxCollider2D>().size = new Vector2(1.28f, 1.28f);

        return newObj;
    }

    public GameObject createBullet(int i, int j, Dir4 dir, GameObject source){
        GameObject newObj = createObject(i, j);

        Action act = new Action(HardActions.move, dir);
        newObj.GetComponent<BehCharacter>().addStatement(new Loop(50, act));

        newObj.GetComponent<BehCharacter>().objectType = Objects.projectile;
        newObj.GetComponent<BehCharacter>().nombre = "Bala";

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
        newObj.GetComponent<BehCharacter>().nombre = "Cofre";
        newObj.GetComponent<SpriteRenderer>().sprite = Pickup;
        

        return newObj;
    }

    public GameObject createStarsPickup(int i, int j, int amount){
        GameObject newObj = createObject(i, j);

        newObj.GetComponent<BehCharacter>().obtainItem(ItemTypes.stars, amount);
        newObj.GetComponent<BehCharacter>().objectType = Objects.pickup;
        newObj.GetComponent<BehCharacter>().nombre = "Estrellas";
        newObj.GetComponent<SpriteRenderer>().sprite = StarSprite;

        return newObj;
    }

    public GameObject createBreakableWall(int i, int j, int HP){
        GameObject newObj = createObject(i, j);
        
        newObj.GetComponent<BehCharacter>().objectType = Objects.breakablewall;
        newObj.GetComponent<BehCharacter>().maxHP = HP;
        newObj.GetComponent<BehCharacter>().currHP = HP;
        newObj.GetComponent<SpriteRenderer>().sprite = breakableSprite;
        newObj.GetComponent<BehCharacter>().nombre= "Muro rompible";

        newObj.GetComponent<BoxCollider2D>().size = new Vector2(1.28f, 1.28f);

        return newObj;
    }

    public GameObject createEnemy(int i, int j, int HP, List<Item> inventory=null, List<Statement> statements=null){
        GameObject newObj = createObject(i, j);
        BehCharacter behavior = newObj.GetComponent<BehCharacter>();
        behavior.objectType = Objects.player;
        behavior.maxHP = newObj.GetComponent<BehCharacter>().currHP = HP;

        newObj.GetComponent<SpriteRenderer>().sprite=RedPlayer;

        if(inventory!=null){
            behavior.inventory = inventory;
        } else{
            behavior.inventory = new List<Item>();
        }

        if(statements!=null){
            behavior.statements = statements;
        } else{
            behavior.statements = new List<Statement>();
            behavior.addStatement(new Instruction(new Action(HardActions.doNothing, Dir4.center)));
        }

        return newObj;
    }

    public GameObject createGoal(int i, int j){
        GameObject newObj = createObject(i, j);
        BehCharacter behavior = newObj.GetComponent<BehCharacter>();
        behavior.objectType = Objects.goal;
        behavior.nombre = "Meta";

       newObj.GetComponent<SpriteRenderer>().sprite=goalSprite;
       goal=newObj;

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



    public void generateMap(int seed){
        if(seed==0){
            mapFirstMostSimple();
        }
        else if(seed==1){
            mapBasicMovement();
        }
        else if(seed==2){
            mapFirstRequiredLoop();
        }
        else if(seed==3){
            mapFirstStar();
        }
        else if(seed==4){
            map1();
        }
        else if(seed==5){
            mapFirstEnemy();
        }
        else if(seed==6){
            map2();
        } else if(seed==7){
            finishWithVictory();
        }
        
    }


    //END
    void defeat(){
        gameActive=false;
        Debug.Log("Has perdido");
    }

    void reset(){
        cleanEverything();
        generateMap(currentLevel);
        scoreScreenGO.GetComponent<BehUIScore>().goAway();
    }


    void victory(){
        gameActive=false;
        //Bring here the score
        int stars = player1.GetComponent<BehCharacter>().currStars;
        int actions = GameObject.Find("UICanvasImageEdit").GetComponent<BehUIEdit>().ruleCounter;
        deltaPoints = scoreScreenGO.GetComponent<BehUIScore>().loadValues(40, stars, actions);
        scoreScreenGO.GetComponent<BehUIScore>().comeHere();
    }

    void finishWithVictory(){
        cleanEverything();
        victoryText.SetActive(true);
        ruleBlockerRetractable.comeHere();
    }

    public void nextLevel(){
        generateMap(++currentLevel);
    } 

    public void cleanEverything(){
        ruleBlockerRetractable.goAway();
        if(things!=null){
            for(int i = 0 ; i<things.Count ; ++i){
                Destroy(things[i]);
            }
        }
       
        things=null;
        player1=null;
        player2=null;
        if(board!=null){
            for(int i=0; i<board.GetLength(0) ; ++i){
                for(int j=0 ; j<board.GetLength(1) ; ++j){
                    Destroy(board[i,j]);
                }
            }
        }
        
        board=null;
        gameActive=false;
        newTurn=true;
        turnTimer=0f;
        currentTurn=0;
        GameObject.Find("UICanvasImageEdit").GetComponent<BehUIEdit>().reset();
    }

    ////MAPS

    void mapFirstMostSimple(){
        GameObject.Find("UICanvasImageEdit").GetComponent<BehUIEdit>().ruleCounter=10;
        prepMap(9,9);

        for(int i=1; i<7;++i) createWall(5,i);
        for(int i=1; i<7;++i) createWall(6,i);
        for(int i=1; i<7;++i) createWall(7,i);
        for(int i=1; i<7;++i) createWall(8,i);
        createWall(2,3); createWall(2,5); createWall(3,1);

        createPlayer(2,2);
        createGoal(3,4);

        endPrepMap();
    }

    void mapBasicMovement(){
        GameObject.Find("UICanvasImageEdit").GetComponent<BehUIEdit>().ruleCounter=10;
        prepMap(9,9);

        createWall(4,5);createWall(5,6);createWall(5,4);
        createWall(5,5);createWall(6,1);createWall(3,3);
        createWall(5,2);createWall(7,3);createWall(3,7);
        for(int i=1; i<8;++i) createWall(1,i);
        for(int i=1; i<8;++i) createWall(2,i);

        createPlayer(4,4);
        createGoal(7,4);

        endPrepMap();
    }

    void mapFirstRequiredLoop(){
        GameObject.Find("UICanvasImageEdit").GetComponent<BehUIEdit>().ruleCounter=10;
        prepMap(9,9);


        for(int i=1; i<7;++i) createWall(3,i);
        for(int i=1; i<7;++i) createWall(4,i);
        for(int i=1; i<7;++i) createWall(6,i);
        for(int i=1; i<7;++i) createWall(5,i);
        for(int i=1; i<7;++i) createWall(7,i);
        for(int i=1; i<7;++i) createWall(2,i);

        createPlayer(1,1);
        createGoal(7,7);

        endPrepMap();
    }

    void mapFirstStar(){
        GameObject.Find("UICanvasImageEdit").GetComponent<BehUIEdit>().ruleCounter=10;
        prepMap(9,9);
        totalStarsThisLevel=2;


        for(int i=1; i<4;++i) createWall(3,i);
        for(int i=1; i<6;++i) createWall(4,i);
        for(int i=1; i<4;++i) createWall(6,i);
        for(int i=1; i<2;++i) createWall(5,i);
        for(int i=1; i<7;++i) createWall(7,i);
        for(int i=1; i<7;++i) createWall(2,i);

        createPlayer(1,1);
        createGoal(5,2);

        createStarsPickup(3,4,1);
        createStarsPickup(7,7,1);

        endPrepMap();
    }

    void map1(){
        GameObject.Find("UICanvasImageEdit").GetComponent<BehUIEdit>().ruleCounter=6;
        prepMap(9,9);
        totalStarsThisLevel=2;


        createPlayer(2,5);
        createWall(7,2); createWall(7,3); createWall(3,1);
        createWall(2,1); createWall(2,2); createWall(4,2);
        createWall(2,6); createWall(2,7); createStarsPickup(4,6, 100); createWall(4,7); createWall(4,5);
        for(int i=1; i<7;++i) createWall(5,i);
        createBreakableWall(3,6, 20);
        createGunPickup(3,4,   5);
        createStarsPickup(3,7,100);

        createGoal(3,2);
        endPrepMap();
    }

    void mapFirstEnemy(){
        GameObject.Find("UICanvasImageEdit").GetComponent<BehUIEdit>().ruleCounter=10;
        GameObject.Find("Main Camera").transform.localPosition=new Vector3(3.9f,-4, -10);
        prepMap(10,9);

        for(int i=1; i<7;++i) createWall(7,i);
        createWall(4,6);createWall(5,6);
        createWall(4,5);createWall(5,5);createWall(6,5);
        createWall(1,6);createWall(2,6);
        createWall(2,3);createWall(4,3);

        BehCharacter ia1 = createEnemy(1,7, 20).GetComponent<BehCharacter>();
        ia1.obtainItem(ItemTypes.gun, 100);
        ia1.statements=new List<Statement>();
        for(int i=0; i<8; ++i){
            ia1.addStatement(new Instruction(new Action(HardActions.shoot, Dir4.right)));
            ia1.addStatement(new Loop(7, new Action(HardActions.doNothing, Dir4.right)));
        }

        createPlayer(8,5);        
        createGoal(3,2);
        endPrepMap();
    }

    void map2(){
        GameObject.Find("UICanvasImageEdit").GetComponent<BehUIEdit>().ruleCounter=7;
        prepMap(9,11);
        totalStarsThisLevel=1;

        createPlayer(3,3);
        createWall(2,3); createWall(3,2); createWall(4,3);
        createStarsPickup(1,9,50);
        createGunPickup(1,3,  5);

        BehCharacter ia1= createEnemy(1,5, 20).GetComponent<BehCharacter>();
        ia1.statements=new List<Statement>();
        for(int i=0; i<4; ++i){
            ia1.addStatement(new Loop(3, new Action(HardActions.shoot, Dir4.right)));
            ia1.addStatement(new Instruction(new Action(HardActions.doNothing, Dir4.right)));
        }
        ia1.addStatement(new Loop(3, new Action(HardActions.shoot, Dir4.right)));
        ia1.obtainItem(ItemTypes.gun, 100);

        createGoal(3,8);

        endPrepMap();
    }

    void map0(){
        prepMap(17,14);
        GameObject.Find("Main Camera").transform.position = new Vector2(9.50f, -4f);
        totalStarsThisLevel=7;

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
                    behlast.nombre="Robito";

                    things[last].GetComponent<SpriteRenderer>().sprite = BluePlayer;

                    player1 = things[last];
                    GameObject.Find("UICanvasImageEdit").GetComponent<BehUIEdit>().player = player1.GetComponent<BehCharacter>();
                    //GameObject.Find("UICanvasImageEdit").GetComponent<BehUIEdit>().updateRuleInfo();


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
        cleanEverything();
        totalStarsThisLevel=0;


        widthInSquares=width;
        heightInSquares=height;
        board = new GameObject[widthInSquares, heightInSquares];
        things = new List<GameObject>();

        Destroy(StateText);
        StateText=null;

        for(int i=0 ; i<widthInSquares ; i++){
            for(int j=0 ; j<heightInSquares ; j++){
                board[i,j] = Instantiate(squareEmpty);
                board[i,j].transform.position = new Vector2( i*squareWH, -j*squareWH );
                board[i,j].SendMessage("setPosition", new Vector2i(i,j));
                
                

                

                if(i==0 || j==0 || i==board.GetLength(0)-1 || j == board.GetLength(1)-1){
                    createWall(i,j);
                }
                
            }
        }
    }

    void endPrepMap(){
        GameObject.Find("UICanvasImageUp").GetComponent<BehUIAbove>().setStuff();
        GameObject.Find("UICanvasImageEdit").GetComponent<BehUIEdit>().reset();
        GameObject.Find("UICanvasImageEdit").GetComponent<BehUIEdit>().updateRuleInfo();
    }

}
