using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    abstract class Construction : ICompilable
    {
        private IScope scope { get; set; }
        private int line;


        public Construction(int line)
        {
            this.line = line;
        }

        public int getLine()
        {
            return this.getLine();
        }

        abstract public void secondPass(LeuterperCompiler compiler);
        abstract public void thirdPass();
        abstract public void generateCode(LeuterperCompiler compiler);

        public void setScope(IScope scope)
        {
            this.scope = scope;
        }

        public IScope getScope()
        {
            return this.scope;
        }
    }
}
