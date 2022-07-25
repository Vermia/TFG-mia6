using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum SoftActions{
    setVariable, incVariable, decVariable
}

public class SoftAction{
    public SoftActions softAction ;
    public Variables affectedVariable ;
    public int affectedNumber;


    public SoftAction(SoftActions paction, Variables pvar, int pnumber = 0){
        softAction = paction; affectedVariable = pvar; affectedNumber = pnumber;
    }


    public void perform(BehCharacter chara){
        switch(softAction){
            case SoftActions.incVariable:
                chara.variables[VarToInt(affectedVariable)]++; if(chara.variables[VarToInt(affectedVariable)]>9) chara.variables[VarToInt(affectedVariable)]=0;
            break;
            case SoftActions.decVariable:
                chara.variables[VarToInt(affectedVariable)]--; if(chara.variables[VarToInt(affectedVariable)]<0) chara.variables[VarToInt(affectedVariable)]=9;
            break;
            case SoftActions.setVariable:
                chara.variables[VarToInt(affectedVariable)] = affectedNumber; //Nunca pasar valores que no esten entre 0 y 9
            break;
        }
    }

    public SoftAction copy(){
        SoftAction res = new SoftAction(softAction, affectedVariable, affectedNumber);
        return res;
    }

    int VarToInt(Variables vari){
        int index=-1;

            switch(vari){
                case Variables.A: index=0; break;
                case Variables.B: index=1; break;
                case Variables.C: index=2; break;
                case Variables.D: index=3; break;
                case Variables.E: index=4; break;
                case Variables.F: index=5; break;
            }

        return index;
    }

}