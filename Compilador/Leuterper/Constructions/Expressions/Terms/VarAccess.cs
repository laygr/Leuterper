using Leuterper.Exceptions;
using System;

namespace Leuterper.Constructions
{
    class VarAccess : Term
    {
        private String name;

        public VarAccess(int line, String name) : base(line)
        {
            this.name = name;
        }

        override public LType getType()
        {
            Declaration d = ScopeManager.getDeclarationLineage(this.getScope(), this.getName());
            if(d == null)
            {
                throw new SemanticErrorException("Using undeclared var " + this.getName(), this.getLine());
            }
            return d.getType();
        }

        public int getIndex()
        {
            return ScopeManager.getIndexOfVarNamed(this.getScope(), this.getName());
        }

        public override void scopeSetting() { }
        public override void symbolsRegistration(LeuterperCompiler compiler) { }
        public override void symbolsUnificationPass() { }
        public override void classesGenerationPass() { }
        public override void simplificationAndValidationPass() { }
        public override void codeGenerationPass(LeuterperCompiler compiler)
        {
            int varIndex = ScopeManager.getIndexOfVarNamed(this.getScope(), this.getName());
            if(varIndex == -1)
            {
                throw new SemanticErrorException("Using undeclared var: " + this.getName(), this.getLine());
            }
            compiler.addAction(new MachineInstructions.Push(varIndex));
        }

        public string getName()
        {
            return this.name;
        }
    }
}