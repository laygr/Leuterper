using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    class LNumber : LObject
    {
        float value;
        new static LType type = null;

        public LNumber(int line, Token sign, String numberAsString) : base(line)
        {
            this.value = float.Parse(numberAsString);
            if (sign != null)
            {
                if (sign.image.Equals("-"))
                {
                    this.value *= -1;
                }else if(!sign.image.Equals("+"))
                {
                    Console.WriteLine("Expected a sign at line " + line);
                    Console.ReadKey();
                    Environment.Exit(0);
                }
            }
        }
        
        override public LType getType()
        {
            if (LNumber.type == null)
            {
                LNumber.type = new LType("Number");
            }
            return LNumber.type;
        }

        public override string encodeAsString()
        {
            return String.Format("{0}", this.value);
        }

        public override void generateCode(LeuterperCompiler compiler)
        {
            compiler.addLiteral(new MachineInstructions.Literal("Number", this.encodeAsString()));
        }
    }
}
