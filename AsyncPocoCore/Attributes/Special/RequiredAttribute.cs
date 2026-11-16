// AsyncPoco is a fork of PetaPoco and is bound by the same licensing terms.
// PetaPoco - A Tiny ORMish thing for your POCO's.
// Copyright © 2011-2012 Topten Software.  All Rights Reserved.
 
using System;
using System.Collections.Generic;

namespace AsyncPoco
{
	/// <summary>
	/// Fork-only validation metadata, never read by the ORM: IsRequired flag (default true). Consumers (Bamboo.Commenter, CMS generator) reflect on it themselves.
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
