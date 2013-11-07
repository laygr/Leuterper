using Leuterper.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Leuterper.Constructions
{
    abstract class Procedure : Declaration, IDefinition, IScope
    {
        public int identifier = -1;
        public List<Parameter> parameters { get; set; }
        public List<Var> vars { get; set; }
        public List<IAction> actions { get; set; }
        public List<Construction> children;

        public Procedure(int line, LType type, String name, List<Parameter> parameters, List<IAction> actions)
            : base(line, type, name)
        {
            this.parameters = parameters;
            this.children = new List<Construction>();

            if (actions == null)
            {
                actions = new List<IAction>();
            }

            //Split actions in vars an actions.
            this.actions = new List<IAction>();
            this.vars = new List<Var>();

            foreach(IAction a in actions)
            {
                if (a is Var)
                {
                    this.vars.Add(a as Var);
                }
                else
                {
                    this.actions.Add(a);
                }
            }
            this.scopeSetting();
        }
        public void checkDeclarationsBeforeActions()
        {
            bool notDeclarationAppeared = false;
            foreach(IAction a in this.actions)
            {
                if(a is Var && notDeclarationAppeared)
                {
                    throw new SemanticErrorException("Declaration not placed at the beginning of a procedure.", a.getLine());
                }
                else if(!(a is Var))
                {
                    notDeclarationAppeared = true;
                }
            }
        }
        public void checkThatVariablesHaveUniqueNames()
        {
            if (this.getLine() != 0)
            {
                SortedList<string, string> names = new SortedList<string, string>();
                foreach (Var v in this.vars)
                {
                    if (names.ContainsValue(v.getName()))
                    {
                        throw new SemanticErrorException("Variable redeclared: " + v.getName(), v.getLine());
                    }
                    names.Add(v.getName(), v.getName());
                }
            }
        }

        public override bool HasSameSignatureAs(IDeclaration otherElement)
        {
            if (!this.getName().Equals(otherElement.getName())) return false;
            return LType.listOfTypesMatch(
                UtilFunctions.listOfParametersAsListOfTypes(this.parameters),
                UtilFunctions.listOfParametersAsListOfTypes((otherElement as Procedure).parameters)
                );
        }

        virtual public bool isCompatibleWithNameAndTypes(string name, List<LType> types)
        {
            if (!this.getName().Equals(name)) return false;
            return LType.listOfTypesUnify(UtilFunctions.listOfParametersAsListOfTypes(this.parameters), types);
        }
        public override void scopeSetting()
        {
            this.addChild(this.getType());
            this.parameters.ForEach(p => this.addChild(p));
            this.vars.ForEach(v => this.addChild(v));
            this.actions.ForEach(a => this.addChild(a as Construction));
        }

        public override void symbolsRegistration(LeuterperCompiler compiler)
        {
            compiler.assignIdentifierToProcedure(this);
        }

        public override void symbolsUnificationPass()
        {
            base.symbolsUnificationPass();

            this.parameters.ForEach(p => p.symbolsUnificationPass());
            this.vars.ForEach(v => v.symbolsUnificationPass());

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
            this.scopeSetting();
        }

        public override void classesGenerationPass()
        {
            this.vars.ForEach(v => v.classesGenerationPass());
        }

        public override void codeGenerationPass(LeuterperCompiler compiler)
        {
            if(compiler.mostVaribalesInAFunction < this.parameters.Count() + this.vars.Count())
            {
                compiler.mostVaribalesInAFunction = this.parameters.Count() + this.vars.Count();
            }
            compiler.functionsParameters.Add(this.parameters.Count());
            compiler.functionActions.Add(new List<MachineInstructions.MachineInstruction>());
            this.actions.ForEach(a => a.codeGenerationPass(compiler));
        }
        public override string ToString()
        {
            return String.Format("{0} {1} {2} {3}", this.identifier, this.getType().SignatureAsString(), this.getName(), UtilFunctions.listOfParametersAsString(this.parameters));
        }

        public List<Declaration> getDeclarations()
        {
            List<Declaration> result = new List<Declaration>();
            this.parameters.ForEach(p => result.Add(p));
            this.vars.ForEach(v => result.Add(v));
            return result;
        }

        public static List<IAction> reinstantiateActions(List<IAction> actions, List<LType> instantiatedTypes)
        {
         List<IAction> newActions = new List<IAction>();
            foreach(IAction a in actions)
            {
                if(a is Var)
                {
                    newActions.Add((a as Var).redefineWithSubstitutionTypes(instantiatedTypes));
                }
                else
                {
                    newActions.Add(a);
                }
            }
            return newActions;
        }
        public static List<Parameter> reinstantiateParameters(List<Parameter> parameters, List<LType> instantiatedTypes)
        {
            List<Parameter> newParameters = new List<Parameter>();
            foreach(Parameter p in parameters)
            {
                newParameters.Add(p.redefineWithSubstitutionTypes(instantiatedTypes));
            }
            return newParameters;
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