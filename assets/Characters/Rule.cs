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
    public bool condAND;

    public Rule(){
        conds = new List<Condition>();
        action = new Action(HardActions.doNothing, Dir4.left);
        condAND = false;
    }

    public Rule(List<Condition> pconds, Action paction){
        conds = pconds;
        action = paction;
        condAND = false;
    }

    public void addCondition(Conditions ptype, Objects pobject, int pnumber, Variables pvar, bool positive = true){
        conds.Add( new Condition(ptype, pobject, pnumber,pvar, positive) );
    }

    public void addCondition(Condition pcond){
        conds.Add( pcond );
    }

    public void removeCondition(int index){
        conds.RemoveAt(index);
    }

    public void setAction(Action paction){
        action = paction;
    }

    public bool run(BehCharacter pchara){
        //Si no hay condiciones, se cumple
        if(conds.Count == 0){
            action.perform(pchara);
            return true;
        }

        //AND, buscamos un falso y solo se cumple si no lo encontramos
        if(condAND){
            for(int i=0 ; i < conds.Count ; i++){
                if(!conds[i].evaluate(pchara)){
                    return false;
                }
            }
            action.perform(pchara);
            return true;
        //OR, buscamos un true y se cumple si encontramos al menos uno
        } else{ //condOR
            
            for(int i=0 ; i < conds.Count ; i++){
                if(conds[i].evaluate(pchara)){
                    action.perform(pchara);
                    return true;
                }
            }
        }
        
        return false;
        
    }

}