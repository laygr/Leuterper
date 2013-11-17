using System.Collections.Generic;
using System.Linq;

namespace Leuterper.Constructions
{
    class Program : Construction, IScope, ICompilable
    {
        //Definitions
        public List<LClass> classes { get; set; }
        public List<Function> functions { get; set; }
        public List<IAction> actions { get; set; }
        public List<Declaration> vars { get; set; }

        public List<Construction> children;
        public Program(List<LClass> classes, List<Function>functions, List<IAction>actions) : base(0)
        {
            this.classes = classes;
            this.functions = functions;
            this.actions = actions;

            this.vars = new List<Declaration>();

            foreach(LClass c in StandardLibrary.singleton.standardClasses)
            {
                this.classes.Insert(0, c);
            }
            foreach(Function f in StandardLibrary.singleton.standardFunctions)
            {
                this.functions.Insert(0, f);
            }
            foreach (IAction a in actions)
            {
                if (a is Var)
                {
                    this.vars.Add(a as Var);

                }
            }
            this.children = new List<Construction>();

            int assignations = 0;
            foreach (Var v in this.vars)
            {
                if (v.initialValue != null)
                {
                    VarAccess var = new VarAccess(v.getLine(), v.getName());
                    this.actions.Insert(assignations, new Assignment(v.getLine(), var, v.initialValue));
                    assignations++;
                }
            }
            this.scopeSetting();
        }
        public Function getFunctionForGivenNameAndTypes(string name, List<LType> parametersTypes)
        {
            foreach (Function f in this.functions)
            {
                if (f.isCompatibleWithNameAndTypes(name, parametersTypes)) return f;
            }
            return null;
        }
        public int getIndexOfProcedureWithNameAndParametersTypes(string name, List<LType> parametersTypes)
        {
            Procedure fun = this.getFunctionForGivenNameAndTypes(name, parametersTypes);
            if (fun != null) return fun.identifier;
            return -1;
        }

        public override void scopeSetting()
        {
            this.classes.ForEach(c => this.addChild(c));
            this.vars.ForEach(v => this.addChild(v));
            this.functions.ForEach(f => this.addChild(f));
            this.actions.ForEach(a => this.addChild(a as Construction));
        }

        public override void symbolsRegistration(LeuterperCompiler compiler)
        {
            this.classes.ForEach(c => c.symbolsRegistration(compiler));
            this.vars.ForEach(v => v.symbolsRegistration(compiler));
            this.functions.ForEach(f => f.symbolsRegistration(compiler));
        }

        public override void symbolsUnificationPass()
        {
            //Crear acciones de asignacion de las declaraciones de variables con valor inicial.

            this.classes.ForEach(c => c.symbolsUnificationPass());
            this.vars.ForEach(v => v.symbolsUnificationPass());
            this.functions.ForEach(f => f.symbolsUnificationPass());
            this.actions.ForEach(a => a.symbolsUnificationPass());
        }
        public override void classesGenerationPass()
        {
            this.classes.ForEach(c => c.classesGenerationPass());
            this.functions.ForEach(f => f.classesGenerationPass());
        }
        public override void simplificationAndValidationPass()
        {
            this.classes.ForEach(c => c.simplificationAndValidationPass());
        }
        public override void codeGenerationPass(LeuterperCompiler compiler)
        {
            compiler.globalVariablesCounter = this.vars.Count();
            this.classes.ForEach(c => c.codeGenerationPass(compiler));
            this.functions.ForEach(f => f.codeGenerationPass(compiler));
            compiler.compilingTopLeveIActions = true;
            this.actions.ForEach(a => a.codeGenerationPass(compiler));
        }
        public List<LClass> getClasses()
        {
            return this.classes;
        }
        public List<Declaration> getDeclarations()
        {
            return this.vars;
        }

        public List<Construction> getChildren()
        {
            return this.children;
        }

        public void addChild(Construction c)
        {
            this.children.Add(c);
            c.setScope(this);
        }
    }
}
