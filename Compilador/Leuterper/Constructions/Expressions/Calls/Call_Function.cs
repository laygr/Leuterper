using Leuterper.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    class Call_Function : Call_Procedure
    {
        public Call_Function(int line, String procedureName, List<Expression> arguments)
            : base(line, procedureName, arguments)
        { }

        public override Procedure getProcedureDefinition()
        {
            return this.getScope().getScopeManager().getFunctionForGivenNameAndArguments(this.procedureName, this.arguments);
        }
    }
}
