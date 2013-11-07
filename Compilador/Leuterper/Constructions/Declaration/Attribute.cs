using System;
using System.Collections.Generic;

namespace Leuterper.Constructions
{
    class LAttribute : Declaration, IRedefinable<LAttribute>
    {
        public LAttribute(int line, LType type, String name) : base(line, type, name)
        {
        }

        public override void scopeSetting()
        {
            base.scopeSetting();
            this.getType().setShouldStartRedefinition(true);
        }

        public override void symbolsRegistration(LeuterperCompiler compiler) { }

        public override void symbolsUnificationPass()
        {
            base.symbolsUnificationPass();
        }

        public LAttribute redefineWithSubstitutionTypes(List<LType> instantiatedTypes)
        {
            return new LAttribute(this.getLine(), this.getType().substituteTypeAndVariableTypesWith(instantiatedTypes), this.getName());
        }
    }
}
