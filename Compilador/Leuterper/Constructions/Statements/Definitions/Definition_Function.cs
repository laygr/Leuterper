using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    class Definition_Function : Definition, IIdentifiable<Definition_Function>
    {
        public int identifier { get; set; }
        public String name { get; set; }
        public ParametersList parameters { get; set; }
        public UniquesList<Declaration_Var> vars { get; set; }
        public List<LAction> actions { get; set; }

        public Definition_Function(int line, LType type, String id, List<Parameter> parameters, List<LAction> actions)
            : base(line, type)
        {
            this.name = id;
            this.parameters = new ParametersList(parameters);
            
            actions.ForEach(a => a.function = this);

            this.actions = new List<LAction>();
            this.actions.AddRange(actions.FindAll(a => !(a is Declaration_Var)));

            this.vars = new UniquesList<Declaration_Var>();
            foreach(LAction a in actions)
            {
                if (a is Declaration_Var) this.vars.Add((Declaration_Var) a );
            }
        }

        public bool HasSameSignatureAs(Definition_Function otherElement)
        {
            if (!this.name.Equals(otherElement.name)) return false;
            return this.parameters.HasSameSignatureAs(otherElement.parameters);
        }

        public string SignatureAsString()
        {
            return String.Format("{0} {1} {2}", this.type.SignatureAsString(), this.name, this.parameters.SignatureAsString());
        }

        public bool matchesWithNameAndParameters(string name, ParametersList parameters)
        {
            if (!this.name.Equals(name)) return false;
            return this.parameters.HasSameSignatureAs(parameters);
        }

        public String generateCode()
        {
            String result = "";

            result += this.parameters.Count();

        }
    }
}
