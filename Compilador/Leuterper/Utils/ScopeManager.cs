using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Leuterper;
using Leuterper.Constructions;

namespace Leuterper
{
    class ScopeManager
    {
        private IScope scope;
        public ScopeManager(IScope scope)
        {
            this.scope = scope;
        }
        public int GetIndexOfFirstVarInScope()
        {
            if (scope.getScope() == null)
            {
                return LObject.literalsCounter;
            }
            else
            {
                return scope.getScope().getScopeManager().GetIndexOfFirstVarInScope() + scope.getScope().getDeclarations().Count();
            }
        }
        public int getIndexOfVarNamed(string name)
        {
            List<IDeclaration> vars = this.scope.getDeclarations();
            for(int i = 0; i < vars.Count(); i++)
            {
                if(vars[i].getName().Equals(name))
                {
                    return i + scope.getScopeManager().GetIndexOfFirstVarInScope();
                }
            }

            IScope parentScope = this.getScope().getScope();
            if(parentScope != null)
            {
                return parentScope.getScopeManager().getIndexOfVarNamed(name);
            }

            return -1;
        }

        public Declaration_Var getVarInLineage(string name)
        {
            Declaration_Var result = this.getVarNamed(name);
            if(result == null && this.getScope().getScope() != null)
            {
                return this.getScope().getScope().getScopeManager().getVarNamed(name);
            }
            return result;
        }
        public Declaration_Var getVarNamed(string name)
        {
            foreach (Declaration_Var var in this.getScope().getDeclarations())
            {
                if (var.getName().Equals(name))
                {
                    return var;
                }
            }
            return null;
        }

        int getIndexOfFunctionWithNameAndArguments(string name, List<Expression> arguments)
        {
            return this.getFunctionForGivenNameAndArguments(name, arguments).identifier;
        }
        

        public Function getFunctionForGivenNameAndTypes(string name, List<LType> types)
        {
            return this.getProgram().getFunctionForGivenNameAndTypes(name, types);
        }

        public Function getFunctionForGivenNameAndArguments(string name, List<Expression> arguments)
        {
            return this.getFunctionForGivenNameAndTypes(name, Expression.expressionsToTypes(arguments));
        }

        public LClass getClassForType(LType type)
        {
            if (type == null) return null;

            foreach (LClass aClassD in this.getProgram().getClasses())
            {
                if (aClassD.getType().getName().Equals(type.getName()))
                {
                    return aClassD;
                }
            }
            return null;
        }

        public Program getProgram()
        {
            if(this.scope is Program)
            {
                return this.scope as Program;
            }
            return this.getScope().getScope().getScopeManager().getProgram();
        }

        public LClass getClassScope()
        {
            if (this.scope is LClass)
            {
                return this.scope as LClass;
            }
            return this.getScope().getScope().getScopeManager().getClassScope();
        }
        /*
        public void validate()
        {
            validateThatOnlyDefinedClassesAreUsed();
            validateThatOnlyDeclaredVariablesAreUsedAndAreOfTheRightTypeInAssignments();
            validateThatFunctionCallsUseOnlyDeclaredVariablesAndDefinedFunctions();
        }
        void validateThatFunctionCallsUseOnlyDeclaredVariablesAndDefinedFunctions()
        {
            List<Call_Function> functionCalls = IAction.GetFunctionCalls(scopable.getActions());
            
            foreach(Call_Function f in functionCalls)
            {
                String functionName = f.functionName;
                Definition_Function functionCalled;
                functionCalled = scopable.getScopeManager().getFunctionForGivenNameAndArguments(f.functionName, f.arguments);
                
                if(functionCalled == null)
                {
                    Console.WriteLine(String.Format("Called undefined function with signature: {0}", f.functionName +  Parameter.listOfParametersAsString(f.argumentsAsParameters())));
                    Console.ReadKey();
                    Environment.Exit(0);
                }
            }
        }
        void validateThatOnlyDeclaredVariablesAreUsedAndAreOfTheRightTypeInAssignments()
        {
            List<Assignment> assignments = IAction.GetAssignments(scopable.getActions());
            
            foreach(Assignment a in assignments)
            {
                Declaration_Var var = scopable.getScopeManager().getVarNamed(a.lhs.getName());
                if(var == null)
                {
                    Console.WriteLine("Assigning to an undefined var: " + a.lhs.getName());
                    Console.ReadKey();
                    Environment.Exit(0);
                }
                if(!var.getType().MatchesWith(a.rhs.getType())){
                    Console.WriteLine("Type mismatch in assignment at line: " + a.getLine());
                    Console.ReadKey();
                    Environment.Exit(0);
                }
            }
        }
        void validateThatOnlyDefinedClassesAreUsed()
        {
            List<Declaration> declarations = IAction.GetDeclarations(scopable.getActions());
            List<LType> definedTypes = scopable.getProgram().getClasses().ConvertAll(new Converter<Definition_Class, LType>(Definition.definitionToType));

            foreach (Declaration d in declarations)
            {
                bool match = false;
                foreach (LType type in definedTypes)
                {
                    if (d.getType().MatchesWith(type))
                    {
                        match = true;
                        break;
                    }
                }
                if (!match)
                {
                    Console.WriteLine("Using undefined type: " + d.getType().SignatureAsString());
                    Console.ReadLine();
                    Environment.Exit(0);
                }
            }
        }
        */
        public IScope getScope()
        {
            return this.scope;
        }
        internal LClass getClassForName(string name)
        {
            foreach (LClass aClassD in this.getProgram().getClasses())
            {
                if (aClassD.getType().isNamed(name))
                {
                    return aClassD;
                }
            }
            foreach (LClass aClassD in StandardLibrary.specialClasses)
            {
                if (aClassD.getType().isNamed(name))
                {
                    return aClassD;
                }
            }
            return null;
        }
    }
}