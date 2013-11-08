
public class Context {
    
    private FunctionDefinition func;
    private int actionsCheckpoint;
    private LeuterperObject[] contextVars;

    public Context(FunctionDefinition func, int size){
        this.func = func;
        actionsCheckpoint = 0;
        contextVars = new LeuterperObject[size];
    }
    
    public FunctionDefinition getFunc() {
        return func;
    }

    public void setFunc(FunctionDefinition func) {
        this.func = func;
    }

    public int getActionsCheckpoint() {
        return actionsCheckpoint;
    }

    public void setActionsCheckpoint(int actionsCheckpoint) {
        this.actionsCheckpoint = actionsCheckpoint;
    }

    public LeuterperObject[] getContextVars() {
        return contextVars;
    }

    public void setContextVars(LeuterperObject[] contextVars) {
        this.contextVars = contextVars;
    }
    
    
}
