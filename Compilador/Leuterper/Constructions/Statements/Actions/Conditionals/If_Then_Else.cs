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

        public override void generateCode(LeuterperCompiler compiler)
        {
            this.booleanExpression.generateCode(compiler);
            MachineInstructions.JMPF jumpToElse = new MachineInstructions.JMPF();
            compiler.addMI(jumpToElse);
            foreach(LAction action in this.thenActions)
            {
                action.generateCode(compiler);
            }
            jumpToElse.whereToJump = compiler.functionInstructionsCounter + 1;

            MachineInstructions.JMPF endOfThenJMP = new MachineInstructions.JMPF();
            compiler.addMI(endOfThenJMP);

            foreach (LAction action in this.elseActions)
            {
                action.generateCode(compiler);
            }
            endOfThenJMP.whereToJump = compiler.functionInstructionsCounter;
        }
    }
}
