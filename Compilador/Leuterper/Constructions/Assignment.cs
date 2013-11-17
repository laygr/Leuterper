using Leuterper.Exceptions;
using System;

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

        public override void scopeSetting()
        {
            rhs.shouldBePushedToStack = true;
            this.getScope().addChild(lhs);
            this.getScope().addChild(rhs);
        }

        public override void symbolsRegistration(LeuterperCompiler compiler) { }

        public override void symbolsUnificationPass()
        {
            lhs.symbolsUnificationPass();
            if (rhs != null)
            {
                rhs.symbolsUnificationPass();
            }
        }

        public override void classesGenerationPass()
        {
            this.lhs.classesGenerationPass();
            this.rhs.classesGenerationPass();
        }
        public override void simplificationAndValidationPass()
        {
            if (!rhs.getType().typeOrSuperTypeUnifiesWith(lhs.getType()))
            {
                throw new SemanticErrorException("Type mismatch", this.getLine());
            }
        }

        public override void codeGenerationPass(LeuterperCompiler compiler)
        {
            rhs.codeGenerationPass(compiler);
            compiler.addAction(new MachineInstructions.Assignment(lhs.getIndex()));
        }
    }
}