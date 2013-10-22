using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leuterper.MachineInstructions;

namespace Leuterper.Constructions
{
    class Program : IScopable, ICodeGenerator
    {
        //Definitions
        public UniquesList<Definition_Class> classes { get; set; }
        public UniquesList<Declaration_Var> vars { get; set; }
        public UniquesList<Definition_Function> functions { get; set; }

        //Actions
        public List<LAction> actions { get; set; }

        public Program(List<Statement> statements)
        {
            this.classes = new UniquesList<Definition_Class>();
            this.functions = new UniquesList<Definition_Function>();
            this.vars = new UniquesList<Declaration_Var>();
            this.actions = new List<LAction>();
            this.scopeManager = new ScopeManager(this);

            foreach (Statement s in statements)
            {
                s.program = this;

                if (s is Definition_Class)
                {
                    this.classes.Add(s as Definition_Class);

                }
                else if (s is Declaration_Var)
                {
                    this.vars.Add(s as Declaration_Var);

                }
                else if (s is Definition_Function)
                {
                    this.functions.Add(s as Definition_Function);

                }
                else
                {
                    this.actions.Add(s as LAction);
                }
            }

            for (int i = 0; i < this.classes.Count(); i++)
            {
                Definition_Class aClass = this.classes.Get(i);
                for (int j = 0; j < aClass.methodsDefinitions.Count(); j++)
                {
                    this.functions.Add(aClass.methodsDefinitions.Get(j));
                }
            }

            this.setIdentifiers();
        }

        public void setIdentifiers()
        {
            for (int i = 0; i < this.classes.Count(); i++)
            {
                this.classes.Get(i).identifier = i;
            }

            for (int i = 0; i < this.functions.Count(); i++)
            {
                this.functions.Get(i).identifier = i;
            }
        }

        public IScopable GetParentScope()
        {
            return null;
        }
        public ScopeManager scopeManager;
        public ScopeManager GetScopeManager()
        {
            return this.scopeManager;
        }

        public List<Definition_Class> getClasses()
        {
            return this.classes.ToList();
        }

        public List<Definition_Function> getFunctions()
        {
            return this.functions.ToList();
        }

        public List<LType> getParameters()
        {
            return new List<LType>(new LType[] { LString.type });
        }

        public List<Declaration_Var> getVars()
        {
            /*
            LType listOfStringsType = LList.CreateLListType();
            listOfStringsType.wildTypes[0].statedType = LString.type;
            ParametersList parametersOfMain = new ParametersList(new List<Parameter>());
            String paramsName = "params";
            parametersOfMain.Add(new Parameter(listOfStringsType, paramsName));
            Definition_Function mainFunc = this.scopeManager.getFunctionForGivenNameAndParameters("main", parametersOfMain);
            if (mainFunc != null)
            {
                paramsName = mainFunc.parameters.Get(0).name;
            }

            List<Declaration_Var> vars = this.vars.ToList();
            vars.Insert(0, new Declaration_Var(0, listOfStringsType, paramsName));
            return vars;
             */
            return this.vars.ToList();
        }
        public List<LAction> getActions()
        {
            return this.actions;
        }
       
        public Definition_Class getClassForType(LType type)
        {
            foreach (Definition_Class aClass in this.classes.ToList())
            {
                if (aClass.type.MatchesWith(type))
                {
                    return aClass;
                }
            }
            return null;
        }

        public int getIndexOfFunctionWithNameAndParameters(string name, ParametersList parameters)
        {
            for(int i = 0; i < this.functions.Count(); i++)
            {
                Definition_Function fun = this.functions.Get(i);
                if(fun.matchesWithNameAndParameters(name, parameters))
                {
                    return i;
                }
            }
            return -1;
        }

        public void generateCode(LeuterperCompiler compiler)
        {
            compiler.addMI(new Number(this.classes.Count()));

            result += String.Format("%d\n", this.classes.Count());
            for(int i = 0; i < this.classes.Count(); i++)
            {
                Definition_Class aClass = this.classes.Get(i);
                result += aClass.generateCode();
            }

            result += String.Format("%d\n", this.vars.Count());
            for (int i = 0; i < this.vars.Count(); i++ )
            {
                Declaration_Var aVar = this.vars.Get(i);
                result += aVar.generateCode();
            }

                result += String.Format("%d\n", this.functions.Count());
            for(int i = 0; i < this.functions.Count(); i++)
            {
                Definition_Function aFunction = this.functions.Get(i);
                result += aFunction.generateCode();
            }

            result += String.Format("%d\n", this.actions.Count());
            for(int i = 0; i < this.actions.Count(); i++)
            {
                LAction anAction = this.actions[i];
                result += anAction.generateCode();
            }

            return result;
        }
    }
}
