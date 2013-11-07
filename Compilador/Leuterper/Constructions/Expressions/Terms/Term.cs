using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    abstract class Term : Expression
    {
        public Term(int line) : base(line)
        { }
       
    }
}