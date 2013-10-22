using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    class Return_From_Block : LAction
    {
        Expression returningExpression;

        public Return_From_Block(int line, Expression returningExpression) : base(line)
        {
            this.returningExpression = returningExpression;
        }

        public override void generateCode(LeuterperCompiler compiler)
        {
            returningExpression.generateCode(compiler);
        }
    }
}
