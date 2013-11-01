using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    class LAttribute : Declaration, IRedefinable<LAttribute>
    {
        public LAttribute(int line, LType type, String name) : base(line, type, name) { }

        public override void secondPass(LeuterperCompiler compiler)
        {
            base.secondPass(compiler);
            this.getType().setShouldStartRedefinition(true);
        }

        public LAttribute redefineWithSubstitutionTypes(List<LType> instantiatedTypes)
        {
            return new LAttribute(this.getLine(), this.getType().substituteTypeAndVariableTypesWith(instantiatedTypes), this.getName());
        }
    }
}
