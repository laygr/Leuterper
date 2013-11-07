using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leuterper.Constructions;

namespace Leuterper
{

    interface ISignable<X>
    { bool HasSameSignatureAs(X otherElement); }

    interface IScope
    {
        List<Declaration> getDeclarations();
        List<Construction> getChildren();
        void addChild(Construction c);
        IScope getScope();
    }

    interface IRedefinable<X>
    {  X redefineWithSubstitutionTypes(List<LType> types); }

    interface IDefinition
    {
        LType getType();
        void setType(LType type);
    }

    interface IDeclaration : IDefinition, ISignable<IDeclaration>
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
        /*
         * Connect strings to their symbols
         * set class identifiers
         * set procedure identifiers
         * should be pushed to stack
         * get defining classes of each type
         */
        void scopeSetting();
        void symbolsRegistration(LeuterperCompiler compiler);
        void symbolsUnificationPass();
        void classesGenerationPass();
        void simplificationAndValidationPass();
        void codeGenerationPass(LeuterperCompiler compiler);
    }
}