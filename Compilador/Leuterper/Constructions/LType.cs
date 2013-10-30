using Leuterper.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    class LType : Construction, IIdentifiable<LType>
    {
        public LType rootType { get; set; }
        private String name { get; set; }
        public List<LType> typeVariables { get; set; }
        public LType parentType { get; set; }
        public Boolean rootIsDefined { get; set; }
        public Boolean isCompletelyDefined { get; set; }
        public LClass definingClass { get; set; }
        public int typeVariableIndex;
        public Boolean shouldRedefinesItsClass;


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
            this.shouldRedefinesItsClass = false;
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

        //Type matching
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
            if (this.getName().Equals("Matrix"))
            {
                Console.WriteLine();
            }
            this.definingClass = this.getScope().getScopeManager().getClassForType(this);
            if (this.definingClass == null)
            {
                this.rootIsDefined = false;
            }
            else
            {
                this.rootIsDefined = true;
            }

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
            if (this.shouldRedefinesItsClass)
            {
                this.reinstantiateWithSubstitution(this.typeVariables);
            }
        }
        public void findVariableTypeIndex()
        {
            if (this.rootIsDefined) return;
            LClass classScope = this.getScope().getScopeManager().getClassScope();
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

        public override void generateCode(LeuterperCompiler compiler)
        {
            throw new NotImplementedException();
        }

        public LType reinstantiateWithSubstitution(List<LType> instantiatedTypes)
        {
            if (this.isCompletelyDefined) return this;
            LType newType = this;
            if (!this.rootIsDefined)
            {
                if(this.typeVariableIndex == -1)
                {
                    Console.WriteLine("What");
                }
                if(this.typeVariableIndex >= instantiatedTypes.Count())
                {
                    Console.WriteLine("What");
                }
                newType = instantiatedTypes[this.typeVariableIndex];
            }
            if(newType.typeVariables.Count() != this.typeVariables.Count())
            {
                throw new SemanticErrorException(String.Format("Types can't unify:\n\tType 1: {0}\n\tType 2: {1}", this, newType), this.getLine());
            }
            if(newType.definingClass != null)
            {
                newType.definingClass.reinstantiateWithSubstitution(this, instantiatedTypes);
            }
            if (newType.parentType != null)
            {
                newType.parentType.reinstantiateWithSubstitution(instantiatedTypes);
            }
            return newType;
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
