using Leuterper.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    class Assignment : LAction
    {
        public VarAccess lhs { get; set; }
        public Expression rhs { get; set; }

        public Assignment(int line, VarAccess lhs, Expression rhs)
            : base(line)
        {
            this.lhs = lhs;
            this.rhs = rhs;
        }

        override public void secondPass()
        {
            lhs.scope = this.scope;
            rhs.scope = this.scope;
            rhs.shouldBePushedToStack = true;
            lhs.secondPass();
            rhs.secondPass();
            if (!rhs.getType().TypeOrSuperTypeMatchWith(lhs.getType()))
            {
                throw new SemanticErrorException("Type mismatch", this.line);
            }
        }

        override public void generateCode(LeuterperCompiler compiler)
        {
            rhs.generateCode(compiler);
            compiler.addAction(new MachineInstructions.Assignment(lhs.getIndex()));
        }
    }
}