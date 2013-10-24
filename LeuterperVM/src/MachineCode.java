
import java.io.FileNotFoundException;
import java.io.FileReader;
import java.util.ArrayList;
import java.util.Scanner;
import java.util.logging.Level;
import java.util.logging.Logger;

/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */

/**
 *
 * @author Laygr
 */
class MachineCode {
    ArrayList<Float> machineCode;
    
    public MachineCode(String file)
    {
        try {
            Scanner s = new Scanner(new FileReader(file));
            
            while(s.hasNextFloat())
            {
                machineCode.add(s.nextFloat());
            }
            
        } catch (FileNotFoundException ex) {
            Logger.getLogger(MachineCode.class.getName()).log(Level.SEVERE, null, ex);
        }
    }
}
