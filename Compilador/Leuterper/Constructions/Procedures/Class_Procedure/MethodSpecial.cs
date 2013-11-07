using System;
using System.Collections.Generic;

namespace Leuterper.Constructions
{
    class MethodSpecial : Method
    {
        public MethodSpecial(int line, LType type, String name, List<Parameter> parameters, List<IAction> actions, int identifier)
            : base(line, type, name, parameters, actions)
        {
            this.identifier = identifier;
        }
        public override void codeGenerationPass(LeuterperCompiler compiler) { }
    }
}
