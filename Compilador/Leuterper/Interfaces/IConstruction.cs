using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper
{
    interface IConstruction : ICompilable
    {
        IScope getScope();
        void setScope(IScope scope);
        int getLine();
    }
}