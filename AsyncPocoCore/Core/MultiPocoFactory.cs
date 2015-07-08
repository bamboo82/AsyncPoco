// AsyncPoco is a fork of PetaPoco and is bound by the same licensing terms.
// PetaPoco - A Tiny ORMish thing for your POCO's.
// Copyright © 2011-2012 Topten Software.  All Rights Reserved.

using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq.Expressions;
using System.Data;

namespace AsyncPoco.Internal
{
    //internal class MultiPocoFactory
    //{
    //    public MultiPocoFactory(IEnumerable<Delegate> dels)
    //    {
    //        Delegates = new List<Delegate>(dels);
    //    }
    //    private List<Delegate> Delegates { get; set; }
    //    private Delegate GetItem(int index) { return Delegates[index]; }

    //    /// <summary>
    //    /// Calls the delegate at the specified index and returns its values
    //    /// </summary>
    //    /// <param name="index"></param>
    //    /// <param name="reader"></param>
    //    /// <returns></returns>
    //    private object CallDelegate(int index, IDataReader reader)
    //    {
    //        var d = GetItem(index);
    //        var output = d.DynamicInvoke(reader, null);
    //        return output;
    //    }

    //    /// <summary>
    //    /// Calls the callback delegate and passes in the output of all delegates as the parameters
    //    /// </summary>
    //    /// <typeparam name="TRet"></typeparam>
    //    /// <param name="callback"></param>
    //    /// <param name="dr"></param>
    //    /// <param name="count"></param>
    //    /// <returns></returns>
    //    public TRet CallCallback<TRet>(Delegate callback, IDataReader dr, int count)
    //    {
    //        var args = new List<object>();
    //        for (var i = 0; i < count; i++)
    //        {
    //            args.Add(CallDelegate(i, dr));
    //        }
    //        return (TRet)callback.DynamicInvoke(args.ToArray());
    //    }

    //    // Automagically guess the property relationships between various POCOs and create a delegate that will set them up
    //    public static Delegate GetAutoMapper(Type[] types)
    //    {
    //        // Build a key
    //        var key = string.Join(":", types.Select(x => x.ToString()).ToArray());

    //        return AutoMappers.Get(key, () =>
    //        {
    //            // Create a method
    //            var m = new DynamicMethod("poco_automapper", types[0], types, true);
    //            var il = m.GetILGenerator();

    //            for (int i = 1; i < types.Length; i++)
    //            {
    //                bool handled = false;
    //                for (int j = i - 1; j >= 0; j--)
    //                {
    //                    // Find the property
    //                    var candidates = types[j].GetProperties().Where(p => p.PropertyType == types[i]).ToList();
    //                    if (candidates.Count == 0)
    //                        continue;
    //                    if (candidates.Count > 1)
    //                        throw new InvalidOperationException(string.Format("Can't auto join {0} as {1} has more than one property of type {0}", types[i], types[j]));

    //                    // Generate code
    //                    var lblIsNull = il.DefineLabel();

    //                    il.Emit(OpCodes.Ldarg_S, j);       // obj
    //                    il.Emit(OpCodes.Ldnull);           // obj, null
    //                    il.Emit(OpCodes.Beq, lblIsNull);   // If obj == null then don't set nested object

    //                    il.Emit(OpCodes.Ldarg_S, j);       // obj
    //                    il.Emit(OpCodes.Ldarg_S, i);       // obj, obj2
    //                    il.Emit(OpCodes.Callvirt, candidates[0].GetSetMethod(true)); // obj = obj2

    //                    il.MarkLabel(lblIsNull);

    //                    handled = true;
    //                }

    //                if (!handled)
    //                    throw new InvalidOperationException(string.Format("Can't auto join {0}", types[i]));
    //            }

    //            il.Emit(OpCodes.Ldarg_0);
    //            il.Emit(OpCodes.Ret);

    //            // Cache it
    //            var del = m.CreateDelegate(Expression.GetFuncType(types.Concat(types.Take(1)).ToArray()));
    //            return del;
    //        });
    //    }

    //    // Find the split point in a result set for two different pocos and return the poco factory for the first
    //    static Delegate FindSplitPoint(Database database, int typeIndex, Type typeThis, Type typeNext, string sql, string connectionString, IDataReader r, ref int pos)
    //    {
    //        // Last?
    //        if (typeNext == null)
    //            return database.PocoDataFactory.ForType(typeThis, true).MappingFactory.GetFactory(sql, connectionString, pos, r.FieldCount - pos, r, null);

    //        // Get PocoData for the two types
    //        PocoData pdThis = database.PocoDataFactory.ForType(typeThis, typeIndex > 0);
    //        PocoData pdNext = database.PocoDataFactory.ForType(typeNext, true);

    //        // Find split point
    //        int firstColumn = pos;
    //        var usedColumns = new Dictionary<string, bool>();
    //        for (; pos < r.FieldCount; pos++)
    //        {
    //            // Split if field name has already been used, or if the field doesn't exist in current poco but does in the next
    //            string fieldName = r.GetName(pos);
    //            if (usedColumns.ContainsKey(fieldName)
    //                || (!pdThis.Columns.ContainsKey(fieldName) && pdNext.Columns.ContainsKey(fieldName))
    //                || (!pdThis.Columns.ContainsKey(fieldName.Replace("_", "")) && pdNext.Columns.ContainsKey(fieldName.Replace("_", "")))
    //                || (!pdThis.Columns.Any(x => fieldName.Equals(x.Value.AutoAlias, StringComparison.OrdinalIgnoreCase)) && pdNext.Columns.Any(x => fieldName.Equals(x.Value.AutoAlias, StringComparison.OrdinalIgnoreCase)))
    //                || (!pdThis.Columns.Any(x => fieldName.Equals(x.Value.ColumnAlias, StringComparison.OrdinalIgnoreCase)) && pdNext.Columns.Any(x => fieldName.Equals(x.Value.ColumnAlias, StringComparison.OrdinalIgnoreCase))))
    //            {
    //                return pdThis.MappingFactory.GetFactory(sql, connectionString, firstColumn, pos - firstColumn, r, null);
    //            }
    //            usedColumns.Add(fieldName, true);
    //        }

    //        throw new InvalidOperationException(string.Format("Couldn't find split point between {0} and {1}", typeThis, typeNext));
    //    }

    //    // Create a multi-poco factory
    //    static Func<IDataReader, Delegate, TRet> CreateMultiPocoFactory<TRet>(Database database, Type[] types, string sql, string connectionString, IDataReader r)
    //    {
    //        // Call each delegate
    //        var dels = new List<Delegate>();
    //        int pos = 0;
    //        for (int i = 0; i < types.Length; i++)
    //        {
    //            // Add to list of delegates to call
    //            var del = FindSplitPoint(database, i, types[i], i + 1 < types.Length ? types[i + 1] : null, sql, connectionString, r, ref pos);
    //            dels.Add(del);
    //        }

    //        var mpFactory = new MultiPocoFactory(dels);
    //        return (reader, arg3) => mpFactory.CallCallback<TRet>(arg3, reader, types.Length);
    //    }

    //    // Various cached stuff
    //    static Cache<string, object> MultiPocoFactories = new Cache<string, object>();
    //    static Cache<string, Delegate> AutoMappers = new Cache<string, Delegate>();

    //    // Get (or create) the multi-poco factory for a query
    //    public static Func<IDataReader, Delegate, TRet> GetMultiPocoFactory<TRet>(Database database, Type[] types, string sql, string connectionString, IDataReader r)
    //    {
    //        // Build a key string  (this is crap, should address this at some point)
    //        var kb = new StringBuilder();
    //        kb.Append(typeof(TRet).ToString());
    //        kb.Append(":");
    //        kb.Append(r.FieldCount);
    //        kb.Append(":");
    //        foreach (var t in types)
    //        {
    //            kb.Append(":" + t);
    //        }
    //        kb.Append(":" + connectionString);
    //        kb.Append(":" + sql);
    //        string key = kb.ToString();

    //        return (Func<IDataReader, Delegate, TRet>)MultiPocoFactories.Get(key, () => CreateMultiPocoFactory<TRet>(database, types, sql, connectionString, r));
    //    }
    //}


    class MultiPocoFactory
    {
        public static IEqualityComparer<string> FieldNameComparer { get; set; } = StringComparer.OrdinalIgnoreCase;

        // Instance data used by the Multipoco factory delegate - essentially a list of the nested poco factories to call
        List<Delegate> _delegates;
        public Delegate GetItem(int index) { return _delegates[index]; }

        // Automagically guess the property relationships between various POCOs and create a delegate that will set them up
        public static object GetAutoMapper(Type[] types)
        {
            // Build a key
            var key = new ArrayKey<Type>(types);

            return AutoMappers.Get(key, () =>
            {
                // Create a method
                var m = new DynamicMethod("petapoco_automapper", types[0], types, true);
                var il = m.GetILGenerator();

                for (int i = 1; i < types.Length; i++)
                {
                    bool handled = false;
                    for (int j = i - 1; j >= 0; j--)
                    {
                        // Find the property
                        var candidates = types[j].GetProperties().Where(p => p.PropertyType == types[i]).ToList();
                        if (candidates.Count() == 0)
                            continue;
                        if (candidates.Count() > 1)
                            throw new InvalidOperationException(string.Format("Can't auto join {0} as {1} has more than one property of type {0}", types[i], types[j]));

                        // Generate code
                        var lblIsNull = il.DefineLabel();
                        il.Emit(OpCodes.Ldarg_S, j);       // obj 
                        il.Emit(OpCodes.Ldnull);           // obj, null
                        il.Emit(OpCodes.Beq, lblIsNull);   // If obj == null then don't set nested object

                        il.Emit(OpCodes.Ldarg_S, j);       // obj
                        il.Emit(OpCodes.Ldarg_S, i);       // obj, obj2
                        il.Emit(OpCodes.Callvirt, candidates[0].GetSetMethod(true)); // obj = obj2

                        il.MarkLabel(lblIsNull);
                        handled = true;
                    }

                    if (!handled)
                        throw new InvalidOperationException(string.Format("Can't auto join {0}", types[i]));
                }

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ret);

                // Cache it
                return m.CreateDelegate(Expression.GetFuncType(types.Concat(types.Take(1)).ToArray()));
            }
            );
        }

        // Find the split point in a result set for two different pocos and return the poco factory for the first
        static Delegate FindSplitPoint(Type typeThis, Type typeNext, string ConnectionString, string sql, IDataReader r, ref int pos)
        {
            // Last?
            if (typeNext == null)
                return PocoData.ForType(typeThis).GetFactory(sql, ConnectionString, pos, r.FieldCount - pos, r);

            // Get PocoData for the two types
            PocoData pdThis = PocoData.ForType(typeThis);
            PocoData pdNext = PocoData.ForType(typeNext);

            // Find split point
            int firstColumn = pos;
            var usedColumns = new Dictionary<string, bool>(FieldNameComparer);
            for (; pos < r.FieldCount; pos++)
            {
                // Split if field name has already been used, or if the field doesn't exist in current poco but does in the next
                string fieldName = r.GetName(pos);
                if (usedColumns.ContainsKey(fieldName) || (!pdThis.Columns.ContainsKey(fieldName) && pdNext.Columns.ContainsKey(fieldName)))
                {
                    return pdThis.GetFactory(sql, ConnectionString, firstColumn, pos - firstColumn, r);
                }
                usedColumns.Add(fieldName, true);
            }

            throw new InvalidOperationException(string.Format("Couldn't find split point between {0} and {1}", typeThis, typeNext));
        }

        // Create a multi-poco factory
        static Func<IDataReader, object, TRet> CreateMultiPocoFactory<TRet>(Type[] types, string ConnectionString, string sql, IDataReader r)
        {
            var m = new DynamicMethod("petapoco_multipoco_factory", typeof(TRet), new Type[] { typeof(MultiPocoFactory), typeof(IDataReader), typeof(object) }, typeof(MultiPocoFactory));
            var il = m.GetILGenerator();

            // Load the callback
            il.Emit(OpCodes.Ldarg_2);

            // Call each delegate
            var dels = new List<Delegate>();
            int pos = 0;
            for (int i = 0; i < types.Length; i++)
            {
                // Add to list of delegates to call
                var del = FindSplitPoint(types[i], i + 1 < types.Length ? types[i + 1] : null, ConnectionString, sql, r, ref pos);
                dels.Add(del);

                // Get the delegate
                il.Emit(OpCodes.Ldarg_0);                                                   // callback,this
                il.Emit(OpCodes.Ldc_I4, i);                                                 // callback,this,Index
                il.Emit(OpCodes.Callvirt, typeof(MultiPocoFactory).GetMethod("GetItem"));   // callback,Delegate
                il.Emit(OpCodes.Ldarg_1);                                                   // callback,delegate, datareader

                // Call Invoke
                var tDelInvoke = del.GetType().GetMethod("Invoke");
                il.Emit(OpCodes.Callvirt, tDelInvoke);                                      // Poco left on stack
            }

            // By now we should have the callback and the N pocos all on the stack.  Call the callback and we're done
            il.Emit(OpCodes.Callvirt, Expression.GetFuncType(types.Concat(new Type[] { typeof(TRet) }).ToArray()).GetMethod("Invoke"));
            il.Emit(OpCodes.Ret);

            // Finish up
            return (Func<IDataReader, object, TRet>)m.CreateDelegate(typeof(Func<IDataReader, object, TRet>), new MultiPocoFactory() { _delegates = dels });
        }

        // Various cached stuff
        static Cache<Tuple<Type, ArrayKey<Type>, string, string>, object> MultiPocoFactories = new Cache<Tuple<Type, ArrayKey<Type>, string, string>, object>();
        static Cache<ArrayKey<Type>, object> AutoMappers = new Cache<ArrayKey<Type>, object>();

        internal static void FlushCaches()
        {
            MultiPocoFactories.Flush();
            AutoMappers.Flush();
        }

        // Get (or create) the multi-poco factory for a query
        public static Func<IDataReader, object, TRet> GetFactory<TRet>(Type[] types, string ConnectionString, string sql, IDataReader r)
        {
            var key = Tuple.Create<Type, ArrayKey<Type>, string, string>(typeof(TRet), new ArrayKey<Type>(types), ConnectionString, sql);

            return (Func<IDataReader, object, TRet>)MultiPocoFactories.Get(key, () =>
            {
                return CreateMultiPocoFactory<TRet>(types, ConnectionString, sql, r);
            }
            );
        }



    }
}
