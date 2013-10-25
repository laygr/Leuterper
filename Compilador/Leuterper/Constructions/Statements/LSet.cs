using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    class LSet : LAction
    {
        public LAttributeAccess la { get; set; }
        public Expression rhs { get; set; }

        public LSet(int line, LAttributeAccess la, Expression rhs) : base(line)
        {
            this.la = la;
            this.rhs = rhs;
        }


        override public void secondPass()
        {
            la.scope = this.scope;
            la.willBeUsedForSet = true;
            rhs.scope = this.scope;
            this.la.shouldBePushedToStack = true;
            rhs.shouldBePushedToStack = true;
            la.secondPass();
            rhs.secondPass();
        }

        override public void generateCode(LeuterperCompiler compiler)
        {
            la.generateCode(compiler);
            rhs.generateCode(compiler);
            compiler.addAction(new MachineInstructions.Set(la.getAttributeIndex()));
        }

    }
}
