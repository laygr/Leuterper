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

        public abstract void generateCode(LeuterperCompiler compiler);
    }
}
