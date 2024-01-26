using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BComponents
{
    /// <summary>
    /// Excepcion en la capa de negocios
    /// </summary>
    /// <remarks></remarks>
    public class BCException : Exception
    {
        private List<string> colErrorMessages = new();
        public List<string> ErrorCollection
        {
            get { return colErrorMessages; }
        }

        public BCException() : base() { }

        public BCException(string strMessage) : base(strMessage) { }

        public BCException(string strMessage, Exception NewException) : base(strMessage, NewException) { }

        public BCException(List<string> ErrorCollection) : base("Existen datos invalidos")
        {
            this.colErrorMessages = ErrorCollection;
        }

        protected BCException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    }
}
