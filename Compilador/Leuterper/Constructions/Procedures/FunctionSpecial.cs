using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    class FunctionSpecial : Function
    {
        public FunctionSpecial(int line, LType type, String id, List<Parameter> parameters, List<IAction> actions, int identifier)
             : base(line, type, id, parameters, actions)
        {
            this.identifier = identifier;
        }

        public override void generateCode(LeuterperCompiler compiler)
        { }
    }
}
