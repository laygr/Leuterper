using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.MachineInstructions
{
    public abstract class MachineInstruction { }
    class Assignment : MachineInstruction
    {
        public int originAddress { get; set; }
        public int destinationAddress { get; set; }

        public Assignment(int destinationAddress)
        {
            this.destinationAddress = destinationAddress;
        }

        public override string ToString()
        {
            return String.Format("ass {0}", destinationAddress);
        }
    }
    class Call : MachineInstruction
    {
        public int functionId { get; set; }
        public Call(int functionId)
        {
            this.functionId = functionId;
        }
        public override string ToString()
        {
            return String.Format("call {0}", this.functionId);
        }
    }
    class CallP : Call
    {
        public CallP(int functionId)
            : base(functionId)
        {
        }
        public override string ToString()
        {
            return String.Format("callp {0}", this.functionId);
        }
    }
    public class JMP : MachineInstruction
    {
        public int whereToJump { get; set; }

        public override string ToString()
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
        public override string ToString()
        {
            return String.Format("jmpf {0}", this.whereToJump);
        }

        public JMPF() : base() { }

        public JMPF(int whereToJump) : base(whereToJump) { }

    }
    class JMPT : JMP
    {
        public override string ToString()
        {
            return String.Format("jmpt {0}", this.whereToJump);
        }
        public JMPT() : base() { }
        public JMPT(int whereToJump) : base(whereToJump) { }
    }
    public class Literal : MachineInstruction
    {
        string type;
        string encodedLiteral;
        public Literal(string type, string encodedLiteral)
        {
            this.type = type;
            this.encodedLiteral = encodedLiteral;
        }

        public override string ToString()
        {
            return String.Format("{0} {1}", type, encodedLiteral);
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

        public override string ToString()
        {
            return String.Format("new {0} {1}", classId, functionId);
        }
    }
    class NewP : New
    {
        public NewP(int classId, int functionId) : base(classId, functionId) { }
        public override string ToString()
        {
            return String.Format("newp {0} {1}", classId, functionId);
        }
    }
    class Number : MachineInstruction
    {
        public int value { get; set; }
        public Number(int value)
        {
            this.value = value;
        }

        public override string ToString()
        {
            return String.Format("{0}", value);
        }
    }
    class Push : MachineInstruction
    {
        public int address { get; set; }

        public override string ToString()
        {
            return String.Format("push {0}", this.address);
        }

        public Push(int address)
        {
            this.address = address;
        }
    }

    class Rtn : MachineInstruction
    {
        public override string ToString()
        {
            return "rtn";
        }
    }
}