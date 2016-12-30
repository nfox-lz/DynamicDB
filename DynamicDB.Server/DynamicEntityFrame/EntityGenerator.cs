using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Compete.DynamicDB.DynamicEntityFrame
{
    /// <summary>
    /// 实体生成器。
    /// </summary>
    public static class EntityGenerator
    {
        public static DynamicEntity ToObject(IDictionary<string, object> obj, string name)
        {
            object[] obejctArray;
            TypeBuilder builder = CreateBuilder(name);
            Queue<object> valueQueue = new Queue<object>();
            ICollection<object> collection = new List<object>();
            foreach (var item in obj)
            {
                if (item.Value is IDictionary<string, object>)
                {
                    BuildProperty(builder, item.Key, typeof(DynamicEntity));
                    valueQueue.Enqueue(ToObject(item.Value as IDictionary<string, object>, name + "_" + item.Key));
                }
                else if (item.Value is object[])
                {
                    BuildProperty(builder, item.Key, typeof(ICollection<object>));
                    obejctArray = item.Value as object[];
                    collection.Clear();
                    foreach (object arrayItem in obejctArray)
                    {
                        if (arrayItem is IDictionary<string, object>)
                            collection.Add(ToObject(arrayItem as IDictionary<string, object>, name + "_" + item.Key + "_Array"));
                        else
                            collection.Add(arrayItem);
                    }
                    valueQueue.Enqueue(collection);
                }
                else
                {
                    BuildProperty(builder, item.Key, item.Value.GetType());
                    valueQueue.Enqueue(item.Value);
                }
            }

            var type = builder.CreateType();
            var result = Activator.CreateInstance(type) as DynamicEntity;
            foreach (var item in obj)
                result[item.Key] = valueQueue.Dequeue();

            return result;
        }

        private static TypeBuilder CreateBuilder(string name = null, Type type = null)
        {
            var assemblyName = new AssemblyName("Mis.DynamicEntities");
            //创建一个永久程序集，设置为AssemblyBuilderAccess.RunAndSave。
            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            type = type ?? typeof(DynamicEntity);
            //创建一个永久单模程序块。
            return assemblyBuilder.DefineDynamicModule(assemblyName.Name).DefineType(name ?? type.Name, TypeAttributes.Public | TypeAttributes.Class, type);
        }

        private static void BuildProperty(TypeBuilder builder, string propertyName, Type propertyType)
        {
            if (propertyType == typeof(byte[]) && propertyName.EndsWith("Id", StringComparison.OrdinalIgnoreCase))
                propertyType = typeof(Guid);

            // 属性Set和Get方法要一个专门的属性。这里设置为Public。
            var getSetAttr = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

            // 添加属性到TypeBuilder。
            //定义字段。
            var customerNameBldr = builder.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);

            //定义属性。
            //最后一个参数为null，因为属性没有参数。
            var custNamePropBldr = builder.DefineProperty(propertyName, System.Reflection.PropertyAttributes.HasDefault, propertyType, null);

            //定义Get方法。
            var custNameGetPropMthdBldr = builder.DefineMethod("Get" + propertyName, getSetAttr, propertyType, Type.EmptyTypes);

            var custNameGetIL = custNameGetPropMthdBldr.GetILGenerator();

            custNameGetIL.Emit(OpCodes.Ldarg_0);
            custNameGetIL.Emit(OpCodes.Ldfld, customerNameBldr);
            custNameGetIL.Emit(OpCodes.Ret);

            //定义Set方法。
            var custNameSetPropMthdBldr = builder.DefineMethod("Set" + propertyName, getSetAttr, null, new Type[] { propertyType });

            var custNameSetIL = custNameSetPropMthdBldr.GetILGenerator();

            custNameSetIL.Emit(OpCodes.Ldarg_0);
            custNameSetIL.Emit(OpCodes.Ldarg_1);
            custNameSetIL.Emit(OpCodes.Stfld, customerNameBldr);
            custNameSetIL.Emit(OpCodes.Ret);

            //把创建的两个方法(Get,Set)加入到PropertyBuilder中。
            custNamePropBldr.SetGetMethod(custNameGetPropMthdBldr);
            custNamePropBldr.SetSetMethod(custNameSetPropMthdBldr);
        }
    }
}
