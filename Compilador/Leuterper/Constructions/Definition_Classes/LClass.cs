using System;
using System.Collections.Generic;
using System.Linq;

namespace Leuterper.Constructions
{
    class LClass : Construction, IConstruction, IDefinition, IScope, ISignable<LClass>
    {
        private LType type;
        private LClass parentClass;
        public int identifier { get; set; }
        public List<LAttribute> attributes { get; set; }
        public List<Constructor> constructorDefinitions { get; set; }
        public List<Method> methodsDefinitions { get; set; }
        public int numberOfLAttributes = -1;
        public List<Construction> children;
        public LClass
            (
                int line,
                LType type,
                LType parentType,
                List<LAttribute> LAttributesDeclarations,
                List<Class_Procedure> classProcedures
            )
            : base(line)
        {
            this.type = type;
            this.children = new List<Construction>();
            if (parentType != null)
            {
                this.getType().parentType = parentType;
            }

            LAttributesDeclarations.ForEach(a => a.setScope(this));
            this.attributes = new List<LAttribute>(LAttributesDeclarations);

            classProcedures.ForEach(cp => cp.setScope(this));

            this.methodsDefinitions = new List<Method>();
            this.constructorDefinitions = new List<Constructor>();
            foreach (Class_Procedure cp in classProcedures)
            {
                if (cp is Constructor)
                {
                    this.constructorDefinitions.Add(cp as Constructor);
                }
                else
                {
                    this.methodsDefinitions.Add(cp as Method);
                }
            }
            this.scopeSetting();
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

        private string getParentClassName()
        {
            LClass parentClass = this.getParentClass();
            if(parentClass == null)
            {
                return "";
            }

            return parentClass.getType().getName();
        }
        public override void scopeSetting()
        {
            if (this.getType().parentType != null)
            {
                this.getType().parentType.setShouldStartRedefinition(true);
            }
            this.addChild(this.getType());

            this.attributes.ForEach(a => this.addChild(a));
            this.constructorDefinitions.ForEach(c => this.addChild(c));
            this.methodsDefinitions.ForEach(m => this.addChild(m));
        }

        public override void symbolsRegistration(LeuterperCompiler compiler)
        {
            compiler.assignIdentifierToClass(this);
            this.constructorDefinitions.ForEach(c => c.symbolsRegistration(compiler));
            this.methodsDefinitions.ForEach(m => m.symbolsRegistration(compiler));
        }



        public override void symbolsUnificationPass()
        {
            this.getType().symbolsUnificationPass();

            this.attributes.ForEach(a => a.symbolsUnificationPass());
            this.constructorDefinitions.ForEach(c => c.symbolsUnificationPass());
            this.methodsDefinitions.ForEach(m => m.symbolsUnificationPass());

            if(this.type.parentType != null)
            {
                this.parentClass = ScopeManager.getClassForType(this, this.type.parentType);
            }
        }
        public override void classesGenerationPass()
        {
            if (getParentClass() != null)
            {
                //this.type.parentType = this.type.parentType.redefineWithSubstitutionTypes(this.type.typeVariables);
                this.parentClass = this.parentClass.reinstantiateWithSubstitution(this.type.parentType, this.type.parentType.typeVariables);
            }

            this.attributes.ForEach(a => a.classesGenerationPass());
            this.methodsDefinitions.ForEach(m => m.classesGenerationPass());
        }
        public override void simplificationAndValidationPass()
        {
            LClass parentClass = this.getParentClass();
            if (parentClass == null) return;
            LType parentClassType = parentClass.getType();
            LAttribute baseClass = new LAttribute(this.getLine(), parentClassType, "super");
            this.attributes.Insert(0, baseClass);
            this.constructorDefinitions.ForEach(c => c.simplificationAndValidationPass());
        }
        public override void codeGenerationPass(LeuterperCompiler compiler)
        {
            compiler.addClassDefinition(this.attributes.Count());
            this.constructorDefinitions.ForEach(c => c.codeGenerationPass(compiler));
            this.methodsDefinitions.ForEach(m => m.codeGenerationPass(compiler));
        }

        public LClass getParentClass()
        {
            return this.parentClass;
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

        public string getClassName()
        {
            return this.getType().getName();
        }

        public Constructor getConstructorForTypes(List<LType>types)
        {

            foreach(Constructor c in this.constructorDefinitions)
            {
                if(c.isCompatibleWithNameAndTypes(this.getClassName(), types)) return c;
            }
            return null;
        }

        internal LType getTypeOfLAttribute(int p)
        {
            LClass pC = this.getParentClass();
            if(this.getIndexOfFirstLAttribute() > p)
            {
                return pC.getTypeOfLAttribute(p);
            }
            return this.attributes[p - pC.attributes.Count()].getType();
        }

        //Private methods
        private int getIndexOfFirstLAttribute()
        {
            LClass p = this.getParentClass();
            if (p == null) return 0;
            return p.attributes.Count();
        }

        public LClass reinstantiateWithSubstitution(LType newType, List<LType> instantiatedTypes)
        {
            this.type = newType;
            if(this.type.getName().Equals("Matrix"))
            {
                Console.WriteLine("WTF");
            }

            List<LAttribute> reinstantiatedLAttributes = new List<LAttribute>();
            this.attributes.ForEach(a => reinstantiatedLAttributes.Add(a.redefineWithSubstitutionTypes(instantiatedTypes)));

            List<Class_Procedure> reinstantiatedProcedures = new List<Class_Procedure>();
            this.methodsDefinitions.ForEach(m => reinstantiatedProcedures.Add(m.redefineWithSubstitutionTypes(instantiatedTypes)));
            this.constructorDefinitions.ForEach(c => reinstantiatedProcedures.Add(c.redefineWithSubstitutionTypes(instantiatedTypes)));
            
            LClass reinstanation = new LClass(this.getLine(), this.getType(), this.getType().parentType, reinstantiatedLAttributes, reinstantiatedProcedures);
            reinstanation.identifier = this.identifier;
            reinstanation.parentClass = this.parentClass;
            this.getScope().addChild(reinstanation);
            return reinstanation;
        }
        public LType getType() {  return this.type; }
        public void setType(LType type) { this.type = type; }
        public List<Declaration> getDeclarations()
        {
            List<Declaration> result = new List<Declaration>();
            this.attributes.ForEach(a => result.Add(a));
            return result;
        }
        public List<Construction> getChildren()
        {
            return this.children;
        }

        public void addChild(Construction c)
        {
            this.children.Add(c);
            c.setScope(this);
        }
    }
}