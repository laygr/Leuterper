using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leuterper;

namespace Leuterper.Constructions
{
    class LClass : Construction, IConstruction, IDefinition, IScope, ISignable<LClass>
    {
        private LType type;
        public int identifier { get; set; }
        public List<LAttribute> attributes { get; set; }
        public List<Constructor> constructorDefinitions { get; set; }
        public List<Method> methodsDefinitions { get; set; }
        public int numberOfLAttributes = -1;
        public LClass
            (
                int line,
                LType type,
                LType parentType,
                List<LAttribute> LAttributesDeclarations,
                List<Class_Procedure> classProcedures
            ) : base(line)
        {
            this.type = type;
            if(parentType != null)
            {
                this.getType().parentType = parentType;
            }

            LAttributesDeclarations.ForEach(a => a.setScope(this));
            this.attributes = new List<LAttribute>(LAttributesDeclarations);

            classProcedures.ForEach(cp => cp.setScope(this));

            this.methodsDefinitions = new List<Method>();
            this.constructorDefinitions = new List<Constructor>();
            foreach(Class_Procedure cp in classProcedures)
            {
                if(cp is Constructor)
                {
                    this.constructorDefinitions.Add(cp as Constructor);
                }else
                {
                    this.methodsDefinitions.Add(cp as Method);
                }
            }
        }

        public bool HasSameSignatureAs(LClass otherElement)
        {
            return this.getType().HasSameSignatureAs(otherElement.getType());
        }

        public override string ToString()
        {
            String result = "+++++\n";
            result += this.getType().SignatureAsString() + "\n";
            this.constructorDefinitions.ForEach(c => result += c+"\n");
            this.methodsDefinitions.ForEach(m => result += m+"\n");
            result += "+++++";
            return result;
        }
        public override void secondPass(LeuterperCompiler compiler)
        {
            compiler.assignIdentifierToClass(this);
            this.getType().setScope(this);
            if(this.getType().parentType != null)
            {
                this.getType().parentType.setShouldStartRedefinition(true);
            }
            this.getType().secondPass(compiler);

            this.calculateNumberOfLAttributes();
            foreach(LAttribute a in this.attributes)
            {
                a.setScope(this);
                a.secondPass(compiler);
            }
            foreach(Constructor c in this.constructorDefinitions)
            {
                c.setScope(this);
                c.secondPass(compiler);
            }
            foreach(Method m in this.methodsDefinitions)
            {
                m.setScope(this);
                m.secondPass(compiler);
            }
        }
        public override void thirdPass()
        {
            this.attributes.ForEach(a => a.thirdPass());
            this.methodsDefinitions.ForEach(m => m.thirdPass());
        }
        public override void generateCode(LeuterperCompiler compiler)
        {
            compiler.addClassDefinition(this.getNumberOfLAttributes());
            this.constructorDefinitions.ForEach(c => c.generateCode(compiler));
            this.methodsDefinitions.ForEach(m => m.generateCode(compiler));
        }

        public LClass getParentClass()
        {
            if(this.getType().parentType == null) return null;
            return this.getType().parentType.getDefiningClass();
        }

        internal int getIndexOfLAttribute(string LAttributeName)
        {
            for (int i = 0; i < this.attributes.Count(); i++ )
            {
                if(this.attributes[i].getName().Equals(LAttributeName))
                {
                    return i + this.getIndexOfFirstLAttribute();
                }
            }
            return -1;
        }

        internal LType getTypeOfLAttribute(int p)
        {
            LClass pC = this.getParentClass();
            if(this.getIndexOfFirstLAttribute() > p)
            {
                return pC.getTypeOfLAttribute(p);
            }
            return this.attributes[p - pC.getNumberOfLAttributes()].getType();
        }

        //Private methods
        private int getIndexOfFirstLAttribute()
        {
            LClass p = this.getParentClass();
            if (p == null) return 0;
            return p.getNumberOfLAttributes();
        }
        private int getNumberOfLAttributes()
        {
            if (this.numberOfLAttributes == -1)
            {
                this.calculateNumberOfLAttributes();
            }
            return this.numberOfLAttributes;
        }
        private void calculateNumberOfLAttributes()
        {
            this.numberOfLAttributes = 0;
            if (this.getType().parentType != null)
            {
                LClass parentClass = this.getType().parentType.getDefiningClass();
                this.numberOfLAttributes += parentClass.getNumberOfLAttributes();
            }
            this.numberOfLAttributes += this.attributes.Count();
        }

        public LClass reinstantiateWithSubstitution(LType newType, List<LType> instantiatedTypes)
        {
            this.type = newType;
            List<LAttribute> reinstantiatedLAttributes = new List<LAttribute>();
            foreach(LAttribute a in this.attributes)
            {
                reinstantiatedLAttributes.Add(a.redefineWithSubstitutionTypes(instantiatedTypes));
            }
            List<Class_Procedure> reinstantiatedProcedures = new List<Class_Procedure>();
            foreach(Method m in this.methodsDefinitions)
            {
                reinstantiatedProcedures.Add(m.redefineWithSubstitutionTypes(instantiatedTypes));
            }
            foreach(Constructor c in this.constructorDefinitions)
            {
                reinstantiatedProcedures.Add(c.redefineWithSubstitutionTypes(instantiatedTypes));
            }
            return new LClass(this.getLine(), this.getType(), this.getType().parentType, reinstantiatedLAttributes, reinstantiatedProcedures);
        }

        public LType getType()
        {
            return this.type;
        }
        public void setType(LType type)
        {
            this.type = type;
        }
        public List<IDeclaration> getDeclarations()
        {
            List<IDeclaration> result = new List<IDeclaration>();
            this.attributes.ForEach(a => result.Add(a));
            return result;
        }
    }
}