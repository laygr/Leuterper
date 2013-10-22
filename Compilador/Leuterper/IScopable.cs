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
        List<Definition_Class> getClasses();
        List<Definition_Function> getFunctions();
        List<LType> getParameters();
        List<Declaration_Var> getVars(); //vars including parameters
        List<LAction> getActions();

        
        int getIndexOfFunctionWithNameAndParameters(string name, ParametersList parameters);
        
    }
}
