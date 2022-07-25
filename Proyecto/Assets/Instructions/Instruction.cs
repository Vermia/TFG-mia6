

public enum InstructionTypes{
    action, loop, conditional
}

public class Instruction : Statement{

    public Action action;

    public Instruction(Action pact){
        action=pact;
    }

    public void run(BehCharacter pchara){
        action.perform(pchara);
    }

}




