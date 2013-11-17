using Leuterper.Constructions;
using System;
using System.Collections.Generic;

namespace Leuterper
{
    class Utils
    {
        public static LType parameterToType(Parameter p)
        {
            return p.getType();
        }

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
    }
}
