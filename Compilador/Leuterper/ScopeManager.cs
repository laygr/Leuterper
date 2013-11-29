using Leuterper.Constructions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Leuterper
{
    class ScopeManager
    {
        
        public static void locateVarNamed(IScope scope, string name, DeclarationLocator<Declaration>locator)
        {
            List<Declaration> vars = scope.getDeclarations();
            for(int i = 0; i < vars.Count(); i++)
            {
                Declaration var = vars[i];
                if(vars[i].getName().Equals(name))
                {
                    locator.wasFound(var, i + ScopeManager.GetIndexOfFirstVarInScope(scope));
                    return;
                }
            }
            IScope parentScope = scope.getScope();
            if(parentScope != null)
            {
                locator.hierarchyDistance ++;
                ScopeManager.locateVarNamed(parentScope, name, locator);
            }
        }
    }
}