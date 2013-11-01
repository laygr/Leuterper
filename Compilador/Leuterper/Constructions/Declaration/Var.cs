using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    class Var : Declaration, IAction
    {
        
        public Expression initialValue { get; set; }
        public Var(int line, LType type, String name, Expression initialValue)
            : base(line, type, name)
        {
            this.initialValue = initialValue;
        }
        public Var(int line, LType type, String id)
            : this(line, type, id, null)
        { }


        public override void secondPass(LeuterperCompiler compiler)
        {
            base.secondPass(compiler);

            this.getType().setScope(this.getScope());
            this.getType().setShouldStartRedefinition(true);
            this.getType().secondPass(compiler);
            if (this.initialValue != null)
            {
                this.initialValue.setScope(this.getScope());
                this.initialValue.shouldBePushedToStack = true;
            }
        }

        public Var redefineWithSubstitutionTypes(List<LType> instantiatedTypes)
        {
            return new Var(this.getLine(), this.getType().substituteTypeAndVariableTypesWith(instantiatedTypes), this.getName());
        }
    }
}
