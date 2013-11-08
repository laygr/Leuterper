public class LBoolean extends LeuterperObject{
    
    boolean value;
    
    public LBoolean(){
        value = false;
    }
    
    public LBoolean(String value){
        if(value.equalsIgnoreCase("True"))
            this.value = true;
        else
            this.value = false;
    }

    public boolean isValue() {
        return value;
    }

    public void setValue(boolean value) {
        this.value = value;
    }
    
    
}
