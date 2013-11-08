using Leuterper.Exceptions;
using System.Collections.Generic;

namespace Leuterper.Constructions
{
    class Constructor : Class_Procedure
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
            LAttributeAccess baseAccess = new LAttributeAccess(this.getLine(), aThis , "super");
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
    }
}
