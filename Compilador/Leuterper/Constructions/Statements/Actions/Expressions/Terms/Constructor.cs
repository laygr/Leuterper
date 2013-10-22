using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    class Constructor : Term
    {
        public LType instanceType { get; set; }
        public List<Parameter> parameters { get; set; }

        public Constructor(int line, LType instanceType, List<Parameter> parameters) : base(line)
        {
            this.instanceType = instanceType;
            this.parameters = parameters;
        }
        override public LType getType()
        {
            return this.instanceType;
        }

    }
}
