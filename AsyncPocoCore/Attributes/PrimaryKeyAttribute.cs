﻿// AsyncPoco is a fork of PetaPoco and is bound by the same licensing terms.
// PetaPoco - A Tiny ORMish thing for your POCO's.
// Copyright © 2011-2012 Topten Software.  All Rights Reserved.
 
using System;

namespace AsyncPoco
{
	/// <summary>
	/// Specifies the primary key column of a poco class, whether the column is auto incrementing
	/// and the sequence name for Oracle sequence columns.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class PrimaryKeyAttribute : Attribute
	{
		public PrimaryKeyAttribute(string primaryKeyOrPrimaryKeys)
		{
			Value = primaryKeyOrPrimaryKeys;
            IsCombo = primaryKeyOrPrimaryKeys.Contains(",");
		    autoIncrement = !IsCombo;
        }

		public string Value 
		{ 
			get; 
			private set; 
		}

        public bool IsCombo { get; set; }
		
		public string sequenceName 
		{ 
			get; 
			set; 
		}
		
		public bool autoIncrement 
		{ 
			get; 
			set; 
		}
	}

}
