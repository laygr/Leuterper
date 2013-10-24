using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    class Parameter : ICompilable
    {
        public IScopable scope;
        public LType type;
        public string name;
        public Parameter(LType type, String name)
        {
            this.type = type;
            this.name = name;
        }

        public void secondPass()
        {
        }

        public void generateCode(LeuterperCompiler compiler)
        {

        }
    }
}
