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
}
