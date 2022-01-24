

public class RuleBuilder{
    Rule currentRule;



    public void buildCondition(Conditions ptype, Objects pobject, int pnumber, Variables pvar, bool positive = true){
        currentRule.addCondition(ptype, pobject, pnumber, pvar, positive);
    }

    public void buildHardAction(HardActions action){
        Action act = new Action(action);

        //Esto es por si, por error o ignorancia, se crean las softs antes que las hards: pues toca copiar las softs en la nueva acción
        if(currentRule.action.softActions.Count>0){
            foreach(SoftAction s_act in currentRule.action.softActions){
                act.addSoftAction(s_act.softAction, s_act.affectedVariable, s_act.affectedNumber);
            }
        }

        currentRule.setAction(act);
    }

    public void buildSoftAction(SoftActions soft, Variables pvar, int pnum=0){
        currentRule.action.addSoftAction(soft, pvar, pnum);
    }

    public void reset(){
        currentRule=new Rule();
    }

    public Rule get(){
        return currentRule;
    }

}