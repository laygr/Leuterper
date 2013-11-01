﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper
{
    interface IDeclaration : IDefinition, ISignable<IDeclaration>
    {
        String getName();
    }
}
