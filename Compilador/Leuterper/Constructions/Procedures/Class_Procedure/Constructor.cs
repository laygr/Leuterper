using Leuterper.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    class Constructor : Class_Procedure
    {
        public Constructor(int line, string name, List<Parameter> parameters, List<IAction> actions)
            : base(line, new LType(line, name), name, parameters, actions)
        {
        }

        public override bool isCompatibleWithNameAndTypes(string name, List<LType> types)
        {
            if (!this.getType().getName().Equals(name)) return false;
            List<LType> parametersTypes = Parameter.listOfParametersAsListOfTypes(this.parameters);
            parametersTypes.RemoveAt(0);
            return LType.listOfTypesUnify(parametersTypes, types);
        }

        public override void secondPass(LeuterperCompiler compiler)
        {
            this.setType(this.getClass().getType());
            if(!this.getClass().getType().getName().Equals(this.getName()))
            {
                throw new SemanticErrorException("Constructor not named as its class.", this.getLine());
            }
            base.secondPass(compiler);
        }
        public override void thirdPass()
        {
        }
    }
}
