using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncPoco.Attributes.Special
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = true)]
    public class TableHideFieldAttribute : Attribute
    {
        public TableHideFieldAttribute(string fieldName)
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
