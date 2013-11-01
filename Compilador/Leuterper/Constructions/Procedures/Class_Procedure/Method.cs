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
            : base(line, type, name, parameters, actions) { }

        public Method redefineWithSubstitutionTypes(List<LType> instantiatedTypes)
        {
            Method result = new Method(
                this.getLine(),
                this.getType().substituteTypeAndVariableTypesWith(instantiatedTypes),
                this.getName(),
                Procedure.reinstantiateParameters(this.parameters, instantiatedTypes),
                Procedure.reinstantiateActions(this.actions, instantiatedTypes));
            result.identifier = this.identifier;
            return result;
        }
    }
}
