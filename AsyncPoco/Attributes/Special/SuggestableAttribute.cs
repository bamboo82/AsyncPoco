using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncPoco.Attributes.Special
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class SuggestableAttribute : Attribute
    {
        public SuggestableAttribute(string groupName, Enum_FieldRole fieldRole)
        {
            this.GroupName = groupName;
            this.FieldRole = fieldRole;
        }

        public string GroupName
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
            public static readonly Enum_FieldRole AS_KEY = new Enum_FieldRole(1, "AS_KEY");
            public static readonly Enum_FieldRole AS_UNIQUE_KEY = new Enum_FieldRole(2, "AS_UNIQUE_KEY");

            private string name;
            private int id;

            private static Enum_FieldRole[] mappingIntToRole = new[]
            {
                NONE,
                AS_KEY,
                AS_UNIQUE_KEY
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
