using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    abstract class Declaration : LAction
    {
        public LType type { get; set; }

        public Declaration(int line, LType type) : base(line)
        {
            this.type = type;
        }
    }
}
