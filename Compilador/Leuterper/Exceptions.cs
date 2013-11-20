using System;

namespace Leuterper.Exceptions
{
    class SemanticErrorException : Exception
    {
        private int line;
        public SemanticErrorException() { }

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
            : base(message, inner) { }
    }
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