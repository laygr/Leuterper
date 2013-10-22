using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    class If_Then_Else : Conditional
    {
        List<LAction> elseActions;

        public If_Then_Else(int line, Expression booleanExpression, List<LAction> thenActions, List<LAction> elseActions)
            : base(line, booleanExpression, thenActions)
        {
            this.elseActions = elseActions;
        }
    }
}
