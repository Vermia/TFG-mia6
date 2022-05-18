



public class Loop : Statement{
    public int times {get; private set;}
    public int currentIteration;
    public Action action;

    public Loop(int ptimes, Action paction){
        times=ptimes;
        action=paction;
        currentIteration=0;
    }

    public void run(BehCharacter pchara){
        currentIteration++;
        action.perform(pchara);
    }
}