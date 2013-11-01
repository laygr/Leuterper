using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    abstract class Construction : IConstruction, ICompilable
    {
        private IScope scope { get; set; }
        private int line;


        public Construction(int line)
        {
            this.line = line;
        }

        public int getLine()
        {
            return this.line;
        }

        virtual public void setScope(IScope scope)
        {
            this.scope = scope;
        }

        public IScope getScope()
        {
            return this.scope;
        }

        public abstract void secondPass(LeuterperCompiler compiler);
        public abstract void thirdPass();
        public abstract void generateCode(LeuterperCompiler compiler);
    }
}
