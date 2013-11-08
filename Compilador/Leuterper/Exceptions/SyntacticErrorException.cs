using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Exceptions
{
    class SyntacticErrorException : Exception
    {
        private int line;
        public SyntacticErrorException() { }

        public SyntacticErrorException(string message, int line)
            : base(message)
        {
            this.line = line;
        }
        public override string ToString()
        {
            return String.Format("Syntactic error.\n\t{0}\n\tLine {1}.", this.Message, this.line);
        }
        public SyntacticErrorException(string message, Exception inner)
            : base(message, inner) { }
    }
}
