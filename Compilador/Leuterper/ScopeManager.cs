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
        public IScopable scopable{ get; set; }
        public ScopeManager(IScopable scopable)
        {
            this.scopable = scopable;
        }
        int getNumOfVars()
        {
            return scopable.getVars().Count();
        }
        public int GetIndexOfFirstVarInScope()
        {
            if (scopable.GetParentScope() == null)
            {
                return LObject.literalsCounter;
            }
            else
            {
                return scopable.GetParentScope().GetScopeManager().GetIndexOfFirstVarInScope() + scopable.GetParentScope().GetScopeManager().getNumOfVars();
            }
        }
        public int getIndexOfVarNamed(string name)
        {
            List<Declaration_Var> vars = scopable.getVars();
            for(int i = 0; i < vars.Count(); i++)
            {
                if(vars[i].name.Equals(name))
                {
                    return i + scopable.GetScopeManager().GetIndexOfFirstVarInScope();
                }
            }

            IScopable parentScope = this.scopable.GetParentScope();
            if(parentScope != null)
            {
                return parentScope.GetScopeManager().getIndexOfVarNamed(name);
            }

            return -1;
        }
        public Declaration_Var getVarNamed(string name)
        {
            foreach (Declaration_Var var in this.scopable.getVars())
            {
                if (var.name.Equals(name))
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
        

        public Definition_Function getFunctionForGivenNameAndTypes(string name, List<LType> types)
        {
            return this.scopable.getProgram().getFunctionWithNameAndParametersTypes(name, types);
        }

        public Definition_Function getFunctionForGivenNameAndArguments(string name, List<Expression> arguments)
        {
            return this.getFunctionForGivenNameAndTypes(name, Expression.expressionsToTypes(arguments));
        }

        public Definition_Class getClassForType(LType type)
        {
            foreach (Definition_Class aClassD in this.scopable.getProgram().getClasses())
            {
                if (aClassD.type.MatchesWith(type))
                {
                    return aClassD;
                }
            }

            foreach(Definition_Class aClassD in StandardLibrary.specialClasses)
            {
                if (aClassD.type.MatchesWith(type))
                {
                    return aClassD;
                }
            }
            return null;
        }
        public void validate()
        {
            validateThatOnlyDefinedClassesAreUsed();
            validateThatOnlyDeclaredVariablesAreUsedAndAreOfTheRightTypeInAssignments();
            validateThatFunctionCallsUseOnlyDeclaredVariablesAndDefinedFunctions();
        }
        void validateThatFunctionCallsUseOnlyDeclaredVariablesAndDefinedFunctions()
        {
            List<Call_Function> functionCalls = LAction.GetFunctionCalls(scopable.getActions());
            
            foreach(Call_Function f in functionCalls)
            {
                String functionName = f.functionName;
                Definition_Function functionCalled;
                functionCalled = scopable.GetScopeManager().getFunctionForGivenNameAndArguments(f.functionName, f.arguments);
                
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
            List<Assignment> assignments = LAction.GetAssignments(scopable.getActions());
            
            foreach(Assignment a in assignments)
            {
                Declaration_Var var = scopable.GetScopeManager().getVarNamed(a.lhs.name);
                if(var == null)
                {
                    Console.WriteLine("Assigning to an undefined var: " + a.lhs.name);
                    Console.ReadKey();
                    Environment.Exit(0);
                }
                if(!var.type.MatchesWith(a.rhs.getType())){
                    Console.WriteLine("Type mismatch in assignment at line: " + a.line);
                    Console.ReadKey();
                    Environment.Exit(0);
                }
            }
        }
        void validateThatOnlyDefinedClassesAreUsed()
        {
            List<Declaration> declarations = LAction.GetDeclarations(scopable.getActions());
            List<LType> definedTypes = scopable.getProgram().getClasses().ConvertAll(new Converter<Definition_Class, LType>(Definition.definitionToType));

            foreach (Declaration d in declarations)
            {
                bool match = false;
                foreach (LType type in definedTypes)
                {
                    if (d.type.MatchesWith(type))
                    {
                        match = true;
                        break;
                    }
                }
                if (!match)
                {
                    Console.WriteLine("Using undefined type: " + d.type.SignatureAsString());
                    Console.ReadLine();
                    Environment.Exit(0);
                }
            }
        }
    }
}