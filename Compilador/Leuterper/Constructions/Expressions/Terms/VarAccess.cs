using Leuterper.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public override void secondPass(LeuterperCompiler compiler) { }
        public override void thirdPass() { }
        public override void generateCode(LeuterperCompiler compiler)
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