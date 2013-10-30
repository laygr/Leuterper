using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    class Method : Class_Procedure
    {
        public Method(int line, LType type, String name, List<Parameter> parameters, List<IAction> actions)
            : base(line, type, name, parameters, actions)
        {
        }

        public Method reinstantiateWithSubstitution(List<LType> instantiatedTypes)
        {
            return new Method(this.getLine(), this.getType().reinstantiateWithSubstitution(instantiatedTypes), name, parameters, actions);
        }
    }
}
