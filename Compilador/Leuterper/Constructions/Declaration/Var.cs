using System;
using System.Collections.Generic;

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

        public override void scopeSetting()
        {
            this.getType().setShouldStartRedefinition(true);
            this.getScope().addChild(this.getType());
            
            if (this.initialValue != null)
            {
                this.initialValue.shouldBePushedToStack = true;
                this.getScope().addChild(this.initialValue);
            }
        }

        public override void symbolsRegistration(LeuterperCompiler compiler) { }

        public override void symbolsUnificationPass()
        {
            base.symbolsUnificationPass();
            scopeSetting();
            this.getType().symbolsUnificationPass();
        }

        public Var redefineWithSubstitutionTypes(List<LType> instantiatedTypes)
        {
            return new Var(this.getLine(), this.getType().substituteTypeAndVariableTypesWith(instantiatedTypes), this.getName());
        }
    }
}
