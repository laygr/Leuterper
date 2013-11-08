import java.io.*;
import java.util.ArrayList;
import java.util.Scanner;
import java.util.Stack;

public class VirtualMachine {
    
    static ArrayList<ClassDefinition> classes = new ArrayList<>();
    static ArrayList<FunctionDefinition> funcs = new ArrayList<>();
    static ArrayList<LeuterperObject> lobjects = new ArrayList<>();
    
    static Stack<LeuterperObject> argumentsStack = new Stack<LeuterperObject>();
    static Stack<LeuterperObject> contextStack = new Stack<LeuterperObject>();
    
    static int totalLiterals;
    static int totalGlobals;
    static int totalNonGlobals;
    static int currentMemoryIndex;
    
    static Scanner sc = new Scanner(System.in);
    
    public static void executeSpecialFunction(int functionIndex)
    {
        
        switch(functionIndex)
        {
            //Special functions class Object
            
            case 0:
                // Constructor for Object
                
                
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
                // toString(Number)
                
                LNumber number1 = (LNumber) argumentsStack.pop();
                LString lstring = new LString(String.valueOf(number1.getValue()));
                
                argumentsStack.push(lstring);
                break;
                
            case 6:
                // +(Number, Number)
                number1 = (LNumber) argumentsStack.pop();
                LNumber number2 = (LNumber) argumentsStack.pop();
                
                double result = number1.getValue() + number2.getValue();
                LNumber number3 = new LNumber();
                number3.setValue(result);
                argumentsStack.push(number3);
                break;
                
            case 7:
                // -(Number, Number)
                number1 = (LNumber) argumentsStack.pop();
                number2 = (LNumber) argumentsStack.pop();
                
                result = number1.getValue() - number2.getValue();
                number3 = new LNumber();
                number3.setValue(result);
                argumentsStack.push(number3);
                break;
                
           case 8:
                // *(Number, Number)
                number1 = (LNumber) argumentsStack.pop();
                number2 = (LNumber) argumentsStack.pop();
                
                result = number1.getValue() * number2.getValue();
                number3 = new LNumber();
                number3.setValue(result);
                argumentsStack.push(number3);
                break;
                
            case 9:
                // /(Number, Number)
                number1 = (LNumber) argumentsStack.pop();
                number2 = (LNumber) argumentsStack.pop();
                
                result = number1.getValue() / number2.getValue();
                number3 = new LNumber();
                number3.setValue(result);
                argumentsStack.push(number3);
                break;

            case 10:
               // ^(Number, Number)
                number1 = (LNumber) argumentsStack.pop();
                number2 = (LNumber) argumentsStack.pop();
                
                result = Math.pow(number1.getValue(), number2.getValue());
                number3 = new LNumber();
                number3.setValue(result);
                argumentsStack.push(number3);
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
           
            case 20:
                
                break;
                
            case 21:
                
                break;
                
            case 22:
                
                break;
                
            case 23:
                
                break;
                
            case 24:
                
                break;
                
            case 25:
                
                break;
                 
            case 26:
                
                break;
                
            case 27:
                
                break;
                
            case 28:
                
                break;
                
            case 29:
                
                break;
                
            case 30:
                
                break;
                
            case 31:
                
                break;
                
            case 32:
                
                break;
                
            case 33:
                
                break;
                
            case 34:
                
                break;
                
            case 35:
                
                break;
                
            case 36:
                
                break;
                
            case 37:
                
                break;
                
            case 38:
                
                break;
                
            case 39:
                
                break;
                
            case 40:
                
                break;
                
            case 41:
                
                break;
                
            case 42:
                // read()
                String line = sc.nextLine();
                lstring = new LString();
                lstring.setValue(line);
                argumentsStack.push(lstring);
                break;
                
            case 43:
                // write(String)
                
                LString printedString = (LString) argumentsStack.pop();
                System.out.println(printedString.getValue());
                break;
        }
    }
    
    public static void executeAction(String action){
        
        LBoolean evaluation;
        String actionArr[] = action.split(" ");
        
        switch(actionArr[0]){
            
                case("ass"):
                    LeuterperObject lobject= argumentsStack.pop(); 
                    lobjects.set(Integer.parseInt(actionArr[1]), lobject);
                    break;
                    
                case("push"):
                    argumentsStack.push(lobjects.get(Integer.parseInt(actionArr[1])));
                    break;
                    
                case("call"):
                    int indiceFuncion = Integer.parseInt(actionArr[1]);
                    if(indiceFuncion<=43){
                        //It is a special function
                        executeSpecialFunction(indiceFuncion);
                    }
                    else{
                        FunctionDefinition fnc = funcs.get(indiceFuncion-44);
                        
                        //A context for this function is created:
                        Context context = new Context(fnc,totalNonGlobals);
                        
                        int parametrosFuncion = fnc.getParameters();
                        for(int i=0; i<fnc.getActions().size(); i++){
                            executeAction(fnc.getActions().get(i));
                            context.setActionsCheckpoint(i);
                        }
                    }
                    argumentsStack.pop();
                    break;
                    
                case("callp"):
                    indiceFuncion = Integer.parseInt(actionArr[1]);
                    if(indiceFuncion<=43){
                        //It is a special function
                        executeSpecialFunction(indiceFuncion);
                    }
                    else{
                        FunctionDefinition fnc = funcs.get(indiceFuncion-44);
                        
                        //A context for this function is created:
                        Context context = new Context(fnc,totalNonGlobals);
                        
                        int parametrosFuncion = fnc.getParameters();
                        for(int i=0; i<fnc.getActions().size(); i++){
                            executeAction(fnc.getActions().get(i));
                            context.setActionsCheckpoint(i);
                        }
                    }
                    break;
                    
                case("new"):
                    
                    break;
                    
                case("get"):
                    
                    break;
                    
                case("set"):
                    
                    break;
                
                case("add"):
                    
                    break;
                    
                case("addp"):
                    
                    break;
                    
                case("jmp"):
                    
                    break;
                    
                case("jmpt"):
                    evaluation = (LBoolean) argumentsStack.pop();
                    
                    if(Boolean.valueOf(evaluation.isValue())==true){
                        
                    }
                    break;
                    
                case("jmpf"):
                    evaluation = (LBoolean) argumentsStack.pop();
                    
                    if(Boolean.valueOf(evaluation.isValue())==true){
                        
                    }
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
        totalLiterals = Integer.parseInt(ic[currentLine]);
        System.out.println("numberOfLiterals: "+totalLiterals);
        currentLine++;
        totalGlobals = Integer.parseInt(ic[currentLine]);
        System.out.println("numberOfGlobals: "+totalGlobals);
        currentLine++;
        totalNonGlobals = Integer.parseInt(ic[currentLine]);
        System.out.println("numberOfNonGlobals: "+totalNonGlobals);
        currentLine++;
        
        //Creating Leuterper Objects for each literal (constant)
        for(int i=0; i<totalLiterals; i++){
           
           String literal = ic[currentLine+i]; //Gets literal
           String[] typeValue = literal.split(" ");
           
           LeuterperObject lobject;
           String type = typeValue[0];
           String value = "";
           
           for(int j=1; j<typeValue.length; j++){
               value += typeValue[j]+" ";
           }
           
           LeuterperObjectsFactory factory = new LeuterperObjectsFactory();
           lobject = factory.makeLobject(type, value);
           System.out.println("Leuterper Object "+(i+1)+" has type: "+type+"    value: "+value);
           lobjects.add(lobject);
        }
            
        //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            
        currentLine += totalLiterals;
        int numberOfActions = Integer.parseInt(ic[currentLine]);
        System.out.println("The number of global actions is "+numberOfActions);
        currentLine++;
        
        currentMemoryIndex = totalLiterals + totalGlobals;
        
        for(int i=0; i<numberOfActions; i++){
            executeAction(ic[currentLine+i]);
        }
    }
        
}

