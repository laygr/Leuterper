using System;
using Leuterper.Exceptions;

namespace Leuterper.Constructions
{
    //Asignacion al attributo de un objeto.
    class LSet : Construction, IAction
    {
        public LAttributeAccess la { get; set; }
        public Expression rhs { get; set; }

        public LSet(int line, LAttributeAccess la, Expression rhs) : base(line)
        {
            this.la = la;
            this.rhs = rhs;
        }
        public override void scopeSetting()
        {
            la.willBeUsedForSet = true;
            this.getScope().addChild(la);

            rhs.shouldBePushedToStack = true;
            this.getScope().addChild(rhs);
        }
        public override void symbolsRegistration(LeuterperCompiler compiler) { }
        override public void symbolsUnificationPass()
        {
            la.symbolsUnificationPass();
            rhs.symbolsUnificationPass();
        }

        public override void classesGenerationPass() { }
        public override void simplificationAndValidationPass() { }
        override public void codeGenerationPass(LeuterperCompiler compiler)
        {
            int attributeIndex = la.getLAttributeIndex();
            if(attributeIndex < 0)
            {
                throw new SemanticErrorException(String.Format("Attribute not defined. Name {0}"), this.getLine());
            }
            la.codeGenerationPass(compiler);
            rhs.codeGenerationPass(compiler);
            compiler.addAction(new MachineInstructions.Set(attributeIndex));
        }
    }
}
