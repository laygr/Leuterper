using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{

    abstract class Definition : Statement
    {
        public LType type;

        public Definition(int line, LType type) : base(line)
        {
            this.type = type;
        }

        public static LType definitionToType(Definition d)
        {
            return d.type;
        }

    }
}
