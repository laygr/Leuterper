﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    class LString : LList
    {
        static LType type = LString.LStringType();
        public LString(int line, String value) : base(line, LChar.getType(), new List<Expression>())
        {
            LType type = LString.type;
            List<Expression> chars = new List<Expression>();

            value = value.Substring(1, value.Length - 1);

            String nextString;
            LChar nextChar;
            while(value.Length > 0)
            {
                if(value[0] == '\\')
                {
                    nextString = value.Substring(0, 1);
                    value = value.Substring(2);
                }else{
                    nextString = value.Substring(0, 0);
                    value = value.Substring(1);
                }
                nextChar = new LChar(line, "\'" + nextString + "\'");
                chars.Add(nextChar);
            }
        }

        public static LType LStringType()
        {
            LType listType = LList.CreateLListType();
            LType charType = LChar.getType();
            listType.wildTypes[0].statedType = charType;

            return new LType("String", new List<Wild_Type>(), listType);
        }

        public new LType getType()
        {
            return LString.type;
        }
    }
}