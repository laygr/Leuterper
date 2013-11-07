using Leuterper.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    class LAttributeAccess : Term
    {
        public Expression theObject { get; set; }
        public String LAttributeName { get; set; }
        public bool willBeUsedForSet { get; set; }
        public LAttributeAccess(int line, Expression theObject, String name) : base(line)
        {
            this.theObject = theObject;
            this.theObject.shouldBePushedToStack = true;
            this.LAttributeName = name;
            this.willBeUsedForSet = false;
        }

        public int getLAttributeIndex()
        {
            LClass c = this.theObject.getType().getDefiningClass();
            return c.getIndexOfLAttribute(LAttributeName);
        }

        override public LType getType()
        {
            LClass c = this.theObject.getType().getDefiningClass();
            return c.getTypeOfLAttribute(this.getLAttributeIndex());
        }

        public override void scopeSetting()
        {
            this.theObject.setScope(this.getScope());
        }
        public override void symbolsUnificationPass()
        {
            theObject.symbolsUnificationPass();
        }
        public override void classesGenerationPass() { }
        public override void simplificationAndValidationPass() { }

        public override void codeGenerationPass(LeuterperCompiler compiler)
        {
            if (!this.shouldBePushedToStack) return;
            theObject.codeGenerationPass(compiler);
            if (!this.willBeUsedForSet)
            {
                LClass c = this.theObject.getType().getDefiningClass();
                int LAttributeIndex = c.getIndexOfLAttribute(this.LAttributeName);
                if(LAttributeIndex < 0)
                {
                    throw new SemanticErrorException("Accessed an undeclared LAttribute: " + this.LAttributeName, this.getLine());
                }
                compiler.addAction(new MachineInstructions.Get(LAttributeIndex));
            }
        }
    }
}
