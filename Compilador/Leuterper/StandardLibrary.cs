using Leuterper.Constructions;
using System.Collections.Generic;

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
                new ConstructorSpecial(0, "Object", new List<Parameter>(), new List<Expression>(), new List<IAction>(), 0),
                new MethodSpecial(0, LBoolean.type, "==", Utils.typesToParameters(o), new List<IAction>(), 1),
                new MethodSpecial(0, LString.type, "toString", Utils.typesToParameters(e), new List<IAction>(), 2),
           
                //number
                //Constructor de valor default:
                new ConstructorSpecial(0, "Number", Utils.typesToParameters(n), new List<Expression>(), new List<IAction>(), 3),
                new MethodSpecial(0, LBoolean.type, "==", Utils.typesToParameters(n), new List<IAction>(), 4),
                new MethodSpecial(0, LString.type, "toString", Utils.typesToParameters(e), new List<IAction>(), 5),
                new MethodSpecial(0, LNumber.type, "+", Utils.typesToParameters(n), new List<IAction>(), 6),
                new MethodSpecial(0, LNumber.type, "-", Utils.typesToParameters(n), new List<IAction>(), 7),
                new MethodSpecial(0, LNumber.type, "*", Utils.typesToParameters(n), new List<IAction>(), 8),
                new MethodSpecial(0, LNumber.type, "/", Utils.typesToParameters(n), new List<IAction>(), 9),
                new MethodSpecial(0, LNumber.type, "^", Utils.typesToParameters(n), new List<IAction>(), 10),
                new MethodSpecial(0, LBoolean.type, "!=", Utils.typesToParameters(n), new List<IAction>(), 11),
                new MethodSpecial(0, LVoid.type, "+=", Utils.typesToParameters(n), new List<IAction>(), 12),
                new MethodSpecial(0, LVoid.type, "-=", Utils.typesToParameters(n), new List<IAction>(), 13),
                new MethodSpecial(0, LVoid.type, "*=", Utils.typesToParameters(n), new List<IAction>(), 14),
                new MethodSpecial(0, LVoid.type, "/=", Utils.typesToParameters(n), new List<IAction>(), 15),
                new MethodSpecial(0, LBoolean.type, "<", Utils.typesToParameters(n), new List<IAction>(), 16),
                new MethodSpecial(0, LBoolean.type, "<=", Utils.typesToParameters(n), new List<IAction>(), 17),
                new MethodSpecial(0, LBoolean.type, ">=", Utils.typesToParameters(n), new List<IAction>(), 18),
                new MethodSpecial(0, LBoolean.type, ">", Utils.typesToParameters(n), new List<IAction>(), 19),

                //boolean
                new ConstructorSpecial(0, "Boolean", new List<Parameter>(), new List<Expression>(), new List<IAction>(), 20),
                new MethodSpecial(0, LBoolean.type, "==", Utils.typesToParameters(b), new List<IAction>(), 21),
                new MethodSpecial(0, LBoolean.type, "||", Utils.typesToParameters(b), new List<IAction>(), 22),
                new MethodSpecial(0, LBoolean.type, "&&", Utils.typesToParameters(b), new List<IAction>(), 23),
                new MethodSpecial(0, LBoolean.type, "not", Utils.typesToParameters(e), new List<IAction>(), 24),
                new MethodSpecial(0, LString.type, "toString", Utils.typesToParameters(e), new List<IAction>(), 25),

                //char
                new ConstructorSpecial(0, "Char", new List<Parameter>(), new List<Expression>(), new List<IAction>(), 26),
                new MethodSpecial(0, LBoolean.type, "==", Utils.typesToParameters(c), new List<IAction>(), 27),
                new MethodSpecial(0, LString.type, "toString", Utils.typesToParameters(e), new List<IAction>(), 28),

                //list
                new ConstructorSpecial(0, "List", new List<Parameter>(), new List<Expression>(), new List<IAction>(), 29),
                new ConstructorSpecial(0, "List", Utils.typesToParameters(n), new List<Expression>(), new List<IAction>(), 30),
                new MethodSpecial(0, LBoolean.type, "==", Utils.typesToParameters(l), new List<IAction>(), 31),
                new MethodSpecial(0, LString.type, "toString", Utils.typesToParameters(e), new List<IAction>(), 32),
                new MethodSpecial(0, LVoid.type, "add", Utils.typesToParameters(a), new List<IAction>(), 33),
                new MethodSpecial(0, LVoid.type, "insertAt", Utils.typesToParameters(na), new List<IAction>(), 34),
                new MethodSpecial(0, LVoid.type, "removeAt", Utils.typesToParameters(n), new List<IAction>(), 35),
                new MethodSpecial(0, LNumber.type, "count", Utils.typesToParameters(e), new List<IAction>(), 36),
                new MethodSpecial(0, new LType(0, "A"), "get", Utils.typesToParameters(n), new List<IAction>(), 37),
                new MethodSpecial(0, LVoid.type, "set", Utils.typesToParameters(na), new List<IAction>(), 38),
            
                //string
                new ConstructorSpecial(0, "String", new List<Parameter>(), new List<Expression>(), new List<IAction>(), 39),
                new MethodSpecial(0, LString.type, "==", Utils.typesToParameters(s), new List<IAction>(), 40),
                new MethodSpecial(0, LNumber.type, "toNumber", Utils.typesToParameters(e), new List<IAction>(), 41),
                new MethodSpecial(0, LString.type, "+", Utils.typesToParameters(s), new List<IAction>(), 42),
                new MethodSpecial(0, LVoid.type, "+=", Utils.typesToParameters(s), new List<IAction>(), 43)
            });

            this.standardClasses = new List<LClass>(new LClass[]{
                //void
                new LClassSpecial(0, LVoid.type, null, new UniquesList<LAttribute>(), new UniquesList<Class_Procedure>(), (int)StandardClasses.LVoid),
                //object
                new LClassSpecial(0, LObject.type, null, new UniquesList<LAttribute>(),
                    new UniquesList<Class_Procedure>(standardProcedures.GetRange(0, 3)), (int)StandardClasses.LObject),
                //number
                new LClassSpecial(0, LNumber.type, LObject.type, new UniquesList<LAttribute>(),
                    new UniquesList<Class_Procedure>(standardProcedures.GetRange(3, 17)), (int)StandardClasses.LNumber),
                //boolean
                new LClassSpecial(0, LBoolean.type, LObject.type, new UniquesList<LAttribute>(),
                    new UniquesList<Class_Procedure>(standardProcedures.GetRange(20, 6)), (int)StandardClasses.LChar),
                //char
                new LClassSpecial(0, LChar.type, LObject.type, new UniquesList<LAttribute>(),
                    new UniquesList<Class_Procedure>(standardProcedures.GetRange(26,3)), (int)StandardClasses.LChar),
                //list
                new LClassSpecial(0, LList.type, LObject.type, new UniquesList<LAttribute>(),
                    new UniquesList<Class_Procedure>(standardProcedures.GetRange(29, 10)), (int)StandardClasses.LList),
                //string
                new LClassSpecial(0, LString.type, LString.parentType, new UniquesList<LAttribute>(),
                    new UniquesList<Class_Procedure>(standardProcedures.GetRange(39, 5)), (int)StandardClasses.LString)
            });

            this.standardFunctions = new List<Function>(new Function[]
            {
                new FunctionSpecial(0, LString.type, "read", Utils.typesToParameters(e), new List<IAction>(), 44),
                new FunctionSpecial(0, LVoid.type, "write", Utils.typesToParameters(s), new List<IAction>(), 45),
            });
        }
    }
}