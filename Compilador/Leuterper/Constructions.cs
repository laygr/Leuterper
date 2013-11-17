using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leuterper.Exceptions;
using Leuterper.Utils;

namespace Leuterper.Constructions
{
    abstract class Construction : IConstruction, ICompilable
    {
        private IScope scope;
        private int line;
        public Construction(int line) { this.line = line; }
        public int getLine() { return this.line; }
        virtual public void setScope(IScope scope)
        {
            this.scope = scope;
            this.scopeSetting();
        }
        public IScope getScope() { return this.scope; }
        public abstract void scopeSetting();
        public abstract void symbolsRegistration(LeuterperCompiler compiler);
        public abstract void symbolsUnificationPass();
        public abstract void classesGenerationPass();
        public abstract void simplificationAndValidationPass();
        public abstract void codeGenerationPass(LeuterperCompiler compiler);
    }

    abstract class Procedure : Declaration, IDefinition, IScope, ISignable<Procedure>
    {
        public int identifier = -1;
        public List<Parameter> parameters { get; set; }
        public UniquesList<Var> vars { get; set; }
        public List<IAction> actions { get; set; }
        public List<Construction> children;

        public Procedure(int line, LType type, String name, List<Parameter> parameters, List<IAction> actions)
            : base(line, type, name)
        {
            this.parameters = parameters;
            this.children = new List<Construction>();

            if (actions == null)
            {
                actions = new List<IAction>();
            }

            //Split actions in vars an actions.
            this.actions = new List<IAction>();
            this.vars = new UniquesList<Var>();

            foreach (IAction a in actions)
            {
                if (a is Var)
                {
                    this.vars.Add(a as Var);
                }
                else
                {
                    this.actions.Add(a);
                }
            }
            this.scopeSetting();
        }
        public void checkThatVariablesHaveUniqueNames()
        {
            if (this.getLine() != 0)
            {
                SortedList<string, string> names = new SortedList<string, string>();
                foreach (Var v in this.vars)
                {
                    if (names.ContainsValue(v.getName()))
                    {
                        throw new SemanticErrorException("Variable redeclared: " + v.getName(), v.getLine());
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
            return LType.listOfTypesMatch(
                UtilFunctions.listOfParametersAsListOfTypes(this.parameters),
                UtilFunctions.listOfParametersAsListOfTypes(otherElement.parameters)
                );
        }

        virtual public bool isCompatibleWithNameAndTypes(string name, List<LType> types)
        {
            if (!this.getName().Equals(name)) return false;
            return LType.listOfTypesUnify(UtilFunctions.listOfParametersAsListOfTypes(this.parameters), types);
        }
        public override void scopeSetting()
        {
            this.addChild(this.getType());
            this.parameters.ForEach(p => this.addChild(p));
            this.vars.ForEach(v => this.addChild(v));
            this.actions.ForEach(a => this.addChild(a as Construction));
        }

        public override void symbolsRegistration(LeuterperCompiler compiler)
        {
            compiler.assignIdentifierToProcedure(this);
        }

        public override void symbolsUnificationPass()
        {
            base.symbolsUnificationPass();

            this.parameters.ForEach(p => p.symbolsUnificationPass());
            this.vars.ForEach(v => v.symbolsUnificationPass());

            //Crear acciones de asignacion de las declaraciones de variables con valor inicial.
            int assignations = 0;
            foreach (Var v in this.vars)
            {
                if (v.initialValue != null)
                {
                    VarAccess var = new VarAccess(v.getLine(), v.getName());
                    this.actions.Insert(assignations, new Assignment(v.getLine(), var, v.initialValue));
                    assignations++;
                }
            }
            this.scopeSetting();
        }

        public override void classesGenerationPass()
        {
            this.parameters.ForEach(p => p.classesGenerationPass());
            this.vars.ForEach(v => v.classesGenerationPass());
        }

        public override void codeGenerationPass(LeuterperCompiler compiler)
        {
            if (compiler.mostVaribalesInAFunction < this.parameters.Count() + this.vars.Count())
            {
                compiler.mostVaribalesInAFunction = this.parameters.Count() + this.vars.Count();
            }
            compiler.functionsParameters.Add(this.parameters.Count());
            compiler.functionActions.Add(new List<MachineInstructions.MachineInstruction>());
            this.actions.ForEach(a => a.codeGenerationPass(compiler));
        }
        public override string ToString()
        {
            return String.Format("{0} {1} {2} {3}", this.identifier, this.getType().SignatureAsString(), this.getName(), UtilFunctions.listOfParametersAsString(this.parameters));
        }
        public UniquesList<Declaration> getDeclarations()
        {
            UniquesList<Declaration> result = new UniquesList<Declaration>();
            this.parameters.ForEach(p => result.Add(p));
            this.vars.ForEach(v => result.Add(v));
            return result;
        }
        public static List<IAction> reinstantiateActions(List<IAction> actions, List<LType> instantiatedTypes)
        {
            List<IAction> newActions = new List<IAction>();
            foreach (IAction a in actions)
            {
                if (a is Var)
                {
                    newActions.Add((a as Var).redefineWithSubstitutionTypes(instantiatedTypes));
                }
                else
                {
                    newActions.Add(a);
                }
            }
            return newActions;
        }
        public static List<Parameter> reinstantiateParameters(List<Parameter> parameters, List<LType> instantiatedTypes)
        {
            List<Parameter> newParameters = new List<Parameter>();
            foreach (Parameter p in parameters)
            {
                newParameters.Add(p.redefineWithSubstitutionTypes(instantiatedTypes));
            }
            return newParameters;
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
    class Assignment : Construction, IAction
    {
        public VarAccess lhs;
        public Expression rhs;

        public Assignment(int line, VarAccess lhs, Expression rhs)
            : base(line)
        {
            this.lhs = lhs;
            this.rhs = rhs;
        }

        public override void scopeSetting()
        {
            rhs.shouldBePushedToStack = true;
            this.getScope().addChild(lhs);
            this.getScope().addChild(rhs);
        }

        public override void symbolsRegistration(LeuterperCompiler compiler) { }

        public override void symbolsUnificationPass()
        {
            lhs.symbolsUnificationPass();
            if (rhs != null)
            {
                rhs.symbolsUnificationPass();
            }
        }

        public override void classesGenerationPass()
        {
            this.lhs.classesGenerationPass();
            this.rhs.classesGenerationPass();
        }
        public override void simplificationAndValidationPass()
        {
            if (!rhs.getType().typeOrSuperTypeUnifiesWith(lhs.getType()))
            {
                throw new SemanticErrorException("Type mismatch", this.getLine());
            }
        }

        public override void codeGenerationPass(LeuterperCompiler compiler)
        {
            rhs.codeGenerationPass(compiler);
            compiler.addAction(new MachineInstructions.Assignment(lhs.getIndex()));
        }
    }

    class LType : Construction, ISignable<LType>, IRedefinable<LType>
    {
        private String name;
        public List<LType> typeVariables;
        public LType parentType;
        public LType ownerType;
        public Boolean rootIsDefined;
        public Boolean isCompletelyDefined;
        private LClass definingClass;
        public int typeVariableIndex;
        private Boolean shouldRedefineItsClass;
        public VariableIndexFindStrategy strategy;

        public LType(int line, String name, List<LType> typeVariables, LType parentType)
            : base(line)
        {
            this.name = name;
            this.isCompletelyDefined = false;
            this.typeVariables = typeVariables;
            typeVariables.ForEach(tv => tv.isCompletelyDefined = false);
            typeVariables.ForEach(tv => tv.ownerType = this);
            this.parentType = parentType;
            if (this.parentType == null && (!this.getName().Equals("Object") || this.getName().Equals("Void")))
            {
                this.parentType = LObject.type;
            }
            this.typeVariableIndex = -1;
            this.shouldRedefineItsClass = false;
        }
        public LType(int line, String name, List<LType> typeVariables)
            : this(line, name, typeVariables, null) { }

        public LType(int line, String name)
            : this(line, name, new List<LType>()) { }

        public string SignatureAsString()
        {
            return String.Format("{0}", this.getName());
        }
        public static Parameter typeToParameter(LType t)
        {
            return new Parameter(t.getLine(), t, "fakeParam");
        }
        public static List<Parameter> typesToParameters(List<LType> types)
        {
            return types.ConvertAll(new Converter<LType, Parameter>(typeToParameter));
        }
        public static String listOfTypesAsString(List<LType> types)
        {
            string result = "";
            foreach (LType t in types)
            {
                result += t.SignatureAsString() + ", ";
            }
            return result;
        }

        //Type matching/unification
        public static Boolean listOfTypesUnify(List<LType> a, List<LType> b)
        {
            if (a.Count() != b.Count()) return false;
            for (int i = 0; i < b.Count(); i++)
            {
                if (!b[i].typeOrSuperTypeUnifiesWith(a[i])) return false;
            }
            return true;
        }
        public static Boolean listOfTypesMatch(List<LType> a, List<LType> b)
        {
            if (a.Count() != b.Count()) return false;
            for (int i = 0; i < b.Count(); i++)
            {
                if (!a[i].HasSameSignatureAs(b[i])) return false;
            }
            return true;
        }
        public Boolean typeOrSuperTypeUnifiesWith(LType otherType)
        {
            if (this.UnifiesWith(otherType))
            {
                return true;
            }
            else if (this.parentType != null)
            {
                return this.parentType.typeOrSuperTypeUnifiesWith(otherType);
            }
            return false;
        }
        public bool UnifiesWith(LType otherType)
        {
            if (this.typeVariables.Count() != otherType.typeVariables.Count()) return false;
            for (int i = 0; i < this.typeVariables.Count(); i++)
            {
                if (!this.typeVariables[i].UnifiesWith(otherType.typeVariables[i])) return false;
            }

            if (!this.rootIsDefined || !otherType.rootIsDefined)
            {
                return true;
            }
            else
            {
                return this.getName().Equals(otherType.getName());
            }
        }

        public bool isNamed(string name)
        {
            return this.getName().Equals(name);
        }

        public bool HasSameSignatureAs(LType otherElement)
        {
            if (this.typeVariables.Count() != otherElement.typeVariables.Count()) return false;
            if (!this.isCompletelyDefined && !otherElement.isCompletelyDefined) return true;
            if (!this.getName().Equals(otherElement.getName())) return false;
            for (int i = 0; i < this.typeVariables.Count(); i++)
            {
                if (!this.typeVariables[i].HasSameSignatureAs(otherElement.typeVariables[i])) return false;
            }
            return true;
        }

        public override void scopeSetting()
        {
            this.typeVariables.ForEach(tv => this.getScope().addChild(tv));
            if (this.parentType != null)
            {
                this.getScope().addChild(this.parentType);
            }
        }
        public override void symbolsRegistration(LeuterperCompiler compiler) { }
        public override void symbolsUnificationPass()
        {
            this.definingClass = this.getDefiningClass();
            if (this.definingClass != null)
            {
                this.parentType = this.definingClass.getType().parentType;
            }

            this.scopeSetting();
            this.typeVariables.ForEach(tv => tv.symbolsUnificationPass());

            if (this.rootIsDefined)
            {
                this.isCompletelyDefined = true;
                foreach (LType v in this.typeVariables)
                {
                    if (!v.isCompletelyDefined)
                    {
                        this.isCompletelyDefined = false;
                        break;
                    }
                }
            }
            else
            {
                this.typeVariableIndex = this.strategy.getVariableIndex(this);
            }

            if (this.parentType != null)
            {
                this.parentType.symbolsUnificationPass();
            }
            if (this.isCompletelyDefined && this.typeVariables.Count() > 0)
            {
                this.shouldRedefineItsClass = true;
            }
        }
        public override void classesGenerationPass()
        {
            if (this.shouldRedefineItsClass)
            {
                this.redefineWithSubstitutionTypes(this.typeVariables);
            }
        }
        public override void simplificationAndValidationPass() { }
        public string getName() { return this.name; }
        public override void codeGenerationPass(LeuterperCompiler compiler) { }
        public void setShouldStartRedefinition(Boolean shouldBeRedefined)
        {
            this.shouldRedefineItsClass = shouldBeRedefined;
        }
        public Boolean getShouldBeRedefined()
        {
            return this.shouldRedefineItsClass;
        }
        public LType clone()
        {
            LType clone = new LType(this.getLine(), this.name);
            if (this.parentType != null)
            {
                clone.parentType = this.parentType.clone();
            }
            else
            {
                clone.parentType = null;
            }
            clone.rootIsDefined = this.rootIsDefined;
            clone.isCompletelyDefined = this.isCompletelyDefined;
            clone.typeVariableIndex = this.typeVariableIndex;
            clone.shouldRedefineItsClass = this.shouldRedefineItsClass;
            clone.definingClass = this.definingClass;

            for (int i = 0; i < this.typeVariables.Count(); i++)
            {
                clone.typeVariables.Add(this.typeVariables[i].clone());
            }

            return clone;
        }

        public LType substituteTypeAndVariableTypesWith(List<LType> instantiatedTypes)
        {
            if (this.isCompletelyDefined) return this;
            LType result = this.clone();
            if (!result.rootIsDefined) return instantiatedTypes[this.typeVariableIndex];
            for (int i = 0; i < this.typeVariables.Count(); i++)
            {
                result.typeVariables[i] = this.typeVariables[i].substituteTypeAndVariableTypesWith(instantiatedTypes);
            }
            return result;
        }
        public LType redefineWithSubstitutionTypes(List<LType> instantiatedTypes)
        {
            LType newType = this;
            if (!this.rootIsDefined)
            {
                newType = instantiatedTypes[this.typeVariableIndex];
            }
            if (newType.definingClass != null)
            {
                newType.definingClass = newType.definingClass.reinstantiateWithSubstitution(this, instantiatedTypes);
            }
            if (newType.parentType != null)
            {
                newType.parentType.redefineWithSubstitutionTypes(instantiatedTypes);
            }
            return newType;
        }
        public LClass getDefiningClass()
        {
            if (this.definingClass == null)
            {
                this.definingClass = ScopeManager.getClassForType(this.getScope(), this);
            }
            this.rootIsDefined = this.definingClass != null;
            return definingClass;
        }

        public override void setScope(IScope scope)
        {
            base.setScope(scope);
            if (this.parentType != null)
            {
                this.parentType.setScope(scope);
            }
            if (ScopeManager.getClassScope(scope) == null)
            {
                this.strategy = new VariableNotInsideAClass();
            }
            else
            {
                this.strategy = new VariableInsideClass();
            }
        }
        public override String ToString()
        {
            string result = this.getName() + "[";
            foreach (LType t in this.typeVariables)
            {
                result += t + " ";
            }
            result += "]";
            return result;
        }
    }

    class LClass : Construction, IConstruction, IDefinition, IScope, ISignable<LClass>
    {
        private LType type;
        private LClass parentClass;
        public int identifier;
        public UniquesList<LAttribute> attributes;
        public UniquesList<Constructor> constructorDefinitions;
        public UniquesList<Method> methodsDefinitions;
        public int numberOfLAttributes = -1;
        public List<Construction> children;
        public LClass
            (
                int line,
                LType type,
                LType parentType,
                UniquesList<LAttribute> LAttributesDeclarations,
                UniquesList<Class_Procedure> classProcedures
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
            this.attributes = LAttributesDeclarations;

            classProcedures.ForEach(cp => cp.setScope(this));

            this.methodsDefinitions = new UniquesList<Method>();
            this.constructorDefinitions = new UniquesList<Constructor>();
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
            this.constructorDefinitions.ForEach(c => result += c + "\n");
            this.methodsDefinitions.ForEach(m => result += m + "\n");
            result += "+++++";
            return result;
        }

        private string getParentClassName()
        {
            LClass parentClass = this.getParentClass();
            if (parentClass == null)
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

            if (this.type.parentType != null)
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
            this.constructorDefinitions.ForEach(c => c.classesGenerationPass());
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
            compiler.addClassDefinition(this);
            this.constructorDefinitions.ForEach(c => c.codeGenerationPass(compiler));
            this.methodsDefinitions.ForEach(m => m.codeGenerationPass(compiler));
        }

        public LClass getParentClass() { return this.parentClass; }

        internal int getIndexOfLAttribute(string LAttributeName)
        {
            for (int i = 0; i < this.attributes.Count(); i++)
            {
                if (this.attributes[i].getName().Equals(LAttributeName))
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

        public Constructor getConstructorForTypes(List<LType> types)
        {
            foreach (Constructor c in this.constructorDefinitions)
            {
                if (c.isCompatibleWithNameAndTypes(this.getClassName(), types)) return c;
            }
            return null;
        }

        internal LType getTypeOfLAttribute(int p)
        {
            LClass pC = this.getParentClass();
            if (this.getIndexOfFirstLAttribute() > p)
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

            UniquesList<LAttribute> reinstantiatedLAttributes = new UniquesList<LAttribute>();
            this.attributes.ForEach(a => reinstantiatedLAttributes.Add(a.redefineWithSubstitutionTypes(instantiatedTypes)));

            UniquesList<Class_Procedure> reinstantiatedProcedures = new UniquesList<Class_Procedure>();
            this.methodsDefinitions.ForEach(m => reinstantiatedProcedures.Add(m.redefineWithSubstitutionTypes(instantiatedTypes)));
            this.constructorDefinitions.ForEach(c => reinstantiatedProcedures.Add(c.redefineWithSubstitutionTypes(instantiatedTypes)));

            LClass reinstanation = new LClass(this.getLine(), this.getType(), this.getType().parentType, reinstantiatedLAttributes, reinstantiatedProcedures);
            reinstanation.identifier = this.identifier;
            reinstanation.parentClass = this.parentClass;
            this.getScope().addChild(reinstanation);
            return reinstanation;
        }
        public LType getType() { return this.type; }
        public void setType(LType type) { this.type = type; }
        public UniquesList<Declaration> getDeclarations()
        {
            UniquesList<Declaration> result = new UniquesList<Declaration>();
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

    class Program : Construction, IScope, ICompilable
    {
        //Definitions
        public UniquesList<LClass> classes;
        public UniquesList<Function> functions;
        public List<IAction> actions;
        public UniquesList<Declaration> vars;

        public List<Construction> children;
        public Program(UniquesList<LClass> classes, UniquesList<Function> functions, List<IAction> actions)
            : base(0)
        {
            this.classes = classes;
            this.functions = functions;
            this.actions = actions;

            this.vars = new UniquesList<Declaration>();
            foreach (LClass c in StandardLibrary.singleton.standardClasses)
            {
                this.classes.Insert(0, c);
            }
            foreach (Function f in StandardLibrary.singleton.standardFunctions)
            {
                this.functions.Insert(0, f);
            }
            foreach (IAction a in actions)
            {
                if (a is Var)
                {
                    this.vars.Add(a as Var);

                }
            }
            this.children = new List<Construction>();

            int assignations = 0;
            foreach (Var v in this.vars)
            {
                if (v.initialValue != null)
                {
                    VarAccess var = new VarAccess(v.getLine(), v.getName());
                    this.actions.Insert(assignations, new Assignment(v.getLine(), var, v.initialValue));
                    assignations++;
                }
            }
            this.scopeSetting();
        }
        public Function getFunctionForGivenNameAndTypes(string name, List<LType> parametersTypes)
        {
            foreach (Function f in this.functions)
            {
                if (f.isCompatibleWithNameAndTypes(name, parametersTypes)) return f;
            }
            return null;
        }
        public int getIndexOfProcedureWithNameAndParametersTypes(string name, List<LType> parametersTypes)
        {
            Procedure fun = this.getFunctionForGivenNameAndTypes(name, parametersTypes);
            if (fun != null) return fun.identifier;
            return -1;
        }

        public override void scopeSetting()
        {
            this.classes.ForEach(c => this.addChild(c));
            this.vars.ForEach(v => this.addChild(v));
            this.functions.ForEach(f => this.addChild(f));
            this.actions.ForEach(a => this.addChild(a as Construction));
        }

        public override void symbolsRegistration(LeuterperCompiler compiler)
        {
            this.classes.ForEach(c => c.symbolsRegistration(compiler));
            this.vars.ForEach(v => v.symbolsRegistration(compiler));
            this.functions.ForEach(f => f.symbolsRegistration(compiler));
        }
        public override void symbolsUnificationPass()
        {
            //Crear acciones de asignacion de las declaraciones de variables con valor inicial.

            this.classes.ForEach(c => c.symbolsUnificationPass());
            this.vars.ForEach(v => v.symbolsUnificationPass());
            this.functions.ForEach(f => f.symbolsUnificationPass());
            this.actions.ForEach(a => a.symbolsUnificationPass());
        }
        public override void classesGenerationPass()
        {
            this.classes.ForEach(c => c.classesGenerationPass());
            this.functions.ForEach(f => f.classesGenerationPass());
        }
        public override void simplificationAndValidationPass()
        {
            this.classes.ForEach(c => c.simplificationAndValidationPass());
        }
        public override void codeGenerationPass(LeuterperCompiler compiler)
        {
            compiler.globalVariablesCounter = this.vars.Count();
            this.classes.ForEach(c => c.codeGenerationPass(compiler));
            this.functions.ForEach(f => f.codeGenerationPass(compiler));
            compiler.compilingTopLeveIActions = true;
            this.actions.ForEach(a => a.codeGenerationPass(compiler));
        }
        public UniquesList<LClass> getClasses() { return this.classes; }
        public UniquesList<Declaration> getDeclarations() { return this.vars; }
        public List<Construction> getChildren() { return this.children; }

        public void addChild(Construction c)
        {
            this.children.Add(c);
            c.setScope(this);
        }
    }
    class Return_From_Block : Construction, IAction
    {
        Expression returningExpression;
        public Return_From_Block(int line, Expression returningExpression)
            : base(line)
        {
            this.returningExpression = returningExpression;
        }

        public override void scopeSetting()
        {
            this.returningExpression.shouldBePushedToStack = true;
            this.getScope().addChild(this.returningExpression);
        }

        public override void symbolsRegistration(LeuterperCompiler compiler) { }

        public override void symbolsUnificationPass() { this.returningExpression.symbolsUnificationPass(); }
        public override void classesGenerationPass() { }
        public override void simplificationAndValidationPass() { }
        public override void codeGenerationPass(LeuterperCompiler compiler)
        {
            returningExpression.codeGenerationPass(compiler);
            compiler.addAction(new MachineInstructions.Rtn());
        }
    }

    //Asignacion al attributo de un objeto.
    class LSet : Construction, IAction
    {
        public LAttributeAccess la;
        public Expression rhs;

        public LSet(int line, LAttributeAccess la, Expression rhs)
            : base(line)
        {
            this.la = la;
            this.rhs = rhs;
        }
        public override void scopeSetting()
        {
            la.willBeUsedForSet = true;
            this.getScope().addChild(la);

            rhs.shouldBePushedToStack = true;
            this.getScope().addChild(rhs);
        }
        public override void symbolsRegistration(LeuterperCompiler compiler) { }
        override public void symbolsUnificationPass()
        {
            la.symbolsUnificationPass();
            rhs.symbolsUnificationPass();
        }

        public override void classesGenerationPass() { }
        public override void simplificationAndValidationPass() { }
        override public void codeGenerationPass(LeuterperCompiler compiler)
        {
            int attributeIndex = la.getLAttributeIndex();
            if (attributeIndex < 0)
            {
                throw new SemanticErrorException(String.Format("Attribute not defined. Name {0}"), this.getLine());
            }
            la.codeGenerationPass(compiler);
            rhs.codeGenerationPass(compiler);
            compiler.addAction(new MachineInstructions.Set(attributeIndex));
        }
    }

    class Function : Procedure, ISignable<Function>
    {
        public Function(int line, LType type, String id, List<Parameter> parameters, List<IAction> actions)
            : base(line, type, id, parameters, actions) { this.scopeSetting(); }

        public bool HasSameSignatureAs(Function otherElement)
        {
            return base.HasSameSignatureAs(otherElement);
        }
    }

    class FunctionSpecial : Function
    {
        public FunctionSpecial(int line, LType type, String id, List<Parameter> parameters, List<IAction> actions, int identifier)
            : base(line, type, id, parameters, actions)
        { this.identifier = identifier; }

        public override void codeGenerationPass(LeuterperCompiler compiler) { }
    }

    abstract class Class_Procedure : Procedure, ISignable<Class_Procedure>
    {
        public Class_Procedure(int line, LType type, String id, List<Parameter> parameters, List<IAction> actions)
            : base(line, type, id, parameters, actions)
        { }

        public LClass getClass()
        {
            return this.getScope() as LClass;
        }

        public override void symbolsUnificationPass()
        {
            LClass aClass = this.getClass();
            Parameter _this = new Parameter(this.getLine(), aClass.getType(), "this");
            _this.setScope(this);
            this.parameters.Insert(0, _this);
            base.symbolsUnificationPass();
        }

        public bool HasSameSignatureAs(Class_Procedure otherElement)
        {
            return base.HasSameSignatureAs(otherElement);
        }
    }

    class Constructor : Class_Procedure, ISignable<Constructor>
    {
        private List<Expression> baseCallArguments;
        public Constructor(int line, string name, List<Parameter> parameters, List<Expression> baseCallArguments, List<IAction> actions)
            : base(line, new LType(line, name), name, parameters, actions)
        {
            this.baseCallArguments = baseCallArguments;
            this.scopeSetting();
        }

        public override bool isCompatibleWithNameAndTypes(string name, List<LType> types)
        {
            if (!this.getType().getName().Equals(name)) return false;
            List<LType> parametersTypes = UtilFunctions.listOfParametersAsListOfTypes(this.parameters);
            parametersTypes.RemoveAt(0);
            return LType.listOfTypesUnify(parametersTypes, types);
        }

        public override void symbolsUnificationPass()
        {
            this.setType(this.getClass().getType());
            string className = this.getClass().getType().getName();
            if (!className.Equals(this.getName()))
            {
                throw new SemanticErrorException(string.Format("Constructor not named as its class.\nClass named: {0}\nNamed instead: {1}", className, this.getName()), this.getLine());
            }
            base.symbolsUnificationPass();
        }
        public override void classesGenerationPass() { }

        public override void simplificationAndValidationPass()
        {
            LClass classOwner = this.getScope() as LClass;
            LClass parentClass = classOwner.getParentClass();
            if (parentClass == null) return;

            Constructor baseConstructor = parentClass.getConstructorForTypes(Expression.expressionsToTypes(this.baseCallArguments));
            if (baseConstructor == null)
            {
                throw new SemanticErrorException("No constructor defined for the base class whose types match.", this.getLine());
            }
            Call_Constructor creation = new Call_Constructor(this.getLine(), parentClass.getType(), this.baseCallArguments);
            VarAccess aThis = new VarAccess(this.getLine(), "this");
            LAttributeAccess baseAccess = new LAttributeAccess(this.getLine(), aThis, "super");
            LSet baseAssignation = new LSet(this.getLine(), baseAccess, creation);
            baseAssignation.setScope(this);
            this.actions.Insert(0, baseAssignation);
        }

        public Constructor redefineWithSubstitutionTypes(List<LType> instantiatedTypes)
        {
            Constructor result = new Constructor(
                this.getLine(),
                this.getName(),
                Procedure.reinstantiateParameters(this.parameters, instantiatedTypes),
                this.baseCallArguments,
                Procedure.reinstantiateActions(this.actions, instantiatedTypes));
            result.identifier = this.identifier;
            result.setScope(this.getScope());
            return result;
        }

        public bool HasSameSignatureAs(Constructor otherElement)
        {
            return base.HasSameSignatureAs(otherElement);
        }
    }

    class Method : Class_Procedure, ISignable<Method>
    {
        public Method(int line, LType type, String name, List<Parameter> parameters, List<IAction> actions)
            : base(line, type, name, parameters, actions)
        {
            this.scopeSetting();
        }

        public Method redefineWithSubstitutionTypes(List<LType> instantiatedTypes)
        {
            Method result = new Method(
                this.getLine(),
                this.getType().substituteTypeAndVariableTypesWith(instantiatedTypes),
                this.getName(),
                Procedure.reinstantiateParameters(this.parameters, instantiatedTypes),
                Procedure.reinstantiateActions(this.actions, instantiatedTypes));
            result.identifier = this.identifier;
            result.setScope(this.getScope());
            return result;
        }


        public bool HasSameSignatureAs(Method otherElement)
        {
            return base.HasSameSignatureAs(otherElement);
        }
    }
    class ConstructorSpecial : Constructor
    {
        public ConstructorSpecial(int line, string name, List<Parameter> parameters, List<Expression> baseCallArguments, List<IAction> actions, int identifier)
            : base(line, name, parameters, baseCallArguments, new List<IAction>())
        {
            this.identifier = identifier;
        }

        public override void codeGenerationPass(LeuterperCompiler compiler) { }
    }

    class MethodSpecial : Method
    {
        public MethodSpecial(int line, LType type, String name, List<Parameter> parameters, List<IAction> actions, int identifier)
            : base(line, type, name, parameters, actions)
        {
            this.identifier = identifier;
        }
        public override void codeGenerationPass(LeuterperCompiler compiler) { }
    }

    abstract class Expression : Construction, IAction
    {
        public Expression(int line) : base(line) { }
        public bool shouldBePushedToStack { get; set; }
        abstract public LType getType();

        public static LType expresionToType(Expression e)
        {
            return e.getType();
        }
        public static List<LType> expressionsToTypes(List<Expression> expression)
        {
            return expression.ConvertAll(new Converter<Expression, LType>(expresionToType));
        }
        public override void symbolsRegistration(LeuterperCompiler compiler) { }
    }

    class LAttributeAccess : Term
    {
        public Expression theObject;
        public String LAttributeName;
        public bool willBeUsedForSet;
        public LAttributeAccess(int line, Expression theObject, String name)
            : base(line)
        {
            this.theObject = theObject;
            this.theObject.shouldBePushedToStack = true;
            this.LAttributeName = name;
            this.willBeUsedForSet = false;
        }

        public int getLAttributeIndex()
        {
            LClass c = this.theObject.getType().getDefiningClass();
            return c.getIndexOfLAttribute(LAttributeName);
        }

        override public LType getType()
        {
            LClass c = this.theObject.getType().getDefiningClass();
            return c.getTypeOfLAttribute(this.getLAttributeIndex());
        }

        public override void scopeSetting()
        {
            this.theObject.setScope(this.getScope());
        }
        public override void symbolsUnificationPass()
        {
            theObject.symbolsUnificationPass();
        }
        public override void classesGenerationPass() { }
        public override void simplificationAndValidationPass() { }

        public override void codeGenerationPass(LeuterperCompiler compiler)
        {
            if (!this.shouldBePushedToStack) return;
            theObject.codeGenerationPass(compiler);
            if (!this.willBeUsedForSet)
            {
                LClass c = this.theObject.getType().getDefiningClass();
                int LAttributeIndex = c.getIndexOfLAttribute(this.LAttributeName);
                if (LAttributeIndex < 0)
                {
                    throw new SemanticErrorException("Accessed an undeclared LAttribute: " + this.LAttributeName, this.getLine());
                }
                compiler.addAction(new MachineInstructions.Get(LAttributeIndex));
            }
        }
    }
    abstract class Term : Expression
    {
        public Term(int line) : base(line) { }

    }

    class VarAccess : Term
    {
        private String name;

        public VarAccess(int line, String name)
            : base(line)
        {
            this.name = name;
        }

        override public LType getType()
        {
            Declaration d = ScopeManager.getDeclarationLineage(this.getScope(), this.getName());
            if (d == null)
            {
                throw new SemanticErrorException("Using undeclared var " + this.getName(), this.getLine());
            }
            return d.getType();
        }

        public int getIndex()
        {
            return ScopeManager.getIndexOfVarNamed(this.getScope(), this.getName());
        }

        public override void scopeSetting() { }
        public override void symbolsRegistration(LeuterperCompiler compiler) { }
        public override void symbolsUnificationPass() { }
        public override void classesGenerationPass() { }
        public override void simplificationAndValidationPass() { }
        public override void codeGenerationPass(LeuterperCompiler compiler)
        {
            int varIndex = ScopeManager.getIndexOfVarNamed(this.getScope(), this.getName());
            if (varIndex == -1)
            {
                throw new SemanticErrorException("Using undeclared var: " + this.getName(), this.getLine());
            }
            compiler.addAction(new MachineInstructions.Push(varIndex));
        }

        public string getName() { return this.name; }
    }

    abstract class LObject : Term
    {
        public static LType type = new LType(0, "Object");
        public static int literalsCounter = 0;
        public int literalIndex;
        public LObject(int line)
            : base(line)
        {
            this.literalIndex = literalsCounter;
            literalsCounter++;
        }

        override public LType getType()
        {
            return type;
        }

        abstract public String encodeAsString();
        public override void scopeSetting() { }
        public override void symbolsUnificationPass() { }
        public override void classesGenerationPass() { }
        public override void simplificationAndValidationPass() { }
        public override void codeGenerationPass(LeuterperCompiler compiler)
        {
            this.literalIndex = compiler.literals.Count();
            if (this.shouldBePushedToStack)
            {
                compiler.addAction(new MachineInstructions.Push(this.literalIndex));
            }
        }
    }

    class LString : LList
    {
        new public static LType type = LString.LStringType();
        public LString(int line, String value)
            : base(line, LChar.type, new List<Expression>())
        {
            LType type = LString.type;
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

        public static LType LStringType()
        {
            LType listType = LList.CreateLListType();
            listType.typeVariables[0] = LChar.type;
            LType stringType = new LType(0, "String");
            stringType.parentType = listType;
            return stringType;
        }

        public new LType getType()
        {
            return LString.type;
        }

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

        public LBoolean(int line, String value)
            : base(line)
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
    }

    class LChar : LObject
    {
        int value;
        Boolean countAsLiteral;
        new public static LType type = new LType(0, "Char");
        public LChar(int line, String aChar, bool countAsLiteral)
            : this(line, aChar)
        {
            this.countAsLiteral = countAsLiteral;
        }
        public LChar(int line, String aChar)
            : base(line)
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
    }

    class LList : LObject
    {
        new public static LType type = LList.CreateLListType();
        public LType instanceType;
        public List<Expression> elements { get; set; }

        public LList(int line, LType type, List<Expression> elements)
            : base(line)
        {
            this.instanceType = new LType(line, "List", new List<LType>(new LType[] { type }));
            this.elements = elements;

            foreach (Expression e in elements)
            {
                if (!e.getType().typeOrSuperTypeUnifiesWith(type))
                {
                    throw new SemanticErrorException("Type mismatch in element of list.", this.getLine());
                }
            }
        }

        public override void scopeSetting()
        {
            this.elements.ForEach(e => e.setScope(this.getScope()));
        }

        override public LType getType()
        {
            return this.instanceType;
        }

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
            foreach (Expression e in this.elements)
            {
                e.shouldBePushedToStack = true;
                e.codeGenerationPass(compiler);
            }
            if (this.shouldBePushedToStack)
            {
                compiler.addAction(new MachineInstructions.NewP(30));
            }
            else
            {
                compiler.addAction(new MachineInstructions.New(30));
            }

            if (this.shouldBePushedToStack)
            {
                compiler.addAction(new MachineInstructions.AddP(this.elements.Count()));
            }
            else
            {
                compiler.addAction(new MachineInstructions.Add(this.elements.Count()));
            }
        }
    }

    class LNumber : LObject
    {
        float value;
        new public static LType type = LNumber.type = new LType(0, "Number");

        public LNumber(int line, Token sign, String numberAsString)
            : base(line)
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
        override public LType getType()
        {
            return type;
        }
        public override string encodeAsString()
        {
            return String.Format("{0}", this.value);
        }

        public override void codeGenerationPass(LeuterperCompiler compiler)
        {
            base.codeGenerationPass(compiler);
            compiler.addLiteral(new MachineInstructions.Literal("Number", this.encodeAsString()));
        }
    }

    class LVoid : LObject
    {
        new public static LType type = new LType(0, "Void");
        public LVoid(int line) : base(line) { }
        public override LType getType()
        {
            return LVoid.type;
        }
        public override string encodeAsString()
        {
            return "";
        }
        public override void codeGenerationPass(LeuterperCompiler compiler)
        {
            compiler.addLiteral(new MachineInstructions.Literal("Void", encodeAsString()));
        }
    }
    class LClassSpecial : LClass
    {
        public LClassSpecial(
            int line,
            LType type,
            LType parentType,
            UniquesList<LAttribute> LAttributesDeclarations,
            UniquesList<Class_Procedure> classProcedures,
            int classIdentifier)
            : base(line, type, parentType, LAttributesDeclarations, classProcedures)
        {
            this.identifier = classIdentifier;
        }
        public override void codeGenerationPass(LeuterperCompiler compiler) { }
    }

    class LAttribute : Declaration, IRedefinable<LAttribute>, ISignable<LAttribute>
    {
        public LAttribute(int line, LType type, String name)
            : base(line, type, name) { }
        public override void scopeSetting()
        {
            base.scopeSetting();
            this.getType().setShouldStartRedefinition(true);
        }

        public override void symbolsRegistration(LeuterperCompiler compiler) { }

        public override void symbolsUnificationPass()
        {
            base.symbolsUnificationPass();
        }
        public LAttribute redefineWithSubstitutionTypes(List<LType> instantiatedTypes)
        {
            return new LAttribute(this.getLine(), this.getType().substituteTypeAndVariableTypesWith(instantiatedTypes), this.getName());
        }
        public bool HasSameSignatureAs(LAttribute otherElement)
        {
            return this.getName().Equals(otherElement.getName());
        }
    }

    abstract class Declaration : Construction, IDeclaration, ISignable<Declaration>
    {
        private LType type;
        private string name;

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
        public override void scopeSetting()
        {
            this.getType().setScope(this.getScope());
        }
        public override void symbolsUnificationPass()
        {
            this.getType().symbolsUnificationPass();
        }
        public override void classesGenerationPass()
        {
            this.type.classesGenerationPass();
        }
        public override void simplificationAndValidationPass() { }
        public override void codeGenerationPass(LeuterperCompiler compiler) { }
        public override String ToString()
        {
            return String.Format("{0} {1}", this.getType().SignatureAsString(), this.getName());
        }
    }

    class Parameter : Declaration, IRedefinable<Parameter>
    {
        public Parameter(int line, LType type, String name) : base(line, type, name) { }

        public override void symbolsRegistration(LeuterperCompiler compiler) { }
        public Parameter redefineWithSubstitutionTypes(List<LType> instantiatedTypes)
        {
            return new Parameter
                (
                    this.getLine(),
                    this.getType().substituteTypeAndVariableTypesWith(instantiatedTypes),
                    this.getName()
                );
        }
    }

    class Var : Declaration, IAction, ISignable<Var>
    {
        public Expression initialValue;
        public Var(int line, LType type, String name, Expression initialValue) : base(line, type, name)
        {
            this.initialValue = initialValue;
        }
        public Var(int line, LType type, String id) : this(line, type, id, null) { }
        public override void scopeSetting()
        {
            this.getType().setShouldStartRedefinition(true);
            this.getScope().addChild(this.getType());
            if (this.initialValue != null)
            {
                this.initialValue.shouldBePushedToStack = true;
                this.getScope().addChild(this.initialValue);
            }
        }
        public override void symbolsRegistration(LeuterperCompiler compiler) { }
        public override void symbolsUnificationPass()
        {
            base.symbolsUnificationPass();
            scopeSetting();
            this.getType().symbolsUnificationPass();
        }
        public Var redefineWithSubstitutionTypes(List<LType> instantiatedTypes)
        {
            return new Var(this.getLine(), this.getType().substituteTypeAndVariableTypesWith(instantiatedTypes), this.getName());
        }
        
        public bool HasSameSignatureAs(Var otherVar)
        {
            return base.HasSameSignatureAs(otherVar);
        }
    }
    abstract class Conditional : Construction, IAction
    {
        public Expression booleanExpression;
        public List<IAction> thenActions;
        public Conditional(int line, Expression booleanExpression, List<IAction> thenActions)
            : base(line)
        {
            this.booleanExpression = booleanExpression;
            this.thenActions = thenActions;
        }
        public override void scopeSetting()
        {
            booleanExpression.shouldBePushedToStack = true;
            booleanExpression.setScope(this.getScope());
            this.thenActions.ForEach(a => a.setScope(this.getScope()));
        }
        public override void symbolsRegistration(LeuterperCompiler compiler)
        {
            this.thenActions.ForEach(a => a.symbolsRegistration(compiler));
        }
        public override void symbolsUnificationPass()
        {
            this.booleanExpression.symbolsUnificationPass();
            this.thenActions.ForEach(a => a.symbolsUnificationPass());
        }
        public override void classesGenerationPass()
        {
            this.booleanExpression.classesGenerationPass();
            this.thenActions.ForEach(a => a.classesGenerationPass());
        }
        public override void simplificationAndValidationPass() { }
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
    }

    class If_Then_Else : Conditional
    {
        List<IAction> elseActions;

        public If_Then_Else(int line, Expression booleanExpression, List<IAction> thenActions, List<IAction> elseActions)
            : base(line, booleanExpression, thenActions)
        {
            this.elseActions = elseActions;
            if (this.elseActions == null)
            {
                this.elseActions = new List<IAction>();
            }
        }
        public override void scopeSetting()
        {
            base.scopeSetting();
            this.elseActions.ForEach(a => a.setScope(this.getScope()));
        }
        public override void symbolsRegistration(LeuterperCompiler compiler)
        {
            base.symbolsRegistration(compiler);
            this.elseActions.ForEach(a => a.symbolsRegistration(compiler));
        }
        public override void symbolsUnificationPass()
        {
            base.symbolsUnificationPass();
            foreach (IAction a in elseActions)
            {
                a.symbolsUnificationPass();
            }
        }

        public override void classesGenerationPass() { }

        public override void codeGenerationPass(LeuterperCompiler compiler)
        {
            MachineInstructions.JMPF jumpToElse = new MachineInstructions.JMPF();
            MachineInstructions.JMPF endOfThen = new MachineInstructions.JMPF();

            this.booleanExpression.codeGenerationPass(compiler);
            compiler.addAction(jumpToElse);
            this.thenActions.ForEach(a => a.codeGenerationPass(compiler));
            compiler.addAction(endOfThen);
            jumpToElse.whereToJump = compiler.getIndexOfNextActionInCurrentFunction();
            this.elseActions.ForEach(a => a.codeGenerationPass(compiler));
            endOfThen.whereToJump = compiler.getIndexOfNextActionInCurrentFunction();
        }
    }

    class Loop_While : Conditional
    {
        public Loop_While(int line, Expression booleanExpression, List<IAction> thenActions)
            : base(line, booleanExpression, thenActions) {}
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
    }

    class Call_Function : Call_Procedure
    {
        public Call_Function(int line, String procedureName, List<Expression> arguments)
            : base(line, procedureName, arguments)
        {
            this.scopeSetting();
        }

        public override Procedure getProcedureDefinition()
        {
            return ScopeManager.getFunctionForGivenNameAndArguments(this.getScope(), this.procedureName, this.arguments);
        }
    }

    abstract class Call_Procedure : Expression
    {
        public String procedureName { get; set; }
        public List<Expression> arguments { get; set; }

        public Call_Procedure(int line, String procedureName, List<Expression> arguments)
            : base(line)
        {
            this.procedureName = procedureName;
            this.arguments = arguments;
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
                throw new SemanticErrorException(
                    String.Format("Called inexistent procedure: {0}\n With types: {1}",
                        this.procedureName,
                        LType.listOfTypesAsString(Expression.expressionsToTypes(this.arguments))),
                    this.getLine());
            }
            return p.identifier;
        }
        public override void scopeSetting()
        {
            foreach (Expression e in arguments)
            {
                e.setScope(this.getScope());
                e.shouldBePushedToStack = true;
                e.scopeSetting();
            }
        }
        public override void symbolsUnificationPass()
        {
            this.arguments.ForEach(a => a.symbolsUnificationPass());
        }
        public override void classesGenerationPass() { }
        public override void simplificationAndValidationPass() { }
        public override void codeGenerationPass(LeuterperCompiler compiler)
        {
            foreach (Expression argument in arguments)
            {
                argument.codeGenerationPass(compiler);
            }
            if (this.shouldBePushedToStack)
            {
                compiler.addAction(new MachineInstructions.CallP(this.getProcedureIdentifier()));
            }
            else
            {
                compiler.addAction(new MachineInstructions.Call(this.getProcedureIdentifier()));
            }
        }
    }
    class Call_Constructor : Call_Procedure
    {
        public LType type { get; set; }
        public Call_Constructor(int line, LType type, List<Expression> arguments)
            : base(line, type.getName(), arguments)
        {
            this.type = type;
            this.scopeSetting();
        }
        public override Procedure getProcedureDefinition()
        {
            LClass c = ScopeManager.getClassForName(this.getScope(), this.procedureName);
            return getConstructorFromClass(c);
        }
        public Constructor getConstructorFromClass(LClass c)
        {
            List<LType> argumentsTypes = Expression.expressionsToTypes(arguments);
            foreach (Constructor cd in c.constructorDefinitions)
            {
                if (cd.isCompatibleWithNameAndTypes(procedureName, argumentsTypes))
                {
                    return cd;
                }
            }
            if (c.getParentClass() != null)
            {
                return getConstructorFromClass(c.getParentClass());
            }
            throw new SemanticErrorException(
                String.Format("Called an undefined constructor.\n\t{0}\n\t{1}",
                procedureName,
                LType.listOfTypesAsString(argumentsTypes)), this.getLine());
        }
        public override void classesGenerationPass()
        {
            this.getType().classesGenerationPass();
        }
        public override void codeGenerationPass(LeuterperCompiler compiler)
        {
            foreach (Expression argument in arguments)
            {
                argument.codeGenerationPass(compiler);
            }
            if (this.shouldBePushedToStack)
            {
                compiler.addAction(new MachineInstructions.NewP(this.getProcedureIdentifier()));
            }
            else
            {
                compiler.addAction(new MachineInstructions.New(this.getProcedureIdentifier()));
            }
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

        public override void scopeSetting()
        {
            base.scopeSetting();
            this.theObject.setScope(this.getScope());
        }

        public override void symbolsUnificationPass()
        {
            base.symbolsUnificationPass();
            this.theObject.symbolsUnificationPass();
        }
        public override void classesGenerationPass()
        {
            theObject.classesGenerationPass();
        }
        public override Procedure getProcedureDefinition()
        {
            return this.getMethodWithNameAndTypes();
        }
        private Method getMethodWithNameAndTypes()
        {
            LType calleeType = theObject.getType();
            return this.getMethodFromClass(calleeType.getDefiningClass());
        }

        public override LType getType()
        {
            return this.getMethodWithNameAndTypes().getType();
        }
        private Method getMethodFromClass(LClass aClass)
        {
            List<LType> argumentsTypes = Expression.expressionsToTypes(this.arguments);
            foreach (Method m in aClass.methodsDefinitions)
            {
                if (m.isCompatibleWithNameAndTypes(this.procedureName, argumentsTypes)) return m;
            }
            if (aClass.getParentClass() != null)
            {
                return this.getMethodFromClass(aClass.getParentClass());
            }
            throw new SemanticErrorException(
                String.Format("Used an undefined method.\n\t{0}\n\t{1}",
                this.procedureName,
                LType.listOfTypesAsString(argumentsTypes)), this.getLine());
        }
    }
}