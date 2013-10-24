using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    class Definition_Function : Definition, IIdentifiable<Definition_Function>, IScopable
    {
        public int identifier { get; set; }
        public String name { get; set; }
        public List<Parameter> parameters { get; set; }
        public List<Declaration_Var> vars { get; set; }
        public List<LAction> actions { get; set; }
        public ScopeManager scopeManager;
        public Definition_Function(int line, LType type, String id, List<Parameter> parameters, List<LAction> actions, int identifier) : this(line, type, id, parameters, actions)
        {
            this.identifier = identifier;
        }
        public Definition_Function(int line, LType type, String id, List<Parameter> parameters, List<LAction> actions)
            : base(line, type)
        {
            if (actions == null)
            {
                actions = new List<LAction>();
            }
            this.name = id;
            this.parameters = parameters;
            
            actions.ForEach(a => a.function = this);

            this.actions = new List<LAction>();
            this.actions.AddRange(actions.FindAll(a => !(a is Declaration_Var)));

            this.vars = new List<Declaration_Var>();
            foreach(LAction a in actions)
            {
                if (a is Declaration_Var) this.vars.Add((Declaration_Var) a );
            }
            this.scopeManager = new ScopeManager(this);
        }

        public bool HasSameSignatureAs(Definition_Function otherElement)
        {
            if (!this.name.Equals(otherElement.name)) return false;
            return this.listOfParametersMatchesWith(otherElement.parameters);
        }

        private bool listOfParametersMatchesWith(List<Parameter> other)
        {
            if(this.parameters.Count() != other.Count()) return false;
            for(int i = 0; i < this.parameters.Count(); i++)
            {
                if(!this.parameters[i].type.MatchesWith(other[i].type)) return false;
            }
            return true;
        }

        public string SignatureAsString()
        {
            return String.Format("{0} {1} {2}", this.type.SignatureAsString(), this.name, Parameter.listOfParametersAsString(this.parameters));
        }

        public bool matchesWithNameAndTypes(string name, List<LType> types)
        {
            if (!this.name.Equals(name)) return false;
            return LType.listOfTypesMatch(Parameter.listOfParametersAsListOfTypes(this.parameters), types);
        }

        public override void secondPass()
        {
            for(int i = 0; i < this.parameters.Count(); i++)
            {
                Parameter p = this.parameters[i];
                this.vars.Insert(i, new Declaration_Var(this.line, p.type, p.name));
            }

            for(int i = 0; i < this.parameters.Count(); i++)
            {
                Parameter p = this.parameters[i];
                p.scope = this;
                p.secondPass();
            }
            foreach(Declaration_Var v in this.vars)
            {
                v.scope = this;
                v.secondPass();
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

            foreach(LAction a in this.actions)
            {
                a.scope = this;
                a.secondPass();
            }
        }

        public override void generateCode(LeuterperCompiler compiler)
        {
            if(compiler.mostVaribalesInAFunction < this.vars.Count())
            {
                compiler.mostVaribalesInAFunction = this.vars.Count();
            }
            compiler.functionsParameters.Add(this.parameters.Count());
            compiler.functionActions.Add(new List<MachineInstructions.MachineInstruction>());
            foreach(LAction action in this.actions)
            {
                action.generateCode(compiler);
            }
        }

        public IScopable GetParentScope()
        {
            return this.scope;
        }

        public ScopeManager GetScopeManager()
        {
            return this.scopeManager;
        }

        public List<Declaration_Var> getVars()
        {
            return this.vars;
        }

        public List<LAction> getActions()
        {
            return this.actions;
        }

        public Program getProgram()
        {
            return this.scope as Program;
        }
    }
}