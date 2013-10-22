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
            if (scopable.GetParentScope() == null) return 0;
            return scopable.GetParentScope().GetScopeManager().GetIndexOfFirstVarInScope() + scopable.GetParentScope().GetScopeManager().getNumOfVars();
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

            Console.WriteLine(String.Format("Using undeclared var {0}", name));
            Console.ReadKey();
            Environment.Exit(0);
            return 0;
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
        public Definition_Function getFunctionForGivenNameAndParameters(string name, ParametersList parameters)
        {
            foreach (Definition_Function func in this.scopable.getFunctions())
            {
                if (func.matchesWithNameAndParameters(name, parameters))
                {
                    return func;
                }
            }
            return null;
        }

        public Definition_Function getFunctionForGivenNameAndArguments(string name, List<Expression> arguments)
        {
            return this.getFunctionForGivenNameAndParameters(name, ParametersList.getParametersFromArguments(arguments));
        }

        Definition_Class getClassForType(LType type)
        {
            foreach (Definition_Class aClassD in this.scopable.getClasses())
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
                ParametersList parameters = ParametersList.getParametersFromArguments(f.arguments);
                String functionName = f.functionName;
                Definition_Function functionCalled;
                functionCalled = scopable.GetScopeManager().getFunctionForGivenNameAndParameters(f.functionName, parameters);
                
                if(functionCalled == null)
                {
                    Console.WriteLine(String.Format("Called undefined function with signature: {0}", f.functionName +  parameters));
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
            List<LType> definedTypes = scopable.getClasses().ConvertAll(new Converter<Definition_Class, LType>(Definition.definitionToType));

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
