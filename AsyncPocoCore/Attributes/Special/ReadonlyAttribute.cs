// AsyncPoco is a fork of PetaPoco and is bound by the same licensing terms.
// PetaPoco - A Tiny ORMish thing for your POCO's.
// Copyright © 2011-2012 Topten Software.  All Rights Reserved.
 
using System;
using System.Collections.Generic;

namespace AsyncPoco
{
	/// <summary>
	/// Fork-only UI metadata, never read by the ORM: IsReadonly flag for the admin UI (default true).
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class ReadonlyAttribute : Attribute
	{
		public ReadonlyAttribute() 
		{
            this.IsReadonly = true;
        }

        public ReadonlyAttribute(bool isReadonly)
        {
            this.IsReadonly = isReadonly;
        }


        public bool IsReadonly
        {
            get;
            set;
        }
    }
}
