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
        IScopable GetParentScope();
        ScopeManager GetScopeManager();
        List<Declaration_Var> getVars(); //vars including parameters
        List<LAction> getActions();

        Program getProgram();
        
        
    }
}
