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
        UniquesList<Declaration> getDeclarations();
    }

    interface IRedefinable<X>
    {  X redefineWithSubstitutionTypes(List<LType> types); }

    interface IDefinition
    {
        LType getType();
        void setType(LType type);
    }

    interface IDeclaration : IDefinition
    {  String getName();  }
    interface IConstruction : ICompilable
    {
        IScope getScope();
        void setScope(IScope scope);
        int getLine();
    }
    interface IAction : IConstruction { }
    interface ICompilable
    {
        void scopeSettingPass();
        void symbolsUnificationPass();

        /*
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