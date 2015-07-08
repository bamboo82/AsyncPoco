using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncPoco.Exceptions
{
    public class ExceedMaxLengthException : Exception
    {
        public ExceedMaxLengthException(string offendingFieldName, int maxLength)
        {
            OffendingFieldName = offendingFieldName;
        }


        public string OffendingFieldName { get; set; }

        public int MaxLength { get; set; }
    }
}
