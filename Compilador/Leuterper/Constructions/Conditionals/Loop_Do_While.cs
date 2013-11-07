using System.Collections.Generic;

namespace Leuterper.Constructions
{
    class Loop_Do_While : Conditional
    {
        public Loop_Do_While(int line, Expression booleanExpression, List<IAction> thenActions) 
            : base(line, booleanExpression, thenActions) 
        {
        }
        public override void codeGenerationPass(LeuterperCompiler compiler)
        {
            int beginningOfLoop = compiler.getIndexOfNextActionInCurrentFunction();

            this.thenActions.ForEach(a => a.codeGenerationPass(compiler));
            booleanExpression.codeGenerationPass(compiler);
            compiler.addAction(new MachineInstructions.JMPT(beginningOfLoop));
        }
    }
}
