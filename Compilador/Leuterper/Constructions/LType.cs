using Leuterper.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    class LType : Construction, ISignable<LType>, IRedefinable<LType>
    {
        private String name { get; set; }
        public List<LType> typeVariables { get; set; }
        public LType parentType { get; set; }
        public Boolean rootIsDefined { get; set; }
        public Boolean isCompletelyDefined { get; set; }
        private LClass definingClass { get; set; }
        public int typeVariableIndex;
        private Boolean shouldRedefineItsClass;


        public LType(int line, String name, List<LType> typeVariables, LType parentType) : base(line)
        {
            this.name = name;
            this.isCompletelyDefined = false;
            this.typeVariables = typeVariables;
            typeVariables.ForEach(tv => tv.isCompletelyDefined = false);
            this.parentType = parentType;
            if (this.parentType == null && (!this.getName().Equals("Object") || this.getName().Equals("Void")))
            {
                this.parentType = LObject.type;
            }
            this.typeVariableIndex = -1;
            this.shouldRedefineItsClass = false;
        }
        public LType(int line, String name, List<LType> typeVariables)
            : this(line, name, typeVariables, null)
        { }

        public LType(int line, String name)
            : this(line, name, new List<LType>())
        { }

        public string SignatureAsString()
        {
            return String.Format("{0}", this.getName());
        }
        public static Parameter typeToParameter(LType t)
        {
            return new Parameter(t.getLine(), t, "fakeParam");
        }
        public static List<Parameter> typesToParameters(List<LType> types)
        {
            return types.ConvertAll(new Converter<LType, Parameter>(typeToParameter));
        }
        public override void setScope(IScope scope)
        {
            base.setScope(scope);
            if (this.parentType != null)
            {
                this.parentType.setScope(scope);
            }
        }
        public static String listOfTypesAsString(List<LType> types)
        {
            string result = "";
            foreach (LType t in types)
            {
                result += t.SignatureAsString() + ", ";
            }
            return result;
        }

        //Type matching/unification
        public static Boolean listOfTypesUnify(List<LType> a, List<LType> b)
        {
            if (a.Count() != b.Count()) return false;
            for (int i = 0; i < b.Count(); i++)
            {
                if (!b[i].typeOrSuperTypeUnifiesWith(a[i])) return false;
            }
            return true;
        }
        public static Boolean listOfTypesMatch(List<LType> a, List<LType> b)
        {
            if (a.Count() != b.Count()) return false;
            for (int i = 0; i < b.Count(); i++)
            {
                if (!a[i].HasSameSignatureAs(b[i])) return false;
            }
            return true;
        }
        public Boolean typeOrSuperTypeUnifiesWith(LType otherType)
        {
            if (this.UnifiesWith(otherType))
            {
                return true;
            }
            else if (this.parentType != null)
            {
                return this.parentType.typeOrSuperTypeUnifiesWith(otherType);
            }
            return false;
        }
        public bool UnifiesWith(LType otherType)
        {
            if (this.typeVariables.Count() != otherType.typeVariables.Count()) return false;
            for (int i = 0; i < this.typeVariables.Count(); i++)
            {
                if (!this.typeVariables[i].UnifiesWith(otherType.typeVariables[i])) return false;
            }

            if (!this.rootIsDefined || !otherType.rootIsDefined)
            {
                return true;
            }
            else
            {
                return this.getName().Equals(otherType.getName());
            }
        }

        public bool isNamed(string name)
        {
            return this.getName().Equals(name);
        }

        public bool HasSameSignatureAs(LType otherElement)
        {
            if (this.typeVariables.Count() != otherElement.typeVariables.Count()) return false;
            if (!this.isCompletelyDefined && !otherElement.isCompletelyDefined) return true;
            if (!this.getName().Equals(otherElement.getName())) return false;
            for (int i = 0; i < this.typeVariables.Count(); i++)
            {
                if (!this.typeVariables[i].HasSameSignatureAs(otherElement.typeVariables[i])) return false;
            }
            return true;
        }

        public override void secondPass(LeuterperCompiler compiler)
        {
            this.definingClass = this.getDefiningClass();

            foreach (LType vt in this.typeVariables)
            {
                vt.setScope(this.getScope());
                vt.secondPass(compiler);
            }

            if (this.rootIsDefined)
            {
                this.isCompletelyDefined = true;
                foreach (LType v in this.typeVariables)
                {
                    if (!v.isCompletelyDefined)
                    {
                        this.isCompletelyDefined = false;
                        break;
                    }
                }
            }
            else
            {
                findVariableTypeIndex();
            }
            if (this.parentType != null)
            {
                this.parentType.setScope(this.getScope());
                this.parentType.secondPass(compiler);
            }
        }
        public override void thirdPass()
        {
            if (this.shouldRedefineItsClass)
            {
                this.redefineWithSubstitutionTypes(this.typeVariables);
            }
        }
        public void findVariableTypeIndex()
        {
            if (this.rootIsDefined) return;
            LClass classScope = ScopeManager.getClassScope(this.getScope());
            if(classScope == null)
            {
                throw new SemanticErrorException("Used undeclared type: " + this, this.getLine());
            }
            List<LType> classTypeVariables = classScope.getType().typeVariables;

            for(int i = 0; i < classTypeVariables.Count(); i++)
            {
                if(classTypeVariables[i].getName().Equals(this.getName()))
                {
                    this.typeVariableIndex = i;
                    return;
                }
            }
            throw new SemanticErrorException("Type variable undeclared: " + this.getName(), this.getLine());
        }

        public string getName()
        {
            return this.name;
        }
        public override void generateCode(LeuterperCompiler compiler) { }

        public void setShouldStartRedefinition(Boolean shouldBeRedefined)
        {
            this.shouldRedefineItsClass = shouldBeRedefined;
        }
        public Boolean getShouldBeRedefined()
        {
            return this.shouldRedefineItsClass;
        }
        public LType clone()
        {
            LType clone = new LType(this.getLine(), this.name);
            if (this.parentType != null)
            {
                clone.parentType = this.parentType.clone();
            }
            else
            {
                clone.parentType = null;
            }
            clone.rootIsDefined = this.rootIsDefined;
            clone.isCompletelyDefined = this.isCompletelyDefined;
            clone.typeVariableIndex = this.typeVariableIndex;
            clone.shouldRedefineItsClass = this.shouldRedefineItsClass;
            clone.definingClass = this.definingClass;
            
            for(int i = 0; i < this.typeVariables.Count(); i++)
            {
                clone.typeVariables.Add(this.typeVariables[i].clone());
            }

            return clone;

        }
        public LType substituteTypeAndVariableTypesWith(List<LType> instantiatedTypes)
        {
            if(this.isCompletelyDefined) return this;
            LType result = this.clone();
            if (!result.rootIsDefined) return instantiatedTypes[this.typeVariableIndex];
            for(int i = 0; i < this.typeVariables.Count(); i++)
            {
                result.typeVariables[i] = this.typeVariables[i].substituteTypeAndVariableTypesWith(instantiatedTypes);
            }
            return result;

        }
        public LType redefineWithSubstitutionTypes(List<LType> instantiatedTypes)
        {
            if (this.isCompletelyDefined) return this;
            LType newType = this;
            if (!this.rootIsDefined)
            {
                newType = instantiatedTypes[this.typeVariableIndex];
            }
            if(newType.definingClass != null)
            {
                newType.definingClass = newType.definingClass.reinstantiateWithSubstitution(this, instantiatedTypes);
            }
            if (newType.parentType != null)
            {
                newType.parentType.redefineWithSubstitutionTypes(instantiatedTypes);
            }
            return newType;
        }
        public LClass getDefiningClass()
        {
            if(this.definingClass == null)
            {
                this.definingClass = ScopeManager.getClassForType(this.getScope(), this);
            }
            this.rootIsDefined = this.definingClass != null;
            return definingClass;
        }
        public override String ToString()
        {
            string result = this.getName() + "[";
            foreach(LType t in this.typeVariables)
            {
                result += t + " ";
            }
            result += "]";
            return result;
        }
    }
}
