using Leuterper.Exceptions;
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
            this.instanceType = new LType(line, "List", new List<LType>(new LType[]{type}));
            this.elements = elements;

            foreach(Expression e in elements)
            {
                if(!e.getType().typeOrSuperTypeUnifiesWith(type))
                {
                    throw new SemanticErrorException("Type mismatch in element of list.", this.getLine());
                }
            }
        }

        public override void scopeSetting()
        {
            this.elements.ForEach(e => e.setScope(this.getScope()));
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
            return String.Format("{0}", this.elements.Count());
        }

        public override void codeGenerationPass(LeuterperCompiler compiler)
        {
            foreach(Expression e in this.elements)
            {
                e.shouldBePushedToStack = true;
                e.codeGenerationPass(compiler);
            }
            if(this.shouldBePushedToStack)
            {
                compiler.addAction(new MachineInstructions.NewP(30));
            }
            else
            {
                compiler.addAction(new MachineInstructions.New(30));
            }

            if (this.shouldBePushedToStack)
            {
                compiler.addAction(new MachineInstructions.AddP(this.elements.Count()));
            }
            else
            {
                compiler.addAction(new MachineInstructions.Add(this.elements.Count()));
            }
        }
    }
}
