using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum SoftActions{
    incVariable, decVariable, setVariable
}

public class SoftAction{
    SoftActions softAction;
    public Variables affectedVariable;
    public int affectedNumber;


    public SoftAction(SoftActions paction, Variables pvar, int pnumber = 0){
        softAction = paction; affectedVariable = pvar; affectedNumber = pnumber;
    }


    public void perform(BehCharacter chara){
        int variableInQuestion = chara.A;
        if(affectedVariable == Variables.B) variableInQuestion = chara.B;
        else if(affectedVariable == Variables.C) variableInQuestion = chara.C;
        else if(affectedVariable == Variables.D) variableInQuestion = chara.D;
        else if(affectedVariable == Variables.E) variableInQuestion = chara.E;
        else if(affectedVariable == Variables.F) variableInQuestion = chara.F;

        switch(softAction){
            case SoftActions.incVariable:
                chara.A++; if(chara.A>9) chara.A=0;
            break;
            case SoftActions.decVariable:
                chara.A--; if(chara.A<0) chara.A=9;
            break;
            case SoftActions.setVariable:
                variableInQuestion = affectedNumber; //Nunca pasar valores que no esten entre 0 y 9
            break;
        }
    }


}