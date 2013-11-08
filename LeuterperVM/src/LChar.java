public class LChar extends LeuterperObject{
    
    char value;
    
    public LChar(){
        value = '\0';
    }
    
    public LChar(String value){
        int asciiValue = Integer.parseInt(value);
        this.value = (char) asciiValue;
    }
}
