using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    class Wild_Type
    {
        public String id { get; set; }
        public LType statedType { get; set; }


        public Wild_Type(String id, LType statedType)
        {
            this.id = id;
            this.statedType = statedType;
        }

        static public LType wildTypePlaceHolder()
        {
            return new LType("A");
        }

        public Wild_Type(String id) : this(id, null)
        { }
    }
}
