using System;
using System.Collections.Generic;

namespace Leuterper.Constructions
{
    class Parameter : Declaration, IRedefinable<Parameter>
    {
        public Parameter(int line, LType type, String name)
            : base(line, type, name)
        {
        }

        public override void symbolsRegistration(LeuterperCompiler compiler) { }
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
