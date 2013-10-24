using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    abstract class Conditional : LAction
    {
        public Expression booleanExpression;
        public List<LAction> thenActions;

        public Conditional(int line, Expression booleanExpression, List<LAction> thenActions) : base(line)
        {
            this.booleanExpression = booleanExpression;
            this.thenActions = thenActions;
        }
        public override void secondPass()
        {
            booleanExpression.shouldBePushedToStack = true;
            booleanExpression.scope = this.scope;
            booleanExpression.secondPass();

            foreach(LAction a in thenActions)
            {
                a.scope = this.scope;
                a.secondPass();
            }
        }
    }
}
