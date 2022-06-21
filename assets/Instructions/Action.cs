
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;



public enum HardActions{
    doNothing, move, shoot, pickup
}

public class Action{
    

    public HardActions hardAction;
    public Dir4 affectedDirection;
    public ItemTypes affectedItem;
    public List<SoftAction> softActions;

    public Action(HardActions pha, Dir4 pdir){
        hardAction = pha;
        affectedItem = ItemTypes.noItem;
        softActions = new List<SoftAction>();
        affectedDirection = pdir;
    }

    public void addSoftAction(SoftActions psoft, Variables pvar, int pnumber = 0){
        softActions.Add(new SoftAction(psoft, pvar, pnumber));
    }

    public void addSoftAction(SoftAction psoft){
        softActions.Add(psoft);
    }



    public void perform(BehCharacter chara){
        BehSquare mySquareBehavior = chara.currentSquare.GetComponent<BehSquare>();
        //GameObject otherObject;

        foreach(SoftAction act in softActions){
            act.perform(chara);
        }

        chara.currentTurn=hardAction;
        chara.currentDir = affectedDirection;
        switch(hardAction){
            case HardActions.move:
                chara.targetSquare=getAdjacentSquare(chara, affectedDirection);
            break;

            case HardActions.shoot:
                chara.useItem(ItemTypes.gun, affectedDirection);
            break;

            case HardActions.pickup:
                if(getAdjacentSquare(chara, affectedDirection) == null) break;
                if(getAdjacentSquare(chara, affectedDirection).GetComponent<BehSquare>().occupant == null) break;
                if(getAdjacentSquare(chara, affectedDirection).GetComponent<BehSquare>().occupant.GetComponent<BehCharacter>().objectType == Objects.pickup){
                    foreach(Item item in getAdjacentSquare(chara, affectedDirection).GetComponent<BehSquare>().occupant.GetComponent<BehCharacter>().inventory){
                        if(item.type==ItemTypes.stars){
                            chara.currStars+=1;
                        }
                        else{
                            chara.obtainItem(item.type, item.charges);
                        }
                    }
                    BehBoard.destroyThing(getAdjacentSquare(chara, affectedDirection).GetComponent<BehSquare>().occupant.GetComponent<BehCharacter>());
                }
            break;
        }
            
    }

    GameObject getAdjacentSquare(BehCharacter chara, Dir4 dir){
        GameObject go=null;
        BehSquare mySquareBehavior = chara.currentSquare.GetComponent<BehSquare>();

        if(affectedDirection==Dir4.right){
            go = BehBoard.board[ mySquareBehavior.i + 1, mySquareBehavior.j ];
        } else if(affectedDirection==Dir4.up){
            go = BehBoard.board[ mySquareBehavior.i, mySquareBehavior.j - 1 ];
        } else if(affectedDirection==Dir4.left){
            go = BehBoard.board[ mySquareBehavior.i - 1, mySquareBehavior.j ];
        } else if(affectedDirection==Dir4.down){
            go = BehBoard.board[ mySquareBehavior.i, mySquareBehavior.j + 1 ];
        }

        return go;
    }
}