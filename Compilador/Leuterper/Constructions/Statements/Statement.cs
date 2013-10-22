using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    abstract class Statement : ICodeGenerator
    {
        public int line { get; set; }
        public IScopable scope { get; set; }
        public Statement(int line)
        {
            this.line = line;
        }
        public Program program { get; set; }

        abstract public void generateCode(LeuterperCompiler compiler);
    }
}
