using Leuterper.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using Leuterper;

namespace Leuterper.Constructions
{
    abstract class Construction : IConstruction
    {
        private IScope scope;
        private int line;
        public Construction(int line) { this.line = line; }
        public int getLine() { return this.line; }
        virtual public void setScope(IScope scope)
        {
            this.scope = scope;
        }
        public Program getProgram()
        {
            IScope iterator = this.getScope();
            while(!(iterator is Program)) iterator = iterator.getScope();
            return iterator as Program;
        }
        public LClassTemplate getClassScope()
        {
            IScope iterator = this.getScope();
            while(!(iterator is LClassTemplate)) iterator = iterator.getScope();
            return iterator as LClassTemplate;
        }
        public IScope getScope() { return this.scope; }
        public abstract void codeGenerationPass(LeuterperCompiler compiler);
        public abstract object Clone();
    }
    abstract class Procedure : Declaration, IDefinition, IScope, ISignable<Procedure>, IBlock
    {
        public List<Parameter> parameters;
        public UniquesList<Declaration> declarations;
        public List<IAction> actions;

        public Procedure(int line, LType type, String name, List<Parameter> parameters, List<IAction> actions) : base(line, type, name)
        {
            this.parameters = parameters;
            this.actions = new List<IAction>();
            this.declarations = new UniquesList<Declaration>();

            this.parameters.ForEach(p => this.declarations.AddUnique(p));
            this.expandActions(actions);
            this.scopeSetting();
        }
        public void checkThatVariablesHaveUniqueNames()
        {
            if (this.getLine() != 0)
            {
                SortedList<string, string> names = new SortedList<string, string>();
                foreach (Var v in this.declarations)
                {
                    if (names.ContainsValue(v.getName()))
                    {
                        throw new SyntacticErrorException("Variable redeclared: " + v.getName(), v.getLine());
                    }
                    names.Add(v.getName(), v.getName());
                }
            }
        }
        public override bool HasSameSignatureAs(Declaration declaration)
        {
            if (declaration is Procedure) return false;
            return this.HasSameSignatureAs(declaration as Procedure);
        }
        public bool HasSameSignatureAs(Procedure otherElement)
        {
            if (!this.getName().Equals(otherElement.getName())) return false;
            return Utils.listOfTypesMatch(
                Utils.listOfParametersAsListOfTypes(this.parameters),
                Utils.listOfParametersAsListOfTypes(otherElement.parameters)
                );
        }
        virtual public bool isCompatibleWithNameAndTypes(string name, List<LType> types)
        {
            if (!this.getName().Equals(name)) return false;
            return Utils.listOfTypesUnify(Utils.listOfParametersAsListOfTypes(this.parameters), types);
        }
        public override void codeGenerationPass(LeuterperCompiler compiler)
        {
            if (compiler.mostVaribalesInAFunction < this.declarations.Count())
            {
                compiler.mostVaribalesInAFunction = this.declarations.Count();
            }
            compiler.functionsParameters.Add(this.parameters.Count());
            compiler.functionActions.Add(new List<MachineInstructions.MachineInstruction>());
            this.actions.ForEach(a => a.codeGenerationPass(compiler));
        }
        public abstract int getIndex(int line);
        public override string ToString()
        {
            return String.Format("{0} {1} {2} {3}", this.getIndex(this.getLine()), this.getType().SignatureAsString(), this.getName(), Utils.listOfParametersAsString(this.parameters));
        }
        public UniquesList<Declaration> getDeclarations()
        {
            return this.declarations;
        }

        public void expandActions(List<IAction> actions)
        {
            Utils.expandActions(declarations, this.actions, actions);
        }
        public virtual void scopeSetting()
        {
            this.getType().setScope(this);
            this.parameters.ForEach(p => p.setScope(this));
            this.declarations.ForEach(v => v.setScope(this));
            this.actions.ForEach(a => a.setScope(this));
        }

        public abstract override object Clone();
    }
    class Assignment : Construction, IAction
    {
        public VarAccess lhs;
        public Expression rhs;
        public Assignment(int line, VarAccess lhs, Expression rhs) : base(line)
        {
            this.lhs = lhs;
            this.rhs = rhs;
            rhs.shouldBePushedToStack = true;
        }
        public override void setScope(IScope scope)
        {
            base.setScope(scope);
            lhs.setScope(scope);
            rhs.setScope(scope);
        }
        public override void codeGenerationPass(LeuterperCompiler compiler)
        {
            if (!rhs.getType().typeOrSuperTypeUnifiesWith(lhs.getType()))
            {
                throw new SyntacticErrorException("Type mismatch", this.getLine());
            }
            rhs.codeGenerationPass(compiler);
            compiler.addAction(new MachineInstructions.Assignment(lhs.locateVar().index));
        }
        public override object Clone()
        {
            return new Assignment(this.getLine(), this.lhs.Clone() as VarAccess, this.rhs.Clone() as Expression);
        }
    }
    class LType : Construction, ISignable<LType>
    {
        private String name;
        public List<LType> typeVariables;

        public LType(int line, String name, List<LType> typeVariables) : base(line)
        {
            this.name = name;
            this.typeVariables = typeVariables;
        }
        public LType(int line, String name) : this(line, name, new List<LType>()) { }
        public string SignatureAsString()
        {
            return String.Format("{0}", this.getName());
        }
        public Boolean typeOrSuperTypeUnifiesWith(LType otherType)
        {
            if (this.UnifiesWith(otherType))
            {
                return true;
            }
            else if (this.getParentType() != null)
            {
                return this.getParentType().typeOrSuperTypeUnifiesWith(otherType);
            }
            return false;
        }
        public LType getParentType()
        {
            return this.getDefinedClass().parentType;
        }
        public bool UnifiesWith(LType otherType)
        {
            if (this.typeVariables.Count() != otherType.typeVariables.Count()) return false;
            for (int i = 0; i < this.typeVariables.Count(); i++)
            {
                if (!this.typeVariables[i].UnifiesWith(otherType.typeVariables[i])) return false;
            }
            return (!this.rootIsDefined() || !otherType.rootIsDefined())? true : this.getName().Equals(otherType.getName());
        }
        public bool HasSameSignatureAs(LType otherElement)
        {
            return this.getName().Equals(otherElement.getName());
        }
        public override void setScope(IScope scope)
        {
            base.setScope(scope);
            this.typeVariables.ForEach(tv => tv.setScope(this.getScope()));
        }
        public Boolean rootIsDefined()
        {
            return this.getProgram().getTemplateClassForType(this) != null;
        }
        public Boolean isCompletelyDefined()
        {
            if (this.rootIsDefined()) return false;
            foreach(LType t in this.typeVariables)
            {
                if(!t.isCompletelyDefined()) return false;
            }
            return true;
        }
        public String getKind()
        {
            if(this.isCompletelyDefined()) return "T";
            if(!this.rootIsDefined()) return "*";
            string result = "";
            for (int i = 0; i < this.typeVariables.Count() - 1; i++ )
            {
                result += this.typeVariables[i].getKind() + "->";
            }
            return result + this.typeVariables.Last().getKind();
        }
        public string getName() { return this.name; }
        public override void codeGenerationPass(LeuterperCompiler compiler) { }
        public override object Clone()
        {
            LType clone = new LType(this.getLine(), this.name);
            clone.typeVariables = Utils.cloneLTypes(this.typeVariables);
            return clone;
        }
        public LClassTemplate getTemplate()
        {
            return this.getProgram().getTemplateClassForType(this);
        }
        public LClassTemplate getDefinedClass()
        {
            return this.getProgram().getTemplateClassForType(this).getDefinedClassWithTypes(this.typeVariables);
        }
        public DefinedClass getCompletlyDefinedClass()
        {
            LClassTemplate dc = this.getDefinedClass();
            if (dc is DefinedClass) return dc as DefinedClass;
            throw new SemanticErrorException("Instantiated a non completly defined class", 0);
        }
        public override String ToString()
        {
            string result = this.getName() + "[";
            this.typeVariables.ForEach(tv => result += tv + " ");
            result += "]";
            return result;
        }
        public List<LType> getNotCompletlyDefinedTypes()
        {
            List<LType> variables = new List<LType>();
            foreach (LType t in this.typeVariables)
            {
                if (!t.isCompletelyDefined())
                {
                    variables.Add(t);
                }
            }
            return variables;
        }
    }
    class LClassTemplate : Construction, IDeclaration, IDefinition, IScope, ISignable<LClassTemplate>
    {
        public LType type;
        public LType parentType;
        public UniquesList<LAttribute> attributes;
        public UniquesList<Constructor> constructorDefinitions = new UniquesList<Constructor>();
        public UniquesList<Method> methodsDefinitions = new UniquesList<Method>();
        public int numberOfLAttributes = -1;

        public Dictionary<List<LType>, LClassTemplate> reinstantiatedClasses;
        public LClassTemplate
            (
                int line,
                LType type,
                LType parentType,
                UniquesList<LAttribute> LAttributesDeclarations,
                UniquesList<Class_Procedure> classProcedures
            ) : base(line)
        {
            this.addToProgram();
            this.type = type;
            this.parentType = parentType;
            this.attributes = LAttributesDeclarations;
            if(parentType == null && !type.HasSameSignatureAs(LVoid.type) && !type.HasSameSignatureAs(LObject.type))
            {
                this.parentType = LObject.type;
            }
            LAttributesDeclarations.ForEach(a => a.setScope(this));
            classProcedures.ForEach(cp => cp.setScope(this));
            foreach (Class_Procedure cp in classProcedures)
            {
                if (cp is Constructor)
                {
                    this.constructorDefinitions.AddUnique(cp as Constructor);
                }
                else
                {
                    this.methodsDefinitions.AddUnique(cp as Method);
                }
            }
            if (this.parentType != null)
            {
                LAttribute baseClass = new LAttribute(this.getLine(), this.parentType, "super");
                this.attributes.InsertUnique(0, baseClass);
            }
            this.setScope(null);
        }
        public LClassTemplate(
           int line, LType newType, LType newParentType,
           UniquesList<LAttribute> reinstantiatedLAttributes,
           UniquesList<Constructor> reinstantiatedConstructors,
           UniquesList<Method> reinstantiatedMethods)
            : base(line)
        {
            this.type = newType;
            this.parentType = newParentType;
            this.attributes = reinstantiatedLAttributes;
            this.constructorDefinitions = reinstantiatedConstructors;
            this.methodsDefinitions = reinstantiatedMethods;
            this.addToProgram();
            this.scopeSetting();
        }
        public LClassTemplate reinstantiateWithSubstitutions(Substitutions substitutions)
        {
            UniquesList<LAttribute> reinstantiatedLAttributes = new UniquesList<LAttribute>();
            this.attributes.ForEach(a => reinstantiatedLAttributes.Add(a.reinstantiateWithSubstitutions(substitutions)));

            UniquesList<Method> reinstantiatedMethods = new UniquesList<Method>();
            UniquesList<Constructor> reinstantiatedConstructors = new UniquesList<Constructor>();
            this.methodsDefinitions.ForEach(m => reinstantiatedMethods.Add(m.reinstantiateWithSubstitutions(substitutions)));
            this.constructorDefinitions.ForEach(c => reinstantiatedConstructors.Add(c.reinstantiateWithSubstitutions(substitutions)));

            LType newType = substitutions.substitute(this.getType());
            LType newParentType = new Substitutions(this.parentType, newType.typeVariables).substitute(this.parentType);
        
            if (newType.isCompletelyDefined())
            {
                return new DefinedClass(this.getLine(), newType, newParentType, reinstantiatedLAttributes, reinstantiatedConstructors, reinstantiatedMethods);
            }
            else
            {
                return new LClassTemplate(this.getLine(), newType, newParentType, reinstantiatedLAttributes, reinstantiatedConstructors, reinstantiatedMethods);
            }
        }
        public virtual void addToProgram()
        {
            Program.singleton.templates.Add(this);
        }
        public void scopeSetting()
        {
            this.getType().setScope(this);
            this.attributes.ForEach(a => a.setScope(this));
            this.constructorDefinitions.ForEach(c => c.setScope(this));
            this.methodsDefinitions.ForEach(m => m.setScope(this));
        }
         public LClassTemplate getParentClass()
        {
             if(this.parentType == null) return null;
             return this.parentType.getDefinedClass();
        }
        public bool HasSameSignatureAs(LClassTemplate otherTemplate)
        {
            return this.type.HasSameSignatureAs(otherTemplate.type);
        }
        public Constructor getConstructor(String procedureName, List<LType> argumentsTypes)
        {
            Constructor c = this.constructorDefinitions.First(cd => cd.isCompatibleWithNameAndTypes(procedureName, argumentsTypes));
            if(c == null && this.getParentClass() != null)
            {
                return this.getParentClass().getConstructor(procedureName, argumentsTypes);
            }
            return c;
        }
        public string getName()
        {
            return this.getType().getName();
        }
        public LType getType() { return this.type; }
        public void setType(LType type) { this.type = type; }
        public UniquesList<Declaration> getDeclarations()
        {
            UniquesList<Declaration> result = new UniquesList<Declaration>();
            this.attributes.ForEach(a => result.Add(a));
            return result;
        }
        public LClassTemplate getDefinedClassWithTypes(List<LType> types)
        {
            LClassTemplate t;
            if (this.reinstantiatedClasses.TryGetValue(types, out t)) return t;
            t = this.reinstantiateWithSubstitutions(new Substitutions(this.type.getTemplate(), types));
            this.reinstantiatedClasses.Add(types, t);
            return t;
        }
        public override string ToString()
        {
            String result = "+++++\n";
            result += this.getType().SignatureAsString() + "\n";
            this.constructorDefinitions.ForEach(c => result += c + "\n");
            this.methodsDefinitions.ForEach(m => result += m + "\n");
            result += "+++++";
            return result;
        }
        internal LType getTypeOfLAttribute(int parentClassesWalked, int p)
        {
            LClassTemplate pC = this;
            while(parentClassesWalked > 0)
            {
                pC = pC.getParentClass();
                parentClassesWalked--;
            }
            return pC.attributes[p].getType();
        }
        private Constructor getConstructorForTypes(List<LType> types)
        {
            foreach (Constructor c in this.constructorDefinitions)
            {
                if (c.isCompatibleWithNameAndTypes(this.getName(), types)) return c;
            }
            return null;
        }
        internal int getIndexOfLAttribute(string p)
        {
            for (int i = 0; i < this.attributes.Count(); i++)
            {
                if (this.attributes[i].getName().Equals(p)) return this.parentType == null ? i : i + 1;
            }
            return -1;
        }

        internal DeclarationLocator<LAttribute> locateLAttribute(LAttributeAccess a)
        {
            DeclarationLocator<LAttribute> locator = new DeclarationLocator<LAttribute>();

            int parentClassesWalked = -1;
            int LAttributeIndex = -1;
            LClassTemplate iterator = this;
            do
            {
                LAttributeIndex = iterator.getIndexOfLAttribute(a.attributeName);
                if (LAttributeIndex < 0)
                {
                    iterator = iterator.getParentClass();
                }else
                {
                    break;
                }
            } while (iterator != null);
            if (LAttributeIndex < 0)
            {
                throw new SyntacticErrorException("Accessed an undeclared LAttribute: " + a.attributeName, a.getLine());
            }
            return new DeclarationLocator<LAttribute>(iterator.attributes[LAttributeIndex], parentClassesWalked, LAttributeIndex, true);
        }

        public override void codeGenerationPass(LeuterperCompiler compiler)
        {
            throw new NotImplementedException();
        }

        public override object Clone()
        {
            throw new NotImplementedException();
        }
    }

    class DefinedClass : LClassTemplate, ISignable<DefinedClass>
    {
        public int identifier;
        public DefinedClass (
                int line,
                LType type,
                LType parentType,
                UniquesList<LAttribute> LAttributesDeclarations,
                UniquesList<Constructor> reinstantiatedConstructors,
                UniquesList<Method> reinstantiatedMethods
            ) : base(line, type, parentType, LAttributesDeclarations, reinstantiatedConstructors, reinstantiatedMethods) {}
        public bool HasSameSignatureAs(DefinedClass otherElement)
        {
            return base.HasSameSignatureAs(otherElement);
        }
        public override void codeGenerationPass(LeuterperCompiler compiler)
        {
            compiler.addClassDefinition(this);
            this.constructorDefinitions.ForEach(c => c.codeGenerationPass(compiler));
            this.methodsDefinitions.ForEach(m => m.codeGenerationPass(compiler));
        }
       public override void addToProgram()
        {
            Program.singleton.definedClasses.Add(this);
        }
    }
    class Program : Procedure
    {
        public static Program singleton;
        public UniquesList<LClassTemplate> templates = new UniquesList<LClassTemplate>();
        public UniquesList<DefinedClass> definedClasses = new UniquesList<DefinedClass>();
        public UniquesList<Function> Functions = new UniquesList<Function>();
        public List<LObject> literals = new List<LObject>();
        public StandardLibrary standardLibrary;
        public Program():base(0, LVoid.type, "program", new List<Parameter>(), new List<IAction>() )
        {
            singleton = this;
            this.standardLibrary = new StandardLibrary(this);
            this.standardLibrary.standardClasses.ForEach(c => this.templates.Add(c));
            this.standardLibrary.standardFunctions.ForEach(f => this.Functions.Add(f));
        }
        public Program(List<IAction> actions) : base(0, LVoid.type, "program", new List<Parameter>(), actions) { }
        public void init(UniquesList<LClassTemplate> classes, UniquesList<Function> functions, List<IAction> actions)
        {
            classes.ForEach(c => this.templates.Add(c));
            functions.ForEach(f => this.Functions.Add(f));
            this.scopeSetting();
        }
        public override void scopeSetting()
        {
            base.scopeSetting();
            this.templates.ForEach(c => c.setScope(this));
            this.Functions.ForEach(f => f.setScope(this));
        }
        public new void codeGenerationPass(LeuterperCompiler compiler)
        {
            compiler.globalVariablesCounter = this.declarations.Count();
            this.definedClasses.ForEach(c => c.codeGenerationPass(compiler));
            this.Functions.ForEach(f => f.codeGenerationPass(compiler));
            compiler.compilingTopLeveIActions = true;
            this.actions.ForEach(a => a.codeGenerationPass(compiler));
        }
        public int getNumberOfVarsInProgram()
        {
            return this.literals.Count() + this.getDeclarations().Count();
        }
        public int getIndexOfClass(LClassTemplate aClass, int line)
        {
            for(int i = 0; i < this.definedClasses.Count(); i++)
            {
                if(this.definedClasses[i] == aClass) return i;
            }
            throw new SemanticErrorException("Tried to use a not defined class", line);
        }
        public int getIndexOfFunction(Function f, int line)
        {
            for(int i = 0; i < this.Functions.Count(); i++)
            {
                if(this.Functions[i] == f) return i + this.getNumberOfMethods();
            }
            throw new SemanticErrorException("Tried to use a not defined function", line);
        }
        public int getIndexOfClassProcedure(Class_Procedure cp, int line)
        {
            int counter = 0;
            foreach(DefinedClass d in this.definedClasses)
            {
                foreach(Constructor c in d.constructorDefinitions)
                {
                    if (c == cp) { return counter; } else { counter++; }
                }
                foreach(Method m in d.methodsDefinitions)
                {
                    if( m == cp) { return counter; } else { counter++; }
                }
            }
            throw new SemanticErrorException("Tried to use a not defined class procedure", line);
        }
        public void addDefinedClass(DefinedClass definedClass)
        {
            this.definedClasses.Add(definedClass);
        }
        public void addFunction(Function f)
        {
            this.Functions.Add(f);
        }
        public void addLiteral(LObject o)
        {
            this.literals.Add(o);
        }
        internal LClassTemplate getTemplateClassForType(LType lType)
        {
            foreach(LClassTemplate t in this.templates)
            {
                if(t.getType().getName().Equals(lType.getName())) {return t;}
            }
            return null;
        }
        public Function getFunctionForGivenNameAndTypes(string name, List<LType> parametersTypes)
        {
            foreach (Function f in this.Functions)
            {
                if (f.isCompatibleWithNameAndTypes(name, parametersTypes)) return f;
            }
            return null;
        }

        public override object Clone()
        {
            throw new NotImplementedException();
        }
        private int getNumberOfMethods()
        {
            int acum = 0;
            this.definedClasses.ForEach(c => {acum += c.constructorDefinitions.Count(); acum += c.methodsDefinitions.Count();});
            return acum;
        }

        public override int getIndex(int line)
        {
            throw new SemanticErrorException("You can't ask for the program's index", line);
        }
    }
    class Return_From_Block : Construction, IAction
    {
        Expression returningExpression;
        public Return_From_Block(int line, Expression returningExpression) : base(line)
        {
            this.returningExpression = returningExpression;
            this.returningExpression.shouldBePushedToStack = true;
        }

        public override void setScope(IScope scope)
        {
            base.setScope(scope);
            this.returningExpression.setScope(this.getScope());
        }
        public override void codeGenerationPass(LeuterperCompiler compiler)
        {
            returningExpression.codeGenerationPass(compiler);
            compiler.addAction(new MachineInstructions.Rtn());
        }
        public override object Clone()
        {
            return new Return_From_Block(this.getLine(), this.returningExpression.Clone() as Expression);
        }
    }

    //Asignacion al attributo de un objeto.
    class LSet : Construction, IAction
    {
        public LAttributeAccess la;
        public Expression rhs;
        public LSet(int line, LAttributeAccess la, Expression rhs) : base(line)
        {
            this.la = la;
            this.rhs = rhs;
            la.shouldBePushedToStack = true;
            la.willBeUsedForSet = true;
            rhs.shouldBePushedToStack = true;
        }
        public override void setScope(IScope scope)
        {
            base.setScope(scope);
            la.setScope(this.getScope());
            rhs.setScope(this.getScope());
        }
        override public void codeGenerationPass(LeuterperCompiler compiler)
        {
            int attributeIndex = la.locate().index;
            if (attributeIndex < 0)
            {
                throw new SyntacticErrorException(String.Format("Attribute not defined. Name {0}"), this.getLine());
            }
            la.codeGenerationPass(compiler);
            rhs.codeGenerationPass(compiler);
            compiler.addAction(new MachineInstructions.Set(attributeIndex));
        }
        public override object Clone()
        {
            return new LSet(this.getLine(), this.la.Clone() as LAttributeAccess, this.rhs.Clone() as Expression);
        }
    }

    class Function : Procedure, ISignable<Function>
    {
        public Function(int line, LType type, String name, List<Parameter> parameters, List<IAction> actions)
            : base(line, type, name, parameters, actions) { this.scopeSetting(); }

        public bool HasSameSignatureAs(Function otherElement)
        {
            return base.HasSameSignatureAs(otherElement);
        }
        public override object Clone()
        {
            return new Function(this.getLine(), this.getType().Clone() as LType, this.name, Utils.cloneParameters(parameters), Utils.cloneIActions(actions));
        }
        public override int getIndex(int line)
        {
            return this.getProgram().getIndexOfFunction(this, line);
        }
    }

    class FunctionSpecial : Function
    {
        public FunctionSpecial(int line, LType type, String id, List<Parameter> parameters, List<IAction> actions)
            : base(line, type, id, parameters, actions) { }

        public override void codeGenerationPass(LeuterperCompiler compiler) { }
    }

    /*Parameter aThis = new Parameter(this.getLine(), aClass.getType(), "this");
            aThis.setScope(this);
            this.parameters.Insert(0, aThis);
            this.declarations.InsertUnique(0, aThis); */
    abstract class Class_Procedure : Procedure, ISignable<Class_Procedure>
    {
        public Class_Procedure(int line, LType type, String id, List<Parameter> parameters, List<IAction> actions)
            : base(line, type, id, parameters, actions) { }
        public bool HasSameSignatureAs(Class_Procedure otherElement)
        {
            LClassTemplate thisClass = this.getClassScope();
            LClassTemplate otherClass = otherElement.getClassScope();
            return (thisClass == null || thisClass.HasSameSignatureAs(otherClass)) && base.HasSameSignatureAs(otherElement);
        }
        public override int getIndex(int line)
        {
            return this.getProgram().getIndexOfClassProcedure(this, line);
        }
    }
    class Constructor : Class_Procedure, ISignable<Constructor>
    {
        protected List<Expression> baseCallArguments;
        Boolean hasBeenSimplified;
        public Constructor(int line, string name, List<Parameter> parameters, List<Expression> baseCallArguments, List<IAction> actions)
            : base(line, new LType(line, name), name, parameters, actions)
        {
            this.hasBeenSimplified = false;
            this.baseCallArguments = baseCallArguments;
            this.scopeSetting();
        }
        public override bool isCompatibleWithNameAndTypes(string name, List<LType> types)
        {
            if (!this.getType().getName().Equals(name)) return false;
            List<LType> parametersTypes = Utils.listOfParametersAsListOfTypes(this.parameters);
            parametersTypes.RemoveAt(0);
            return Utils.listOfTypesUnify(parametersTypes, types);
        }
        public void simplificate()
        {
            LClassTemplate classOwner = this.getClassScope() as LClassTemplate;
            LClassTemplate parentClass = classOwner.getParentClass();
            if (parentClass == null) return;
            List<LType> argsAsTypes = Utils.expressionsToTypes(this.baseCallArguments);
            Constructor baseConstructor = parentClass.getConstructor(parentClass.getName(), argsAsTypes);
            if (baseConstructor == null)
            {
                throw new SyntacticErrorException("No constructor defined for the base class whose types match.", this.getLine());
            }
            Call_Constructor creation = new Call_Constructor(this.getLine(), parentClass.getType(), this.baseCallArguments);
            VarAccess aThis = new VarAccess(this.getLine(), "this");
            LAttributeAccess baseAccess = new LAttributeAccess(this.getLine(), aThis, "super");
            LSet baseAssignation = new LSet(this.getLine(), baseAccess, creation);
            baseAssignation.setScope(this);
            this.actions.Insert(0, baseAssignation);
        }
        public Constructor reinstantiateWithSubstitutions(Substitutions s)
        {
            Constructor result = this.Clone() as Constructor;
            result.parameters = Utils.reinstantiateParametersWithSubstitutions(result.parameters, s);
            result.actions = Utils.reinstantiateActionsWithSubstitutions(result.actions, s);
            new Constructor(
                this.getLine(),
                this.getName(),
                Utils.reinstantiateParametersWithSubstitutions(this.parameters, s),
                this.baseCallArguments,
                Utils.reinstantiateActionsWithSubstitutions(this.actions, s));
            return result;
        }
        public bool HasSameSignatureAs(Constructor otherElement)
        {
            return base.HasSameSignatureAs(otherElement);
        }
        public override void scopeSetting()
        {
            base.scopeSetting();
            if (this.baseCallArguments != null)
            {
                this.baseCallArguments.ForEach(a => a.setScope(this));
            }
        }
        public override object Clone()
        {
            return new Constructor(this.getLine(), this.name, Utils.cloneParameters(this.parameters), Utils.cloneExpressions(baseCallArguments), Utils.cloneIActions(actions));
        }
    }

    class Method : Class_Procedure, ISignable<Method>
    {
        public Method(int line, LType type, String name, List<Parameter> parameters, List<IAction> actions)
            : base(line, type, name, parameters, actions) { }

        public Method reinstantiateWithSubstitutions(Substitutions substitutions)
        {
            Method result = this.Clone() as Method;
            result.type = substitutions.substitute(result.type);
            result.parameters = Utils.reinstantiateParametersWithSubstitutions(result.parameters, substitutions);
            result.actions = Utils.reinstantiateActionsWithSubstitutions(result.actions, substitutions);
            return result;
        }

        public bool HasSameSignatureAs(Method otherElement)
        {
            return base.HasSameSignatureAs(otherElement);
        }

        public override object Clone()
        {
            return new Method(this.getLine(), this.getType().Clone() as LType, this.name, Utils.cloneParameters(this.parameters), Utils.cloneIActions(this.actions));
        }
    }
    class ConstructorSpecial : Constructor
    {
        public ConstructorSpecial(int line, string name, List<Parameter> parameters, List<Expression> baseCallArguments, List<IAction> actions)
            : base(line, name, parameters, baseCallArguments, new List<IAction>()) {}
        public override void codeGenerationPass(LeuterperCompiler compiler) { }
    }
    class MethodSpecial : Method
    {
        public MethodSpecial(int line, LType type, String name, List<Parameter> parameters, List<IAction> actions)
            : base(line, type, name, parameters, actions) {}
        public override void codeGenerationPass(LeuterperCompiler compiler) { }
    }
    abstract class Expression : Construction, IAction
    {
        public Expression(int line) : base(line) { }
        public bool shouldBePushedToStack { get; set; }
        abstract public LType getType();

    }

    class LAttributeAccess : Expression
    {
        public Expression theObject;
        public String attributeName;
        public bool willBeUsedForSet;
        public LAttributeAccess(int line, Expression theObject, String name) : base(line)
        {
            this.theObject = theObject;
            this.theObject.shouldBePushedToStack = true;
            this.attributeName = name;
            this.willBeUsedForSet = false;
        }
        override public LType getType()
        {
            return this.locate().declaration.getType();
        }
        public override void setScope(IScope scope)
        {
            base.setScope(scope);
            this.theObject.setScope(this.getScope());
        }
        public override void codeGenerationPass(LeuterperCompiler compiler)
        {
            theObject.codeGenerationPass(compiler);
            if (!this.willBeUsedForSet)
            {
                DeclarationLocator<LAttribute> locator = this.locate();
                while(locator.hierarchyDistance > 0)
                {
                    compiler.addAction(new MachineInstructions.Get(0));
                    locator.hierarchyDistance--;
                }
                compiler.addAction(new MachineInstructions.Get(locator.index));
            }
        }
        public DeclarationLocator<LAttribute> locate()
        {
            return this.getType().getDefinedClass().locateLAttribute(this);
        }
        public override object Clone()
        {
            return new LAttributeAccess(this.getLine(), theObject.Clone() as Expression, this.attributeName);
        }
}
    class VarAccess : Expression
    {
        private String name;
        public VarAccess(int line, String name) : base(line)  { this.name = name; }
        override public LType getType()
        {
            return this.locateVar().declaration.getType();
        }
        public override void codeGenerationPass(LeuterperCompiler compiler)
        {
            DeclarationLocator<Declaration> dL = this.locateVar();
            while(dL.hierarchyDistance > 0)
            {
                dL.hierarchyDistance--;
                compiler.addAction(new MachineInstructions.Get(0));
            }
            compiler.addAction(new MachineInstructions.Push(dL.index));
        }
        public string getName() { return this.name; }
        public DeclarationLocator<Declaration>locateVar()
        {
            DeclarationLocator<Declaration> dL = new DeclarationLocator<Declaration>();
            ScopeManager.locateVarNamed(this.getScope(), this.getName(), dL);
            if (!dL.found)
            {
                throw new SyntacticErrorException("Using undeclared var: " + this.getName(), this.getLine());
            }
            return dL;
        }
        public override object Clone()
        {
            return new VarAccess(this.getLine(), this.name);
        }
    }
    abstract class LObject : Expression
    {
        public static LType type = new LType(0, "Object");
        public int literalIndex;
        public LObject(int line) : base(line)
        {
            if(!(this is LList))
            {
                Program.singleton.addLiteral(this);
            }
        }
        override public LType getType() { return type; }
        abstract public String encodeAsString();
        public override void codeGenerationPass(LeuterperCompiler compiler)
        {
            if (this.shouldBePushedToStack)
            {
                compiler.addAction(new MachineInstructions.Push(this.literalIndex));
            }
        }
    }
    class LString : LList
    {
        new public static LType type = new LType(0, "String");
        public static LType parentType = LString.LStringParentType();
        public LString(int line, String value) : base(line, LChar.type, new List<Expression>())
        {
            List<Expression> chars = new List<Expression>();
            value = value.Substring(1, value.Length - 2);
            String nextString;
            LChar nextChar;
            while (value.Length > 0)
            {
                if (value[0] == '\\')
                {
                    nextString = value.Substring(0, 2);
                    value = value.Substring(2);
                }
                else
                {
                    nextString = value.Substring(0, 1);
                    value = value.Substring(1);
                }
                nextChar = new LChar(line, "\'" + nextString + "\'");
                elements.Add(nextChar);
            }
        }
        public static LType LStringParentType()
        {
            LType listType = LList.CreateLListType();
            listType.typeVariables[0] = LChar.type;
            return listType;
        }
        public override LType getType() { return LString.type; }
        public override string encodeAsString()
        {
            string result = "";
            for (int i = 0; i < this.elements.Count() - 1; i++)
            {
                result += ((LChar)this.elements[i]).encodeAsString() + " ";
            }
            if (elements.Count() > 0)
            {
                result += ((LChar)this.elements[this.elements.Count() - 1]).encodeAsString();
            }
            return result;
        }
        public override void codeGenerationPass(LeuterperCompiler compiler)
        {
            this.literalIndex = compiler.literals.Count();
            if (this.shouldBePushedToStack)
            {
                compiler.addAction(new MachineInstructions.Push(this.literalIndex));
            }
            compiler.addLiteral(new MachineInstructions.Literal("String", this.encodeAsString()));
        }
    }
    class LBoolean : LObject
    {
        new public static LType type = new LType(0, "Boolean");
        Boolean value;
        public LBoolean(int line, String value) : base(line)
        {
            if (value.Equals("true"))
            {
                this.value = true;
            }
            else if (value.Equals("false"))
            {
                this.value = false;
            }
        }
        public override string encodeAsString()
        {
            return this.value ? "1" : "0";
        }
        public override void codeGenerationPass(LeuterperCompiler compiler)
        {
            base.codeGenerationPass(compiler);
            compiler.addLiteral(new MachineInstructions.Literal("Boolean", this.encodeAsString()));
        }
        public override object Clone()
        {
            return new LBoolean(this.getLine(), this.value? "true" : "false");
        }
    }

    class LChar : LObject
    {
        int value;
        Boolean countAsLiteral;
        new public static LType type = new LType(0, "Char");
        public LChar(int line, String aChar, bool countAsLiteral) : this(line, aChar)
        {
            this.countAsLiteral = countAsLiteral;
        }
        public LChar(int line, String aChar) : base(line)
        {
            this.countAsLiteral = true;
            aChar = aChar.Substring(1, aChar.Length - 2);
            if (aChar.Length == 1)
            {
                this.value = aChar[0];
            }
            else
            {
                switch (aChar)
                {
                    case "\n":
                        this.value = '\n';
                        break;

                    case "\\":
                        this.value = '\\';
                        break;

                    case "\t":
                        this.value = '\t';
                        break;
                    default:
                        throw new Exception("Char invalido");
                }
            }
        }
        public override string encodeAsString()
        {
            return String.Format("{0}", this.value);
        }
        public override void codeGenerationPass(LeuterperCompiler compiler)
        {
            if (this.countAsLiteral)
            {
                base.codeGenerationPass(compiler);
                compiler.addLiteral(new MachineInstructions.Literal("Char", this.encodeAsString()));
            }
        }
        public override object Clone()
        {
            return new LChar(this.getLine(), "" + (char)this.value, this.countAsLiteral);
        }
    }

    class LList : LObject
    {
        new public static LType type = LList.CreateLListType();
        public LType instanceType;
        public List<Expression> elements;
        public LList(int line, LType type, List<Expression> elements) : base(line)
        {
            this.instanceType = new LType(line, "List", new List<LType>(new LType[] { type }));
            this.elements = elements;
        }
        public override void setScope(IScope scope)
        {
            base.setScope(scope);
            this.elements.ForEach(e => e.setScope(this.getScope()));
        }
        override public LType getType() { return this.instanceType; }

        public static LType CreateLListType()
        {
            LType membersType = new LType(0, "A");
            List<LType> typeVariables = new List<LType>(new LType[] { membersType });

            LType listType = new LType(0, "List", typeVariables);
            return listType;
        }
        public override string encodeAsString()
        {
            return String.Format("{0}", this.elements.Count());
        }
        public override void codeGenerationPass(LeuterperCompiler compiler)
        {
            foreach (Expression e in elements)
            {
                if (!e.getType().typeOrSuperTypeUnifiesWith(this.instanceType.typeVariables[0]))
                {
                    throw new SyntacticErrorException("Type mismatch in element of list.", this.getLine());
                }
            }
            foreach (Expression e in this.elements)
            {
                e.shouldBePushedToStack = true;
                e.codeGenerationPass(compiler);
            }
            compiler.addAction(new MachineInstructions.List(this.elements.Count(), !this.shouldBePushedToStack));
        }
        public override object Clone()
        {
            return new LList(this.getLine(), this.getType().Clone() as LType, Utils.cloneExpressions(this.elements));
        }
    }
    class LNumber : LObject
    {
        float value;
        new public static LType type = new LType(0, "Number");
        public LNumber(int line, float value) : base(line)
        {
            this.value = value;
        }
        public LNumber(int line, Token sign, String numberAsString) : base(line)
        {
            this.value = float.Parse(numberAsString);
            if (sign != null)
            {
                if (sign.image.Equals("-"))
                {
                    this.value *= -1;
                }
                else if (!sign.image.Equals("+"))
                {
                    throw new SyntacticErrorException("Expected a sign.", line);
                }
            }
        }
        override public LType getType() { return type; }
        public override string encodeAsString()
        {
            return String.Format("{0}", this.value);
        }
        public override void codeGenerationPass(LeuterperCompiler compiler)
        {
            base.codeGenerationPass(compiler);
            compiler.addLiteral(new MachineInstructions.Literal("Number", this.encodeAsString()));
        }
        public override object Clone()
        {
            return new LNumber(this.getLine(), this.value);
        }
    }

    class LVoid : LObject
    {
        new public static LType type = new LType(0, "Void");
        public LVoid(int line) : base(line) { }
        public override LType getType() { return LVoid.type; }
        public override string encodeAsString() { return ""; }
        public override void codeGenerationPass(LeuterperCompiler compiler)
        {
            base.codeGenerationPass(compiler);
            compiler.addLiteral(new MachineInstructions.Literal("Void", encodeAsString()));
        }
        public override object Clone()
        {
            return new LVoid(this.getLine());
        }
    }
    class SpecialTemplate : LClassTemplate
    {
        public SpecialTemplate(
            int line,
            LType type,
            LType parentType,
            UniquesList<LAttribute> LAttributesDeclarations,
            UniquesList<Class_Procedure> classProcedures
            ) : base(line, type, parentType, LAttributesDeclarations, classProcedures) {}
        public override void codeGenerationPass(LeuterperCompiler compiler) { }
    }

    class SpecialDefinedClass : DefinedClass
    {
        public SpecialDefinedClass(
            int line,
            LType type,
            LType parentType,
            UniquesList<LAttribute> LAttributesDeclarations,
            UniquesList<Constructor> constructors,
            UniquesList<Method> methods) : base(line, type, parentType, LAttributesDeclarations, constructors, methods) {}
        public override void codeGenerationPass(LeuterperCompiler compiler) { }
    }

    class LAttribute : Declaration, IReinstantiatable<LAttribute>, ISignable<LAttribute>
    {
        public LAttribute(int line, LType type, String name) : base(line, type, name) { }
        public LAttribute reinstantiateWithSubstitutions(Substitutions s)
        {
            return new LAttribute(this.getLine(), s.substitute(this.getType()), this.getName());
        }
        public bool HasSameSignatureAs(LAttribute otherElement)
        {
            return this.getName().Equals(otherElement.getName());
        }
        public override object Clone()
        {
            return new LAttribute(this.getLine(), this.type.Clone() as LType, this.name);
        }
    }
    abstract class Declaration : Construction, IDeclaration, ISignable<Declaration>
    {
        public LType type;
        public string name;
        public Declaration(int line, LType type, String name)
            : base(line)
        {
            this.type = type;
            this.name = name;
        }
        public string getName() { return this.name; }
        public LType getType() { return this.type; }
        public void setType(LType type) { this.type = type; }
        virtual public bool HasSameSignatureAs(Declaration otherElement)
        {
            return
                this.getType().HasSameSignatureAs(otherElement.getType())
                &&
                this.getName().Equals(otherElement.getName());
        }
        public override void setScope(IScope scope)
        {
            base.setScope(scope);
            this.type.setScope(scope);
        }
        public override void codeGenerationPass(LeuterperCompiler compiler) { }
        public override String ToString()
        {
            return String.Format("{0} {1}", this.getType().SignatureAsString(), this.getName());
        }
    }
    class Parameter : Declaration, IReinstantiatable<Parameter>
    {
        public Parameter(int line, LType type, String name) : base(line, type, name) { }
        public Parameter reinstantiateWithSubstitutions(Substitutions substitutions)
        {
            Parameter newParameter = this.Clone() as Parameter;
            newParameter.type = substitutions.substitute(this.type);
            return newParameter;
        }
        public override object Clone()
        {
            return new Parameter(this.getLine(), this.type.Clone() as LType, this.name);
        }
    }
    class Var : Declaration, IAction, ISignable<Var>
    {
        public Expression initialValue;
        public Var(int line, LType type, String name, Expression initialValue) : base(line, type, name)
        {
            this.initialValue = initialValue;
            this.initialValue.shouldBePushedToStack = true;
        }
        public Var(int line, LType type, String id) : this(line, type, id, null) { }
        public override void setScope(IScope scope)
        {
            base.setScope(scope);
            this.getType().setScope(this.getScope());
            if (this.initialValue != null)
            {
                this.initialValue.setScope(this.getScope());
            }
        }
        public Var redefineWithSubstitutions(Substitutions substitutions)
        {
            return new Var(this.getLine(), substitutions.substitute(this.getType()), this.getName());
        }
        public bool HasSameSignatureAs(Var otherVar)
        {
            return base.HasSameSignatureAs(otherVar);
        }
        public override object Clone()
        {
            if (this.initialValue == null) return new Var(this.getLine(), this.getType().Clone() as LType, this.getName());
            return new Var(this.getLine(), this.getType().Clone() as LType, this.getName(), this.initialValue.Clone() as Expression);
        }
    }
    abstract class Conditional : Construction, IAction, IBlock
    {
        public Expression booleanExpression;
        public List<IAction> thenActions = new List<IAction>();
        public UniquesList<Declaration> declarations = new UniquesList<Declaration>();
        public Conditional(int line, Expression booleanExpression, List<IAction> thenActions) : base(line)
        {
            this.booleanExpression = booleanExpression;
            this.expandActions(thenActions);
            booleanExpression.shouldBePushedToStack = true;
        }
        public override void setScope(IScope scope)
        {
            base.setScope(scope);
            booleanExpression.setScope(this.getScope());
            this.thenActions.ForEach(a => a.setScope(this.getScope()));
        }
        virtual public void expandActions(List<IAction>actions)
        {
            Utils.expandActions(declarations, this.thenActions, actions);
        }
        public UniquesList<Declaration> getDeclarations()
        {
            return this.declarations;
        }
    }
    class Loop_Do_While : Conditional
    {
        public Loop_Do_While(int line, Expression booleanExpression, List<IAction> thenActions)
            : base(line, booleanExpression, thenActions) { }
        public override void codeGenerationPass(LeuterperCompiler compiler)
        {
            int beginningOfLoop = compiler.getIndexOfNextActionInCurrentFunction();

            this.thenActions.ForEach(a => a.codeGenerationPass(compiler));
            booleanExpression.codeGenerationPass(compiler);
            compiler.addAction(new MachineInstructions.JMPT(beginningOfLoop));
        }
        public override object Clone()
        {
            return new Loop_Do_While(this.getLine(), this.booleanExpression.Clone() as Expression, Utils.cloneIActions(this.actions));
        }
    }

    class If_Then_Else : Conditional
    {
        List<IAction> elseActions;
        public If_Then_Else(int line, Expression booleanExpression, List<IAction> thenActions, List<IAction> elseActions)
            : base(line, booleanExpression, thenActions)
        {
            this.elseActions = new List<IAction>();
            this.expandActions(elseActions);
        }
        public override void setScope(IScope scope)
        {
            base.setScope(scope);
            this.booleanExpression.setScope(this.getScope());
            this.thenActions.ForEach(a => a.setScope(this.getScope()));
            this.elseActions.ForEach(a => a.setScope(this.getScope()));
        }
        public override void codeGenerationPass(LeuterperCompiler compiler)
        {
            MachineInstructions.JMPF jumpToElse = new MachineInstructions.JMPF();
            MachineInstructions.JMP endOfThen = new MachineInstructions.JMP();

            this.booleanExpression.codeGenerationPass(compiler);
            compiler.addAction(jumpToElse);
            this.thenActions.ForEach(a => a.codeGenerationPass(compiler));
            if(this.elseActions.Count() > 0) compiler.addAction(endOfThen);
            jumpToElse.whereToJump = compiler.getIndexOfNextActionInCurrentFunction();
            this.elseActions.ForEach(a => a.codeGenerationPass(compiler));
            endOfThen.whereToJump = compiler.getIndexOfNextActionInCurrentFunction();
        }
        public new void expandActions(List<IAction> elseActions)
        {
            Utils.expandActions(this.declarations, this.elseActions, elseActions);
        }
        public override object Clone()
        {
            return new If_Then_Else(this.getLine(), this.booleanExpression.Clone() as Expression, Utils.cloneIActions(this.thenActions), Utils.cloneIActions(this.elseActions);
        }
    }
    class Loop_While : Conditional
    {
        public Loop_While(int line, Expression booleanExpression, List<IAction> thenActions)
            : base(line, booleanExpression, thenActions) { }
        public override void codeGenerationPass(LeuterperCompiler compiler)
        {
            int jumpToTheBeginningInstructioIndex = compiler.getIndexOfNextActionInCurrentFunction();
            MachineInstructions.JMP jumpTotheBeginning = new MachineInstructions.JMP(jumpToTheBeginningInstructioIndex);
            MachineInstructions.JMPF jumpToTheEnd = new MachineInstructions.JMPF();

            booleanExpression.codeGenerationPass(compiler);
            compiler.addAction(jumpToTheEnd);
            this.thenActions.ForEach(a => a.codeGenerationPass(compiler));
            compiler.addAction(jumpTotheBeginning);

            jumpToTheEnd.whereToJump = compiler.getIndexOfNextActionInCurrentFunction();
        }
        public override object Clone()
        {
            return new Loop_While(this.getLine(), this.booleanExpression.Clone() as Expression, Utils.cloneExpressions(this.thenActions));
        }
    }
    class Call_Function : Call_Procedure
    {
        public Call_Function(int line, String procedureName, List<Expression> arguments) : base(line, procedureName, arguments) {}
        public override Procedure getProcedureDefinition()
        {
            return this.getProgram().getFunctionForGivenNameAndTypes(this.procedureName, Utils.expressionsToTypes(this.arguments));
        }
    }
    abstract class Call_Procedure : Expression
    {
        public String procedureName;
        public List<Expression> arguments;
        public Call_Procedure(int line, String procedureName, List<Expression> arguments) : base(line)
        {
            this.procedureName = procedureName;
            this.arguments = arguments;
            this.arguments.ForEach(a => a.shouldBePushedToStack = true);
        }
        abstract public Procedure getProcedureDefinition();
        public override LType getType()
        {
            return this.getProcedureDefinition().getType();
        }
        public int getProcedureIdentifier()
        {
            Procedure p = this.getProcedureDefinition();
            if (p == null)
            {
                throw new SyntacticErrorException(
                    String.Format("Called inexistent procedure: {0}\n With types: {1}",
                        this.procedureName,
                        Utils.listOfTypesAsString(Utils.expressionsToTypes(this.arguments))),
                    this.getLine());
            }
            return p.identifier;
        }
        public override void setScope(IScope scope)
        {
            base.setScope(scope);
            this.arguments.ForEach(e => e.setScope(scope));
        }
        public override void codeGenerationPass(LeuterperCompiler compiler)
        {
            this.arguments.ForEach(a => a.codeGenerationPass(compiler));
            compiler.addAction(new MachineInstructions.Call(this.getProcedureIdentifier(), !this.shouldBePushedToStack));
        }
    }
    class Call_Constructor : Call_Procedure
    {
        public LType type;
        public Call_Constructor(int line, LType type, List<Expression> arguments) : base(line, type.getName(), arguments)
        {
            this.type = type;
        }
        public override Procedure getProcedureDefinition()
        {
            List<LType> argumentsTypes = Utils.expressionsToTypes(this.arguments);
            Constructor constructor = this.getClassScope().getConstructor(this.procedureName, argumentsTypes);
            if(constructor == null)
            {
            throw new SyntacticErrorException(
                String.Format("Called an undefined constructor.\n\t{0}\n\t{1}",
                procedureName,
                Utils.listOfTypesAsString(argumentsTypes)), this.getLine());
            }
            return constructor;
        }
        public override void setScope(IScope scope)
        {
            base.setScope(scope);
            this.type.setScope(scope);
        }
        public override void codeGenerationPass(LeuterperCompiler compiler)
        {
            this.arguments.ForEach(a => a.codeGenerationPass(compiler));
            compiler.addAction(new MachineInstructions.New(this.getProcedureIdentifier(), this.type.getCompletlyDefinedClass().identifier, !this.shouldBePushedToStack));
        }
    }
    class Call_Method : Call_Procedure
    {
        public Expression theObject { get; set; }
        public Call_Method(int line, Expression theObject, String methodId, List<Expression> arguments)
            : base(line, methodId, arguments)
        {
            this.theObject = theObject;
            this.arguments.Insert(0, theObject);
        }
        public override void setScope(IScope scope)
        {
            base.setScope(scope);
            this.theObject.setScope(this.getScope());
        }
        public override Procedure getProcedureDefinition()
        {
            return this.getMethodWithNameAndTypes().declaration;
        }
        private DeclarationLocator<Method> getMethodWithNameAndTypes()
        {
            LType calleeType = theObject.getType();
            DeclarationLocator<Method> methodLocator = new DeclarationLocator<Method>();
            this.locateMethodFromClass(calleeType.getDefinedClass(), methodLocator);
            return methodLocator;
        }
        public override LType getType() { return this.getMethodWithNameAndTypes().declaration.getType(); }
        private void locateMethodFromClass(LClassTemplate aClass, DeclarationLocator<Method> methodLocator)
        {
            List<LType> argumentsTypes = Utils.expressionsToTypes(this.arguments);
            foreach (Method m in aClass.methodsDefinitions)
            {
                if (m.isCompatibleWithNameAndTypes(this.procedureName, argumentsTypes))
                {
                    methodLocator.declaration = m;
                    return;
                }
            }
            if (aClass.getParentClass() != null)
            {
                methodLocator.hierarchyDistance++;
                this.locateMethodFromClass(aClass.getParentClass(), methodLocator);
                return;
            }
            throw new SyntacticErrorException(
                String.Format("Used an undefined method.\n\t{0}\n\t{1}",
                this.procedureName,
                Utils.listOfTypesAsString(argumentsTypes)), this.getLine());
        }
        public override void codeGenerationPass(LeuterperCompiler compiler)
        {
            DeclarationLocator<Method> methodLocator = this.getMethodWithNameAndTypes();
            while(methodLocator.hierarchyDistance > 0)
            {
                methodLocator.hierarchyDistance--;
               this.arguments[0] = new LAttributeAccess(this.getLine(), this.arguments[0], "super");
            }
            this.arguments.ForEach(a => a.codeGenerationPass(compiler));
            compiler.addAction(new MachineInstructions.Call(this.getProcedureIdentifier(), !this.shouldBePushedToStack));
        }
    }
}