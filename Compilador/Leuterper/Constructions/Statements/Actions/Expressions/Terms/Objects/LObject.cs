using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    abstract class LObject : Term
    {
        public static LType type = new LType("Object");
        public int literalIndex;

        public LObject(int line) : base(line) { }

        override public LType getType()
        {
            return type;
        }

        abstract public String encodeAsString();

        public override void generateCode(LeuterperCompiler compiler)
        {
            this.literalIndex = compiler.literals.Count();
        }
    }
}
