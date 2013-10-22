using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    class This : Term
    {

        public This(int line) : base(line) { }

        override public LType getType()
        {
            if(this.scope is Program)
            {
                Console.WriteLine("this inside a program makes no sense.");
                Console.ReadKey();
                Environment.Exit(0);
            }

            if(this.scope is Definition_Function)
            {
                Console.WriteLine("this inside a function makes no sense.");
                Console.ReadKey();
                Environment.Exit(0);
            }

            Definition_Class classDefinition = null;
            if(this.scope is Definition_Class)
            {
                classDefinition = this.scope as Definition_Class;
                return classDefinition.type;
            }else if(this.scope is Definition_Method)
            {
                Definition_Method methodDefinition = this.scope as Definition_Method;
                classDefinition = methodDefinition.scope as Definition_Class;
                return classDefinition.type;
            }
            return null;
        }

        public override void generateCode(LeuterperCompiler compiler)
        {
            int index = this.scope.GetScopeManager().getIndexOfVarNamed("this");
            compiler.addMI(new MachineInstructions.Push(index));
        }
    }
}
