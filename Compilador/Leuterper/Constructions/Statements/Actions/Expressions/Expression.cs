using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leuterper.Constructions
{
    abstract class Expression : LAction
    {
        public Expression(int line) : base(line) { }
        public bool shouldBePushedToStack { get; set; }
        abstract public LType getType();

        public static LType expresionToType(Expression e)
        {
            return e.getType();
        }
        public static List<LType> expressionsToTypes(List<Expression> expression)
        {
            return expression.ConvertAll(new Converter<Expression, LType>(expresionToType));
        }
    }
}
