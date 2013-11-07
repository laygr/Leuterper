namespace Leuterper.Constructions
{
    class Return_From_Block : Construction, IAction
    {
        Expression returningExpression;
        public Return_From_Block(int line, Expression returningExpression) : base(line)
        {
            this.returningExpression = returningExpression;
        }

        public override void scopeSetting()
        {
            this.returningExpression.shouldBePushedToStack = true;
            this.getScope().addChild(this.returningExpression);
        }

        public override void symbolsRegistration(LeuterperCompiler compiler) { }

        public override void symbolsUnificationPass()
        {
            this.returningExpression.symbolsUnificationPass();
        }
        public override void classesGenerationPass(){ }
        public override void simplificationAndValidationPass() { }
        public override void codeGenerationPass(LeuterperCompiler compiler)
        {
            returningExpression.codeGenerationPass(compiler);
            compiler.addAction(new MachineInstructions.Rtn());
        }
    }
}
