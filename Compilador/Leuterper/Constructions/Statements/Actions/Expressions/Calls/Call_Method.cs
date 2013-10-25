using Leuterper.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    class Call_Method : Call_Function
    {
        public Expression theObject { get; set; }

        public Call_Method(int line, Expression theObject, String methodId, List<Expression> arguments)
            : base(line, methodId, arguments)
        {
            this.theObject = theObject;
        }

        public override void secondPass()
        {
            this.arguments.Insert(0, theObject);
            base.secondPass();
        }

        public override int getFunctionIdentifier()
        {
            Definition_Method m = this.getMethodWithNameAndTypes();

           if(m == null)
           {
               String errorMessage = String.Format("Called an undefined method\n\tName: {0}\n\tParameters: {1}", this.functionName, LType.listOfTypesAsString(Expression.expressionsToTypes(this.arguments)));
               throw new SemanticErrorException(errorMessage, this.line);
           }
            return m.identifier;
        }
        public Definition_Method getMethodWithNameAndTypes ()
        {
            LType calleeType = theObject.getType();
            Definition_Class typesClass = this.scope.GetScopeManager().getClassForType(calleeType);
            return typesClass.getMethodWithNameAndTypes(this.functionName, Expression.expressionsToTypes(this.arguments));
        }

    }
}
