
import java.util.ArrayList;

/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */

/**
 *
 * @author Laygr
 */
public class Memory {
    ArrayList<Float> values;
    
    public Float get(int address)
    {
        return this.values.get(address);
    }
    
    public void set(int index, Float value)
    {
        this.values.set(index, value);
    }
    
    public int deref(int address)
    {
        return this.values.get(address).intValue();
    }
    
}
