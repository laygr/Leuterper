using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leuterper.Constructions;

namespace Leuterper
{
    abstract class VariableIndexFindStrategy
    {
        public abstract int getVariableIndex(LType type);
    }
}
