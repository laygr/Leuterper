import java.util.ArrayList;

public class LList extends LeuterperObject<ArrayList<LeuterperObject>>{
    
    public LList(){
        value = new ArrayList<>();
    }
    
    public LList(double size){
        value = new ArrayList<>();
        value.ensureCapacity((int)size);
    }
    
    public LList(ArrayList<LeuterperObject> lobjects){
        this.value = lobjects;
    }
    
}
