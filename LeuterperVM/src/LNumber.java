public class LNumber extends LeuterperObject<Double>
{
    
    public LNumber()
    {
        this.value = 0.0;
    }
    
    public LNumber(String value)
    {
        this.value = Double.parseDouble(value);
    }
}
