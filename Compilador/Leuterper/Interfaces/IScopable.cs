using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leuterper.Constructions;

namespace Leuterper
{
    interface IScopable
    {
        void setScope(IScope scope);
        IScope getScope();
    }
}
