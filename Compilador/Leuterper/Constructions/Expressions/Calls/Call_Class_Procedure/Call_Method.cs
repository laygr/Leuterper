using Leuterper.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    class Call_Method : Call_Procedure
    {
        public Expression theObject { get; set; }
        public Call_Method(int line, Expression theObject, String methodId, List<Expression> arguments)
            : base(line, methodId, arguments)
        {
            this.theObject = theObject;
        }
        public override void secondPass(LeuterperCompiler compiler)
        {
            this.arguments.Insert(0, theObject);
            base.secondPass(compiler);
        }
        public override void thirdPass()
        {
            theObject.thirdPass();
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
            foreach(Method m in aClass.methodsDefinitions)
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
