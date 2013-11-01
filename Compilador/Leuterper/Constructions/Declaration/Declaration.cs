using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    abstract class Declaration : Construction, IDeclaration
    {
        private LType type { get; set; }
        private string name { get; set; }

        public Declaration(int line, LType type, String name)
            : base(line)
        {
            this.type = type;
            this.name = name;
        }
        public string getName()
        {
            return this.name;
        }
        public LType getType()
        {
            return this.type;
        }
        public void setType(LType type)
        {
            this.type = type;
        }
        virtual public bool HasSameSignatureAs(IDeclaration otherElement)
        {
            return
                this.getType().HasSameSignatureAs(otherElement.getType())
                &&
                this.getName().Equals(otherElement.getName());
        }
        public override void secondPass(LeuterperCompiler compiler)
        {
            this.getType().setScope(this.getScope());
            this.getType().secondPass(compiler);
        }
        public override void thirdPass()
        {   
            this.type.thirdPass();
        }
        public override void generateCode(LeuterperCompiler compiler) { }
        public override String ToString()
        {
            return String.Format("{0} {1}", this.getType().SignatureAsString(), this.getName());
        }
    }
}
