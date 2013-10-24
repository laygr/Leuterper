import java.util.ArrayList;

public class FunctionDefinition {
    
    private int parameters;
    private ArrayList<String> actions;

    public FunctionDefinition(int parameters) {
        this.parameters = parameters;
        actions = new ArrayList<>();
    }
    
    public ArrayList<String> getActions() {
        return actions;
    }

    public void setActions(ArrayList<String> actions) {
        this.actions = actions;
    }
    
    public void addAction(String action) {
        actions.add(action);
    }

    public int getParameters() {
        return parameters;
    }

    public void setParameters(int parameters) {
        this.parameters = parameters;
    }
    
    
}
