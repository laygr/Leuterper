using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leuterper.Constructions;

namespace Leuterper
{
    class StandardLibrary
    {
        public enum StandardClasses:int { LVoid=0, LObject, LNumber, LBoolean, LChar, LList, LString };


        static List<LType> e = new List<LType>();
        static List<LType> o = new List<LType>(new LType[] { LObject.type });
        static List<LType> oo = new List<LType>(new LType[] { LObject.type, LObject.type });
        static List<LType> n = new List<LType>(new LType[] { LNumber.type });
        static List<LType> na = new List<LType>(new LType[] { LNumber.type, new LType(0, "A") });
        static List<LType> nn = new List<LType>(new LType[] { LNumber.type, LNumber.type });
        static List<LType> l = new List<LType>(new LType[] { LList.type });
        static List<LType> ll = new List<LType>(new LType[] { LList.type, LList.type });
        static List<LType> la = new List<LType>(new LType[] { LList.type, new LType(0, "A") });
        static List<LType> a = new List<LType>(new LType[] { new LType(0, "A") });
        static List<LType> ln = new List<LType>(new LType[] { LList.type, LNumber.type });
        static List<LType> an = new List<LType>(new LType[] { new LType(0, "A"), LNumber.type });
        static List<LType> c = new List<LType>(new LType[] { LChar.type });
        static List<LType> cc = new List<LType>(new LType[] { LChar.type, LChar.type });
        static List<LType> s = new List<LType>(new LType[] { LString.type });
        static List<LType> ss = new List<LType>(new LType[] { LString.type, LString.type });
        static List<LType> b = new List<LType>(new LType[] { LBoolean.type });
        static List<LType> bb = new List<LType>(new LType[] { LBoolean.type, LBoolean.type });

        public static StandardLibrary singleton;

        public List<Class_Procedure> standardProcedures;
        public List<LClass> standardClasses;
        public List<Function> standardFunctions;

        public StandardLibrary()
        {
            this.standardProcedures = new List<Class_Procedure>(new Class_Procedure[]{
                //void

                //object
                //Constructor default
                new ConstructorSpecial(0, "Object", new List<Parameter>(), new List<Expression>(), null, 0),
                new MethodSpecial(0, LBoolean.type, "==", LType.typesToParameters(o), null, 1),
                new MethodSpecial(0, LString.type, "toString", LType.typesToParameters(e), null, 2),
           
                //number
                //Constructor de valor default:
                new ConstructorSpecial(0, "Number", new List<Parameter>(), new List<Expression>(), null, 3),
                new MethodSpecial(0, LBoolean.type, "==", LType.typesToParameters(n), null, 4),
                new MethodSpecial(0, LString.type, "toString", LType.typesToParameters(e), null, 5),
                new MethodSpecial(0, LNumber.type, "+", LType.typesToParameters(n), null, 6),
                new MethodSpecial(0, LNumber.type, "-", LType.typesToParameters(n), null, 7),
                new MethodSpecial(0, LNumber.type, "*", LType.typesToParameters(n), null, 8),
                new MethodSpecial(0, LNumber.type, "/", LType.typesToParameters(n), null, 9),
                new MethodSpecial(0, LNumber.type, "^", LType.typesToParameters(n), null, 10),
                new MethodSpecial(0, LBoolean.type, "!=", LType.typesToParameters(n), null, 11),
                new MethodSpecial(0, LNumber.type, "+=", LType.typesToParameters(n), null, 12),
                new MethodSpecial(0, LNumber.type, "-=", LType.typesToParameters(n), null, 13),
                new MethodSpecial(0, LBoolean.type, "<", LType.typesToParameters(n), null, 14),
                new MethodSpecial(0, LBoolean.type, "<=", LType.typesToParameters(n), null, 15),
                new MethodSpecial(0, LBoolean.type, ">=", LType.typesToParameters(n), null, 16),
                new MethodSpecial(0, LBoolean.type, ">", LType.typesToParameters(n), null, 17),
           
                //boolean
                new ConstructorSpecial(0, "Boolean", new List<Parameter>(), new List<Expression>(), null, 18),
                new MethodSpecial(0, LBoolean.type, "==", LType.typesToParameters(b), null, 19),
                new MethodSpecial(0, LString.type, "toString", LType.typesToParameters(e), null, 20),

                //char
                new ConstructorSpecial(0, "Char", new List<Parameter>(), new List<Expression>(), null, 21),
                new MethodSpecial(0, LBoolean.type, "==", LType.typesToParameters(c), null, 22),
                new MethodSpecial(0, LString.type, "toString", LType.typesToParameters(e), null, 23),

                //list
                new ConstructorSpecial(0, "List", new List<Parameter>(), new List<Expression>(), null, 24),
                new MethodSpecial(0, LBoolean.type, "==", LType.typesToParameters(l), null, 25),
                new MethodSpecial(0, LString.type, "toString", LType.typesToParameters(e), null, 26),
                new MethodSpecial(0, LVoid.type, "add", LType.typesToParameters(a), null, 27),
                new MethodSpecial(0, LVoid.type, "insertAt", LType.typesToParameters(na), null, 28),
                new MethodSpecial(0, LVoid.type, "removeAt", LType.typesToParameters(n), null, 29),
                new MethodSpecial(0, LNumber.type, "count", LType.typesToParameters(e), null, 30),
                new MethodSpecial(0, new LType(0, "A"), "get", LType.typesToParameters(n), null, 31),
                new MethodSpecial(0, LVoid.type, "set", LType.typesToParameters(an), null, 32),
            
            
                //string
                new ConstructorSpecial(0, "String", new List<Parameter>(), new List<Expression>(), null, 33),
                new MethodSpecial(0, LString.type, "==", LType.typesToParameters(s), null, 34)
            });

            this.standardClasses = new List<LClass>(new LClass[]{
                //void
                new LClassSpecial(0, LVoid.type, null, new List<LAttribute>(), new List<Class_Procedure>(), (int)StandardClasses.LVoid),
                //object
                new LClassSpecial(0, LObject.type, null, new List<LAttribute>(), new List<Class_Procedure>(new Class_Procedure[]{
                    standardProcedures[0], //Constructor
                    standardProcedures[1],
                    standardProcedures[2]}
                    ), (int)StandardClasses.LObject),

                //number
                new LClassSpecial(0, LNumber.type, LObject.type, new List<LAttribute>(), new List<Class_Procedure>(new Class_Procedure[]{
                    standardProcedures[3], //Constructor
                    standardProcedures[4],
                    standardProcedures[5],
                    standardProcedures[6],
                    standardProcedures[7],
                    standardProcedures[8],
                    standardProcedures[9],
                    standardProcedures[10],
                    standardProcedures[11],
                    standardProcedures[12],
                    standardProcedures[13],
                    standardProcedures[14],
                    standardProcedures[15],
                    standardProcedures[16],
                    standardProcedures[17]}
                    ), (int)StandardClasses.LNumber),

                //boolean
                new LClassSpecial(0, LBoolean.type, LObject.type, new List<LAttribute>(), new List<Class_Procedure>(new Class_Procedure[]{
                    standardProcedures[18],
                    standardProcedures[19],
                    standardProcedures[20]
                }), (int)StandardClasses.LChar),

                //char
                new LClassSpecial(0, LChar.type, LObject.type, new List<LAttribute>(), new List<Class_Procedure>(new Class_Procedure[]{
                    standardProcedures[21],
                    standardProcedures[22],
                    standardProcedures[23]
                }), (int)StandardClasses.LChar),

                //list
                new LClassSpecial(0, LList.type, LObject.type, new List<LAttribute>(), new List<Class_Procedure>(new Class_Procedure[]{
                    standardProcedures[24],
                    standardProcedures[25],
                    standardProcedures[26],
                    standardProcedures[27],
                    standardProcedures[28],
                    standardProcedures[29],
                    standardProcedures[30],
                    standardProcedures[31],
                    standardProcedures[32],
                }), (int)StandardClasses.LList),

                //string
                new LClassSpecial(0, LString.type, LString.type.parentType, new List<LAttribute>(), new List<Class_Procedure>(new Class_Procedure[]{
                    standardProcedures[33],
                    standardProcedures[34]
                }), (int)StandardClasses.LString)
            });

            this.standardFunctions = new List<Function>(new Function[]{
                new FunctionSpecial(0, LString.type, "read", LType.typesToParameters(e), null, 35),
                new FunctionSpecial(0, LVoid.type, "write", LType.typesToParameters(s), null, 36),
                new FunctionSpecial(0, LNumber.type, "stringToNumber", LType.typesToParameters(s), null, 37)});
        }
    }
}