using Leuterper.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    abstract class Procedure : Construction, IDefinition, IScope, IIdentifiable<Function>
    {
        private LType type;
        private ScopeManager scopeManager;
        public int identifier = -1;
        public String name { get; set; }
        public List<Parameter> parameters { get; set; }
        public List<Declaration_Var> vars { get; set; }
        public List<IAction> actions { get; set; }

        public Procedure(int line, LType type, String id, List<Parameter> parameters, List<IAction> actions)
            : base(line)
        {
            if (actions == null)
            {
                actions = new List<IAction>();
            }
            this.name = id;
            this.parameters = parameters;
            this.scopeManager = new ScopeManager(this);

            //Split actions in vars an actions.
            this.actions = new List<IAction>();
            this.vars = new List<Declaration_Var>();

            foreach(IAction a in actions)
            {
                if (a is Declaration_Var)
                {
                    this.vars.Add(a as Declaration_Var);
                }
                else
                {
                    this.actions.Add(a);
                }
            }
        }
        public void checkDeclarationsBeforeActions()
        {
            bool notDeclarationAppeared = false;
            foreach(IAction a in this.actions)
            {
                if(a is Declaration_Var && notDeclarationAppeared)
                {
                    throw new SemanticErrorException("Declaration not placed at the beginning of a procedure.", a.getLine());
                }
                else if(!(a is Declaration_Var))
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
                foreach (Declaration_Var v in this.vars)
                {
                    if (names.ContainsValue(v.getName()))
                    {
                        throw new SemanticErrorException("Variable redeclared: " + v.getName(), v.getLine());
                    }
                    names.Add(v.getName(), v.getName());
                }
            }
        }

        public bool HasSameSignatureAs(Function otherElement)
        {
            if (!this.getName().Equals(name)) return false;
            return LType.listOfTypesMatch(
                Parameter.listOfParametersAsListOfTypes(this.parameters),
                Parameter.listOfParametersAsListOfTypes(otherElement.parameters)
                );
        }

        virtual public bool isCompatibleWithNameAndTypes(string name, List<LType> types)
        {
            if (!this.getName().Equals(name)) return false;
            return LType.listOfTypesUnify(Parameter.listOfParametersAsListOfTypes(this.parameters), types);
        }

        public override void secondPass(LeuterperCompiler compiler)
        {

            compiler.assignIdentifierToProcedure(this);

            //Parameters
            for (int i = 0; i < this.parameters.Count(); i++)
            {
                Parameter p = this.parameters[i];
                p.setScope(this);
                p.secondPass(compiler);
            }

            /*
            //Insert Parameters as vars.
            for (int i = 0; i < this.parameters.Count(); i++)
            {
                Parameter p = this.parameters[i];
                this.vars.Insert(i, new Declaration_Var(this.getLine(), p.getType(), p.getName()));
            }
             */
            
            //Var declarations
            foreach(Declaration_Var v in this.vars)
            {
                v.setScope(this);
                v.secondPass(compiler);
            }

            //Crear acciones de asignacion de las declaraciones de variables con valor inicial.
            int assignations = 0;
            foreach (Declaration_Var v in this.vars)
            {
                if (v.initialValue != null)
                {
                    VarAccess var = new VarAccess(v.getLine(), v.getName());
                    this.actions.Insert(assignations, new Assignment(v.getLine(), var, v.initialValue));
                    assignations++;
                }
            }

            //actions
            foreach(IAction a in this.actions)
            {
                a.setScope(this);
                a.secondPass(compiler);
            }
        }

        public override void thirdPass()
        {
            foreach(Declaration_Var v in this.vars)
            {
                v.thirdPass();
            }
        }

        public override void generateCode(LeuterperCompiler compiler)
        {
            if(compiler.mostVaribalesInAFunction < this.parameters.Count() + this.vars.Count())
            {
                compiler.mostVaribalesInAFunction = this.parameters.Count() + this.vars.Count();
            }
            compiler.functionsParameters.Add(this.parameters.Count());
            compiler.functionActions.Add(new List<MachineInstructions.MachineInstruction>());
            foreach(IAction action in this.actions)
            {
                action.generateCode(compiler);
            }
        }
        public override string ToString()
        {
            return String.Format("{0} {1} {2} {3}", this.identifier, this.getType().SignatureAsString(), this.getName(), Parameter.listOfParametersAsString(this.parameters));
        }

        public LType getType()
        {
            return this.type;
        }

        public void setType(LType type)
        {
            this.type = type; 
        }

        public string getName()
        {
            return this.name;
        }

        public List<IDeclaration> getDeclarations()
        {
            List<IDeclaration> result = new List<IDeclaration>();
            this.parameters.ForEach(p => result.Add(p));
            this.vars.ForEach(v => result.Add(v));
            return result;
        }

        public ScopeManager getScopeManager()
        {
            return this.scopeManager;
        }
    }
}