﻿// AsyncPoco is a fork of PetaPoco and is bound by the same licensing terms.
// PetaPoco - A Tiny ORMish thing for your POCO's.
// Copyright © 2011-2012 Topten Software.  All Rights Reserved.
 
using System;

namespace AsyncPoco
{

	/// <summary>
	/// Marks a poco property as a computed column that is populated in queries
	/// but not used for updates or inserts.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class ComputedColumnAttribute : ColumnAttribute
	{
        public ComputedColumnAttribute()
        {
            ForceToUtc = false;
        }

        public ComputedColumnAttribute(string Name)
        {
            this.Name = Name;
            ForceToUtc = false;
        }

        public string Name
        {
            get;
            set;
        }

        public bool ForceToUtc
        {
            get;
            set;
        }
    }

}
