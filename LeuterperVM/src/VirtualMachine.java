import java.io.*;
import java.nio.charset.Charset;
import java.nio.file.Files;
import java.nio.file.Path;
import java.util.ArrayList;
import java.util.Scanner;
import java.util.Stack;

public class VirtualMachine {
    
    static ArrayList<ClassDefinition> classes = new ArrayList<>();
    static ArrayList<FunctionDefinition> funcs = new ArrayList<>();
    static ArrayList<LeuterperObject> lobjects = new ArrayList<>();
    
    private Stack<Float> argumentsStack;
    private Stack<Integer> contextStack;
    
    public void excecuteSpecialFunction(int functionIndex)
    {
        int functionIndexInt = (int)functionIndex;
        
        switch(functionIndexInt)
        {
            //LList Special Functions
            
            case 0:
                
                break;
                
            case 1:
                
                break;

            case 2:
               
                break;
                
            case 3:
                
                break;
                
            case 4:
                
                break;
                
            case 5:
                
                break;
                
            case 6:
                
                break;
                
            case 7:
                
                break;
                
           case 8:
                
                break;
                
            case 9:
                
                break;

            case 10:
               
                break;
                
            case 11:
                
                break;
                
            case 12:
                
                break;
                
            case 13:
                
                break;
                
            case 14:
                
                break;
                
            case 15:
                
                break;
                
            case 16:
                
                break;
                
            case 17:
                
                break;
                
            case 18:
                
                break;
                
            case 19:
                
                break;
        }
    }
    
    public static void main(String[] args) throws FileNotFoundException{
        
        if(args.length < 1){
            System.out.println("Incorrect usage, expecting 1 parameter.");
            System.exit(0);
        }
        
        Scanner sc = new Scanner(new File(args[0]));
        String intermediateCode = "";
        while(sc.hasNextLine()){
            intermediateCode += sc.nextLine();
            intermediateCode += "\n";
        }
        
        String[] ic = intermediateCode.split("\n");
        
        //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        
        //First line corresponds to number of classes
        int numberOfClasses = Integer.parseInt(ic[0]);
        System.out.println("numberOfClasses: "+numberOfClasses);
        new VirtualMachine();
        
        //Creating and storing instances of ClassDefinition
        for(int i=0; i<numberOfClasses; i++){
           
            int numberOfAttributes = Integer.parseInt(ic[i+1]);
            System.out.println("Class "+(i+1)+" has "+numberOfAttributes+" attributes");
            ClassDefinition classDefinition = new ClassDefinition(numberOfAttributes);
            
            classes.add(classDefinition);
        }
        
        //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        
        int currentLine = numberOfClasses+1;
        int numberOfFunctions = Integer.parseInt(ic[currentLine]);
        System.out.println("numberOfFunctions: "+numberOfFunctions);
        
        //Creating and storing instances of FunctionDefinition
        for(int i=0; i<numberOfFunctions; i++){
            
            int numberOfParameters = Integer.parseInt(ic[currentLine+1]);
            FunctionDefinition funcDef = new FunctionDefinition(numberOfParameters);
            
            int numberOfActions = Integer.parseInt(ic[currentLine+2]);
            
            for(int j=0; j<numberOfActions; j++){
 
                String action = ic[currentLine+j+3];
                funcDef.addAction(action);
            }
            
            currentLine += numberOfActions+2;
            
            System.out.println("Funtion "+(i+1)+" has "+numberOfParameters+" parameters and "+numberOfActions+" actions:");
            for(int j=0; j<numberOfActions; j++){
                System.out.println(funcDef.getActions().get(j));
            }
            
            funcs.add(funcDef);
        }
            
        //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        
        //Next line corresponds to number of literals
        currentLine++;
        int totalVariables = Integer.parseInt(ic[currentLine]);
        lobjects.ensureCapacity(totalVariables);
        currentLine++;
        int numberOfLiterals = Integer.parseInt(ic[currentLine]);
        System.out.println("numberOfLiterals: "+numberOfLiterals);
        currentLine++;
        
        //Creating Leuterper Objects for each literal (constant)
        for(int i=0; i<numberOfLiterals; i++){
           
           String literal = ic[currentLine+i]; //Gets literal
           String[] typeValue = literal.split(" ");
           
           LeuterperObject lobject = new LeuterperObject();
           lobject.setType(typeValue[0]);
           
           for(int j=1; j<typeValue.length; j++){
               String value = lobject.getValue();
               value += typeValue[j]+" ";
               lobject.setValue(value);
           }
           System.out.println("Leuterper Object "+(i+1)+" has type: "+lobject.getType()+"    value: "+lobject.getValue());
           lobjects.add(lobject);
        }
            
        //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            
        currentLine += numberOfLiterals;
        int numberOfActions = Integer.parseInt(ic[currentLine]);
        System.out.println("The number of global actions is "+numberOfActions);
        currentLine++;
        
        for(int i=0; i<numberOfActions; i++){
            
            System.out.println(ic[currentLine+i]);
            
        }
    }
        
}

