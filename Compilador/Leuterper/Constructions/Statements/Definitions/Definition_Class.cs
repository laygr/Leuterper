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
        public List<Declaration_LAttribute> attributesDeclarations { get; set; }
        public List<Definition_Method> methodsDefinitions { get; set; }

        public int numberOfAttributes = -1;

        public Definition_Class
            (
                int line,
                LType type,
                LType parentType,
                List<Declaration_LAttribute> attributesDeclarations,
                List<Definition_Method> methodsDefinitions
            ) : base(line, type)
        {
            if(parentType == null)
            {
                this.parentType = LObject.type;
            }
            else
            {
                this.parentType = parentType;
            }

            attributesDeclarations.ForEach(a => a.aClass = this);
            this.attributesDeclarations = new List<Declaration_LAttribute>(attributesDeclarations);

            methodsDefinitions.ForEach(m => m.aClass = this);
            this.methodsDefinitions = new List<Definition_Method>(methodsDefinitions);
            
        }

        public bool HasSameSignatureAs(Definition_Class otherElement)
        {
            return this.type.HasSameSignatureAs(otherElement.type);
        }

        public string SignatureAsString()
        {
            return this.type.SignatureAsString();
        }

        public int getNumberOfAttributes()
        {
            if (this.numberOfAttributes == -1)
            {
                this.calculateNumberOfAttributes();
            }
            return this.numberOfAttributes;
        }
        public void calculateNumberOfAttributes()
        {
            this.numberOfAttributes = 0;
            if (this.parentType != null)
            {
                this.numberOfAttributes += this.scope.getProgram().getClassForType(this.parentType).getNumberOfAttributes();
            }
            this.numberOfAttributes += this.attributesDeclarations.Count();
        }
        public override void secondPass()
        {
            this.calculateNumberOfAttributes();
            foreach(Declaration_LAttribute a in this.attributesDeclarations)
            {
                a.scope = this.scope;
                a.secondPass();
            }
        }
        public override void generateCode(LeuterperCompiler compiler)
        {
            compiler.addClassDefinition(this.getNumberOfAttributes());
        }
    }
}
