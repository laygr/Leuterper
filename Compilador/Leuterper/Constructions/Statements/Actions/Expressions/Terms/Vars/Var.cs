using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    class Var : Term
    {
        public String name;

        public Var(int line, String name) : base(line)
        {
            this.name = name;
        }

        override public LType getType()
        {
            IScopable currentScope = this.scope;
            Declaration_Var var = null;
            while(currentScope != null)
            {
               var = currentScope.GetScopeManager().getVarNamed(this.name);
               if (var != null) break;
               currentScope = currentScope.GetParentScope();
            }
            return var.type;
        }

        public int getIndex()
        {
            return this.scope.GetScopeManager().getIndexOfVarNamed(this.name);
        }

        public override void secondPass() { }

        override public void generateCode(LeuterperCompiler compiler)
        {
            int varIndex = this.scope.GetScopeManager().getIndexOfVarNamed(this.name);
            compiler.addAction(new MachineInstructions.Push(varIndex));
        }
       
    }
}