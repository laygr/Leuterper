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
            Declaration_Var var = this.getScope().getScopeManager().getVarInLineage(this.getName());
            if(var == null)
            {
                throw new SemanticErrorException("Using undeclared var " + this.getName(), this.getLine());
            }
            return var.getType();
        }

        public int getIndex()
        {
            return this.getScope().getScopeManager().getIndexOfVarNamed(this.getName());
        }

        public override void secondPass(LeuterperCompiler compiler) { }

        override public void generateCode(LeuterperCompiler compiler)
        {
            int varIndex = this.getScope().getScopeManager().getIndexOfVarNamed(this.getName());
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