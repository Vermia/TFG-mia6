using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;





public enum Variables{
    A,B,C,D,E,F
}


public class Rule{
    public List<Condition> conds {get; private set;}
    public Action action {get; private set;}

    public Rule(){
        conds = new List<Condition>();
        action = new Action(HardActions.doNothing);
    }

    public Rule(List<Condition> pconds, Action paction){
        conds = pconds;
        action = paction;
    }

    public void addCondition(Conditions ptype, Objects pobject, int pnumber, Variables pvar, bool positive = true){
        conds.Add( new Condition(ptype, pobject, pnumber,pvar, positive) );
    }

    public void addCondition(Condition pcond){
        conds.Add( pcond );
    }

    public void setAction(Action paction){
        action = paction;
    }

    public bool run(BehCharacter pchara){
        for(int i=0 ; i< conds.Count ; i++){
            if(!conds[i].evaluate(pchara)){
                return false;
            }
        }

        action.perform(pchara);
        return true;
    }

}