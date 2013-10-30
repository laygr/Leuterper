using Leuterper.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    abstract class Call_Procedure : Expression
    {
        public String procedureName { get; set; }
        public List<Expression> arguments { get; set; }

        public Call_Procedure(int line, String procedureName, List<Expression> arguments) : base(line)
        {
            this.procedureName = procedureName;
            this.arguments = arguments;
        }

        abstract  public Procedure getProcedureDefinition();

        public override LType getType()
        {
            return this.getProcedureDefinition().getType();
        }

        public int getProcedureIdentifier()
        {
            return this.getProcedureDefinition().identifier;
        }
        public override void secondPass(LeuterperCompiler compiler)
        {
            foreach(Expression e in arguments)
            {
                e.setScope(this.getScope());
                e.shouldBePushedToStack = true;
                e.secondPass(compiler);
            }
        }
        public override void thirdPass()
        {
        }

        public override void generateCode(LeuterperCompiler compiler)
        {
            foreach(Expression argument in arguments)
            {
                argument.generateCode(compiler);
            }
            if (this.shouldBePushedToStack)
            {
                compiler.addAction(new MachineInstructions.CallP(this.getProcedureIdentifier()));
            }
            else
            {
                compiler.addAction(new MachineInstructions.Call(this.getProcedureIdentifier()));
            }
        }
    }
}
