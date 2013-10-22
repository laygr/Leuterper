using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    class LVoid : LObject
    {
        static LType type = new LType("Void");
        public LVoid(int line) : base(line)
        {

        }

        public LType getType()
        {
            return LVoid.type;
        }

        public override string encodeAsString()
        {
            return "";
        }

        public override void generateCode(LeuterperCompiler compiler)
        {
            compiler.addLiteral(new MachineInstructions.Literal("Void", encodeAsString()));
        }
    }
}
