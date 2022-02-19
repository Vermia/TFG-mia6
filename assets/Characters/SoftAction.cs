using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum SoftActions{
    incVariable, decVariable, setVariable
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
                chara.variables[(int)affectedVariable]++; if(chara.variables[(int)affectedVariable]>9) chara.variables[(int)affectedVariable]=0;
            break;
            case SoftActions.decVariable:
                chara.variables[(int)affectedVariable]--; if(chara.variables[(int)affectedVariable]<0) chara.variables[(int)affectedVariable]=9;
            break;
            case SoftActions.setVariable:
                chara.variables[(int)affectedVariable] = affectedNumber; //Nunca pasar valores que no esten entre 0 y 9
            break;
        }
    }


}