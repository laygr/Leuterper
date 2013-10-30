using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    abstract class Class_Declaration : Construction, IStatement, IDeclaration, IIdentifiable<Class_Declaration>
    {
        private LType type { get; set; }
        private string name { get; set; }
        public Class_Declaration(int line, LType type, string name) : base(line)
        {
            this.type = type;
            this.name = name;
        }
        public LType getType()
        {
            return this.getType();
        }

        public void setType(LType type)
        {
            this.type = type;
        }
        public string getName()
        {
            return this.getName();
        }

        public bool HasSameSignatureAs(Class_Declaration otherElement)
        {
            return
                this.getType().HasSameSignatureAs(otherElement.getType())
                &&
                this.getName().Equals(otherElement.getName());
        }
        public override String ToString()
        {
            return String.Format("{0} {1}", this.getType().SignatureAsString(), this.getName());
        }

        public override void secondPass(LeuterperCompiler compiler)
        {
            throw new NotImplementedException();
        }

        public override void thirdPass()
        {
            throw new NotImplementedException();
        }

        public override void generateCode(LeuterperCompiler compiler)
        {
            throw new NotImplementedException();
        }
    }
}
