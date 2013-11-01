using Leuterper.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    class Function : Procedure
    {
         public Function(int line, LType type, String id, List<Parameter> parameters, List<IAction> actions)
             : base(line, type, id, parameters, actions)
         { }
    }
}