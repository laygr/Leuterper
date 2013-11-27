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


        public List<Class_Procedure> standardProcedures;
        public List<LClassTemplate> standardClasses;
        public List<Function> standardFunctions;

        public StandardLibrary(Program program)
        {
            this.standardProcedures = new List<Class_Procedure>(new Class_Procedure[]{
                //void

                //object
                //Constructor default
                new ConstructorSpecial(0, "Object", new List<Parameter>(), new List<Expression>(), new List<IAction>()),
                new MethodSpecial(0, LBoolean.type, "==", Utils.typesToParameters(o), new List<IAction>()),
                new MethodSpecial(0, LString.type, "toString", Utils.typesToParameters(e), new List<IAction>()),
           
                //number
                //Constructor de valor default:
                new ConstructorSpecial(0, "Number", Utils.typesToParameters(n), new List<Expression>(), new List<IAction>()),
                new MethodSpecial(0, LBoolean.type, "==", Utils.typesToParameters(n), new List<IAction>()),
                new MethodSpecial(0, LString.type, "toString", Utils.typesToParameters(e), new List<IAction>()),
                new MethodSpecial(0, LNumber.type, "+", Utils.typesToParameters(n), new List<IAction>()),
                new MethodSpecial(0, LNumber.type, "-", Utils.typesToParameters(n), new List<IAction>()),
                new MethodSpecial(0, LNumber.type, "*", Utils.typesToParameters(n), new List<IAction>()),
                new MethodSpecial(0, LNumber.type, "/", Utils.typesToParameters(n), new List<IAction>()),
                new MethodSpecial(0, LNumber.type, "^", Utils.typesToParameters(n), new List<IAction>()),
                new MethodSpecial(0, LBoolean.type, "!=", Utils.typesToParameters(n), new List<IAction>()),
                new MethodSpecial(0, LVoid.type, "+=", Utils.typesToParameters(n), new List<IAction>()),
                new MethodSpecial(0, LVoid.type, "-=", Utils.typesToParameters(n), new List<IAction>()),
                new MethodSpecial(0, LVoid.type, "*=", Utils.typesToParameters(n), new List<IAction>()),
                new MethodSpecial(0, LVoid.type, "/=", Utils.typesToParameters(n), new List<IAction>()),
                new MethodSpecial(0, LBoolean.type, "<", Utils.typesToParameters(n), new List<IAction>()),
                new MethodSpecial(0, LBoolean.type, "<=", Utils.typesToParameters(n), new List<IAction>()),
                new MethodSpecial(0, LBoolean.type, ">=", Utils.typesToParameters(n), new List<IAction>()),
                new MethodSpecial(0, LBoolean.type, ">", Utils.typesToParameters(n), new List<IAction>()),

                //boolean
                new ConstructorSpecial(0, "Boolean", new List<Parameter>(), new List<Expression>(), new List<IAction>()),
                new MethodSpecial(0, LBoolean.type, "==", Utils.typesToParameters(b), new List<IAction>()),
                new MethodSpecial(0, LBoolean.type, "||", Utils.typesToParameters(b), new List<IAction>()),
                new MethodSpecial(0, LBoolean.type, "&&", Utils.typesToParameters(b), new List<IAction>()),
                new MethodSpecial(0, LBoolean.type, "not", Utils.typesToParameters(e), new List<IAction>()),
                new MethodSpecial(0, LString.type, "toString", Utils.typesToParameters(e), new List<IAction>()),

                //char
                new ConstructorSpecial(0, "Char", new List<Parameter>(), new List<Expression>(), new List<IAction>()),
                new MethodSpecial(0, LBoolean.type, "==", Utils.typesToParameters(c), new List<IAction>()),
                new MethodSpecial(0, LString.type, "toString", Utils.typesToParameters(e), new List<IAction>()),

                //list
                new ConstructorSpecial(0, "List", new List<Parameter>(), new List<Expression>(), new List<IAction>()),
                new ConstructorSpecial(0, "List", Utils.typesToParameters(n), new List<Expression>(), new List<IAction>()),
                new MethodSpecial(0, LBoolean.type, "==", Utils.typesToParameters(l), new List<IAction>()),
                new MethodSpecial(0, LString.type, "toString", Utils.typesToParameters(e), new List<IAction>()),
                new MethodSpecial(0, LVoid.type, "add", Utils.typesToParameters(a), new List<IAction>()),
                new MethodSpecial(0, LVoid.type, "insertAt", Utils.typesToParameters(na), new List<IAction>()),
                new MethodSpecial(0, LVoid.type, "removeAt", Utils.typesToParameters(n), new List<IAction>()),
                new MethodSpecial(0, LNumber.type, "count", Utils.typesToParameters(e), new List<IAction>()),
                new MethodSpecial(0, new LType(0, "A"), "get", Utils.typesToParameters(n), new List<IAction>()),
                new MethodSpecial(0, LVoid.type, "set", Utils.typesToParameters(na), new List<IAction>()),
            
                //string
                new ConstructorSpecial(0, "String", new List<Parameter>(), new List<Expression>(), new List<IAction>()),
                new MethodSpecial(0, LString.type, "==", Utils.typesToParameters(s), new List<IAction>()),
                new MethodSpecial(0, LNumber.type, "toNumber", Utils.typesToParameters(e), new List<IAction>()),
                new MethodSpecial(0, LString.type, "+", Utils.typesToParameters(s), new List<IAction>()),
                new MethodSpecial(0, LVoid.type, "+=", Utils.typesToParameters(s), new List<IAction>())
            });

            this.standardClasses = new List<LClassTemplate>(new LClassTemplate[]{
                //void
                new SpecialDefinedClass(0, LVoid.type, null, new UniquesList<LAttribute>(), new UniquesList<Class_Procedure>(), program),
                //object
                new SpecialDefinedClass(0, LObject.type, null, new UniquesList<LAttribute>(),
                    new UniquesList<Class_Procedure>(standardProcedures.GetRange(0, 3)), program),
                //number
                new SpecialDefinedClass(0, LNumber.type, LObject.type, new UniquesList<LAttribute>(),
                    new UniquesList<Class_Procedure>(standardProcedures.GetRange(3, 17)), program),
                //boolean
                new SpecialDefinedClass(0, LBoolean.type, LObject.type, new UniquesList<LAttribute>(),
                    new UniquesList<Class_Procedure>(standardProcedures.GetRange(20, 6)), program),
                //char
                new SpecialDefinedClass(0, LChar.type, LObject.type, new UniquesList<LAttribute>(),
                    new UniquesList<Class_Procedure>(standardProcedures.GetRange(26,3)), program),
                //list
                new SpecialDefinedClass(0, LList.type, LObject.type, new UniquesList<LAttribute>(),
                    new UniquesList<Class_Procedure>(standardProcedures.GetRange(29, 10)), program),
                //string
                new SpecialDefinedClass(0, LString.type, LString.parentType, new UniquesList<LAttribute>(),
                    new UniquesList<Class_Procedure>(standardProcedures.GetRange(39, 5)), program)
            });

            this.standardFunctions = new List<Function>(new Function[]
            {
                new FunctionSpecial(0, LString.type, "read", Utils.typesToParameters(e), new List<IAction>()),
                new FunctionSpecial(0, LVoid.type, "write", Utils.typesToParameters(s), new List<IAction>()),
                new FunctionSpecial(0, LVoid.type, "error", Utils.typesToParameters(s), new List<IAction>())
            });
        }
    }
}