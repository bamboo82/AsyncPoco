using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncPoco.Attributes.Special
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ReferenceToPassiveAttribute : Attribute
    {
        public ReferenceToPassiveAttribute(string lookupGroup, string recieveAsFieldName, string redirectToFieldName, string tag)
            : this(lookupGroup, redirectToFieldName, recieveAsFieldName, tag, false, false, true, false, true)
        {
        }

        public ReferenceToPassiveAttribute(string lookupGroup, string recieveAsFieldName, string redirectToFieldName, string tag, bool isRequired, bool isReadonly, bool isVisible, bool shouldSave, bool isLookupable)
        {
            this.RecieveAsFieldName = recieveAsFieldName;
            this.RedirectToFieldName = redirectToFieldName;
            this.Tag = tag;

            this.LookupGroup = lookupGroup;
            this.IsRequired = isRequired;
            this.IsReadonly = isReadonly;
            this.IsVisible = isVisible;
            this.ShouldSave = shouldSave;
            this.IsLookupable = isLookupable;
        }

        public bool ShouldSave { get; set; }
        public bool IsRequired { get; set; }
        public bool IsReadonly { get; set; }
        public bool IsVisible { get; set; }
        public bool IsLookupable { get; set; }
        public string LookupGroup { get; set; }

        public string RedirectToFieldName { get; set; }
        public string RecieveAsFieldName
        {
            get;
            set;
        }

        public object Tag
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

            private static Enum_Behavior[] _mappingIntToType = new[]
            {
                NONE,
            };

            public static Enum_Behavior MapFrom(int identifier)
            {
                return _mappingIntToType[identifier];
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
