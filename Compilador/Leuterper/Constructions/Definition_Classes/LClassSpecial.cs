using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    class LClassSpecial : LClass
    {
        public LClassSpecial(
            int line,
            LType type,
            LType parentType,
            List<Attribute> attributesDeclarations,
            List<Class_Procedure> classProcedures,
            int classIdentifier)
            : base(line, type, parentType, attributesDeclarations, classProcedures)
        {
            this.identifier = classIdentifier;
        }

        public override void generateCode(LeuterperCompiler compiler)
        { }
    }
}
