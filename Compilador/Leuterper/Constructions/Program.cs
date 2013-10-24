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

        static List<LType> e = new List<LType>();
        static List<LType> o = new List<LType>(new LType[] { LObject.type });
        static List<LType> oo = new List<LType>(new LType[] { LObject.type, LObject.type });
        static List<LType> n = new List<LType>(new LType[] { LNumber.type });
        static List<LType> nn = new List<LType>(new LType[] { LNumber.type, LNumber.type });
        static List<LType> l = new List<LType>(new LType[] { LList.type });
        static List<LType> ll = new List<LType>(new LType[] { LList.type, LList.type });
        static List<LType> la = new List<LType>(new LType[] { LList.type, Wild_Type.wildTypePlaceHolder() });
        static List<LType> ln = new List<LType>(new LType[] { LList.type, LNumber.type });
        static List<LType> lan = new List<LType>(new LType[] { LList.type, Wild_Type.wildTypePlaceHolder(), LNumber.type });
        static List<LType> c = new List<LType>(new LType[] { LChar.type });
        static List<LType> cc = new List<LType>(new LType[] { LChar.type, LChar.type });
        static List<LType> s = new List<LType>(new LType[] { LString.type });
        static List<LType> ss = new List<LType>(new LType[] { LString.type, LString.type });

        public static List<Definition_Function> specialFunctions = new List<Definition_Function>(new Definition_Function[]{
            new Definition_Function(0, LBoolean.type, "==", Parameter.typesToParameter(oo), null, 0),
            new Definition_Function(0, LString.type, "toString", Parameter.typesToParameter(oo), null, 1),
            new Definition_Function(0, LBoolean.type, "==", Parameter.typesToParameter(nn), null, 2),
            new Definition_Function(0, LString.type, "toString", Parameter.typesToParameter(n), null, 3),
            new Definition_Function(0, LNumber.type, "+", Parameter.typesToParameter(nn), null, 4),
            new Definition_Function(0, LNumber.type, "-", Parameter.typesToParameter(o), null, 5),
            new Definition_Function(0, LNumber.type, "*", Parameter.typesToParameter(o), null, 6),
            new Definition_Function(0, LNumber.type, "/", Parameter.typesToParameter(o), null, 7),
            new Definition_Function(0, LBoolean.type, "==", Parameter.typesToParameter(ll), null, 8),
            new Definition_Function(0, LString.type, "toString", Parameter.typesToParameter(l), null, 9),
            new Definition_Function(0, LVoid.type, "add", Parameter.typesToParameter(la), null, 10),
            new Definition_Function(0, LNumber.type, "count", Parameter.typesToParameter(l), null, 11),
            new Definition_Function(0, Wild_Type.wildTypePlaceHolder(), "get", Parameter.typesToParameter(ln), null, 12),
            new Definition_Function(0, LVoid.type, "set", Parameter.typesToParameter(lan), null, 13),
            new Definition_Function(0, LBoolean.type, "==", Parameter.typesToParameter(cc), null, 14),
            new Definition_Function(0, LString.type, "toString", Parameter.typesToParameter(c), null, 15),
            new Definition_Function(0, LString.type, "==", Parameter.typesToParameter(ss), null, 16),
            new Definition_Function(0, LString.type, "read", Parameter.typesToParameter(e), null, 17),
            new Definition_Function(0, LVoid.type, "write", Parameter.typesToParameter(s), null, 18),
            new Definition_Function(0, LNumber.type, "stringToNumber", Parameter.typesToParameter(s), null, 19)});

        public Definition_Function getFunctionWithNameAndParametersTypes(string name, List<LType> parametersTypes)
        {
            for (int i = 0; i < Program.specialFunctions.Count(); i++)
            {
                Definition_Function fun = Program.specialFunctions[i];
                if (fun.matchesWithNameAndTypes(name, parametersTypes))
                {
                    return fun;
                }
            }

            for (int i = 0; i < this.functions.Count(); i++)
            {
                Definition_Function fun = this.functions[i];
                if (fun.matchesWithNameAndTypes(name, parametersTypes))
                {
                    return fun;
                }
            }
            return null;
        }
        public int getIndexOfFunctionWithNameAndParametersTypes(string name, List<LType> parametersTypes)
        {
            for (int i = 0; i < Program.specialFunctions.Count(); i++)
            {
                Definition_Function fun = Program.specialFunctions[i];
                if (fun.matchesWithNameAndTypes(name, parametersTypes))
                {
                    return fun.identifier;
                }
            }
            
            for (int i = 0; i < this.functions.Count(); i++)
            {
                Definition_Function fun = this.functions[i];
                if (fun.matchesWithNameAndTypes(name, parametersTypes))
                {
                    return fun.identifier;
                }
            }
            return -1;
        }

        public void secondPass()
        {
            for (int i = 0; i < this.classes.Count(); i++)
            {
                Definition_Class aClass = this.classes.Get(i);
                aClass.scope = this;
                aClass.identifier = i;
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
                aFunc.identifier = i + LeuterperCompiler.STANDARD_FUNCTIONS;
                aFunc.secondPass();
            }

            //Crear acciones de asignacion de las declaraciones de variables con valor inicial.
            int assignations = 0;
            foreach (Declaration_Var v in this.vars)
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

            for (int i = 0; i < this.classes.Count(); i++)
            {
                Definition_Class aClass = this.classes.Get(i);
                aClass.generateCode(compiler);
            }

            for (int i = 0; i < this.functions.Count(); i++)
            {
                Definition_Function aFunction = this.functions[i];
                aFunction.generateCode(compiler);
            }

            compiler.compilingTopLevelActions = true;
            for (int i = 0; i < this.actions.Count(); i++)
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
