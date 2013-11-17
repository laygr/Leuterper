using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leuterper.Constructions;
using Leuterper.Exceptions;

namespace Leuterper
{
    class VariableInsideClass : VariableIndexFindStrategy
    {
        public override int getVariableIndex(LType type)
        {
            LClass definingClass = ScopeManager.getClassScope(type.getScope());

            if(definingClass == null)
            {
                throw new SyntacticErrorException("Wrong strategy. Type variable not inside a class." + this, type.getLine());
            }
            List<LType> classTypeVariables = definingClass.getType().typeVariables;

            for(int i = 0; i < classTypeVariables.Count(); i++)
            {
                if(classTypeVariables[i].getName().Equals(type.getName()))
                {
                    return i;
                }
            }
            throw new SyntacticErrorException("Type variable undeclared: " + type.getName(), type.getLine());
        }
    }
}
