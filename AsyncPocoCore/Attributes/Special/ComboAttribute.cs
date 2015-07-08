// AsyncPoco is a fork of PetaPoco and is bound by the same licensing terms.
// PetaPoco - A Tiny ORMish thing for your POCO's.
// Copyright © 2011-2012 Topten Software.  All Rights Reserved.
 
using System;

namespace AsyncPoco
{
	/// <summary>
	/// For explicit poco properties, marks the property maxlength
	/// null if not determined or appliable
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class ComboAttribute : Attribute
	{
		public ComboAttribute() 
		{
		}
	}
}
