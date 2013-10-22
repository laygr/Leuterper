using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    abstract class Expression : LAction
    {
        public Expression(int line) : base(line) { }
        abstract public LType getType();
    }
}
