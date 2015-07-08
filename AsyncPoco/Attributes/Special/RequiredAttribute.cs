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
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class RequiredAttribute : Attribute
	{
		public RequiredAttribute() 
		{
            this.IsRequired = true;
        }

        public RequiredAttribute(bool isRequired)
        {
            this.IsRequired = isRequired;
        }


        public bool IsRequired
        {
            get;
            set;
        }
    }
}
