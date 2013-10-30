using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    class LList : LObject
    {
        new public static LType type = LList.CreateLListType();
        public LType instanceType;
        public List<Expression> elements { get; set; }

        public LList(int line, LType type, List<Expression> elements) : base(line)
        {
            this.instanceType= type;
            this.elements = elements;

            foreach(Expression e in elements)
            {
                if(!e.getType().typeOrSuperTypeUnifiesWith(type))
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
            LType membersType = new LType(0, "A");
            List<LType> typeVariables = new List<LType>(new LType[] { membersType });

            LType listType = new LType(0, "List", typeVariables);
            return listType;
        }

        public override string encodeAsString()
        {
            
            throw new NotImplementedException();
        }

        public override void secondPass(LeuterperCompiler compiler)
        { }

        public override void generateCode(LeuterperCompiler compiler)
        {
            base.generateCode(compiler);
            compiler.addLiteral(new MachineInstructions.Literal("list", this.encodeAsString()));
        }
    }
}
