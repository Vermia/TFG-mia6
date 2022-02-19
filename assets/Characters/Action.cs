
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;



public enum HardActions{
    doNothing,
    moveRight,
    moveUp,
    moveLeft,
    moveDown
}

public class Action{
    

    public HardActions hardAction;
    public List<SoftAction> softActions;

    public Action(HardActions pha){
        hardAction = pha;
        softActions = new List<SoftAction>();
    }

    public void addSoftAction(SoftActions psoft, Variables pvar, int pnumber = 0){
        softActions.Add(new SoftAction(psoft, pvar, pnumber));
    }

    public void addSoftAction(SoftAction psoft){
        softActions.Add(psoft);
    }

    public void perform(BehCharacter chara){
        BehSquare mySquareBehavior = chara.currentSquare.GetComponent<BehSquare>();
        GameObject otherObject;

        foreach(SoftAction act in softActions){
            act.perform(chara);
        }


        switch(hardAction){
            case HardActions.moveRight:
                otherObject = BehBoard.getObjectInSquare(mySquareBehavior.i+1, mySquareBehavior.j);
                if(!otherObject || otherObject.GetComponent<BehCharacter>().objectType != Objects.wall ){
                    chara.currentTurn = HardActions.moveRight; chara.targetSquare = BehBoard.board[ mySquareBehavior.i + 1, mySquareBehavior.j ];
                } else chara.currentTurn = HardActions.doNothing;
            break;
            case HardActions.moveLeft:
                otherObject = BehBoard.getObjectInSquare(mySquareBehavior.i-1, mySquareBehavior.j);
                if(!otherObject || otherObject.GetComponent<BehCharacter>().objectType != Objects.wall ){
                    chara.currentTurn = HardActions.moveLeft; chara.targetSquare = BehBoard.board[ mySquareBehavior.i - 1, mySquareBehavior.j ];
                } else chara.currentTurn = HardActions.doNothing;
            break;
            case HardActions.moveUp:
                otherObject = BehBoard.getObjectInSquare(mySquareBehavior.i, mySquareBehavior.j-1);
                if(!otherObject || otherObject.GetComponent<BehCharacter>().objectType != Objects.wall ){
                    chara.currentTurn = HardActions.moveUp;chara.targetSquare = BehBoard.board[ mySquareBehavior.i, mySquareBehavior.j - 1 ];
                } else chara.currentTurn = HardActions.doNothing;
            break;
            case HardActions.moveDown:
                otherObject = BehBoard.getObjectInSquare(mySquareBehavior.i, mySquareBehavior.j+1);
                if(!otherObject || otherObject.GetComponent<BehCharacter>().objectType != Objects.wall ){
                    chara.currentTurn = HardActions.moveDown; chara.targetSquare = BehBoard.board[ mySquareBehavior.i, mySquareBehavior.j +1 ];
                } else chara.currentTurn = HardActions.doNothing;
            break;
        }
    }
}