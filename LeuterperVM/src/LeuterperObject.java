public abstract class LeuterperObject<genericType> {
    
    genericType value;
    
    public genericType getValue(){
        return this.value;
    }
    
    public void setValue(genericType gt){
        this.value = gt;
    }
    
}