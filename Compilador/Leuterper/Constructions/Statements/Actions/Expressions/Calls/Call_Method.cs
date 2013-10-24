using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    class Call_Method : Call_Function
    {
        public Expression theObject { get; set; }

        public Call_Method(int line, Expression theObject, String methodId, List<Expression> arguments)
            : base(line, methodId, arguments)
        {
            this.theObject = theObject;
        }

        public override void secondPass()
        {
            this.arguments.Insert(0, theObject);
            base.secondPass();
        }

    }
}
