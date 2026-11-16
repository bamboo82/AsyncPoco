// AsyncPoco is a fork of PetaPoco and is bound by the same licensing terms.
// PetaPoco - A Tiny ORMish thing for your POCO's.
// Copyright © 2011-2012 Topten Software.  All Rights Reserved.
 
using System;

namespace AsyncPoco
{
	/// <summary>
	/// Fork-only UI metadata, never read by the ORM: render the property as a drop-down whose options are the [ComboOption]s on it.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class ComboAttribute : Attribute
	{
		public ComboAttribute() 
		{
		}
	}
}
