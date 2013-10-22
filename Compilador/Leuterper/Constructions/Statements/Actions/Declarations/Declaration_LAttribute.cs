using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    class Declaration_LAttribute : Declaration_Var, IIdentifiable<Declaration_LAttribute>
    {
        public Definition_Class aClass { get; set; }

        public Declaration_LAttribute(int line, LType type, String id, Expression initialValue)
            : base(line, type, id, initialValue) { }

        public Declaration_LAttribute(int line, LType type, String id) : this(line, type, id, null) { }


        public bool HasSameSignatureAs(Declaration_LAttribute otherElement)
        {
            return this.HasSameSignatureAs(otherElement as Declaration_Var);
        }
    }
}
