using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncPoco.Attributes.Special
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class MultiReferenceToAttribute : Attribute
    {
        public MultiReferenceToAttribute(string tableName, string fieldName)
        {
            TableName = tableName;
            FieldName = fieldName;
        }

        public string TableName
        {
            get;
            set;
        }

        public object FieldName
        {
            get;
            set;
        }

        public Enum_FieldRole FieldRole
        {
            get;
            set;
        }

        public sealed class Enum_FieldRole
        {
            public static readonly Enum_FieldRole NONE = null;
            public static readonly Enum_FieldRole COMMA_SEPERATED = new Enum_FieldRole(1, "COMMA_SEPERATED");
            
            private string name;
            private int id;

            private static Enum_FieldRole[] mappingIntToRole = new[]
            {
                NONE,
            };

            public static Enum_FieldRole MapFrom(int identifier)
            {
                return mappingIntToRole[identifier];
            }

            private Enum_FieldRole(int id, string name)
            {
                this.id = id;
                this.name = name;
            }

            public string Name
            {
                get { return this.name; }
            }

            public int ID
            {
                get { return this.id; }
            }

            public override String ToString()
            {
                return "Name:" + Name + "," + "ID:" + ID;
            }

            public static implicit operator string(Enum_FieldRole self)
            {
                return self.ToString();
            }
        }
    }
}
