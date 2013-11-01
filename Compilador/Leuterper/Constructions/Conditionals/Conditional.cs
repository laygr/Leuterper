using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    abstract class Conditional : Construction, IAction
    {
        public Expression booleanExpression;
        public List<IAction> thenActions;
        public Conditional(int line, Expression booleanExpression, List<IAction> thenActions) : base(line)
        {
            this.booleanExpression = booleanExpression;
            this.thenActions = thenActions;
        }
        public override void secondPass(LeuterperCompiler compiler)
        {
            booleanExpression.shouldBePushedToStack = true;
            booleanExpression.setScope(this.getScope());
            booleanExpression.secondPass(compiler);

            foreach(IAction a in thenActions)
            {
                a.setScope(this.getScope());
                a.secondPass(compiler);
            }
        }
        public override void thirdPass() { }
    }
}
