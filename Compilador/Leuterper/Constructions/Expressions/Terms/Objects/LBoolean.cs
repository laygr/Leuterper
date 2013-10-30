using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    class LBoolean : LObject
    {
        new public static LType type = new LType(0, "Boolean");
        Boolean value;

        public LBoolean(int line, String value) : base(line)
        {
            if(value.Equals("true"))
            {
                this.value = true;
            } else if(value.Equals("false"))
            {
                this.value = false;
            }
        }

        public override string encodeAsString()
        {
            return this.value ? "1" : "0";
        }

        public override void secondPass(LeuterperCompiler compiler)
        {
            
        }

        public override void generateCode(LeuterperCompiler compiler)
        {
            base.generateCode(compiler);
            compiler.addLiteral(new MachineInstructions.Literal("Boolean", this.encodeAsString()));
        }
    }
}
