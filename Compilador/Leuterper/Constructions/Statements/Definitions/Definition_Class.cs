using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leuterper;

namespace Leuterper.Constructions
{
    class Definition_Class : Definition, IIdentifiable<Definition_Class>, IScopable
    {
        public int identifier { get; set; }

        public Declaration_Inheritance inheritanceDeclaration { get; set; }
        public UniquesList<Declaration_LAttribute> attributesDeclarations { get; set; }
        public UniquesList<Definition_Method> methodsDefinitions { get; set; }

        public Definition_Class
            (
                int line,
                LType type,
                Declaration_Inheritance inheritanceDeclaration,
                List<Declaration_LAttribute> attributesDeclarations,
                List<Definition_Method> methodsDefinitions
            ) : base(line, type)
        {
            this.inheritanceDeclaration = inheritanceDeclaration;

            attributesDeclarations.ForEach(a => a.aClass = this);
            this.attributesDeclarations = new UniquesList<Declaration_LAttribute>(attributesDeclarations);

            methodsDefinitions.ForEach(m => m.aClass = this);
            this.methodsDefinitions = new UniquesList<Definition_Method>(methodsDefinitions);
            
        }

        public bool HasSameSignatureAs(Definition_Class otherElement)
        {
            return this.type.HasSameSignatureAs(otherElement.type);
        }

        public string SignatureAsString()
        {
            return this.type.SignatureAsString();
        }

        //IScopable
        public IScopable getParentScope()
        {
            return this.program;
        }

        public List<Definition_Class> getClasses()
        {
            return new List<Definition_Class>();
        }

        public List<Definition_Function> getFunctions()
        {
            return new List<Definition_Function>();
        }

        public List<LType> getParameters()
        {
            return new List<LType>();
        }

        public List<Declaration_Var> getVars()
        {
            throw new NotImplementedException();
        }

        public List<LAction> getActions()
        {
            return new List<LAction>();
        }

        public Declaration_Var getVarNamed(string name)
        {
            foreach(Declaration_LAttribute a in this.attributesDeclarations.ToList())
            {
                if(a.name.Equals(name))
                {
                    return a;
                }
            }
            return null;
        }

        public Definition_Class getClassForType(LType type)
        {
            return null;
        }

        public Definition_Function getFunctionForGivenNameAndParameters(string name, ParametersList parameters)
        {
            return null;
        }

        public Definition_Method getMethodForGivenTypeNameAndParameters(LType type, string name, ParametersList parameters)
        {
            if (!type.MatchesWith(this.type)) return null;

            foreach(Definition_Method method in this.methodsDefinitions.ToList())
            {
                if(method.matchesWithNameAndParameters(name, parameters))
                {
                    return method;
                }
            }
            return null;
        }

        public int getIndexOfFunctionWithNameAndParameters(string name, ParametersList parameters)
        {
            throw new NotImplementedException();
        }

        public int getNumOfVars()
        {
            return this.attributesDeclarations.Count();
        }

        public int getIndexOfVarNamed(string name)
        {
            for(int i = 0; i < this.attributesDeclarations.Count(); i++)
            {
                if(this.attributesDeclarations.ToList()[i].name.Equals(name))
                {
                    return i + this.GetScopeManager().GetIndexOfFirstVarInScope();
                }
            }
            return -1;
        }

        public IScopable GetParentScope()
        {
            return this.program; 
        }

        public ScopeManager GetScopeManager()
        {
            throw new NotImplementedException();
        }
    }
}
