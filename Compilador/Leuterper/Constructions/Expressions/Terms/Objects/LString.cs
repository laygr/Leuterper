using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    class LString : LList
    {
        new public static LType type = LString.LStringType();
        public LString(int line, String value) : base(line, LChar.type, new List<Expression>())
        {
            LType type = LString.type;
            List<Expression> chars = new List<Expression>();

            value = value.Substring(1, value.Length - 2);

            String nextString;
            LChar nextChar;
            while(value.Length > 0)
            {
                if(value[0] == '\\')
                {
                    nextString = value.Substring(0, 2);
                    value = value.Substring(2);
                }else{
                    nextString = value.Substring(0, 1);
                    value = value.Substring(1);
                }
                nextChar = new LChar(line, "\'" + nextString + "\'");
                elements.Add(nextChar);
            }
        }

        public static LType LStringType()
        {
            LType listType = LList.CreateLListType();
            listType.typeVariables[0] = LChar.type;
            LType stringType = new LType(0, "String");
            stringType.parentType = listType;
            return stringType;
        }

        public new LType getType()
        {
            return LString.type;
        }

        public override string encodeAsString()
        {
            string result = "";
            for(int i = 0; i < this.elements.Count() - 1; i++)
            {
                result += ((LChar)this.elements[i]).encodeAsString() + " ";
            }
            if(elements.Count() > 0){
                result += ((LChar)this.elements[this.elements.Count() - 1]).encodeAsString();
            }
            return result;
        }

        public override void codeGenerationPass(LeuterperCompiler compiler)
        {
            this.literalIndex = compiler.literals.Count();
            if (this.shouldBePushedToStack)
            {
                compiler.addAction(new MachineInstructions.Push(this.literalIndex));
            }
            compiler.addLiteral(new MachineInstructions.Literal("String", this.encodeAsString()));
        }
    }
}
