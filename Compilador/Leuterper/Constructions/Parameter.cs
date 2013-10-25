using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    class Parameter : ICompilable
    {
        public IScopable scope;
        public LType type { get; set; }
        public string name;
        public Parameter(LType type, String name)
        {
            this.type = type;
            this.name = name;
        }

        public void secondPass()
        {
        }

        public void generateCode(LeuterperCompiler compiler)
        {

        }

        public static LType parameterToType(Parameter p)
        {
            return p.type;
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
                result += p.type.SignatureAsString() + " ";
            }
            return result;
        }
    }
}
