using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    class Loop_Do_While : Conditional
    {
        public Loop_Do_While(int line, Expression booleanExpression, List<LAction> thenActions) 
            : base(line, booleanExpression, thenActions)
        { }
    }
}
