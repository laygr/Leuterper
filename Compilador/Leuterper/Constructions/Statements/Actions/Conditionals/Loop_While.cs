using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    class Loop_While : Conditional
    {
        public Loop_While(int line, Expression booleanExpression, List<LAction>thenActions)
            : base(line, booleanExpression, thenActions)
        { }

        public override void generateCode(LeuterperCompiler compiler)
        {
            MachineInstructions.JMP jumpTotheBeginning = new MachineInstructions.JMPT(compiler.getIndexOfNextActionInCurrentFunction());
            MachineInstructions.JMPF jumpToTheEnd = new MachineInstructions.JMPF();
            
            booleanExpression.generateCode(compiler);

            compiler.addAction(jumpToTheEnd);
            foreach(LAction action in this.thenActions)
            {
                action.generateCode(compiler);
            }
            compiler.addAction(jumpToTheEnd);
            jumpToTheEnd.whereToJump = compiler.getIndexOfNextActionInCurrentFunction();
        }
    }
}
