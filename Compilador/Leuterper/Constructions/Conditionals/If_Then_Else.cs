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

        public override void thirdPass() { }

        public override void generateCode(LeuterperCompiler compiler)
        {
            MachineInstructions.JMPF jumpToElse = new MachineInstructions.JMPF();
            MachineInstructions.JMPF endOfThen = new MachineInstructions.JMPF();

            this.booleanExpression.generateCode(compiler);
            compiler.addAction(jumpToElse);
            this.thenActions.ForEach(a => a.generateCode(compiler));
            compiler.addAction(endOfThen);
            jumpToElse.whereToJump = compiler.getIndexOfNextActionInCurrentFunction();
            this.elseActions.ForEach(a => a.generateCode(compiler));
            endOfThen.whereToJump = compiler.getIndexOfNextActionInCurrentFunction();
        }
    }
}
