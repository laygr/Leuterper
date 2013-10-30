using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    abstract class Call_Class_Procedure : Call_Procedure
    {
        public Call_Class_Procedure(int line, String procedureName, List<Expression> arguments)
            : base(line, procedureName, arguments)
        {
        }
    }
}
