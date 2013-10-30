using Leuterper.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    class AttributeAccess : Term
    {
        public Expression theObject { get; set; }
        public String attributeName { get; set; }
        public bool willBeUsedForSet { get; set; }
        public AttributeAccess(int line, Expression theObject, String name) : base(line)
        {
            this.theObject = theObject;
            this.theObject.shouldBePushedToStack = true;
            this.attributeName = name;
            this.willBeUsedForSet = false;
        }

        public int getAttributeIndex()
        {
            LClass c = this.theObject.getType().definingClass;
            return c.getIndexOfAttribute(attributeName);
        }

        override public LType getType()
        {
            LClass c = this.theObject.getType().definingClass;
            return c.getTypeOfAttribute(this.getAttributeIndex());
        }

        public override void secondPass(LeuterperCompiler compiler)
        {
            theObject.setScope(this.getScope());
            theObject.secondPass(compiler);
        }

        public override void generateCode(LeuterperCompiler compiler)
        {
            if (!this.shouldBePushedToStack) return;
            theObject.generateCode(compiler);
            if (!this.willBeUsedForSet)
            {
                LClass c = this.theObject.getType().definingClass;
                int attributeIndex = c.getIndexOfAttribute(this.attributeName);
                if(attributeIndex < 0)
                {
                    throw new SemanticErrorException("Accessed an undeclared attribute: " + this.attributeName, this.getLine());
                }
                compiler.addAction(new MachineInstructions.Get(attributeIndex));
            }
        }
    }
}
