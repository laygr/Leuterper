using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    abstract class Class_Procedure : Procedure
    {
        public Class_Procedure(int line, LType type, String id, List<Parameter> parameters, List<IAction> actions)
            : base(line, type, id, parameters, actions)
        { }

        public LClass getClass()
        {
            return this.getScope() as LClass;
        }

        public override void secondPass(LeuterperCompiler compiler)
        {
            LClass aClass = this.getClass();
            this.parameters.Insert(0, new Parameter(this.getLine(), aClass.getType(), "this"));
            base.secondPass(compiler);
        }

        
    }
}
