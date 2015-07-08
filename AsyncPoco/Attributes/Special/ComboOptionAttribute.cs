// AsyncPoco is a fork of PetaPoco and is bound by the same licensing terms.
// PetaPoco - A Tiny ORMish thing for your POCO's.
// Copyright © 2011-2012 Topten Software.  All Rights Reserved.
 
using System;
using System.Collections.Generic;

namespace AsyncPoco
{
	/// <summary>
	/// For explicit poco properties, marks the property maxlength
	/// null if not determined or appliable
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
	public class ComboOptionAttribute : Attribute
	{
		public ComboOptionAttribute(object key, string value) : this(key, value, false)
		{
        }

        public ComboOptionAttribute(object key, string value, bool isDefault)
        {
            this.Key = key;
            this.Value = value;
            this.IsDefault = isDefault;
        }

        public string Value 
		{ 
			get;
			set;
		}

        public object Key
        {
            get;
            set;
        }

        public bool IsDefault
        {
            get;
            set;
        }
    }
}
