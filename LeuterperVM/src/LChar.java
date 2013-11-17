public class LChar extends LeuterperObject<Character>{
    
    public LChar(){
        value = '\0';
    }
    
    public LChar(String value){
        int asciiValue = Integer.parseInt(value);
        this.value = (char) asciiValue;
    }

}
