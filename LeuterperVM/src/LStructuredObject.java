public class LStructuredObject extends LeuterperObject
{
    
    private LeuterperObject lobjects[];
    
    public LStructuredObject()
    {
        this.lobjects = new LeuterperObject[0];
    }
    
    public LStructuredObject(int attributes)
    {
        this.lobjects = new LeuterperObject[attributes];
    }

    public LeuterperObject[] getLobjects() {
        return lobjects;
    }

    public void setLobjects(LeuterperObject[] lobjects) {
        this.lobjects = lobjects;
    }
    
}
