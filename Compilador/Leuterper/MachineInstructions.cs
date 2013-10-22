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
            return String.Format("{0}", value);
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
            return String.Format("ass {0} {1}", originAddress, destinationAddress);
        }
    }

    class Call : MachineInstruction
    {
        public int functionId { get; set; }
        public Call(int functionId)
        {
            this.functionId = functionId;
        }
        public string toString(int n)
        {
            return String.Format("call {0}", this.functionId);
        }
    }

    class JMP : MachineInstruction
    {
        public int whereToJump { get; set; }

        public string toString()
        {
            return String.Format("jmp {0}", this.whereToJump);
        }
    }

    class JMPF : JMP
    {
        new public string toString()
        {
            return String.Format("jmpf {0}", this.whereToJump);
        }

    }

    class JMPT : JMP
    {
        new public string toString()
        {
            return String.Format("jmpt {0}", this.whereToJump);
        }
    }

    class Push : MachineInstruction
    {
        public int address { get; set; }

        public string toString()
        {
            return String.Format("push {0}", this.address);
        }
    }

    class New : MachineInstruction
    {
        public int classId;
        public int functionId;

        public New(int classId, int functionId)
        {
            this.functionId = functionId;
            this.classId = classId;
        }

        public string toString()
        {
            return String.Format("new {0} {1}",classId, functionId);
        }
    }

    class NewP : New
    {
        public NewP(int classId, int functionId) : base(classId, functionId) { }
        public string toString()
        {
            return String.Format("newp {0} {1}", classId, functionId);
        }
    }
}