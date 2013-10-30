using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper
{
    interface ICompilable : IScopable
    {
        /*
         * Connect strings to their symbols
         * set class identifiers
         * set procedure identifiers
         * should be pushed to stack
         * get defining classes of each type
         */
        void secondPass(LeuterperCompiler compiler);
        
        //Generic classes to non-generic
        void thirdPass();
        void generateCode(LeuterperCompiler compiler);

    }
}
