using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper{

    interface IIdentifiable<A>
    {
        Boolean HasSameSignatureAs(A otherElement);
        String SignatureAsString();
    }
}
