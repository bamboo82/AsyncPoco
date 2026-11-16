// AsyncPoco is a fork of PetaPoco and is bound by the same licensing terms.
// PetaPoco - A Tiny ORMish thing for your POCO's.
// Copyright © 2011-2012 Topten Software.  All Rights Reserved.
 
using System;
using System.Collections.Generic;

namespace AsyncPoco
{
	/// <summary>
	/// Fork-only UI metadata, never read by the ORM: flags the property as a date field (IsDateType, default true).
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class DateTypeAttribute : Attribute
	{
		public DateTypeAttribute() 
		{
            this.IsDateType = true;
        }

        public DateTypeAttribute(bool isDateType)
        {
            this.IsDateType = isDateType;
        }


        public bool IsDateType
        {
            get;
            set;
        }
    }
}
