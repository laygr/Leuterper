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


        public List<Constructor> standardConstructors;
        public List<Method> standardMethods;
        public List<LClassTemplate> standardClasses;
        public List<Function> standardFunctions;

        public StandardLibrary(Program program)
        {
            this.standardConstructors = new List<Constructor>(new Constructor[]{
                new ConstructorSpecial(0, "Object", new List<Parameter>(), new List<Expression>(), new List<IAction>()),
                new ConstructorSpecial(0, "Number", Utils.typesToParameters(n), new List<Expression>(), new List<IAction>()),
                new ConstructorSpecial(0, "Boolean", new List<Parameter>(), new List<Expression>(), new List<IAction>()),
                new ConstructorSpecial(0, "Char", new List<Parameter>(), new List<Expression>(), new List<IAction>()),
                new ConstructorSpecial(0, "String", new List<Parameter>(), new List<Expression>(), new List<IAction>()),
            });
            this.standardMethods = new List<Method>(new Method[]{
                //void

                //object
                new MethodSpecial(0, LBoolean.type, "==", Utils.typesToParameters(o), new List<IAction>()),
                new MethodSpecial(0, LString.type, "toString", Utils.typesToParameters(e), new List<IAction>()),
           
                //number
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
                new MethodSpecial(0, LBoolean.type, "==", Utils.typesToParameters(b), new List<IAction>()),
                new MethodSpecial(0, LBoolean.type, "||", Utils.typesToParameters(b), new List<IAction>()),
                new MethodSpecial(0, LBoolean.type, "&&", Utils.typesToParameters(b), new List<IAction>()),
                new MethodSpecial(0, LBoolean.type, "not", Utils.typesToParameters(e), new List<IAction>()),
                new MethodSpecial(0, LString.type, "toString", Utils.typesToParameters(e), new List<IAction>()),

                //char
                new MethodSpecial(0, LBoolean.type, "==", Utils.typesToParameters(c), new List<IAction>()),
                new MethodSpecial(0, LString.type, "toString", Utils.typesToParameters(e), new List<IAction>()),
            
                //string
                new MethodSpecial(0, LString.type, "==", Utils.typesToParameters(s), new List<IAction>()),
                new MethodSpecial(0, LNumber.type, "toNumber", Utils.typesToParameters(e), new List<IAction>()),
                new MethodSpecial(0, LString.type, "+", Utils.typesToParameters(s), new List<IAction>()),
                new MethodSpecial(0, LVoid.type, "+=", Utils.typesToParameters(s), new List<IAction>())
            });

            this.standardClasses = new List<LClassTemplate>(new LClassTemplate[]{
                //void
                new SpecialDefinedClass(0, LVoid.type, null, new UniquesList<LAttribute>(), new UniquesList<Constructor>(), new UniquesList<Method>()),
                //object
                new SpecialDefinedClass(0, LObject.type, null, new UniquesList<LAttribute>(),
                    new UniquesList<Constructor>(standardConstructors.GetRange(0, 1)),
                    new UniquesList<Method>(standardMethods.GetRange(0, 2))),
                //number
                new SpecialDefinedClass(0, LNumber.type, LObject.type, new UniquesList<LAttribute>(),
                    new UniquesList<Constructor>(standardConstructors.GetRange(1, 1)),
                    new UniquesList<Method>(standardMethods.GetRange(2, 16))),
                //boolean
                new SpecialDefinedClass(0, LBoolean.type, LObject.type, new UniquesList<LAttribute>(),
                    new UniquesList<Constructor>(standardConstructors.GetRange(2, 1)),
                    new UniquesList<Method>(standardMethods.GetRange(18, 5))),
                //char
                new SpecialDefinedClass(0, LChar.type, LObject.type, new UniquesList<LAttribute>(),
                    new UniquesList<Constructor>(standardConstructors.GetRange(3, 1)),
                    new UniquesList<Method>(standardMethods.GetRange(23,2))),
                //string
                new SpecialDefinedClass(0, LString.type, LString.parentType, new UniquesList<LAttribute>(),
                    new UniquesList<Constructor>(standardConstructors.GetRange(4, 1)),
                    new UniquesList<Method>(standardMethods.GetRange(25, 4)))
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