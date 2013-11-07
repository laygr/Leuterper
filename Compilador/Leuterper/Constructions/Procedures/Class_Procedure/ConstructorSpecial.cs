using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    class ConstructorSpecial : Constructor
    {
        public ConstructorSpecial(int line, string name, List<Parameter> parameters, List<Expression> baseCallArguments, List<IAction> actions, int identifier)
            : base(line, name, parameters, baseCallArguments, new List<IAction>())
        {
            this.identifier = identifier;
        }

        public override void codeGenerationPass(LeuterperCompiler compiler) { }
    }
}
