using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    class Declaration_Var : Declaration, IIdentifiable<Declaration_Var>
    {
        public String name { get; set; }
        public Expression initialValue { get; set; }
        public Declaration_Var(int line, LType type, String name, Expression initialValue)
            : base(line, type)
        {
            this.name = name;
            this.initialValue = initialValue;
        }

        public Declaration_Var(int line, LType type, String id)
            : this(line, type, id, null)
        { }

        public Boolean HasSameSignatureAs(Declaration_Var otherElement)
        {
            return this.name.Equals(otherElement.name);
        }

        public String SignatureAsString()
        {
            return String.Format("{0} {1}", this.type.SignatureAsString(), this.name);
        }
        
        override public void generateCode(LeuterperCompiler compiler)
        {
            return;
        }
    }
}
