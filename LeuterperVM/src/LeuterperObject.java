import java.util.ArrayList;

public class LeuterperObject {

    private ArrayList<LeuterperObject> lobjects;
    private String type;
    private String value;

    public LeuterperObject() {
        type = "";
        value = "";
        lobjects = new ArrayList<>();
    }

    public LeuterperObject(ArrayList<LeuterperObject> lobjects) {
        this.lobjects = lobjects;
    }

    public ArrayList<LeuterperObject> getLobjects() {
        return lobjects;
    }

    public void setLobjects(ArrayList<LeuterperObject> lobjects) {
        this.lobjects = lobjects;
    }

    public String getType() {
        return type;
    }

    public void setType(String type) {
        this.type = type;
    }

    public String getValue() {
        return value;
    }

    public void setValue(String value) {
        this.value = value;
    }
    
}
