using Leuterper.Constructions;
using Leuterper.Exceptions;
using System.Collections.Generic;
using System.Linq;

namespace Leuterper
{
    abstract class VariableIndexFindStrategy
    {
        public static VariableIndexFindStrategy getSingleton() { return null; }
        public abstract int getVariableIndex(LType type);
    }
    class VariableInsideClass : VariableIndexFindStrategy
    {
        public static VariableInsideClass singleton;
        public static new VariableIndexFindStrategy getSingleton()
        {
            if (singleton == null) singleton = new VariableInsideClass();
            return singleton;
        }
        public override int getVariableIndex(LType type)
        {
            LClass definingClass = ScopeManager.getClassScope(type.getScope());

            if (definingClass == null)
            {
                throw new SyntacticErrorException("Wrong strategy. Type variable not inside a class." + this, type.getLine());
            }
            List<LType> typeVariables = definingClass.getType().typeVariables;

            for (int i = 0; i < typeVariables.Count(); i++)
            {
                if (typeVariables[i].getName().Equals(type.getName()))
                {
                    return i;
                }
            }
            throw new SyntacticErrorException("Type variable undeclared: " + type.getName(), type.getLine());
        }
    }
    class VariableInParentClass : VariableIndexFindStrategy
    {
        public static VariableInParentClass singleton;
        public static new VariableIndexFindStrategy getSingleton()
        {
            if (singleton == null) singleton = new VariableInParentClass();
            return singleton;
        }
        public override int getVariableIndex(LType type)
        {
            List<LType> typeVariables = (type.getScope() as LClass).getType().typeVariables;
            for (int i = 0; i < typeVariables.Count(); i++)
            {
                if (typeVariables[i].getName().Equals(type.getName()))
                {
                    return i;
                }
            }
            throw new SyntacticErrorException("Type variable undeclared: " + type.getName(), type.getLine());
        }
    }
}