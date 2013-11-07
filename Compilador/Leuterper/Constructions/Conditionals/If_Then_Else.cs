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
        public override void scopeSetting()
        {
            base.scopeSetting();
            this.elseActions.ForEach(a=> a.setScope(this.getScope()));
        }
        public override void symbolsRegistration(LeuterperCompiler compiler)
        {
            base.symbolsRegistration(compiler);
            this.elseActions.ForEach(a => a.symbolsRegistration(compiler));
        }
        public override void symbolsUnificationPass()
        {
            base.symbolsUnificationPass();
            foreach(IAction a in elseActions)
            {
                a.symbolsUnificationPass();
            }
        }

        public override void classesGenerationPass() { }

        public override void codeGenerationPass(LeuterperCompiler compiler)
        {
            MachineInstructions.JMPF jumpToElse = new MachineInstructions.JMPF();
            MachineInstructions.JMPF endOfThen = new MachineInstructions.JMPF();

            this.booleanExpression.codeGenerationPass(compiler);
            compiler.addAction(jumpToElse);
            this.thenActions.ForEach(a => a.codeGenerationPass(compiler));
            compiler.addAction(endOfThen);
            jumpToElse.whereToJump = compiler.getIndexOfNextActionInCurrentFunction();
            this.elseActions.ForEach(a => a.codeGenerationPass(compiler));
            endOfThen.whereToJump = compiler.getIndexOfNextActionInCurrentFunction();
        }
    }
}
