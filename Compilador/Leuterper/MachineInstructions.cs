using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.MachineInstructions
{
    abstract class MachineInstruction { }

    class Number : MachineInstruction
    {
        public int value { get; set; }
        public Number(int value)
        {
            this.value = value;
        }

        public string toString()
        {
            return String.Format("%d", value);
        }
    }

    class Assignment : MachineInstruction
    {
        public int originAddress { get; set; }
        public int destinationAddress { get; set; }

        public Assignment(int destinationAddress)
        {
            this.destinationAddress = destinationAddress;
        }

        public string toString()
        {
            return String.Format("ass\t%d\t%d", originAddress, destinationAddress);
        }
    }

    class Call : MachineInstruction
    {
        public int functionId { get; set; }
        public string toString(int n)
        {
            return String.Format("call\t%d", this.functionId);
        }
    }

    class JMP : MachineInstruction
    {
        public int whereToJump { get; set; }

        public string toString()
        {
            return String.Format("%jmp\t%d", this.whereToJump);
        }
    }

    class JMPF : JMP
    {
        new public string toString()
        {
            return String.Format("%jmpf\t%d", this.whereToJump);
        }

    }

    class JMPT : JMP
    {
        new public string toString()
        {
            return String.Format("%jmpt\t%d", this.whereToJump);
        }
    }

    class Push : MachineInstruction
    {
        public int address { get; set; }

        public string toString()
        {
            return String.Format("push\t%d", this.address);
        }
    }
}
