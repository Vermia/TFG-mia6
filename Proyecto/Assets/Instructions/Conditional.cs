



public class Conditional : Statement{

    public Condition cond;
    public Action action;
    public Action elseAction;

    public Conditional(Condition pcond, Action paction, Action pelse=null){
        cond=pcond;
        action=paction;
        elseAction=pelse;
    }

    public void run(BehCharacter pchara){
        if(cond.evaluate(pchara)){
            action.perform(pchara);
        } else{
            elseAction.perform(pchara);
        }
    }
}