using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncPoco.Attributes.Special
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false)]
    public class OrderableAttribute : Attribute
    {
        public OrderableAttribute() : this(Enum_FieldRole.DEFAULT_NONE)
        {
            
        }

        public OrderableAttribute(Enum_FieldRole defaultOrder)
        {
            this.FieldRole = defaultOrder;
        }

        public Enum_FieldRole FieldRole
        {
            get;
            set;
        }

        public sealed class Enum_FieldRole
        {
            public static readonly Enum_FieldRole DEFAULT_NONE = null;
            public static readonly Enum_FieldRole DEFAULT_ASC = new Enum_FieldRole(1, "DEFAULT_ASC");
            public static readonly Enum_FieldRole DEFAULT_DESC = new Enum_FieldRole(2, "DEFAULT_DESC");

            private string name;
            private int id;

            private static Enum_FieldRole[] mappingIntToRole = new[]
            {
                DEFAULT_NONE,
                DEFAULT_ASC,
                DEFAULT_DESC
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
