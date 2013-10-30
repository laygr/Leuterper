using Leuterper.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    class Call_Constructor : Call_Class_Procedure
    {
        public LType type { get; set; }
        public Call_Constructor(int line, LType type, List<Expression> arguments)
            : base(line, type.getName(), arguments) {
                this.type = type;
        }

        public override Procedure getProcedureDefinition()
        {
            LClass c = this.getScope().getScopeManager().getClassForName(this.procedureName);
            return getConstructorFromClass(c);    
        }
        public Constructor getConstructorFromClass(LClass c)
        {
            List<LType> argumentsTypes = Expression.expressionsToTypes(arguments);
            foreach (Constructor cd in c.constructorDefinitions)
            {
                if (cd.isCompatibleWithNameAndTypes(procedureName, argumentsTypes))
                {
                    return cd;
                }
            }
            if (c.getParentClass() != null)
            {
                return getConstructorFromClass(c.getParentClass());
            }
            throw new SemanticErrorException(
                String.Format("Called an undefined constructor.\n\t{0}\n\t{1}",
                procedureName,
                LType.listOfTypesAsString(argumentsTypes)), this.getLine());
        }
        public override void thirdPass()
        {
            this.getType().thirdPass();
        }
        public override void generateCode(LeuterperCompiler compiler)
        {
            foreach (Expression argument in arguments)
            {
                argument.generateCode(compiler);
            }
            if (this.shouldBePushedToStack)
            {
                compiler.addAction(new MachineInstructions.NewP(this.getProcedureIdentifier()));
            }
            else
            {
                compiler.addAction(new MachineInstructions.New(this.getProcedureIdentifier()));
            }
        }
    }
}
