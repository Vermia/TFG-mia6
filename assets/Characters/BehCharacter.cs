using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Objects{
    player, wall, projectile, pickup
}

public class BehCharacter : MonoBehaviour
{
    public GameObject currentSquare;
    public GameObject targetSquare;
    public GameObject source;
    public HardActions currentTurn;

    public List<Rule> rules; 
    private float speedUnit = 1.28f;
    public Objects objectType;


    //Atributos del elemento
    public int maxHP;
    public int currHP;
    public int maxStars;
    public int currStars;
    public int[] variables;
    public string nombre;
    public bool arrived;

    public List<Item> inventory;

    // Start is called before the first frame update
    void Start(){
        
    }

    void Awake(){
        rules = new List<Rule>();
        inventory = new List<Item>();
        variables = new int[6];
        for(int i=0 ; i<6; i++) variables[i]=1;
        maxHP = 60;
        currHP = 60;
        maxStars=100;
        currStars=0;
        arrived=true;
    }

    // Update is called once per frame
    void Update(){
        if(BehBoard.gameActive){
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
        if(!arrived){
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

        if(distanceTwoPoints(gameObject.transform.position.x, gameObject.transform.position.y, targetSquare.gameObject.transform.position.x, targetSquare.gameObject.transform.position.y) < 0.01){
            arrived=true;
        }
        
    }

    public void decideNextTurn(){
        arrived=false;
        occupy(targetSquare);
        BehSquare mySquareBehavior = currentSquare.GetComponent<BehSquare>();

        currentTurn=HardActions.doNothing;
        for(int i=0 ; i<rules.Count ; ++i){
            if(rules[i].run(this)){
                
                break;
            }
        }
    }

    void occupy(GameObject newSquare){
        currentSquare = newSquare;
        transform.position = new Vector2(newSquare.transform.position.x, newSquare.transform.position.y);
    }

    //MANIPULACION DE REGLAS
    public void addRule(Rule prule){
        rules.Add(prule);
    }

    public void moveRuleUp(int index){
        if(index>0){
            Rule aux = rules[index-1];
            rules[index-1] = rules[index];
            rules[index] = aux;
        }
    }

    public void moveRuleDown(int index){
        if(index<rules.Count-1){
            Rule aux = rules[index+1];
            rules[index+1] = rules[index];
            rules[index] = aux;
        }
    }

    public void removeRule(int index){
        rules.RemoveAt(index);
    }

    public void incVariable(Variables vari){
        if(variables[(int)vari]<9){
            variables[(int)vari]++;
        } else{
            variables[(int)vari]=0;
        }
    }

    public void decVariable(Variables vari){
        if(variables[(int)vari]>0){
            variables[(int)vari]--;
        } else{
            variables[(int)vari]=9;
        }
    }

    public void useItem(ItemTypes itemToUse, seeDirections dir){
        //Check if we have the item
        int index = -1;
        for(int i=0 ; i<inventory.Count ; ++i){
            if(inventory[i].type == itemToUse) index = i;
        }
        if(index == -1) return;

        //Do the thing
        switch(itemToUse){
            case ItemTypes.gun:
                GameObject.Find("Board").GetComponent<BehBoard>().createBullet(currentSquare.GetComponent<BehSquare>().i, currentSquare.GetComponent<BehSquare>().j, dir, gameObject);
            break;

            default: break;
        }

        //Expend one charge
        inventory[index].charges--;
        if(inventory[index].charges<=0) discardItem(index);
    }

    //ITEMS
    public void obtainItem(ItemTypes type, int charges){
        //Check if we have an item of that type
        int itemIndex = -1;
        for(int i=0 ; i<inventory.Count ; ++i){
            if(inventory[i].type == type){
                itemIndex=i; break;
            }
        }
        //if we have an item, add the charges
        if(itemIndex != -1){
            inventory[itemIndex].charges += charges;
        }
        //if we don't have the item, create it
        else{
            inventory.Add(new Item(type, charges));
        }
    }

    public void discardItem(int index){
        inventory.RemoveAt(index);
    }

    public void discardItem(ItemTypes ptype){
        int index= -1;
        for(int i = 0 ; i<inventory.Count ; ++i){
            if(inventory[i].type == ptype){
                index = i;
                break;
            }
        }
        if(index!=-1) inventory.RemoveAt(index);
    }

    /// COLLISIONS

    public void OnTriggerEnter2D(Collider2D col){
        BehCharacter otherChar=col.gameObject.GetComponent<BehCharacter>();
        if(objectType == Objects.player){

            if(otherChar.objectType == Objects.player){ //PLAYER WITH PLAYER
                targetSquare=currentSquare;
                if(currentTurn==HardActions.moveRight) currentTurn=HardActions.moveLeft;
                else if(currentTurn==HardActions.moveUp) currentTurn=HardActions.moveDown;
                else if(currentTurn==HardActions.moveLeft) currentTurn=HardActions.moveRight;
                else if(currentTurn==HardActions.moveDown) currentTurn=HardActions.moveUp;
            }
            
            else if(otherChar.objectType == Objects.projectile){ //PLAYER WITH PROJECTILE
                if(otherChar.source != gameObject){
                    currHP -= otherChar.currHP;
                    BehBoard.destroyThing(otherChar);
                }
            }

            else if(otherChar.objectType == Objects.wall){ //PLAYER WITH WALL
                targetSquare=currentSquare;
                if(currentTurn==HardActions.moveRight) currentTurn=HardActions.moveLeft;
                else if(currentTurn==HardActions.moveUp) currentTurn=HardActions.moveDown;
                else if(currentTurn==HardActions.moveLeft) currentTurn=HardActions.moveRight;
                else if(currentTurn==HardActions.moveDown) currentTurn=HardActions.moveUp;
            }

            else if(otherChar.objectType == Objects.pickup){ //PLAYER WITH PICKUP
                foreach(Item item in otherChar.inventory){
                    obtainItem(item.type, item.charges);
                }
                BehBoard.destroyThing(otherChar);
            }
        }

        else if(objectType == Objects.projectile){

            if(otherChar.objectType == Objects.wall){ //PROJECTILE WITH WALL
                BehBoard.destroyThing(this);
            }

        }
    }

    public static float distanceTwoPoints(float x1, float y1, float x2, float y2){
        float res = 0.0f;

        float ressq= (x2-x1)*(x2-x1) + (y2-y1)*(y2-y1);
        res = Mathf.Sqrt(ressq);

        return res;
    }

}
