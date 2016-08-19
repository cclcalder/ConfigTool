using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Exceedra.Common.Utilities
{
    public class Item
    {
        private List<Property> _properties;

        public List<Property> Properties
        {
            get
            {
                if (_properties == null) _properties = new List<Property>();
                return _properties;
            }
            set
            {
                _properties = value;
            }
        }
    }

    public class Property
    {
        public Type Type { get; set; }
        public string Name { get; set; }
        public object Value { get; set; }
    }

    public static class DynamicObjectCreator
    {
        public static dynamic New(string typeName, IList<Property> properties)
        {
            var dynamicType = CreateType(typeName, properties);
            var dynamicTypeInstance = CreateInstanceOf(dynamicType);

            AssignProperties(dynamicType, dynamicTypeInstance, properties);

            return dynamicTypeInstance;
        }

        /// <summary>
        /// Dynamically creates a new type of "Item" as a base class
        /// </summary>
        /// <param name="typeName">name of the dynamically created type</param>
        /// <param name="properties">set of properties to assign to a type being created</param>
        /// <param name="moduleName">name of the module</param>
        /// <returns>Dynamically created type</returns>
        private static dynamic CreateType(string typeName = "DefaultTypeName", IList<Property> properties = null, string moduleName = "DefaultModuleName")
        {
            AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
                new AssemblyName(typeName),
                AssemblyBuilderAccess.Run);

            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(moduleName);

            TypeBuilder typeBuilder = DefineType(typeName, moduleBuilder);

            // create properties for the type above (with default values)
            if (properties != null)
                foreach (var property in properties)
                    DefineProperty(typeBuilder, property.Type, property.Name);

            return typeBuilder.CreateType();
        }

        /// <summary>
        /// Defining a new type as a public class derived from the "Item" class
        /// </summary>
        /// <param name="typeName">name of the dynamically created type</param>
        /// <param name="moduleBuilder">object created by the AssemblyBuilder (DefineDynamicModule(moduleName) method)</param>
        /// <returns>Type builder with set type details</returns>
        private static TypeBuilder DefineType(string typeName, ModuleBuilder moduleBuilder)
        {
            return moduleBuilder.DefineType(
                typeName,
                TypeAttributes.Public |
                TypeAttributes.Class |
                TypeAttributes.AutoClass |
                TypeAttributes.AnsiClass |
                TypeAttributes.BeforeFieldInit |
                TypeAttributes.AutoLayout,
                null);
            //typeof(Item));
        }

        /// <summary>
        /// Dynamically defines a new property to the type inside of typeBuilder.
        /// Creates a property and a field associated with get & set methods for this property.
        /// </summary>
        /// <param name="typeBuilder">type builder with created type that the property being created should be assigned to</param>
        /// <param name="propertyType">type of the dynamically created property</param>
        /// <param name="propertyName">name of the dynamically created property</param>
        private static void DefineProperty(TypeBuilder typeBuilder, Type propertyType, string propertyName)
        {
            // example: someTypeBuilder, int, Total

            // creating property Total
            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);

            // creating field _total
            string fieldName = "_" + propertyName.Substring(0, 1).ToLower() + propertyName.Substring(1);
            FieldBuilder fieldBuilder = typeBuilder.DefineField(fieldName, propertyType, FieldAttributes.Private);

            // creating get method for Total property -> get_Total; the method will return the value of _total
            MethodBuilder getMethodBuilder = typeBuilder.DefineMethod(
                "get_" + propertyName,
                MethodAttributes.Public |
                MethodAttributes.SpecialName |
                MethodAttributes.HideBySig,
                propertyType,
                Type.EmptyTypes);

            ILGenerator getIl = getMethodBuilder.GetILGenerator();

            getIl.Emit(OpCodes.Ldarg_0);
            getIl.Emit(OpCodes.Ldfld, fieldBuilder);
            getIl.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getMethodBuilder);

            // creating set method for Total property -> set_Total; the method will assign the value of _total
            MethodBuilder setPropMthdBldr = typeBuilder.DefineMethod(
                "set_" + propertyName,
                MethodAttributes.Public |
                MethodAttributes.SpecialName |
                MethodAttributes.HideBySig,
                null,
                new[] { propertyType });

            ILGenerator setIl = setPropMthdBldr.GetILGenerator();

            Label modifyProperty = setIl.DefineLabel();
            Label exitSet = setIl.DefineLabel();

            setIl.MarkLabel(modifyProperty);
            setIl.Emit(OpCodes.Ldarg_0);
            setIl.Emit(OpCodes.Ldarg_1);
            setIl.Emit(OpCodes.Stfld, fieldBuilder);

            setIl.Emit(OpCodes.Nop);
            setIl.MarkLabel(exitSet);
            setIl.Emit(OpCodes.Ret);

            propertyBuilder.SetSetMethod(setPropMthdBldr);
        }

        /// <summary>
        /// Creates a new instance of a given type
        /// </summary>
        /// <param name="type">A type to create a new instance of; must derive from the "Item" class</param>
        /// <returns>A new instance of the type casted to the "Item"</returns>
        private static dynamic CreateInstanceOf(Type type)
        {
            return Activator.CreateInstance(type);
        }

        /// <summary>
        /// Assigns properties for a given type instance based on the propertiesValues argument
        /// </summary>
        /// <param name="type">A type that contains properties to be assigned</param>
        /// <param name="typeInstance">An instance of which properties have to be assigned</param>
        /// <param name="properties">Set of properties</param>
        private static void AssignProperties(Type type, dynamic typeInstance, IList<Property> properties)
        {
            foreach (PropertyInfo propertyInfo in type.GetProperties())
            {
                var property = properties.FirstOrDefault(prop => prop.Name == propertyInfo.Name);
                if (property != null) propertyInfo.SetValue((object)typeInstance, property.Value);
            }
        }
    }
}
