using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncPoco.Attributes.Special
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class QuickSearchableGroupAttribute : Attribute
    {
        public QuickSearchableGroupAttribute(string groupName, int groupOrderIndex, int fieldOrderIndex, string description) : this(groupName, groupOrderIndex, fieldOrderIndex, description, Enum_FieldRole.NONE)
        {
        }

        public QuickSearchableGroupAttribute(string groupName, int groupOrderIndex, int fieldOrderIndex, string description, Enum_FieldRole fieldRole)
        {
            this.GroupName = groupName;
            this.GroupOrderIndex = groupOrderIndex;
            this.FieldOrderIndex = fieldOrderIndex;
            this.FieldRole = fieldRole;
            this.Description = description;
        }

        public string GroupName { get; set; }
        public int GroupOrderIndex { get; set; }
        public int FieldOrderIndex { get; set; }
        public string Description { get; set; }

        public Enum_FieldRole FieldRole
        {
            get;
            set;
        }

        public sealed class Enum_FieldRole
        {
            public static readonly Enum_FieldRole NONE = null;
            //public static readonly Enum_FieldRole AS_KEY = new Enum_FieldType(1, "AS_KEY");
            //public static readonly Enum_FieldRole AS_UNIQUE_KEY = new Enum_FieldType(2, "AS_UNIQUE_KEY");

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
