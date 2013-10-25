using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    class LType : IIdentifiable<LType>
    {
        public String name { get; set; }
        public List<Wild_Type> wildTypes { get; set; }
        public LType parentType { get; set; }

        public LType(String name, List<Wild_Type> typeVariables, LType parentType)
        {
            this.name = name;
            this.wildTypes = typeVariables;
            this.parentType = parentType;
        }

        public LType(String name, List<Wild_Type> typeVariables)
            : this(name, typeVariables, null)
        { }

        public LType(String name)
            : this(name, new List<Wild_Type>())
        { }

        public Boolean TypeOrSuperTypeMatchWith(LType otherType)
        {
            if (this.MatchesWith(otherType))
            {
                return true;
            }
            else if(this.parentType != null)
            {
                return this.parentType.TypeOrSuperTypeMatchWith(otherType);
            }
            return false;
        }

        public Boolean MatchesWith(LType otherType)
        {
            if (!this.HasSameSignatureAs(otherType)) return false;
            if (this.wildTypes.Count() != otherType.wildTypes.Count()) return false;

            for (int i = 0; i < this.wildTypes.Count(); i++)
            {
                if (this.wildTypes[i].statedType == null || otherType.wildTypes[i].statedType == null) continue;
                if (!this.wildTypes[i].statedType.MatchesWith(otherType.wildTypes[i].statedType)) return false;
            }
            return true;
        }

        public bool HasSameSignatureAs(LType otherElement)
        {
            return this.name.Equals(otherElement.name);
        }

        public string SignatureAsString()
        {
            return String.Format("{0}", this.name);
        }

        public static Boolean listOfTypesAreCompatible(List<LType> a, List<LType> b)
        {
            if (a.Count() != b.Count()) return false;
            for (int i = 0; i < b.Count(); i++)
            {
                if (!b[i].TypeOrSuperTypeMatchWith(a[i])) return false;
            }
            return true;
        }

        public static Boolean listOfTypesMatch(List<LType> a, List<LType> b)
        {
            if (a.Count() != b.Count()) return false;
            for (int i = 0; i < b.Count(); i++)
            {
                if (!a[i].MatchesWith(b[i])) return false;
            }
            return true;
        }

        public int getIndexOfAttribute(string attributeName)
        {
            return 0;
        }

        public static Parameter typeToParameter(LType t)
        {
            return new Parameter(t, "");
        }
        public static List<Parameter> typesToParameters(List<LType> types)
        {
            return types.ConvertAll(new Converter<LType, Parameter>(typeToParameter));
        }

        public static String listOfTypesAsString(List<LType> types)
        {
            string result = "";
            foreach(LType t in types)
            {
                result += t.SignatureAsString() + ", ";
            }
            return result;
        }
    }
}
