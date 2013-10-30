using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    class Parameter : Class_Declaration
    {
        public Parameter(int line, LType type, String name) : base(line, type, name)
        {
        }

        public Parameter reinstantiateWithSubstitution(List<LType> instantiatedTypes)
        {
            return new Parameter(this.getLine(), this.getType().reinstantiateWithSubstitution(instantiatedTypes), this.getName());
        }
        public static LType parameterToType(Parameter p)
        {
            return p.getType();
        }

        public static List<LType> listOfParametersAsListOfTypes(List<Parameter>parameters)
        {
            return parameters.ConvertAll(new Converter<Parameter, LType>(parameterToType));
        }

        public static String listOfParametersAsString(List<Parameter> parameters)
        {
            string result = "";
            foreach(Parameter p in parameters)
            {
                result += p.getType().SignatureAsString() + " ";
            }
            return result;
        }
    }
}
