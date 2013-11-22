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
        public int howMany { get; set; }
        public List(int howMany) { this.howMany = howMany; }
        public override string ToString() { return String.Format("list {0}", this.howMany); }
    }
    class ListP : List
    {
        public ListP(int howMany) : base(howMany) { }
        public override string ToString() { return String.Format("listp {0}", this.howMany); }
    }
    class Call : MachineInstruction
    {
        public int functionId { get; set; }
        public Call(int functionId) { this.functionId = functionId; }
        public override string ToString() { return String.Format("call {0}", this.functionId); }
    }
    class CallP : Call
    {
        public CallP(int functionId) : base(functionId) {}
        public override string ToString() { return String.Format("callp {0}", this.functionId); }
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
        public New(int procedureId, int classId) { this.procedureId = procedureId; this.classId = classId; }
        public override string ToString() { return String.Format("new {0} {1}", procedureId, classId); }
    }
    class NewP : New
    {
        public NewP(int procedureId, int classId) : base(procedureId, classId) { }
        public override string ToString() { return String.Format("newp {0} {1}", procedureId, classId); }
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