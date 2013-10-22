using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    class LBoolean : LObject
    {
        new static LType type = new LType("Boolean");
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
        public new LType getType()
        {
            return LBoolean.type;
        }

        public override string encodeAsString()
        {
            return this.value ? "1" : "0";
        }

        public override void generateCode(LeuterperCompiler compiler)
        {
            base.generateCode(compiler);
            compiler.addLiteral(new MachineInstructions.Literal("Boolean", this.encodeAsString()));
        }
    }
}
