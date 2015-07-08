using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncPoco.Attributes.Special
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ReferenceToAttribute : Attribute
    {
        public ReferenceToAttribute(string foreign_tablename, string foreign_fieldname)
            : this(foreign_tablename, foreign_fieldname, "", 1)
        {

        }

        public ReferenceToAttribute(string foreign_tablename, string foreign_fieldname, string lookgroup, int behavior)
        {
            TableName = foreign_tablename;
            FieldName = foreign_fieldname;

            LookupGroup = lookgroup;
            Behavior = Enum_Behavior.MapFromKey(behavior);
        }


        public string LookupGroup { get; set; }
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

        public Enum_Behavior Behavior
        {
            get;
            set;
        }


        public sealed class Enum_Behavior
        {
            public static readonly Enum_Behavior NONE = null;
            public static readonly Enum_Behavior SINGLE_LOOKUP = new Enum_Behavior(1, "SINGLE_LOOKUP");
            public static readonly Enum_Behavior MULTI_LOOKUP = new Enum_Behavior(2, "MULTI_LOOKUP");

            private string name;
            private int id;

            private static Enum_Behavior[] _keyToEnumMap = new[]
            {
                NONE,
                SINGLE_LOOKUP,
                MULTI_LOOKUP
            };


            public static Enum_Behavior MapFromKey(int identifier)
            {
                return _keyToEnumMap[identifier];
            }

            private Enum_Behavior(int id, string name)
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

            public static implicit operator string(Enum_Behavior self)
            {
                return self.ToString();
            }
        }
    }
}
