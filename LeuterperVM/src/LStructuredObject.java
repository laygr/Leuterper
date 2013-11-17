import java.util.ArrayList;

public class LStructuredObject extends LeuterperObject<ArrayList<LeuterperObject>>{
    
    public LStructuredObject(int attributes)
    {
        this.value = new ArrayList<>(attributes);
    }
    
}
