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
        public Assignment(int destinationAddress) { this.destinationAddress = destinationAddress; }
        public override string ToString() { return String.Format("ass {0}", destinationAddress); }
    }
    class List : MachineInstruction
    {
        public int howMany;
        bool shouldPop;
        public List(int howMany, bool shouldPop) { this.howMany = howMany; this.shouldPop = shouldPop; }
        public override string ToString() { return String.Format("list {0} {1}", this.howMany, this.shouldPop); }
    }
    class Call : MachineInstruction
    {
        public int functionId;
        public bool shouldPop;
        public Call(int functionId, bool shouldPop) { this.functionId = functionId; this.shouldPop = shouldPop; }
        public override string ToString() { return String.Format("call {0} {1}", this.functionId, this.shouldPop); }
    }
    public class JMP : MachineInstruction
    {
        public int whereToJump;
        public override string ToString() { return String.Format("jmp {0}", this.whereToJump); }
        public JMP() { }
        public JMP(int whereToJMP) { this.whereToJump = whereToJMP; }
    }
    public class JMPF : JMP
    {
        public override string ToString() { return String.Format("jmpf {0}", this.whereToJump); }
        public JMPF() : base() { }
        public JMPF(int whereToJump) : base(whereToJump) { }
    }
    class JMPT : JMP
    {
        public override string ToString() { return String.Format("jmpt {0}", this.whereToJump); }
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
        public override string ToString() { return String.Format("{0} {1}", type, encodedLiteral); }
    }
    class New : MachineInstruction
    {
        public int procedureId;
        public int classId;
        bool shouldPush;
        public New(int procedureId, int classId, bool shouldPush) { this.procedureId = procedureId; this.classId = classId; this.shouldPush = shouldPush; }
        public override string ToString() { return String.Format("new {0} {1} {2}", procedureId, classId, shouldPush); }
    }
    class Number : MachineInstruction
    {
        public int value { get; set; }
        public Number(int value) { this.value = value; }
        public override string ToString() { return String.Format("{0}", value); }
    }
    class Push : MachineInstruction
    {
        public int address;
        public override string ToString() { return String.Format("push {0}", this.address); }
        public Push(int address) { this.address = address; }
    }
    class Rtn : MachineInstruction
    {
        public override string ToString() { return "rtn"; }
    }
    class Get : MachineInstruction
    {
        public int LAttributeIndex;
        public Get(int LAttributeIndex) { this.LAttributeIndex = LAttributeIndex; }
        public override string ToString()
        {
            return String.Format("get {0}", this.LAttributeIndex);
        }
    }

    class Set : MachineInstruction
    {
        public int LAttributeIndex;
        public Set(int LAttributeIndex) { this.LAttributeIndex = LAttributeIndex; }
        public override string ToString()
        {
            return String.Format("set {0}", this.LAttributeIndex);
        }
    }
}