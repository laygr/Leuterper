using Leuterper.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    class Assignment : Construction, IAction
    {
        public VarAccess lhs { get; set; }
        public Expression rhs { get; set; }

        public Assignment(int line, VarAccess lhs, Expression rhs)
            : base(line)
        {
            this.lhs = lhs;
            this.rhs = rhs;
        }

        public override void secondPass(LeuterperCompiler compiler)
        {
            lhs.setScope(this.getScope());
            rhs.setScope(this.getScope());
            rhs.shouldBePushedToStack = true;
            lhs.secondPass(compiler);
            rhs.secondPass(compiler);
            if (!rhs.getType().typeOrSuperTypeUnifiesWith(lhs.getType()))
            {
                throw new SemanticErrorException("Type mismatch", this.getLine());
            }
        }

        public override void thirdPass()
        {
            this.lhs.thirdPass();
            this.rhs.thirdPass();
        }

        public override void generateCode(LeuterperCompiler compiler)
        {
            rhs.generateCode(compiler);
            compiler.addAction(new MachineInstructions.Assignment(lhs.getIndex()));
        }
    }
}