﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    class LChar : LObject
    {
        int value;
        Boolean countAsLiteral;
        new public static LType type = new LType(0, "Char");
        public LChar(int line, String aChar, bool countAsLiteral) : this(line, aChar)
        {
            this.countAsLiteral = countAsLiteral;
        }
        public LChar(int line, String aChar) : base(line)
        {
            this.countAsLiteral = true;
            aChar = aChar.Substring(1, aChar.Length - 2);
            if(aChar.Length == 1)
            {
                this.value = aChar[0];
            }
            else
            {
                switch(aChar)
                {
                    case "\n":
                        this.value = '\n';
                        break;

                    case "\\":
                        this.value = '\\';
                        break;

                    case "\t":
                        this.value = '\t';
                        break;

                    default:
                        throw new Exception("Char invalido");
                }
            }
        }

        public override string encodeAsString()
        {
            return String.Format("{0}", this.value);
        }


        public override void codeGenerationPass(LeuterperCompiler compiler)
        {
            if (this.countAsLiteral)
            {
                base.codeGenerationPass(compiler);
                compiler.addLiteral(new MachineInstructions.Literal("Char", this.encodeAsString()));
            }
        }
    }
}
