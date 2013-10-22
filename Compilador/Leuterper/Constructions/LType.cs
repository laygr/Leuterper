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
            : this(name, typeVariables, LObject.type)
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
    }
}
