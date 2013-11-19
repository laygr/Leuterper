using Leuterper.Constructions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Leuterper
{
    class ScopeManager
    {
        public static int GetIndexOfFirstVarInScope(IScope scope)
        {
            return getProgram(scope).literalsCounter
                 +
                 getProgram(scope).getDeclarations().Count();
        }
        public static int getIndexOfVarNamed(IScope scope, string name)
        {
            if (name.Equals("super")) return ScopeManager.GetIndexOfFirstVarInScope(scope);
            List<Declaration> vars = scope.getDeclarations();
            for(int i = 0; i < vars.Count(); i++)
            {
                if(vars[i].getName().Equals(name))
                {
                    return i + ScopeManager.GetIndexOfFirstVarInScope(scope);
                }
            }
            IScope parentScope = scope.getScope();
            if(parentScope != null)
            {
                return ScopeManager.getIndexOfVarNamed(parentScope, name);
            }
            return -1;
        }
        public static Declaration getDeclarationLineage(IScope scope, string name)
        {
            Declaration result = ScopeManager.getDeclarationNamed(scope, name);
            if(result == null && scope.getScope() != null)
            {
                return ScopeManager.getDeclarationNamed(scope.getScope(), name);
            }
            return result;
        }
        public static Declaration getDeclarationNamed(IScope scope, string name)
        {
            if(scope == null)
            {
                Console.WriteLine();
            }
            foreach (Declaration d in scope.getDeclarations())
            {
                if (d.getName().Equals(name))
                {
                    return d;
                }
            }
            return null;
        }
        static int getIndexOfFunctionWithNameAndArguments(IScope scope, string name, List<Expression> arguments)
        {
            return ScopeManager.getFunctionForGivenNameAndArguments(scope, name, arguments).identifier;
        }
        public static Function getFunctionForGivenNameAndTypes(IScope scope, string name, List<LType> types)
        {
            return ScopeManager.getProgram(scope).getFunctionForGivenNameAndTypes(name, types);
        }
        public static Function getFunctionForGivenNameAndArguments(IScope scope, string name, List<Expression> arguments)
        {
            return ScopeManager.getFunctionForGivenNameAndTypes(scope, name, Utils.expressionsToTypes(arguments));
        }
        public static LClass getClassForType(IScope scope, LType type)
        {
            if (type == null) return null;

            foreach (LClass aClassD in ScopeManager.getProgram(scope).getClasses())
            {
                if (aClassD.getType().getName().Equals(type.getName()))
                {
                    return aClassD;
                }
            }
            return null;
        }
        public static Program getProgram(IScope scope)
        {
            if(scope is Program)
            {
                return scope as Program;
            }
            return ScopeManager.getProgram(scope.getScope());
        }

        public static LClass getClassScope(IScope scope)
        {
            if (scope is LClass)
            {
                return scope as LClass;
            }
            if(scope == null)
            {
                return null;
            }
            return ScopeManager.getClassScope(scope.getScope());
        }
        public static LClass getClassForName(IScope scope, String name)
        {
            List<LClass> classes = ScopeManager.getProgram(scope).getClasses();
            foreach (LClass aClassD in classes)
            {
                if (aClassD.getType().getName().Equals(name))
                {
                    return aClassD;
                }
            }
            return null;
        }
    }
}