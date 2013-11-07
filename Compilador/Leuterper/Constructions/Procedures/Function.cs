using System;
using System.Collections.Generic;

namespace Leuterper.Constructions
{
    class Function : Procedure
    {
         public Function(int line, LType type, String id, List<Parameter> parameters, List<IAction> actions)
             : base(line, type, id, parameters, actions)
         {
             this.scopeSetting();
         }
    }
}