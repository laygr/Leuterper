import java.util.ArrayList;

public class FunctionDefinition {
    
    private int parameters;
    private String[] actions;

    public FunctionDefinition(int parameters, int numberOfActions) {
        this.parameters = parameters;
        actions = new String[numberOfActions];
    }

    public String[] getActions() {
        return actions;
    }

    public void setActions(String[] actions) {
        this.actions = actions;
    }

    public int getParameters() {
        return parameters;
    }

    public void setParameters(int parameters) {
        this.parameters = parameters;
    }
    
    
}
