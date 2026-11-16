// AsyncPoco is a fork of PetaPoco and is bound by the same licensing terms.
// PetaPoco - A Tiny ORMish thing for your POCO's.
// Copyright © 2011-2012 Topten Software.  All Rights Reserved.
 
using System;

namespace AsyncPoco
{
	/// <summary>
	/// Column max length as a string; the T4 generator emits it from the DB schema. Not enforced by the ORM (ExceedMaxLengthException is only defined, never thrown here).
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class MaxLengthAttribute : Attribute
	{
		public MaxLengthAttribute() 
		{
		}
		
		public MaxLengthAttribute(string MaxLength) 
		{ 
			this.MaxLength = MaxLength;
		}

		public string MaxLength 
		{ 
			get; 
			set; 
		}
	}
}
