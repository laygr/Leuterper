using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leuterper.Exceptions;

namespace Leuterper
{
    class UniquesList<A> where A : IIdentifiable<A>
    {
        List<A> elements;

        public UniquesList()
        {
            this.elements = new List<A>();
        }

        public List<A> ToList()
        {
            return this.elements;
        }

        public UniquesList(List<A> aList) : this()
        {
            aList.ForEach(e => this.Add(e));
        }

        public A Get(int index)
        {
            return this.elements[index];
        }

        public int Count()
        {
            return this.elements.Count();
        }

        public void Add(A newElement)
        {
            if (null != elements.Find(e => e.HasSameSignatureAs(newElement)))
            {
                Console.WriteLine(String.Format("Element {0} already declared.", newElement.SignatureAsString()));
                Console.ReadKey();
                Environment.Exit(0);
                //throw new AlreadyDeclaredException(String.Format("Element {0} already declared.", newElement.SignatureAsString()));
            }
            this.elements.Add(newElement);
        }
    }
}