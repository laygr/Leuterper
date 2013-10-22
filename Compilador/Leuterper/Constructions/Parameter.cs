using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    class Parameter : Declaration_Var
    {

        public Parameter(int line, LType type, String name) : base(line, type, name)
        {
        }
    }
}
