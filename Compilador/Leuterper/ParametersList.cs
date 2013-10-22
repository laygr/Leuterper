using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leuterper.Constructions;

namespace Leuterper
{
    class ParametersList : IIdentifiable<ParametersList>
    {
        private List<Parameter> Parameters { get; set; }

        public ParametersList()
        {
            this.Parameters = new List<Parameter>();
        }

        public void Add(Parameter parameter)
        {
            this.Parameters.Add(parameter);
        }

        public ParametersList(List<Parameter> parameters)
        {
            this.Parameters = parameters;
        }

        public Parameter Get(int index)
        {
            return this.Parameters[index];
        }

        public int Count()
        {
            return this.Parameters.Count();
        }

        public bool HasSameSignatureAs(ParametersList otherElement)
        {
            if (this.Parameters.Count() != otherElement.Parameters.Count()) return false;
            for (int i = 0; i < this.Parameters.Count(); i++ )
            {
                if (!this.Parameters[i].type.HasSameSignatureAs(otherElement.Parameters[i].type)) return false;
            }
            return true;
        }

        public string SignatureAsString()
        {
            String acum = "";
            foreach(Parameter p in this.Parameters)
            {
                acum += p.type.SignatureAsString() + " ";
            }
            return acum;
        }

        public static ParametersList getParametersFromArguments(List<Expression> expressionList)
        {
            ParametersList result = new ParametersList();
            expressionList.ForEach(e => result.Add(new Parameter(0, e.getType(), "")));
            return result;
        }
    }
}
