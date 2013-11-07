
using System;
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
            this.scopeSetting();
        }

        public IScope getScope()
        {
            return this.scope;
        }
        public abstract void scopeSetting();
        public abstract void symbolsRegistration(LeuterperCompiler compiler);
        public abstract void symbolsUnificationPass();
        public abstract void classesGenerationPass();
        public abstract void simplificationAndValidationPass();
        public abstract void codeGenerationPass(LeuterperCompiler compiler);


    }
}
