using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    class Call_Function : Expression
    {
        public String functionName { get; set; }
        public List<Expression> arguments { get; set; }

        public Call_Function(int line, String id, List<Expression> arguments) : base(line)
        {
            this.functionName = id;
            this.arguments = arguments;
        }

        public override LType getType()
        {
            Definition_Function func = this.scope.GetScopeManager().getFunctionForGivenNameAndParameters(
                this.functionName, ParametersList.getParametersFromArguments(this.arguments));

            return func.type;
        }

        public int getFunctionIdentifier()
        {
            Definition_Function fun = this.scope.GetScopeManager().getFunctionForGivenNameAndArguments(this.functionName, this.arguments);
            return fun.identifier;
        }
        override public void secondPass()
        {
            foreach(Expression e in arguments)
            {
                e.scope = this.scope;
                e.shouldBePushedToStack = true;
                e.secondPass();
            }
        }

        override public void generateCode(LeuterperCompiler compiler)
        {
            foreach(Expression argument in arguments)
            {
                argument.generateCode(compiler);
            }
            if (this.shouldBePushedToStack)
            {
                compiler.addAction(new MachineInstructions.CallP(this.getFunctionIdentifier()));
            }
            else
            {
                compiler.addAction(new MachineInstructions.Call(this.getFunctionIdentifier()));
            }
        }
    }
}
