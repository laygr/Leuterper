/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */

/**
 *
 * @author Laygr
 */
public class LNumber
{
    float value;
    public LNumber(int index, float[] memory)
    {
        this.value = memory[index];
    }
    
    public float add(float otherValue)
    {
        return this.value + otherValue;
    }
    
    public float subtract(float otherValue)
    {
        return this.value - otherValue;
    }
}
