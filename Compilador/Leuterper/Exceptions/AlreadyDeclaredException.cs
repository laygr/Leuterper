using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Exceptions
{
    class SemanticErrorException : Exception
    {
        private string p1;
        private Constructions.LType lType;
        private Constructions.LType newType;
        private int p2;

        private int line;
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
