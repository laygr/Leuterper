using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leuterper.MachineInstructions;
using Leuterper.Exceptions;

namespace Leuterper.Constructions
{
    class Program : Construction, IScope, ICompilable
    {
        //Definitions
        public List<LClass> classes { get; set; }
        public List<Function> functions { get; set; }
        public List<IAction> actions { get; set; }
        public List<Var> vars { get; set; }
        public Program(List<LClass> classes, List<Function>functions, List<IAction>actions) : base(0)
        {
            this.classes = classes;
            this.functions = functions;
            this.actions = actions;

            this.vars = new List<Var>();

            foreach(LClass c in StandardLibrary.specialClasses)
            {
                this.classes.Insert(0, c);
            }
            foreach(Function f in StandardLibrary.standardFunctions)
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

        public override void secondPass(LeuterperCompiler compiler)
        {
            foreach(LClass c in this.classes)
            {
                c.setScope(this);
                c.secondPass(compiler);
            }
            foreach(Var v in this.vars)
            {
                v.setScope(this);
                v.secondPass(compiler);
            }
            foreach(Function f in this.functions)
            {
                f.setScope(this);
                f.secondPass(compiler);
            }

            //Crear acciones de asignacion de las declaraciones de variables con valor inicial.
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

            foreach (IAction a in this.actions)
            {
                a.setScope(this);
                a.secondPass(compiler);
            }
        }
        public override void thirdPass()
        {
            this.classes.ForEach(c => c.thirdPass());
        }
        public override void generateCode(LeuterperCompiler compiler)
        {
            compiler.globalVariablesCounter = this.vars.Count();
            this.classes.ForEach(c => c.generateCode(compiler));
            this.functions.ForEach(f => f.generateCode(compiler));
            compiler.compilingTopLeveIActions = true;
            this.actions.ForEach(a => a.generateCode(compiler));
        }
        public List<LClass> getClasses()
        {
            return this.classes;
        }
        public List<IDeclaration> getDeclarations()
        {
            List<IDeclaration> result = new List<IDeclaration>();
            this.vars.ForEach(v => result.Add(v));
            return result;
        }
        
    }
}
