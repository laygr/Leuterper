using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    class Definition_Method : Definition_Function, IIdentifiable<Definition_Method>
    {
        public Definition_Class aClass
        {
            get;
            set;
        }

        public Definition_Method(int line, LType type, String name, List<Parameter> parameters, List<LAction> actions)
            : base(line, type, name, parameters, actions)
        {
        }

        public override void secondPass()
        {
            this.parameters.Insert(0, new Parameter(this.aClass.type, "this"));
            base.secondPass();
        }

        public bool HasSameSignatureAs(Definition_Method otherElement)
        {
            return this.HasSameSignatureAs(otherElement as Definition_Function);
        }
    }
}
