using Leuterper.Constructions;
using System;
using System.Collections.Generic;

namespace Leuterper
{
    interface ISignable<X>
    { bool HasSameSignatureAs(X otherElement); }

    interface IScope
    {
        IScope getScope();
        void scopeSetting();
        UniquesList<Declaration> getDeclarations();
    }

    interface IReinstantiatable<X>
    { X reinstantiateWithSubstitutions(Substitutions s); }

    interface IDefinition
    {
        LType getType();
        void setType(LType type);
    }

    interface IDeclaration : IDefinition
    {  String getName();  }
    interface IConstruction : ICompilable, ICloneable
    {
        IScope getScope();
        void setScope(IScope scope);
        int getLine();
    }
    interface IAction : IConstruction { }
    interface ICompilable
    {
        /*
        void symbolsUnificationPass();

        
        void symbolsRegistrationPass();
        void classesGenerationPass();
        void simplificationPass();
         */
        void codeGenerationPass(LeuterperCompiler compiler);
    }
    interface IBlock
    {
        UniquesList<Declaration> getDeclarations();
        void expandActions(List<IAction> actions);
    }
}