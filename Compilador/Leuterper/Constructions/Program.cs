using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leuterper.MachineInstructions;

namespace Leuterper.Constructions
{
    class Program : IScopable, ICompilable
    {
        //Definitions
        public UniquesList<Definition_Class> classes { get; set; }
        public List<Declaration_Var> vars { get; set; }
        public List<Definition_Function> functions { get; set; }

        //Actions
        public List<LAction> actions { get; set; }

        public Program(List<Statement> statements)
        {
            this.classes = new UniquesList<Definition_Class>();
            this.functions = new List<Definition_Function>();
            this.vars = new List<Declaration_Var>();
            this.actions = new List<LAction>();
            this.scopeManager = new ScopeManager(this);

            foreach (Statement s in statements)
            {
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
                    this.functions.Add(aClass.methodsDefinitions[j]);
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
                this.functions[i].identifier = i;
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
                Definition_Function fun = this.functions[i];
                if(fun.matchesWithNameAndParameters(name, parameters))
                {
                    return i;
                }
            }
            return -1;
        }

        public void secondPass()
        {
            for(int i = 0; i < this.classes.Count(); i++)
            {
                Definition_Class aClass = this.classes.Get(i);
                aClass.scope = this;
                aClass.secondPass();
            }
            for (int i = 0; i < this.vars.Count(); i++)
            {
                Declaration_Var var = this.vars[i];
                var.scope = this;
                var.secondPass();
            }
            for (int i = 0; i < this.functions.Count(); i++)
            {
                Definition_Function aFunc = this.functions[i];
                aFunc.scope = this;
                aFunc.identifier = i;
                aFunc.secondPass();
            }

            //Crear acciones de asignacion de las declaraciones de variables con valor inicial.
            int assignations = 0;
            foreach(Declaration_Var v in this.vars)
            {
                if (v.initialValue != null)
                {
                    Var var = new Var(v.line, v.name);
                    this.actions.Insert(assignations, new Assignment(v.line, var, v.initialValue));
                    assignations++;
                }
            }

            foreach (LAction a in this.actions)
            {
                a.scope = this;
                a.secondPass();
            }
        }

        public void generateCode(LeuterperCompiler compiler)
        {
            compiler.globalVariablesCounter = this.vars.Count();

            for(int i = 0; i < this.classes.Count(); i++)
            {
                Definition_Class aClass = this.classes.Get(i);
                aClass.generateCode(compiler);
            }

            for(int i = 0; i < this.functions.Count(); i++)
            {
                Definition_Function aFunction = this.functions[i];
                aFunction.generateCode(compiler);
            }

            compiler.compilingTopLevelActions = true;
            for(int i = 0; i < this.actions.Count(); i++)
            {
                LAction anAction = this.actions[i];
                anAction.generateCode(compiler);
            }

        }


        public Program getProgram()
        {
            return this;
        }
    }
}
