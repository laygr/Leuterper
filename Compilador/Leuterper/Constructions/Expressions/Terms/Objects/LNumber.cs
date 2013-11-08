using Leuterper.Exceptions;
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
        new public static LType type = LNumber.type = new LType(0, "Number");

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
                    throw new SyntacticErrorException("Expected a sign.", line);
                }
            }
        }
        override public LType getType()
        {
            return type;
        }
        public override string encodeAsString()
        {
            return String.Format("{0}", this.value);
        }

        public override void codeGenerationPass(LeuterperCompiler compiler)
        {
            base.codeGenerationPass(compiler);
            compiler.addLiteral(new MachineInstructions.Literal("Number", this.encodeAsString()));
        }
    }
}
