using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    class LChar : LObject
    {
        char value;

        public LChar(int line, String aChar) : base(line)
        {
            aChar = aChar.Substring(1, aChar.Length - 1);
            if(aChar.Length == 1)
            {
                this.value = aChar[0];
            }
            else
            {
                switch(aChar)
                {
                    case "\n":
                        this.value = '\n';
                        break;

                    case "\\":
                        this.value = '\\';
                        break;

                    case "\t":
                        this.value = '\t';
                        break;

                    default:
                        throw new Exception("Char invalido");
                }
            }
        }

        public new static LType getType()
        {
            return new LType("Char");
        }

        public override string encodeAsString()
        {
            return String.Format("{0}", this.value);
        }

        public override void generateCode(LeuterperCompiler compiler)
        {
            compiler.addLiteral(new MachineInstructions.Literal("Char", this.encodeAsString()));
        }
    }
}
