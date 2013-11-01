using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leuterper.Constructions;

namespace Leuterper
{
    class UtilFunctions
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

        public static List<Assignment> GetAssignments(List<IAction> actions)
        {
            List<Assignment> assignments = new List<Assignment>();
            foreach (IAction a in actions)
            {
                if (a is Assignment) assignments.Add(a as Assignment);
            }
            return assignments;
        }

        public static List<Call_Function> GetFunctionCalls(List<IAction> actions)
        {
            List<Call_Function> functionCalls = new List<Call_Function>();
            foreach (IAction a in actions)
            {
                if (a is Call_Function) functionCalls.Add(a as Call_Function);
            }
            return functionCalls;
        }
    }
}
