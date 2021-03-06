﻿using System.Collections.Generic;

namespace Leuterper.Constructions
{
    class LClassSpecial : LClass
    {
        public LClassSpecial(
            int line,
            LType type,
            LType parentType,
            List<LAttribute> LAttributesDeclarations,
            List<Class_Procedure> classProcedures,
            int classIdentifier)
            : base(line, type, parentType, LAttributesDeclarations, classProcedures)
        {
            this.identifier = classIdentifier;
        }

        public override void codeGenerationPass(LeuterperCompiler compiler) { }
    }
}
