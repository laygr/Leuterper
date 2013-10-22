using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leuterper;

namespace Leuterper.Constructions
{
    class Definition_Class : Definition, IIdentifiable<Definition_Class>
    {
        public int identifier { get; set; }
        public LType parentType { get; set; }
        public UniquesList<Declaration_LAttribute> attributesDeclarations { get; set; }
        public UniquesList<Definition_Method> methodsDefinitions { get; set; }

        public Definition_Class
            (
                int line,
                LType type,
                LType parentType,
                List<Declaration_LAttribute> attributesDeclarations,
                List<Definition_Method> methodsDefinitions
            ) : base(line, type)
        {
            this.parentType = parentType;

            attributesDeclarations.ForEach(a => a.aClass = this);
            this.attributesDeclarations = new UniquesList<Declaration_LAttribute>(attributesDeclarations);

            methodsDefinitions.ForEach(m => m.aClass = this);
            this.methodsDefinitions = new UniquesList<Definition_Method>(methodsDefinitions);
            
        }

        public bool HasSameSignatureAs(Definition_Class otherElement)
        {
            return this.type.HasSameSignatureAs(otherElement.type);
        }

        public string SignatureAsString()
        {
            return this.type.SignatureAsString();
        }
        public int getNumberOfVars()
        {
            int acum = 0;
            if(this.parentType != null)
            {
                acum += this.program.getClassForType(this.parentType).getNumberOfVars();
            }
            acum += this.attributesDeclarations.Count();
            return acum;
        }

        public override void generateCode(LeuterperCompiler compiler)
        {
            this.program.getClassForType(this.type);
        }
    }
}
