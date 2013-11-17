
public class Context {
    
    private FunctionDefinition func;
    private LeuterperObject[] contextVars;
    private int actionsCheckpoint;

    public Context(FunctionDefinition func, int size){
        this.func = func;
        contextVars = new LeuterperObject[size];
        actionsCheckpoint = 0;
    }
    
    public FunctionDefinition getFunc() {
        return func;
    }

    public void setFunc(FunctionDefinition func) {
        this.func = func;
    }

    public LeuterperObject[] getContextVars() {
        return contextVars;
    }

    public void setContextVars(LeuterperObject[] contextVars) {
        this.contextVars = contextVars;
    }

    public int getActionsCheckpoint() {
        return actionsCheckpoint;
    }

    public void setActionsCheckpoint(int actionsCheckpoint) {
        this.actionsCheckpoint = actionsCheckpoint;
    }
    
}
