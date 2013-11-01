using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    class Loop_Do_While : Conditional
    {
        public Loop_Do_While(int line, Expression booleanExpression, List<IAction> thenActions) 
            : base(line, booleanExpression, thenActions) { }
        public override void generateCode(LeuterperCompiler compiler)
        {
            int beginningOfLoop = compiler.getIndexOfNextActionInCurrentFunction();

            this.thenActions.ForEach(a => a.generateCode(compiler));
            booleanExpression.generateCode(compiler);
            compiler.addAction(new MachineInstructions.JMPT(beginningOfLoop));
        }
    }
}
