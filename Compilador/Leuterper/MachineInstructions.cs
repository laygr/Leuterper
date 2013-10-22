using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.MachineInstructions
{
    public abstract class MachineInstruction { }

    public class Literal : MachineInstruction
    {
        string type;
        string encodedLiteral;
        public Literal(string type, string encodedLiteral)
        {
            this.type = type;
            this.encodedLiteral = encodedLiteral;
        }
    }

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

    public class JMP : MachineInstruction
    {
        public int whereToJump { get; set; }

        public string toString()
        {
            return String.Format("jmp {0}", this.whereToJump);
        }

        public JMP() { }

        public JMP(int whereToJMP)
        {
            this.whereToJump = whereToJump;
        }
    }

    public class JMPF : JMP
    {
        new public string toString()
        {
            return String.Format("jmpf {0}", this.whereToJump);
        }

        public JMPF() : base() { }

        public JMPF(int whereToJump) : base(whereToJump) { }

    }

    class JMPT : JMP
    {
        new public string toString()
        {
            return String.Format("jmpt {0}", this.whereToJump);
        }
        public JMPT() : base() { }
        public JMPT(int whereToJump) : base(whereToJump) { }
    }

    class Push : MachineInstruction
    {
        public int address { get; set; }

        public string toString()
        {
            return String.Format("push {0}", this.address);
        }

        public Push(int address)
        {
            this.address = address;
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