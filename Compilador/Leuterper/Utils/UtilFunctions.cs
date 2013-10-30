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
        public static List<Class_Declaration> GetDeclarations(List<IAction> actions)
        {
            List<Class_Declaration> declarations = new List<Class_Declaration>();
            foreach (IAction a in actions)
            {
                if (a is Class_Declaration) declarations.Add(a as Class_Declaration);
            }
            return declarations;
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
