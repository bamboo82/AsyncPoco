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
    public class FileTypePassiveAttribute : Attribute
    {
        public FileTypePassiveAttribute(string lookupGroup, string recieveAsFieldName, string redirectToFieldName,
            string tag)
            : this(lookupGroup, redirectToFieldName, recieveAsFieldName, tag, false, false, true, false)
        {
        }

        public FileTypePassiveAttribute(string lookupGroup, string recieveAsFieldName, string redirectToFieldName,
            string tag, bool isRequired, bool isReadonly, bool isVisible, bool shouldSave)
        {
            this.RecieveAsFieldName = recieveAsFieldName;
            this.RedirectToFieldName = redirectToFieldName;
            this.Tag = tag;

            this.LookupGroup = lookupGroup;
            this.IsRequired = isRequired;
            this.IsReadonly = isReadonly;
            this.IsVisible = isVisible;
            this.ShouldSave = shouldSave;
        }

        public bool ShouldSave { get; set; }
        public bool IsRequired { get; set; }
        public bool IsReadonly { get; set; }
        public bool IsVisible { get; set; }
        public string LookupGroup { get; set; }

        public string RedirectToFieldName { get; set; }
        public string RecieveAsFieldName { get; set; }

        public object Tag { get; set; }

    }
}
