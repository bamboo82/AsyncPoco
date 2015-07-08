using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncPoco.Attributes.Special
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = true)]
    public class TableShowFieldAttribute : Attribute
    {
        public TableShowFieldAttribute(string fieldName)
        {
            FieldName = fieldName;
        }

        public string FieldName
        {
            get;
            set;
        }
    }
}
