public class LBoolean extends LeuterperObject<Boolean>{
    
    public LBoolean(){
        this.value = false;
    }
    
    public LBoolean(String value){
        this.value = value.equalsIgnoreCase("True");
    }
    
    public LBoolean(boolean value){
        this.value = value;
    }
}
