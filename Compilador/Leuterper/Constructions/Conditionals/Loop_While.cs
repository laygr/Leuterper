using System.Collections.Generic;

namespace Leuterper.Constructions
{
    class Loop_While : Conditional
    {
        public Loop_While(int line, Expression booleanExpression, List<IAction>thenActions)
            : base(line, booleanExpression, thenActions)
        {
        }
        public override void codeGenerationPass(LeuterperCompiler compiler)
        {
            int jumpToTheBeginningInstructioIndex = compiler.getIndexOfNextActionInCurrentFunction();
            MachineInstructions.JMP jumpTotheBeginning = new MachineInstructions.JMP(jumpToTheBeginningInstructioIndex);
            MachineInstructions.JMPF jumpToTheEnd = new MachineInstructions.JMPF();
            
            booleanExpression.codeGenerationPass(compiler);
            compiler.addAction(jumpToTheEnd);
            this.thenActions.ForEach(a => a.codeGenerationPass(compiler));
            compiler.addAction(jumpTotheBeginning);
            
            jumpToTheEnd.whereToJump = compiler.getIndexOfNextActionInCurrentFunction();
        }
    }
}
