// AsyncPoco is a fork of PetaPoco and is bound by the same licensing terms.
// PetaPoco - A Tiny ORMish thing for your POCO's.
// Copyright © 2011-2012 Topten Software.  All Rights Reserved.
 
using System;
using System.Collections.Generic;

namespace AsyncPoco
{
	/// <summary>
	/// Fork-only UI metadata, never read by the ORM: names a custom editor/type (TypeName) for the property.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class CustomizedTypeAttribute : Attribute
	{
		public CustomizedTypeAttribute() 
		{
        }

        public CustomizedTypeAttribute(string type)
        {
            this.TypeName = type;
        }


        public string TypeName
        {
            get;
            set;
        }
    }
}
