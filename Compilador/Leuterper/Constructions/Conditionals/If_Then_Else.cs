using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    class If_Then_Else : Conditional
    {
        List<IAction> elseActions;

        public If_Then_Else(int line, Expression booleanExpression, List<IAction> thenActions, List<IAction> elseActions)
            : base(line, booleanExpression, thenActions)
        {
            this.elseActions = elseActions;
            if(this.elseActions == null)
            {
                this.elseActions = new List<IAction>();
            }
            
        }

        public override void secondPass(LeuterperCompiler compiler)
        {
            foreach(IAction a in elseActions)
            {
                a.setScope(this.getScope());
                a.secondPass(compiler);
            }
        }

        public override void thirdPass()
        {
        }

        public override void generateCode(LeuterperCompiler compiler)
        {
            this.booleanExpression.generateCode(compiler);
            MachineInstructions.JMPF jumpToElse = new MachineInstructions.JMPF();
            compiler.addAction(jumpToElse);
            foreach(IAction action in this.thenActions)
            {
                action.generateCode(compiler);
            }
            jumpToElse.whereToJump = compiler.getIndexOfNextActionInCurrentFunction() + 1;

            MachineInstructions.JMPF endOfThenJMP = new MachineInstructions.JMPF();
            compiler.addAction(endOfThenJMP);

            foreach (IAction action in this.elseActions)
            {
                action.generateCode(compiler);
            }
            endOfThenJMP.whereToJump = compiler.getIndexOfNextActionInCurrentFunction();
        }
    }
}
