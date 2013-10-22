using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    class LAttributeAccess : Var
    {
        public Expression theObject { get; set; }
        public String id { get; set; }

        public LAttributeAccess(int line, Expression theObject, String name) : base(line, name)
        {
            this.theObject = theObject;
            this.id = name;
        }

        override public LType getType()
        {
            return this.theObject.getType();
        }
    }
}
