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

        public override void secondPass()
        {
            foreach(LAction a in elseActions)
            {
                a.scope = this.scope;
                a.secondPass();
            }
        }

        public override void generateCode(LeuterperCompiler compiler)
        {
            this.booleanExpression.generateCode(compiler);
            MachineInstructions.JMPF jumpToElse = new MachineInstructions.JMPF();
            compiler.addAction(jumpToElse);
            foreach(LAction action in this.thenActions)
            {
                action.generateCode(compiler);
            }
            jumpToElse.whereToJump = compiler.getIndexOfNextActionInCurrentFunction() + 1;

            MachineInstructions.JMPF endOfThenJMP = new MachineInstructions.JMPF();
            compiler.addAction(endOfThenJMP);

            foreach (LAction action in this.elseActions)
            {
                action.generateCode(compiler);
            }
            endOfThenJMP.whereToJump = compiler.getIndexOfNextActionInCurrentFunction();
        }
    }
}
