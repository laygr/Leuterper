using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    class Parameter : Declaration, IRedefinable<Parameter>
    {
        public Parameter(int line, LType type, String name) : base(line, type, name) { }

        public Parameter redefineWithSubstitutionTypes(List<LType> instantiatedTypes)
        {
            return new Parameter
                (
                    this.getLine(),
                    this.getType().substituteTypeAndVariableTypesWith(instantiatedTypes),
                    this.getName()
                );
        }
    }
}
