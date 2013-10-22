using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    class LList : LObject
    {
        LType instanceType;
        List<Expression> elements { get; set; }

        public LList(int line, LType type, List<Expression> elements) : base(line)
        {
            this.instanceType= type;
            this.elements = elements;

            foreach(Expression e in elements)
            {
                if(!e.getType().TypeOrSuperTypeMatchWith(type))
                {
                    Console.WriteLine("Type mismatch in element of list.");
                    Console.ReadKey();
                    Environment.Exit(0);
                }
            }
        }

        override public LType getType()
        {
            return this.instanceType;
        }

        public static LType CreateLListType()
        {
            Wild_Type wildType = new Wild_Type("A");
            List<Wild_Type> wildTypes = new List<Wild_Type>(new Wild_Type[] { wildType });

            return new LType("List", wildTypes);
        }
    }
}
