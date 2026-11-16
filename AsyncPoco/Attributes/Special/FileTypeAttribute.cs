// AsyncPoco is a fork of PetaPoco and is bound by the same licensing terms.
// PetaPoco - A Tiny ORMish thing for your POCO's.
// Copyright © 2011-2012 Topten Software.  All Rights Reserved.

using System;
using System.Collections.Generic;

namespace AsyncPoco
{
    /// <summary>
    /// Fork-only UI metadata, never read by the ORM: file-upload field received as RecieveAsFieldName and redirected to RedirectToFieldName within LookupGroup.
    /// NOTE: file is FileTypeAttribute.cs but the class is FileTypePassiveAttribute. The 4-arg ctor passes recieveAs/redirectTo swapped into the long ctor; existing consumers may depend on it.
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
