public class LString extends LeuterperObject{
    
    String value;
    
    public LString(){
        value = "";
    }//I see.. a rhinoceros
    
    public LString(String value){
        String[] asciiValues = value.split(" ");
        value = "";
        for(int i=0; i<asciiValues.length; i++){
            value = "";
            value += (char) Integer.parseInt(asciiValues[i].trim());
        }
    }

    public String getValue() {
        return value;
    }

    public void setValue(String value) {
        this.value = value;
    }
    
}
