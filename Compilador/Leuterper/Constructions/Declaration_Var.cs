using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    class Declaration_Var : Construction, IAction, IDeclaration, IIdentifiable<Declaration_Var>
    {
        private LType type;
        private String name;
        public Expression initialValue { get; set; }
        public Declaration_Var(int line, LType type, String name, Expression initialValue)
            : base(line)
        {
            this.type = type;
            this.name = name;
            this.initialValue = initialValue;
        }
        public Declaration_Var(int line, LType type, String id)
            : this(line, type, id, null)
        { }

        public bool HasSameSignatureAs(Declaration_Var otherElement)
        {
            return
                this.getType().HasSameSignatureAs(otherElement.getType())
                &&
                this.getName().Equals(otherElement.getName());
        }

        public override void secondPass(LeuterperCompiler compiler)
        {
            this.getType().setScope(this.getScope());
            this.getType().shouldRedefinesItsClass = true;
            this.getType().secondPass(compiler);
            if (this.initialValue != null)
            {
                this.initialValue.setScope(this.getScope());
                this.initialValue.shouldBePushedToStack = true;
            }
        }

        public override void thirdPass()
        {
            this.getType().thirdPass();
            if (this.initialValue != null)
            {
                this.initialValue.thirdPass();
            }
        }

        override public void generateCode(LeuterperCompiler compiler)
        {
            throw new NotImplementedException();
        }

        public LType getType()
        {
            return this.type;
        }

        public void setType(LType type)
        {
            this.type = type;
        }

        public String getName()
        {
            return this.name;
        }
    }
}
