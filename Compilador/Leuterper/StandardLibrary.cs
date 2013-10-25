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
        public static List<Definition_Class> specialClasses = new List<Definition_Class>(new Definition_Class[]{
            
            new Definition_Class(0, LObject.type, null, new List<Declaration_LAttribute>(), new List<Definition_Method>(new Definition_Method[]{
                (Definition_Method)Program.specialFunctions[0],
                (Definition_Method)Program.specialFunctions[1]}), 0),

            new Definition_Class(0, LNumber.type, LObject.type, new List<Declaration_LAttribute>(), new List<Definition_Method>(new Definition_Method[]{
                (Definition_Method)Program.specialFunctions[2],
                (Definition_Method)Program.specialFunctions[3],
                (Definition_Method)Program.specialFunctions[4],
                (Definition_Method)Program.specialFunctions[5],
                (Definition_Method)Program.specialFunctions[6],
                (Definition_Method)Program.specialFunctions[7]}), 1),

            new Definition_Class(0, LList.type, LObject.type, new List<Declaration_LAttribute>(), new List<Definition_Method>(new Definition_Method[]{
                (Definition_Method)Program.specialFunctions[8],
                (Definition_Method)Program.specialFunctions[9],
                (Definition_Method)Program.specialFunctions[10],
                (Definition_Method)Program.specialFunctions[11],
                (Definition_Method)Program.specialFunctions[12],
                (Definition_Method)Program.specialFunctions[13]}), 2),

            new Definition_Class(0, LChar.type, LObject.type, new List<Declaration_LAttribute>(), new List<Definition_Method>(new Definition_Method[]{
                (Definition_Method)Program.specialFunctions[14],
                (Definition_Method)Program.specialFunctions[15]}), 3),

            new Definition_Class(0, LString.type, LString.type.parentType, new List<Declaration_LAttribute>(), new List<Definition_Method>(new Definition_Method[]{
                (Definition_Method)Program.specialFunctions[16]}), 4)

                });
    }
}