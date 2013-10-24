using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leuterper.MachineInstructions;

namespace Leuterper.Constructions
{
    class Constructor : Term
    {
        public LType instanceType { get; set; }
        public List<Expression> arguments { get; set; }
        
        public Constructor(int line, LType instanceType, List<Expression> arguments) : base(line)
        {
            this.instanceType = instanceType;
            arguments.Insert(0, new Var(line, "this"));
            this.arguments = arguments;
        }
        override public LType getType()
        {
            return this.instanceType;
        }

        public override void secondPass()
        {
            foreach(Expression e in arguments)
            {
                e.scope = this.scope;
                e.shouldBePushedToStack = true;
                e.secondPass();
            }
        }

        public override void generateCode(LeuterperCompiler compiler)
        {
            for(int i = 0; i < arguments.Count(); i++)
            {
                Expression e = arguments[i];
                e.generateCode(compiler);
            }

            MachineInstruction creationInstruction = null;
            int classId = this.scope.getProgram().getClassForType(this.instanceType).identifier;
            int functionId = this.scope.getProgram().GetScopeManager().getFunctionForGivenNameAndArguments("", this.arguments).identifier;
            if (this.shouldBePushedToStack)
            {
                creationInstruction = new MachineInstructions.NewP(classId, functionId);
            }
            else
            {
                creationInstruction = new MachineInstructions.New(classId, functionId);
            }
            compiler.addAction(creationInstruction);
        }

    }
}
