public class LNumber extends LeuterperObject
{
    double value;
    
    public LNumber()
    {
        this.value = 0;
    }
    
    public LNumber(String value)
    {
        this.value = Double.parseDouble(value);
    }

    public double getValue() {
        return value;
    }

    public void setValue(double value) {
        this.value = value;
    }  
    
}
