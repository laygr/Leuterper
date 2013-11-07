using System;
using System.Collections.Generic;

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

        public override void symbolsUnificationPass()
        {
            LClass aClass = this.getClass();
            Parameter _this = new Parameter(this.getLine(), aClass.getType(), "this");
            _this.setScope(this);
            this.parameters.Insert(0, _this);
            base.symbolsUnificationPass();
        }
    }
}
