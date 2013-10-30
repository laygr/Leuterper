using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    //Asignacion al attributo de un objeto.
    class LSet : Construction, IAction
    {
        public AttributeAccess la { get; set; }
        public Expression rhs { get; set; }

        public LSet(int line, AttributeAccess la, Expression rhs) : base(line)
        {
            this.la = la;
            this.rhs = rhs;
        }


        override public void secondPass(LeuterperCompiler compiler)
        {
            la.setScope(this.getScope());
            la.willBeUsedForSet = true;
            rhs.setScope(this.getScope());
            rhs.shouldBePushedToStack = true;
            la.secondPass(compiler);
            rhs.secondPass(compiler);
        }

        public override void thirdPass()
        {
        }

        override public void generateCode(LeuterperCompiler compiler)
        {
            la.generateCode(compiler);
            rhs.generateCode(compiler);
            compiler.addAction(new MachineInstructions.Set(la.getAttributeIndex()));
        }

    }
}
