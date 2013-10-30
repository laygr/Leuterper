using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    class Return_From_Block : Construction, IAction
    {
        Expression returningExpression;

        public Return_From_Block(int line, Expression returningExpression) : base(line)
        {
            this.returningExpression = returningExpression;
        }

        public override void secondPass(LeuterperCompiler compiler)
        {
            this.returningExpression.shouldBePushedToStack = true;
            this.returningExpression.setScope(this.getScope());
            this.returningExpression.secondPass(compiler);
        }
        public override void thirdPass(){ }

        public override void generateCode(LeuterperCompiler compiler)
        {
            returningExpression.generateCode(compiler);
            compiler.addAction(new MachineInstructions.Rtn());
        }
    }
}
