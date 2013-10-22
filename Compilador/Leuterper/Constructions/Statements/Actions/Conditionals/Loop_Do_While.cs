using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    class Loop_Do_While : Conditional
    {
        public Loop_Do_While(int line, Expression booleanExpression, List<LAction> thenActions) 
            : base(line, booleanExpression, thenActions)
        { }

        public override void generateCode(LeuterperCompiler compiler)
        {
            int beginningOfLoop = compiler.functionInstructionsCounter;

            foreach(LAction a in thenActions)
            {
                a.generateCode(compiler);
            }
            booleanExpression.generateCode(compiler);
            compiler.addMI(new MachineInstructions.JMPT(beginningOfLoop));

        }
    }
}
