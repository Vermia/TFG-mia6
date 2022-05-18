using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Conditions{
    see, numberEqualTo, numberMoreThan, numberLessThan
}

public enum seeDirections{
    right, rightup, up, leftup, left, leftdown, down, rightdown, rightfar, upfar, leftfar, downfar
}

public class Condition{
    public Objects affectedObject ;
    public int affectedNumber;
    public Variables affectedVariable;
    public bool positive;
    public Conditions type;

    public Condition(Conditions ptype, Objects pobject, int pnumber, Variables pvar, bool positive = true){
        type = ptype; affectedObject = pobject; affectedNumber = pnumber; affectedVariable=pvar; this.positive=positive;
    }

    public bool evaluate(BehCharacter chara){
        bool res = false;
        GameObject otherObject;

        if(type==Conditions.see){
            BehSquare squareOfCharacter = chara.GetComponent<BehCharacter>().currentSquare.GetComponent<BehSquare>();

            switch(affectedNumber){
                case 0: //derecha
                    otherObject = BehBoard.getObjectInSquare(squareOfCharacter.i+1, squareOfCharacter.j);
                    if(otherObject && otherObject.GetComponent<BehCharacter>().objectType == affectedObject){
                        res = true;
                    }
                break;
                case 1: //arriba derecha
                     otherObject = BehBoard.getObjectInSquare(squareOfCharacter.i+1, squareOfCharacter.j-1);
                    if(otherObject && otherObject.GetComponent<BehCharacter>().objectType == affectedObject){
                        res = true;
                    }
                break;
                case 2: //arriba
                    otherObject = BehBoard.getObjectInSquare(squareOfCharacter.i, squareOfCharacter.j-1);
                    if(otherObject && otherObject.GetComponent<BehCharacter>().objectType == affectedObject){
                        res = true;
                    }
                break;
                case 3: //arriba izquierda
                    otherObject = BehBoard.getObjectInSquare(squareOfCharacter.i-1, squareOfCharacter.j-1);
                    if(otherObject && otherObject.GetComponent<BehCharacter>().objectType == affectedObject){
                        res = true;
                    }
                break;
                case 4: //izquierda
                    otherObject = BehBoard.getObjectInSquare(squareOfCharacter.i-1, squareOfCharacter.j);
                    if(otherObject && otherObject.GetComponent<BehCharacter>().objectType == affectedObject){
                        res = true;
                    }
                break;
                case 5: //abajo izquierda
                    otherObject = BehBoard.getObjectInSquare(squareOfCharacter.i-1, squareOfCharacter.j+1);
                    if(otherObject && otherObject.GetComponent<BehCharacter>().objectType == affectedObject){
                        res = true;
                    }
                break;
                case 6: //abajo
                    otherObject = BehBoard.getObjectInSquare(squareOfCharacter.i, squareOfCharacter.j+1);
                    if(otherObject && otherObject.GetComponent<BehCharacter>().objectType == affectedObject){
                        res = true;
                    }
                break;
                case 7: //abajo derecha
                    otherObject = BehBoard.getObjectInSquare(squareOfCharacter.i+1, squareOfCharacter.j+1);
                    if(otherObject && otherObject.GetComponent<BehCharacter>().objectType == affectedObject){
                        res = true;
                    }
                break;
                case 8: //lejos derecha
                    otherObject = BehBoard.getObjectInSquare(squareOfCharacter.i+2, squareOfCharacter.j);
                    if(otherObject && otherObject.GetComponent<BehCharacter>().objectType == affectedObject){
                        res = true;
                    }
                break;
                case 9: //lejos arriba
                    otherObject = BehBoard.getObjectInSquare(squareOfCharacter.i, squareOfCharacter.j-2);
                    if(otherObject && otherObject.GetComponent<BehCharacter>().objectType == affectedObject){
                        res = true;
                    }
                break;
                case 10: //lejos izquierda
                    otherObject = BehBoard.getObjectInSquare(squareOfCharacter.i-2, squareOfCharacter.j);
                    if(otherObject && otherObject.GetComponent<BehCharacter>().objectType == affectedObject){
                        res = true;
                    }
                break;
                case 11: //lejos abajo
                    otherObject = BehBoard.getObjectInSquare(squareOfCharacter.i, squareOfCharacter.j+2);
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