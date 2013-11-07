using Leuterper.Exceptions;
using System;
using System.Collections.Generic;

namespace Leuterper.Constructions
{
    abstract class Call_Procedure : Expression
    {
        public String procedureName { get; set; }
        public List<Expression> arguments { get; set; }

        public Call_Procedure(int line, String procedureName, List<Expression> arguments) : base(line)
        {
            this.procedureName = procedureName;
            this.arguments = arguments;
        }

        abstract  public Procedure getProcedureDefinition();

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
            foreach(Expression argument in arguments)
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
}
