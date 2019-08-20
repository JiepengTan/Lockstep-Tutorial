using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
#if UNITY_EDITOR
using UnityEngine;
#endif
using System.Collections;

namespace Lockstep.Util {
    public static class ReflectionUtility {
        public class ReflectionSearchIgnoreAttribute : Attribute {
            public ReflectionSearchIgnoreAttribute(){ }
        }

        private static Type[] types;

        public static Type[] GetTypes(){
            if (types == null) {
                types = AppDomain.CurrentDomain.GetAssemblies()
                    .Where((Assembly assembly) => assembly.FullName.Contains("Assembly"))
                    .SelectMany((Assembly assembly) => assembly.GetTypes()).ToArray();
            }

            return types;
        }

        /// <summary>
        /// Gets all non-abstract types extending the given base type and with the given attribute
        /// </summary>
        public static Type[] GetAttriTypes(Type hasAttribute, bool inherit){
            return GetTypes().Where((Type T) =>
                    (!T.IsAbstract)
                    && T.GetCustomAttributes(hasAttribute, inherit).Any())
                .ToArray();
        }

        /// <summary>
        /// Gets all non-abstract types extending the given base type
        /// </summary>
        public static Type[] GetSubTypes(Type baseType){
            return GetTypes().Where((Type T) =>
                (T.IsClass && !T.IsAbstract)
                && T.IsSubclassOf(baseType)
                && !T.GetCustomAttributes(typeof(ReflectionSearchIgnoreAttribute), false).Any()).ToArray();
        }

        public static Type[] GetInterfaces(Type iType){
            return GetTypes().Where((Type T) => iType.IsAssignableFrom(T)).ToArray();
        }

        /// <summary>
        /// Gets all non-abstract types extending the given base type and with the given attribute
        /// </summary>
        public static Type[] GetSubTypes(Type baseType, Type hasAttribute){
            return GetTypes().Where((Type T) =>
                    (T.IsClass && !T.IsAbstract)
                    && T.IsSubclassOf(baseType)
                    && T.GetCustomAttributes(hasAttribute, false).Any()
                    && !T.GetCustomAttributes(typeof(ReflectionSearchIgnoreAttribute), false).Any())
                .ToArray();
        }

#if UNITY_EDITOR
    /// <summary>
    /// Returns all fields that should be serialized in the given type
    /// </summary>
    public static FieldInfo[] GetSerializedFields(Type type)
    {
        return type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where((FieldInfo field) =>
                (field.IsPublic && !field.GetCustomAttributes(typeof(NonSerializedAttribute), true).Any())
                || field.GetCustomAttributes(typeof(SerializeField), true).Any()
                && !field.GetCustomAttributes(typeof(ReflectionSearchIgnoreAttribute), false).Any())
            .ToArray();
    }

    /// <summary>
    /// Returns all fields that should be serialized in the given type, minus the fields declared in or above the given base type
    /// </summary>
    public static FieldInfo[] GetSerializedFields(Type type, Type hiddenBaseType)
    {
        return type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where((FieldInfo field) =>
                (hiddenBaseType == null || !field.DeclaringType.IsAssignableFrom(hiddenBaseType))
                && ((field.IsPublic && !field.GetCustomAttributes(typeof(NonSerializedAttribute), true).Any())
                    || field.GetCustomAttributes(typeof(SerializeField), true).Any()
                    && !field.GetCustomAttributes(typeof(ReflectionSearchIgnoreAttribute), false).Any()))
            .ToArray();
    }
#endif
        /// <summary>
        /// Gets all fields in the classType of the specified fieldType
        /// </summary>
        public static FieldInfo[] GetFieldsOfType(Type classType, Type fieldType){
            return classType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where((FieldInfo field) =>
                    field.FieldType == fieldType || field.FieldType.IsSubclassOf(fieldType)
                    && !field.GetCustomAttributes(typeof(ReflectionSearchIgnoreAttribute), false).Any())
                .ToArray();
        }
    }
}