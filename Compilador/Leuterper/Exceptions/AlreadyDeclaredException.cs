using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Exceptions
{
    class SemanticErrorException : Exception
    {
        public int line { get; set; }
        public SemanticErrorException()
    {
    }

        public SemanticErrorException(string message, int line)
            : base(message)
        {
            this.line = line;
        }

        public override string ToString()
        {
            return String.Format("Semantic error.\n\t{0}\n\tLine {1}.", this.Message, this.line);
        }

    public SemanticErrorException(string message, Exception inner)
        : base(message, inner)
    {
    }

    }
}
