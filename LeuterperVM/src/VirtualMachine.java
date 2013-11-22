import java.io.*;
import java.util.ArrayList;
import java.util.Scanner;
import java.util.Stack;

public class VirtualMachine {
    
    private final ArrayList<ClassDefinition> classes;
    private final ArrayList<FunctionDefinition> funcs;
    private final ArrayList<LeuterperObject> lobjects;
    
    private final Stack<LeuterperObject> argumentsStack;
    private final Stack<Context> contextStack;
    private Context currentContext = null;
    
    private int totalLiterals;
    private int totalGlobals;
    private int totalNonGlobals;
    
    private boolean returnFound;
    
    static Scanner sc = new Scanner(System.in);

    public VirtualMachine() {
        this.contextStack = new Stack<>();
        this.argumentsStack = new Stack<>();
        this.lobjects = new ArrayList<>();
        this.funcs = new ArrayList<>();
        this.classes = new ArrayList<>();
        this.returnFound = false;
    }
    
    public void executeSpecialFunction(int functionIndex)
    {
        
        switch(functionIndex)
        {
            //Special functions class Object
            
            case 0:
                // Constructor for Object
                argumentsStack.push(new LUndefined());
                break;
                
            case 1:
                // ==(Object, Object)
                LeuterperObject lobject1 = argumentsStack.pop();
                LeuterperObject lobject2 = argumentsStack.pop();
                boolean sameLObjects = lobject1.getClass().equals(lobject2.getClass());
                argumentsStack.push(new LBoolean(sameLObjects && lobject1.getValue().equals(lobject2.getValue())));
                break;
                
            case 2:
                // toString(Object)
                lobject1 = argumentsStack.pop();
                argumentsStack.push(new LString(String.valueOf(lobject1.getValue())));
                break;
                
            case 3:
                LNumber number1 = (LNumber) argumentsStack.pop();
                LNumber number2 = new LNumber();
                number2.setValue(number1.getValue());
                argumentsStack.push(number2);
                break;
                
            case 4:
                // ==(Number, Number)
                lobject2 = argumentsStack.pop();
                lobject1 = argumentsStack.pop();
                boolean numberLObjects = lobject1.getClass().equals(lobject2.getClass()) && lobject1.getClass().equals(LNumber.class);
                argumentsStack.push(new LBoolean(numberLObjects && lobject1.getValue().equals(lobject2.getValue())));
                break;
                
            case 5:
                // toString(Number)
                number1 = (LNumber) argumentsStack.pop();
                LString lstring = new LString();
                lstring.setValue(String.valueOf(number1.getValue()));
                argumentsStack.push(lstring);
                break;
                
            case 6:
                // +(Number, Number)
                number2 = (LNumber) argumentsStack.pop();
                number1 = (LNumber) argumentsStack.pop();
                double result = number1.getValue() + number2.getValue();
                LNumber number3 = new LNumber();
                number3.setValue(result);
                argumentsStack.push(number3);
                break;
                
            case 7:
                // -(Number, Number)
                number2 = (LNumber) argumentsStack.pop();
                number1 = (LNumber) argumentsStack.pop();
                result = number1.getValue() - number2.getValue();
                number3 = new LNumber();
                number3.setValue(result);
                argumentsStack.push(number3);
                break;
                
           case 8:
                // *(Number, Number)
                number2 = (LNumber) argumentsStack.pop();
                number1 = (LNumber) argumentsStack.pop();
                result = number1.getValue() * number2.getValue();
                number3 = new LNumber();
                number3.setValue(result);
                argumentsStack.push(number3);
                break;
                
            case 9:
                // /(Number, Number)
                number2 = (LNumber) argumentsStack.pop();
                number1 = (LNumber) argumentsStack.pop();
                result = number1.getValue() / number2.getValue();
                number3 = new LNumber();
                number3.setValue(result);
                argumentsStack.push(number3);
                break;

            case 10:
               // ^(Number, Number)
                number2 = (LNumber) argumentsStack.pop();
                number1 = (LNumber) argumentsStack.pop();
                result = Math.pow(number1.getValue(), number2.getValue());
                number3 = new LNumber();
                number3.setValue(result);
                argumentsStack.push(number3);
                break;
                
            case 11:
                // !=(Number, Number)
                number2 = (LNumber) argumentsStack.pop();
                number1 = (LNumber) argumentsStack.pop();
                if(number1.getValue().equals(number2.getValue())){
                    argumentsStack.push(new LBoolean("False"));
                }
                else{
                    argumentsStack.push(new LBoolean("True"));
                }
                break;
                
            case 12:
                // +=(Number, Number)
                number2 = (LNumber) argumentsStack.pop();
                number1 = (LNumber) argumentsStack.pop();
                number1.setValue(number1.getValue()+number2.getValue());
                argumentsStack.push(new LVoid());
                break; 
                
            case 13:
                // -=(Number, Number)
                number2 = (LNumber) argumentsStack.pop();
                number1 = (LNumber) argumentsStack.pop();
                number1.setValue(number1.getValue()-number2.getValue());
                argumentsStack.push(new LVoid());
                break;    
                
            case 14:
                // *=(Number, Number)
                number2 = (LNumber) argumentsStack.pop();
                number1 = (LNumber) argumentsStack.pop();
                number1.setValue(number1.getValue()*number2.getValue());
                argumentsStack.push(new LVoid());
                break;   
                
            case 15:
                // /=(Number, Number)
                number2 = (LNumber) argumentsStack.pop();
                number1 = (LNumber) argumentsStack.pop();
                number1.setValue(number1.getValue()/number2.getValue());
                argumentsStack.push(new LVoid());
                break;
            
            case 16:
                // <(Number, Number)
                number2 = (LNumber) argumentsStack.pop();
                number1 = (LNumber) argumentsStack.pop();
                if(number1.getValue() < number2.getValue()){
                    argumentsStack.push(new LBoolean("True"));
                }
                else{
                     argumentsStack.push(new LBoolean("False"));
                }
                break;
                
            case 17:
                // <=(Number, Number)
                number2 = (LNumber) argumentsStack.pop();
                number1 = (LNumber) argumentsStack.pop();
                if(number1.getValue() <= number2.getValue()){
                    argumentsStack.push(new LBoolean("True"));
                }
                else{
                     argumentsStack.push(new LBoolean("False"));
                }
                break;
                
            case 18:
                // >=(Number, Number)
                number2 = (LNumber) argumentsStack.pop();
                number1 = (LNumber) argumentsStack.pop();
                if(number1.getValue() >= number2.getValue()){
                    argumentsStack.push(new LBoolean("True"));
                }
                else{
                     argumentsStack.push(new LBoolean("False"));
                }
                break;
                
            case 19:
                // >(Number, Number)
                number2 = (LNumber) argumentsStack.pop();
                number1 = (LNumber) argumentsStack.pop();
                if(number1.getValue() > number2.getValue()){
                    argumentsStack.push(new LBoolean("True"));
                }
                else{
                     argumentsStack.push(new LBoolean("False"));
                }
                break;
           
            case 20:
                argumentsStack.push(new LBoolean());
                break;
                
            case 21:
                // ==(Boolean, Boolean)
                lobject2 = argumentsStack.pop();
                lobject1 = argumentsStack.pop();
                boolean booleanLObjects = lobject1.getClass().equals(lobject2.getClass()) && lobject1.getClass().equals(LChar.class);
                argumentsStack.push(new LBoolean(booleanLObjects && lobject1.getValue().equals(lobject2.getValue())));
                break;
                
            case 22:
                // ||(Boolean, Boolean)
                LBoolean boolean2 = (LBoolean) argumentsStack.pop();
                LBoolean boolean1 = (LBoolean) argumentsStack.pop();
                if(boolean1.getValue() || boolean2.getValue()){
                    argumentsStack.push(new LBoolean("True"));
                }
                else{
                     argumentsStack.push(new LBoolean("False"));
                }
                break;
                
            case 23:
                // &&(Boolean, Boolean)
                boolean2 = (LBoolean) argumentsStack.pop();
                boolean1 = (LBoolean) argumentsStack.pop();
                if(boolean1.getValue() || boolean2.getValue()){
                    argumentsStack.push(new LBoolean("True"));
                }
                else{
                     argumentsStack.push(new LBoolean("False"));
                }
                break;
                
            case 24:
                // not(Boolean)
                boolean1 = (LBoolean) argumentsStack.pop();
                if(!boolean1.getValue()){
                    argumentsStack.push(new LBoolean("True"));
                }
                else{
                     argumentsStack.push(new LBoolean("False"));
                }
                break;
                
            case 25:
                // toString(Boolean)
                boolean1 = (LBoolean) argumentsStack.pop();
                lstring = new LString();
                lstring.setValue(String.valueOf(boolean1.getValue()));
                argumentsStack.push(lstring);
                break;
                 
            case 26:
                argumentsStack.push(new LChar());
                break;
                
            case 27:
                // ==(Char, Char)
                lobject2 = argumentsStack.pop();
                lobject1 = argumentsStack.pop();
                boolean charLObjects = lobject1.getClass().equals(lobject2.getClass()) && lobject1.getClass().equals(LChar.class);
                argumentsStack.push(new LBoolean(charLObjects && lobject1.getValue().equals(lobject2.getValue())));
                break;
                
            case 28:
                // toString(Boolean)
                LChar lchar1 = (LChar) argumentsStack.pop();
                lstring = new LString();
                lstring.setValue(Character.toString(lchar1.getValue()));
                argumentsStack.push(lstring);
                break;
                
            case 29:
                argumentsStack.push(new LList());
                break;
                
            case 30:
                number1 = (LNumber) argumentsStack.pop();
                argumentsStack.push(new LList(number1.getValue()));
                break;
                
            case 31:
                // ==(LList<A>, LList<A>)
                LList llist2 = (LList) argumentsStack.pop();
                LList llist1 = (LList) argumentsStack.pop();
                boolean sameList = compareLists(llist1.getValue(), llist2.getValue());
                if(sameList)
                    argumentsStack.push(new LBoolean("True"));
                else
                   argumentsStack.push(new LBoolean("False"));
                break;
                
            case 32:
                // String toString(LList<A>)
                llist1 = (LList) argumentsStack.pop();
                lstring = new LString();
                lstring.setValue(listToString(llist1));
                argumentsStack.push(lstring);
                break;
                
            case 33:
                //Void add(LList<A>, A other)
                LeuterperObject lobject = argumentsStack.pop();
                llist1 = (LList) argumentsStack.pop();
                llist1.getValue().add(lobject);
                argumentsStack.push(llist1);
                break;
                
            case 34:
                //Void insertAt(LList<A>, Number, A other)
                lobject = argumentsStack.pop();
                number1 = (LNumber) argumentsStack.pop();
                llist1 = (LList) argumentsStack.pop();
                double dindex = number1.getValue();
                int index = (int)dindex;
                llist1.getValue().add(index, lobject);
                argumentsStack.push(llist1);
                break;
                
            case 35:
                //Void removeAt(LList<A>, Number)
                number1 = (LNumber) argumentsStack.pop();
                llist1 = (LList) argumentsStack.pop();
                dindex = number1.getValue();
                index = (int)dindex;
                llist1.getValue().remove(index);
                argumentsStack.push(llist1);
                break;
                
            case 36:
                // Number count(LList<A>)
                llist1 = (LList) argumentsStack.pop();
                number1 = new LNumber(String.valueOf(llist1.getValue().size()));
                argumentsStack.push(number1);
                break;
                
            case 37:
                // A get(LList<A>, Number)
                number1 = (LNumber) argumentsStack.pop();
                llist1 = (LList) argumentsStack.pop();
                dindex = number1.getValue();
                index = (int)dindex;
                argumentsStack.push(llist1.getValue().get(index));
                break;
                
            case 38:
                // set(LList<A>, Number, A other)
                lobject = argumentsStack.pop();
                number1 = (LNumber) argumentsStack.pop();
                llist1 = (LList) argumentsStack.pop();
                dindex = number1.getValue();
                index = (int)dindex;
                llist1.getValue().set(index, lobject);
                argumentsStack.push(new LVoid());
                break;
                
            case 39:
                argumentsStack.push(new LString());
                break;
                
            case 40:
                // ==(String, String)
                lobject2 = argumentsStack.pop();
                lobject1 = argumentsStack.pop();
                boolean stringLObjects = lobject1.getClass().equals(lobject2.getClass()) && lobject1.getClass().equals(LString.class);
                argumentsStack.push(new LBoolean(stringLObjects && lobject1.getValue().equals(lobject2.getValue())));
                break;
                
            case 41:
                // toNumber(String)
                LString lstring1 = (LString) argumentsStack.pop();
                number1 = new LNumber(lstring1.getValue());
                argumentsStack.push(number1);
                break;
              
            case 42:
                // +(String, String)
                LString lstring2 = (LString) argumentsStack.pop();
                lstring1 = (LString) argumentsStack.pop();
                String appended = lstring1.getValue() + lstring2.getValue();
                LString resultString = new LString();
                resultString.setValue(appended);
                argumentsStack.push(resultString);
                break;
                
            case 43:
                // +=(String, String)
                lstring2 = (LString) argumentsStack.pop();
                lstring1 = (LString) argumentsStack.pop();
                appended = lstring1.getValue() + lstring2.getValue();
                lstring1.setValue(appended);
                argumentsStack.push(new LVoid());
                break;
                
            case 44:
                // read()
                String line = sc.nextLine();
                lstring = new LString();
                lstring.setValue(line);
                argumentsStack.push(lstring);
                break;
                
            case 45:
                // write(String)
                LString printedString = (LString) argumentsStack.pop();
                System.out.println("Leuterper says: "+printedString.getValue());
                argumentsStack.push(new LUndefined());
                break;
        }
    }
    
    public void executeAction(String action){
        //System.out.println("Executing  "+action+"..."+argumentsStack.size()+" elements in stack atm..."+contextStack.size()+" contexts atm");
        String actionArr[] = action.split(" ");
        
        switch(actionArr[0]){
            
                case("ass"):
                    LeuterperObject lobject= argumentsStack.pop(); 
                    int memoryLocation = Integer.parseInt(actionArr[1]);
                    if(memoryLocation<totalLiterals+totalGlobals){
                        lobjects.set(memoryLocation, lobject);
                    }
                    else{
                        int contextLocation = memoryLocation - totalLiterals - totalGlobals;
                        LeuterperObject[] contextVars;
                        contextVars = currentContext.getContextVars();
                        contextVars[contextLocation] = lobject;
                    }
                    break;
                    
                case("push"):
                    memoryLocation = Integer.parseInt(actionArr[1]);
                    if(memoryLocation<totalLiterals+totalGlobals){
                        argumentsStack.push(lobjects.get(Integer.parseInt(actionArr[1])));
                    }
                    else{
                        LeuterperObject[] contextVars;
                        contextVars = currentContext.getContextVars();
                        argumentsStack.push(contextVars[memoryLocation-totalLiterals-totalGlobals]);
                    }
                    break;
                    
                case("call"):
                    int indiceFuncion = Integer.parseInt(actionArr[1]);
                    if(indiceFuncion<=45)
                        //It is a special function
                        executeSpecialFunction(indiceFuncion);
                    else{
                        FunctionDefinition fnc = funcs.get(indiceFuncion-46);
                        //A context for this function is created:
                        if(currentContext!=null)
                            contextStack.push(currentContext);
                        Context context = new Context(fnc,totalNonGlobals);
                        currentContext = context;
                        int parametrosFuncion = fnc.getParameters();
                        LeuterperObject[] contextVars;
                        contextVars = context.getContextVars();
                        for(int i=parametrosFuncion-1; i>=0; i--){
                            contextVars[i] = argumentsStack.pop();
                        }
                        while(currentContext.getActionsCheckpoint()<fnc.getActions().length){
                            executeAction(fnc.getActions()[currentContext.getActionsCheckpoint()]);
                            if(returnFound){
                                returnFound = false;
                                break;
                            }
                            else{
                                currentContext.setActionsCheckpoint(currentContext.getActionsCheckpoint()+1); //actionsCheckpoint++
                            }
                        }
                        if(!contextStack.empty())
                            currentContext = contextStack.pop();
                    }
                    argumentsStack.pop();
                    break;
                    
                case("callp"):
                    indiceFuncion = Integer.parseInt(actionArr[1]);
                    if(indiceFuncion<=45)
                        //It is a special function
                        executeSpecialFunction(indiceFuncion);
                    else{
                        FunctionDefinition fnc = funcs.get(indiceFuncion-46);
                        //A context for this function is created:
                        if(currentContext!=null)
                            contextStack.push(currentContext);
                        Context context = new Context(fnc,totalNonGlobals);
                        currentContext = context;
                        int parametrosFuncion = fnc.getParameters();
                        LeuterperObject[] contextVars;
                        contextVars = context.getContextVars();
                        for(int i=parametrosFuncion-1; i>=0; i--){
                            contextVars[i] = argumentsStack.pop();
                        }
                        while(currentContext.getActionsCheckpoint()<fnc.getActions().length){
                            executeAction(fnc.getActions()[currentContext.getActionsCheckpoint()]);
                            if(returnFound){
                                returnFound = false;
                                break;
                            }
                            else{
                                currentContext.setActionsCheckpoint(currentContext.getActionsCheckpoint()+1); //actionsCheckpoint++
                            }
                        }
                        if(!contextStack.empty())
                            currentContext = contextStack.pop();
                    }
                    break;
                    
                case("new"):
                    indiceFuncion = Integer.parseInt(actionArr[1]);
                    int indiceClase = Integer.parseInt(actionArr[2]);
                    if(indiceFuncion<=45)
                        //It is a special function
                        executeSpecialFunction(indiceFuncion);
                    else{
                        FunctionDefinition fnc = funcs.get(indiceFuncion-46);
                        //A context for this function is created:
                        if(currentContext!=null)
                            contextStack.push(currentContext);
                        Context context = new Context(fnc,totalNonGlobals);
                        currentContext = context;
                        int parametrosFuncion = fnc.getParameters();
                        LeuterperObject[] contextVars;
                        contextVars = context.getContextVars();
                        int numberOfAttributes = classes.get(indiceClase).getNumberOfAttributes();
                        LStructuredObject lstructuredObject = new LStructuredObject(numberOfAttributes);
                        for(int i=0; i<numberOfAttributes; i++){
                            lstructuredObject.getValue().add(new LUndefined());
                        }
                        contextVars[0] = lstructuredObject;
                        for(int i=parametrosFuncion-1; i>0; i--){
                            contextVars[i] = argumentsStack.pop();
                        }
                        while(currentContext.getActionsCheckpoint()<fnc.getActions().length){
                            executeAction(fnc.getActions()[currentContext.getActionsCheckpoint()]);
                            if(returnFound){
                                returnFound = false;
                                break;
                            }
                            else{
                                currentContext.setActionsCheckpoint(currentContext.getActionsCheckpoint()+1); //actionsCheckpoint++
                            }
                        }
                        if(!contextStack.empty())
                            currentContext = contextStack.pop();
                    }
                    argumentsStack.pop();
                    break;
                    
                case("newp"):
                    indiceFuncion = Integer.parseInt(actionArr[1]);
                    if(indiceFuncion<=45)
                        //It is a special function
                        executeSpecialFunction(indiceFuncion);
                    else{
                        indiceClase = Integer.parseInt(actionArr[2])-7;
                        FunctionDefinition fnc = funcs.get(indiceFuncion-46);
                        //A context for this function is created:
                        if(currentContext!=null)
                            contextStack.push(currentContext);
                        Context context = new Context(fnc,totalNonGlobals);
                        currentContext = context;
                        int parametrosFuncion = fnc.getParameters();
                        LeuterperObject[] contextVars;
                        contextVars = context.getContextVars();
                        int numberOfAttributes = classes.get(indiceClase).getNumberOfAttributes();
                        LStructuredObject lstructuredObject = new LStructuredObject(numberOfAttributes);
                        for(int i=0; i<numberOfAttributes; i++){
                            lstructuredObject.getValue().add(new LUndefined());
                        }
                        contextVars[0] = lstructuredObject;
                        for(int i=parametrosFuncion-1; i>0; i--){
                            contextVars[i] = argumentsStack.pop();
                        }
                        while(currentContext.getActionsCheckpoint()<fnc.getActions().length){
                            executeAction(fnc.getActions()[currentContext.getActionsCheckpoint()]);
                            if(returnFound){
                                returnFound = false;
                                break;
                            }
                            else{
                                currentContext.setActionsCheckpoint(currentContext.getActionsCheckpoint()+1); //actionsCheckpoint++
                            }
                        }
                        if(!contextStack.empty())
                            currentContext = contextStack.pop();
                    }
                    break;
                    
                case("get"):
                    LStructuredObject lstructuredObject = (LStructuredObject) argumentsStack.pop();
                    int attributeNumber = Integer.parseInt(actionArr[1]);
                    ArrayList<LeuterperObject> lobjectList = lstructuredObject.getValue();
                    argumentsStack.push(lobjectList.get(attributeNumber));
                    break;
                    
                case("set"):
                    lobject = argumentsStack.pop();
                    lstructuredObject = (LStructuredObject) argumentsStack.pop();
                    attributeNumber = Integer.parseInt(actionArr[1]);
                    lobjectList = lstructuredObject.getValue();
                    lobjectList.set(attributeNumber, lobject);
                    break;
                
                case("list"):
                    int numElements = Integer.parseInt(actionArr[1]);
                    for(int i=0; i<numElements; i++){
                        argumentsStack.pop();
                    }
                    break;
                    
                case("listp"):
                    numElements = Integer.parseInt(actionArr[1]);
                    LList llist = new LList();
                    for(int i=0; i<numElements; i++){
                        llist.getValue().add(0, argumentsStack.pop());
                    }
                    argumentsStack.push(llist);
                    break;
                    
                case("jmp"):
                    int jumpWhere = Integer.parseInt(actionArr[1]);
                    currentContext.setActionsCheckpoint(jumpWhere-1);
                    break;
                    
                case("jmpt"):
                    LBoolean evaluation = (LBoolean) argumentsStack.pop();
                    
                    if(Boolean.valueOf(evaluation.getValue())){
                        jumpWhere = Integer.parseInt(actionArr[1]);
                        currentContext.setActionsCheckpoint(jumpWhere-1);
                    }
                    break;
                    
                case("jmpf"):
                    evaluation = (LBoolean) argumentsStack.pop();
                    
                    if(!Boolean.valueOf(evaluation.getValue())){
                        jumpWhere = Integer.parseInt(actionArr[1]);
                        currentContext.setActionsCheckpoint(jumpWhere-1);
                    }
                    break;
                    
                case("rtn"):
                    returnFound = true;
                    break;
                    
                default:
                    System.out.println("Action not found error!");
                    break;
        } 
    }
    
    public boolean compareLists(ArrayList<LeuterperObject> llist1, ArrayList<LeuterperObject> llist2){
        boolean sameList = true;
        //*Note: The "value" of a LList object is its actual list.
        
        if(llist1.size() == llist2.size()){
            for(int i=0; i<llist1.size(); i++){
                if(llist1.get(i).getClass() == llist2.get(i).getClass()){
                    if(llist1.get(i).getClass() == LList.class || llist2.get(i).getClass() == LStructuredObject.class){
                        sameList = compareLists((ArrayList<LeuterperObject>)llist1.get(i).getValue(), (ArrayList<LeuterperObject>)llist2.get(i).getValue());
                    }
                    else{
                        if(!(llist1.get(i).getValue().equals(llist2.get(i).getValue()))){
                            sameList = false;
                        }
                    }
                }
                else{
                    sameList = false;
                }
            }
        }
        else{
            sameList = false;
        }
        return sameList;
    }
    
    public String listToString(LList llist){
        String string = "";
        ArrayList<LeuterperObject> list= llist.getValue();
        for(int i=0; i<list.size(); i++){
            if(list.get(i).getClass() == LList.class){
                string += listToString((LList) list.get(i));
            }
            else{
                string += list.get(i).getValue();
            }
            string += " ";
        }
        string = string.trim();
        return string;
    }
    
    public void parseIntermediateCode(String fileName) throws FileNotFoundException{
        
        //Scanner reads every line from given file and stores it in "intermediateCode"
        Scanner scFile = new Scanner(new File(fileName));
        String intermediateCode = "";
        while(scFile.hasNextLine()){
            intermediateCode += scFile.nextLine();
            intermediateCode += "\n";
        }
        
        //Intermediate code is split into array of strings line by line
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
            int numberOfActions = Integer.parseInt(ic[currentLine+2]);
            FunctionDefinition funcDef = new FunctionDefinition(numberOfParameters, numberOfActions);
            String[] actions;
            actions = funcDef.getActions();
            
            for(int j=0; j<numberOfActions; j++){
 
                String action = ic[currentLine+j+3];
                actions[j] = action;
            }
            
            currentLine += numberOfActions+2;
            
            System.out.println("Funtion "+(i+1)+" has "+numberOfParameters+" parameters and "+numberOfActions+" actions:");
            for(int j=0; j<numberOfActions; j++){
                System.out.println(actions[j]);
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
        
        System.out.println("\n\n\n");
        
        for(int i=0; i<numberOfActions; i++){
            executeAction(ic[currentLine+i]);
        }
    }
    
    public static void main(String[] args) throws FileNotFoundException{
 
        if(args.length < 1){
            System.out.println("Incorrect usage, expecting 1 parameter.");
            System.exit(0);
        }
        
        //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        //Creating virtual machine...
        VirtualMachine lvm = new VirtualMachine();
        
        //Parsing intermediate code...
        lvm.parseIntermediateCode(args[0]);
     
    }
        
}

