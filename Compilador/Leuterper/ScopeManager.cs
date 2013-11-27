using Leuterper.Constructions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Leuterper
{
    class ScopeManager
    {
        public static int GetIndexOfFirstVarInScope(Program program)
        {
            return program.literalsCounter
                 +
                 program.getDeclarations().Count();
        }
        public static void locateVarNamed(IScope scope, string name, DeclarationLocator<Declaration>locator)
        {
            /*
            if (name.Equals("super"))
            {
                ScopeManager.GetIndexOfFirstVarInScope(scope, locator);
                return;
            }
             */
            List<Declaration> vars = scope.getDeclarations();
            for(int i = 0; i < vars.Count(); i++)
            {
                Declaration var = vars[i];
                if(vars[i].getName().Equals(name))
                {
                    locator.wasFound(var, i + ScopeManager.GetIndexOfFirstVarInScope(locator.declaration.getProgram()));
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
            foreach (Declaration d in scope.getDeclarations())
            {
                if (d.getName().Equals(name))
                {
                    return d;
                }
            }
            return null;
        }
        static int getIndexOfFunctionWithNameAndArguments(Program program, string name, List<Expression> arguments)
        {
            return ScopeManager.getFunctionForGivenNameAndArguments(program, name, arguments).identifier;
        }
        public static Function getFunctionForGivenNameAndArguments(Program program, string name, List<Expression> arguments)
        {
            return program.getFunctionForGivenNameAndTypes(name, Utils.expressionsToTypes(arguments));
        }
        public static LClassTemplate getClassForType(IScope scope, LType type)
        {
            if (type == null) return null;

            foreach (LClassTemplate aClassD in ScopeManager.getProgram(scope).getClasses())
            {
                if (aClassD.getType().getName().Equals(type.getName()))
                {
                    return aClassD;
                }
            }
            return null;
        }
        public static LClassTemplate getClassForName(IScope scope, String name)
        {
            List<LClassTemplate> classes = ScopeManager.getProgram(scope).getClasses();
            foreach (LClassTemplate aClassD in classes)
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