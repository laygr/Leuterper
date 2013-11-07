using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leuterper.Constructions;
namespace Leuterper
{
    class VariableNotInsideAClass : VariableIndexFindStrategy
    {
        public override int getVariableIndex(LType type)
        {
            return -1;
        }
    }
}
