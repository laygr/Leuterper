using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    class LVoid : LObject
    {
        new public static LType type = new LType(0, "Void");
        public LVoid(int line) : base(line) { }
        public override LType getType()
        {
            return LVoid.type;
        }
        public override string encodeAsString()
        {
            return "";
        }
        public override void codeGenerationPass(LeuterperCompiler compiler)
        {
            compiler.addLiteral(new MachineInstructions.Literal("Void", encodeAsString()));
        }
    }
}
