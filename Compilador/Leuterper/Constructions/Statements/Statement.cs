using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    abstract class Statement : ICompilable
    {
        public int line { get; set; }
        public IScopable scope { get; set; }
        public Statement(int line)
        {
            this.line = line;
        }

        abstract public void secondPass();
        abstract public void generateCode(LeuterperCompiler compiler);
    }
}
