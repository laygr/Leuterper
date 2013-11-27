using Leuterper.Constructions;
using Leuterper.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Leuterper
{
    class Utils
    {
        public static LType parameterToType(Parameter p) { return p.getType(); }
        public static List<LType> listOfParametersAsListOfTypes(List<Parameter> parameters)
        {
            return parameters.ConvertAll(new Converter<Parameter, LType>(parameterToType));
        }
        public static String listOfParametersAsString(List<Parameter> parameters)
        {
            string result = "";
            foreach (Parameter p in parameters)
            {
                result += p.getType().SignatureAsString() + " ";
            }
            return result;
        }
        static int uniquesGenerator = 0;
        public static int getNextUnique()
        {
            uniquesGenerator++;
            return uniquesGenerator;
        }
        public static Parameter typeToParameter(LType t)
        {
            return new Parameter(t.getLine(), t, "fakeParam" + getNextUnique());
        }
        public static List<Parameter> typesToParameters(List<LType> types)
        {
            return types.ConvertAll(new Converter<LType, Parameter>(typeToParameter));
        }
        public static String listOfTypesAsString(List<LType> types)
        {
            string result = "";
            foreach (LType t in types)
            {
                result += t.SignatureAsString() + ", ";
            }
            return result;
        }
        public static Boolean listOfTypesUnify(List<LType> a, List<LType> b)
        {
            if (a.Count() != b.Count()) return false;
            for (int i = 0; i < b.Count(); i++)
            {
                if (!b[i].typeOrSuperTypeUnifiesWith(a[i])) return false;
            }
            return true;
        }
        public static Boolean listOfTypesMatch(List<LType> a, List<LType> b)
        {
            if (a.Count() != b.Count()) return false;
            for (int i = 0; i < b.Count(); i++)
            {
                if (!a[i].HasSameSignatureAs(b[i])) return false;
            }
            return true;
        }
        public static List<IAction> reinstantiateActions(List<IAction> actions, List<LType> instantiatedTypes)
        {
            List<IAction> newActions = new List<IAction>();
            foreach (IAction a in actions)
            {
                if (a is Var)
                {
                    newActions.Add((a as Var).redefineWithSubstitutionTypes(instantiatedTypes));
                }
                else
                {
                    newActions.Add(a);
                }
            }
            return newActions;
        }
        public static List<Parameter> reinstantiateParameters(List<Parameter> parameters, List<LType> instantiatedTypes)
        {
            List<Parameter> newParameters = new List<Parameter>();
            parameters.ForEach(p => newParameters.Add(p.redefineWithSubstitutionTypes(instantiatedTypes)));
            return newParameters;
        }
        public static LType expresionToType(Expression e) { return e.getType(); }
        public static List<LType> expressionsToTypes(List<Expression> expression)
        {
            return expression.ConvertAll(new Converter<Expression, LType>(expresionToType));
        }
        public static void expandActions(List<Declaration> declarations, List<IAction>deposit, List<IAction> actions)
        {
            for (int i = 0; i < actions.Count(); i++)
            {
                IAction a = actions[i];
                if (a is Var)
                {
                    declarations.Add(a as Var);
                    Var v = a as Var;
                    if (v.initialValue != null)
                    {
                        VarAccess var = new VarAccess(v.getLine(), v.getName());
                        actions.Insert(i + 1, new Assignment(v.getLine(), var, v.initialValue));
                        v.initialValue = null;
                    }
                }
                else if (a is IBlock)
                {
                    (a as IBlock).getDeclarations().ForEach(d => declarations.Add(d));
                }
                deposit.Add(a);
            }
        }
    }

    class UniquesList<X> : List<X> where X : Construction, ISignable<X>
    {
        public UniquesList(X[] elements) : base(elements) { }
        public UniquesList(List<X> elements) : base(elements) { }
        public UniquesList() { }
        private void validateUniqueness(X newElement)
        {
            Boolean reallyNew = this.TrueForAll(e => !e.HasSameSignatureAs(newElement));
            if (!reallyNew) throw new SemanticErrorException(String.Format("Two definitions defined with same sinature: {0}", newElement), newElement.getLine());
        }
        public void AddUnique(X newElement)
        {
            this.validateUniqueness(newElement);
            this.Add(newElement);
        }
        internal void InsertUnique(int p, X newElement)
        {
            this.validateUniqueness(newElement);
            this.Insert(p, newElement);
        }
    }
    class TypeVariablesComparer : IEqualityComparer<List<LType>>
    {
        public bool Equals(List<LType> x, List<LType> y)
        {
            if(x.Count() != y.Count()) return false;
            for (int i = 0; i < x.Count(); i++)
            {
                if (!x[i].UnifiesWith(y[i])) return false;
            }
            return true;
        }

        public int GetHashCode(List<LType> obj)
        {
            throw new NotImplementedException();
        }
    }
    class DeclarationLocator<X> where X : Declaration
    {
        public X declaration;
        public int index;
        public int hierarchyDistance;
        public bool found;
        public DeclarationLocator(X declaration, int hierarchyDistance, int index, bool found)
        {
            this.hierarchyDistance = hierarchyDistance;
            this.index = index;
            this.declaration = declaration;
            this.found = found;
        }
        public DeclarationLocator() : this(null, 0, 0, false) { }
        public void wasFound(X declaration, int index)
        {
            this.declaration = declaration;
            this.index = index;
            this.found = true;
        }
    }
}
