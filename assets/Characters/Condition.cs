using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Conditions{
    see, numberEqualTo, numberLessThan, numberMoreThan
}

public class Condition{
    public Objects affectedObject {get; private set;}
    public int affectedNumber{get; private set;}
    public Variables affectedVariable{get; private set;}
    public bool positive{get; private set;}
    public Conditions type{get; private set;}

    public Condition(Conditions ptype, Objects pobject, int pnumber, Variables pvar, bool positive = true){
        type = ptype; affectedObject = pobject; affectedNumber = pnumber; affectedVariable=pvar; this.positive=positive;
    }

    public bool evaluate(BehCharacter chara){
        bool res = false;
        GameObject otherObject;

        if(type==Conditions.see){
            BehSquare squareOfCharacter = chara.GetComponent<BehCharacter>().currentSquare.GetComponent<BehSquare>();

            switch(affectedNumber){
                case 0:
                    otherObject = BehBoard.getObjectInSquare(squareOfCharacter.i+1, squareOfCharacter.j);
                    if(otherObject && otherObject.GetComponent<BehCharacter>().objectType == affectedObject){
                        res = true;
                    }
                break;
                case 1:
                    
                break;
                case 2:
                    otherObject = BehBoard.getObjectInSquare(squareOfCharacter.i, squareOfCharacter.j-1);
                    if(otherObject && otherObject.GetComponent<BehCharacter>().objectType == affectedObject){
                        res = true;
                    }
                break;
            }
        }

        if(type == Conditions.numberEqualTo || type == Conditions.numberLessThan ||type == Conditions.numberMoreThan){
            if ( type == Conditions.numberEqualTo ) res = chara.GetComponent<BehCharacter>().variables[((int)affectedVariable)] == affectedNumber;
            else if ( type == Conditions.numberLessThan )res = chara.GetComponent<BehCharacter>().variables[((int)affectedVariable)] < affectedNumber;
            else if ( type == Conditions.numberMoreThan )res = chara.GetComponent<BehCharacter>().variables[((int)affectedVariable)] > affectedNumber;
        }

        if(!positive) res = !res;

        return res;
    }
}