using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leuterper;

namespace Leuterper.Constructions
{
    class LClass : Construction, IStatement, IDefinition, IScope, IIdentifiable<LClass>
    {
        private LType type;
        private ScopeManager scopeManager;
        public int identifier { get; set; }
        public List<Attribute> attributesDeclarations { get; set; }
        public List<LClass> classDefinitions { get; set; }
        public List<Constructor> constructorDefinitions { get; set; }
        public List<Method> methodsDefinitions { get; set; }
        public int numberOfAttributes = -1;
        public LClass
            (
                int line,
                LType type,
                LType parentType,
                List<Attribute> attributesDeclarations,
                List<Class_Procedure> classProcedures
            ) : base(line)
        {
            this.type = type;
            if(parentType != null)
            {
                this.getType().parentType = parentType;
            }
            this.scopeManager = new ScopeManager(this);

            attributesDeclarations.ForEach(a => a.setScope(this));
            this.attributesDeclarations = new List<Attribute>(attributesDeclarations);

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
                this.getType().parentType.shouldRedefinesItsClass = true;
            }

            this.calculateNumberOfAttributes();
            foreach(Attribute a in this.attributesDeclarations)
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
            this.methodsDefinitions.ForEach(m => m.thirdPass());
        }
        public override void generateCode(LeuterperCompiler compiler)
        {
            compiler.addClassDefinition(this.getNumberOfAttributes());
            this.constructorDefinitions.ForEach(c => c.generateCode(compiler));
            this.methodsDefinitions.ForEach(m => m.generateCode(compiler));
        }

        public LClass getParentClass()
        {
            if(this.getType().parentType == null) return null;
            return this.getType().parentType.definingClass;
        }

        internal int getIndexOfAttribute(string attributeName)
        {
            for (int i = 0; i < this.attributesDeclarations.Count(); i++ )
            {
                if(this.attributesDeclarations[i].getName().Equals(attributeName))
                {
                    return i + this.getIndexOfFirstAttribute();
                }
            }
            return -1;
        }

        internal LType getTypeOfAttribute(int p)
        {
            LClass pC = this.getParentClass();
            if(this.getIndexOfFirstAttribute() > p)
            {
                return pC.getTypeOfAttribute(p);
            }
            return this.attributesDeclarations[p - pC.getNumberOfAttributes()].getType();
        }

        //Private methods
        private int getIndexOfFirstAttribute()
        {
            LClass p = this.getParentClass();
            if (p == null) return 0;
            return p.getNumberOfAttributes();
        }
        private int getNumberOfAttributes()
        {
            if (this.numberOfAttributes == -1)
            {
                this.calculateNumberOfAttributes();
            }
            return this.numberOfAttributes;
        }
        private void calculateNumberOfAttributes()
        {
            this.numberOfAttributes = 0;
            if (this.getType().parentType != null)
            {
                LClass parentClass = this.getType().parentType.definingClass;
                this.numberOfAttributes += parentClass.getNumberOfAttributes();
            }
            this.numberOfAttributes += this.attributesDeclarations.Count();
        }

        public LClass reinstantiateWithSubstitution(LType newType, List<LType> instantiatedTypes)
        {
            this.type = newType;
            List<Attribute> reinstantiatedAttributes = new List<Attribute>();
            foreach(Attribute a in this.attributesDeclarations)
            {
                reinstantiatedAttributes.Add(a.reinstantiateWithSubstitution(instantiatedTypes));
            }
            List<Class_Procedure> reinstantiatedProcedures = new List<Class_Procedure>();
            foreach(Method m in this.methodsDefinitions)
            {
                reinstantiatedProcedures.Add(m.reinstantiateWithSubstitution(instantiatedTypes));
            }
            foreach(Constructor c in this.constructorDefinitions)
            {
                reinstantiatedProcedures.Add(c);
            }
            return new LClass(this.getLine(), this.getType(), this.getType().parentType, reinstantiatedAttributes, reinstantiatedProcedures);
        }

        public LType getType()
        {
            return this.getType();
        }
        public void setType(LType type)
        {
            this.type = type;
        }
        public ScopeManager getScopeManager()
        {
            return this.scopeManager;
        }

        public List<IDeclaration> getDeclarations()
        {
            List<IDeclaration> result = new List<IDeclaration>();
            this.attributesDeclarations.ForEach(a => result.Add(a));
            return result;
        }

    }
}