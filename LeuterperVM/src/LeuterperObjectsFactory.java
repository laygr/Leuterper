public class LeuterperObjectsFactory {
    
    public LeuterperObjectsFactory(){
    }
    
    public LeuterperObject makeLobject(String type, String value){
        
        LeuterperObject lobject;
        
        switch(type){
            
            case("Number"):
                lobject = new LNumber(value);
                break;
                
            case("Boolean"):
                lobject = new LBoolean(value);
                break;
                
           case("Char"):
                lobject = new LChar(value);
                break;
               
           case("String"):
                lobject = new LString(value);
                break;
           
           default:
                lobject = new LStructuredObject();
                break;
        }
        
        return lobject;
    }
}
