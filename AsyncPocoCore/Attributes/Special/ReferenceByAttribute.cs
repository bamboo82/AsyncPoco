using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncPoco.Attributes.Special
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ReferenceByPassiveAttribute : Attribute
    {
        public ReferenceByPassiveAttribute(string lookupGroup, string fromFieldName, string asFieldName, string tag)
        {
            this.AsFieldName = asFieldName;
            this.FromFieldName = fromFieldName;
            this.Tag = tag;

            this.LookupGroup = lookupGroup;
        }

        public string LookupGroup { get; set; }

        public string FromFieldName { get; set; }
        public string AsFieldName
        {
            get;
            set;
        }

        public object Tag
        {
            get;
            set;
        }

    }
}
