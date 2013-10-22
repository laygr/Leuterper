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
        public ParametersList parameters { get; set; }
        public bool shouldBePushedToStack { get; set; }
        public Constructor(int line, LType instanceType, List<Parameter> parameters) : base(line)
        {
            this.instanceType = instanceType;
            this.parameters = new ParametersList(parameters);
        }
        override public LType getType()
        {
            return this.instanceType;
        }

        public override void generateCode(LeuterperCompiler compiler)
        {
            for(int i = 0; i < parameters.Count(); i++)
            {
                Parameter p = parameters.Get(i);
                p.generateCode(compiler);
            }
            this.sc

        }

    }
}
