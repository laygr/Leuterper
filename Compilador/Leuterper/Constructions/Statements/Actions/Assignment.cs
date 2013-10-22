using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    class Assignment : LAction
    {
        public Var lhs { get; set; }
        public Expression rhs { get; set; }

        public Assignment(int line, Var lhs, Expression rhs)
            : base(line)
        {
            this.lhs = lhs;
            this.rhs = rhs;
        }

        public void generateCode(LeuterperCompiler compiler)
        {
            compiler.addMI(new MachineInstructions.Assignment(lhs.getIndex()));
        }
    }
}