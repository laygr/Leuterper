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
        public String attributeName { get; set; }
        public bool willBeUsedForSet { get; set; }
        public LAttributeAccess(int line, Expression theObject, String name) : base(line)
        {
            this.theObject = theObject;
            this.theObject.shouldBePushedToStack = true;
            this.attributeName = name;
            this.willBeUsedForSet = false;
        }

        public int getAttributeIndex()
        {
            return theObject.getType().getIndexOfAttribute(attributeName);
        }

        override public LType getType()
        {
            return this.theObject.getType();
        }

        public override void secondPass()
        {
            theObject.scope = this.scope;
            theObject.secondPass();
        }

        public override void generateCode(LeuterperCompiler compiler)
        {
            if (!this.shouldBePushedToStack) return;
            theObject.generateCode(compiler);
            if (!this.willBeUsedForSet)
            {
                int attributeIndex = theObject.getType().getIndexOfAttribute(this.attributeName);
                compiler.addAction(new MachineInstructions.Get(attributeIndex));
            }
        }
    }
}
