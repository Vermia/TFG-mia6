
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
                if(affectedDirection==Dir4.right){
                    chara.targetSquare = BehBoard.board[ mySquareBehavior.i + 1, mySquareBehavior.j ];
                } else if(affectedDirection==Dir4.up){
                    chara.targetSquare = BehBoard.board[ mySquareBehavior.i, mySquareBehavior.j - 1 ];
                } else if(affectedDirection==Dir4.left){
                    chara.targetSquare = BehBoard.board[ mySquareBehavior.i - 1, mySquareBehavior.j ];
                } else if(affectedDirection==Dir4.down){
                    chara.targetSquare = BehBoard.board[ mySquareBehavior.i, mySquareBehavior.j + 1 ];
                }
            break;

            case HardActions.shoot:
                chara.useItem(ItemTypes.gun, affectedDirection);
            break;
        }
            
    }
}