using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    abstract class LAction : Statement
    {
        public Definition_Function function { get; set; }
        public LAction(int line) : base(line) { }
        public static List<Declaration> GetDeclarations(List<LAction> actions)
        {
            List<Declaration> declarations = new List<Declaration>();
            foreach(LAction a in actions)
            {
                if(a is Declaration) declarations.Add(a as Declaration);
            }
            return declarations;
        }

        public static List<Assignment> GetAssignments(List<LAction> actions)
        {
            List<Assignment> assignments = new List<Assignment>();
            foreach(LAction a in actions)
            {
                if (a is Assignment) assignments.Add(a as Assignment);
            }
            return assignments;
        }

        public static List<Call_Function> GetFunctionCalls(List<LAction> actions)
        {
            List<Call_Function> functionCalls = new List<Call_Function>();
            foreach (LAction a in actions)
            {
                if (a is Call_Function) functionCalls.Add(a as Call_Function);
            }
            return functionCalls;
        }
    }
}
