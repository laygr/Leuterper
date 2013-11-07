
using System;
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
            la.codeGenerationPass(compiler);
            rhs.codeGenerationPass(compiler);
            if(la.getLAttributeIndex() < 0)
            {
                Console.WriteLine();
            }
            compiler.addAction(new MachineInstructions.Set(la.getLAttributeIndex()));
        }

    }
}
