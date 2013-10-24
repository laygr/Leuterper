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
        new public static LType type = LNumber.type = new LType("Number");

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

        public override string encodeAsString()
        {
            return String.Format("{0}", this.value);
        }

        public override void secondPass(){ }

        public override void generateCode(LeuterperCompiler compiler)
        {
            base.generateCode(compiler);
            compiler.addLiteral(new MachineInstructions.Literal("Number", this.encodeAsString()));
        }
    }
}
