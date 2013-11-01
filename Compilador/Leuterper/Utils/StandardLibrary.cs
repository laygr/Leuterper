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


        public static List<Class_Procedure> specialMethods = new List<Class_Procedure>(new Class_Procedure[]{
            //void

            //object
            new MethodSpecial(0, LBoolean.type, "==", LType.typesToParameters(o), null, 0),
            new MethodSpecial(0, LString.type, "toString", LType.typesToParameters(e), null, 1),
           
            //number
            new MethodSpecial(0, LBoolean.type, "==", LType.typesToParameters(n), null, 2),
            new MethodSpecial(0, LString.type, "toString", LType.typesToParameters(e), null, 3),
            new MethodSpecial(0, LNumber.type, "+", LType.typesToParameters(n), null, 4),
            new MethodSpecial(0, LNumber.type, "-", LType.typesToParameters(n), null, 5),
            new MethodSpecial(0, LNumber.type, "*", LType.typesToParameters(n), null, 6),
            new MethodSpecial(0, LNumber.type, "/", LType.typesToParameters(n), null, 7),
           
            //boolean
            new MethodSpecial(0, LBoolean.type, "==", LType.typesToParameters(b), null, 8),
            new MethodSpecial(0, LString.type, "toString", LType.typesToParameters(e), null, 9),

            //char
            new MethodSpecial(0, LBoolean.type, "==", LType.typesToParameters(c), null, 10),
            new MethodSpecial(0, LString.type, "toString", LType.typesToParameters(e), null, 11),

            //list
            new MethodSpecial(0, LBoolean.type, "==", LType.typesToParameters(l), null, 12),
            new MethodSpecial(0, LString.type, "toString", LType.typesToParameters(e), null, 13),
            new MethodSpecial(0, LVoid.type, "add", LType.typesToParameters(a), null, 14),
            new MethodSpecial(0, LNumber.type, "count", LType.typesToParameters(e), null, 15),
            new MethodSpecial(0, new LType(0, "A"), "get", LType.typesToParameters(n), null, 16),
            new MethodSpecial(0, LVoid.type, "set", LType.typesToParameters(an), null, 17),
            
            
            //string
            new MethodSpecial(0, LString.type, "==", LType.typesToParameters(s), null, 18)
        });

        public static List<Function> standardFunctions = new List<Function>(new Function[]{
            new FunctionSpecial(0, LString.type, "read", LType.typesToParameters(e), null, 18),
            new FunctionSpecial(0, LVoid.type, "write", LType.typesToParameters(s), null, 19),
            new FunctionSpecial(0, LNumber.type, "stringToNumber", LType.typesToParameters(s), null, 20)});
        
        public static List<LClass> specialClasses = new List<LClass>(new LClass[]{
            //void
            new LClassSpecial(0, LVoid.type, null, new List<LAttribute>(), new List<Class_Procedure>(), (int)StandardClasses.LVoid),
            //object
            new LClassSpecial(0, LObject.type, null, new List<LAttribute>(), new List<Class_Procedure>(new Class_Procedure[]{
                specialMethods[0],
                specialMethods[1]}), (int)StandardClasses.LObject),

            //number
            new LClassSpecial(0, LNumber.type, LObject.type, new List<LAttribute>(), new List<Class_Procedure>(new Class_Procedure[]{
                specialMethods[2],
                specialMethods[3],
                specialMethods[4],
                specialMethods[5],
                specialMethods[6],
                specialMethods[7]}), (int)StandardClasses.LNumber),

            //boolean
            new LClassSpecial(0, LBoolean.type, LObject.type, new List<LAttribute>(), new List<Class_Procedure>(new Class_Procedure[]{
                specialMethods[8],
                specialMethods[9]}), (int)StandardClasses.LChar),

            //char
            new LClassSpecial(0, LChar.type, LObject.type, new List<LAttribute>(), new List<Class_Procedure>(new Class_Procedure[]{
                specialMethods[10],
                specialMethods[11]}), (int)StandardClasses.LChar),

            //list
            new LClassSpecial(0, LList.type, LObject.type, new List<LAttribute>(), new List<Class_Procedure>(new Class_Procedure[]{
                specialMethods[12],
                specialMethods[13],
                specialMethods[14],
                specialMethods[15],
                specialMethods[16],
                specialMethods[17]}), (int)StandardClasses.LList),

            //string
            new LClassSpecial(0, LString.type, LString.type.parentType, new List<LAttribute>(), new List<Class_Procedure>(new Class_Procedure[]{
                specialMethods[18]}), (int)StandardClasses.LString)

                });

        public static void initializeStandardLibrary()
        {
            /*
            LVoid.type.definingClass = specialClasses[(int)StandardClasses.LVoid];
            LObject.type.definingClass = specialClasses[(int)StandardClasses.LObject];
            LNumber.type.definingClass = specialClasses[(int)StandardClasses.LNumber];
            LBoolean.type.definingClass = specialClasses[(int)StandardClasses.LBoolean];
            LChar.type.definingClass = specialClasses[(int)StandardClasses.LChar];
            LList.type.definingClass = specialClasses[(int)StandardClasses.LList];
            LString.type.definingClass = specialClasses[(int)StandardClasses.LString];
             */
        }
    }
}
