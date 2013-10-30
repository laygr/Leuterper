using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    class Attribute : Class_Declaration
    {
        public Attribute(int line, LType type, String name) : base(line, type, name) { }

        public Attribute reinstantiateWithSubstitution(List<LType> instantiatedTypes)
        {
            return new Attribute(this.getLine(), this.getType().reinstantiateWithSubstitution(instantiatedTypes), this.getName());
        }
    }
}
