public class LString extends LeuterperObject<String>{
    
    public LString(){
        value = "";
    }//I see.. a rhinoceros
    
    public LString(String numbers){
        if(numbers.equals("")){
            value="";
            return;
        }
        String[] asciiValues = numbers.split(" ");
        value = "";
        for(int i=0; i<asciiValues.length; i++){
            value += (char) Integer.parseInt(asciiValues[i].trim());
        }
        System.out.println("String = "+value);
    }
}
