using System;
using System.Collections.Generic;
using System.Reflection;

namespace AssemblyBrowser
{
	public class TypeInfo
	{
		public string TypeName { get; }
		public string FullName { get; }

		private delegate void Method(MemberInfo member);

		private Dictionary<MemberTypes, Method> TypeDelegates;

		private readonly List<TypeData> typeMethods = new List<TypeData>();
		public IEnumerable<TypeData> TypeMethods { get { return typeMethods; } }

		private readonly List<TypeData> typeFields = new List<TypeData>();
		public IEnumerable<TypeData> TypeFields { get { return typeFields; } }

		private readonly List<TypeData> typeProperties = new List<TypeData>();
		public IEnumerable<TypeData> TypeProperties { get { return typeProperties; } }

		internal TypeInfo(string _name, string _fullname)
		{
			TypeName = _name;
			FullName = _fullname;
			TypeDelegates = new Dictionary<MemberTypes, Method>();
			TypeDelegates.Add(MemberTypes.Field, AddField);
			TypeDelegates.Add(MemberTypes.Property, AddProperty);
			TypeDelegates.Add(MemberTypes.Method, AddMethod);
		}

		public void AddMember(Type type)
		{
			BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Static;
			foreach (MemberInfo member in type.GetMembers(flags))
			{
				if (TypeDelegates.ContainsKey(member.MemberType))
					TypeDelegates[member.MemberType](member);
			}
		}

		internal void AddMethod(MemberInfo info )
		{
			MethodInfo methodInfo = (MethodInfo)info;
			if (methodInfo.IsSpecialName == true)
				return;
			string result;
			result = GetAccessor(info) + " " + TypeNameFormat(methodInfo.ReturnType) + " " + methodInfo.Name + "(";
			ParameterInfo[] parameters = methodInfo.GetParameters();
			for (int i = 0; i < parameters.Length; i++)
			{
				if (i != 0)
					result += ", ";
				result += TypeNameFormat(parameters[i].ParameterType);
			}
			result += ")";
			typeMethods.Add(new TypeData(result));
		}

		internal void AddField(MemberInfo info)
		{
			string result;
			FieldInfo fieldInfo = (FieldInfo)info;
			result = GetAccessor(info) + " " + TypeNameFormat(fieldInfo.FieldType) + " " + info.Name;
			typeFields.Add(new TypeData(result));
		}

		internal void AddProperty(MemberInfo info)
		{
			string result;
			PropertyInfo propertyInfo = (PropertyInfo)info;
			result = GetAccessor(info) + " " + TypeNameFormat(propertyInfo.PropertyType) + " " + info.Name;
			typeProperties.Add(new TypeData(result));
		}

		private string TypeNameFormat(Type type)
		{
			string result;
			if (type.IsGenericType)
			{
				result = type.GetGenericTypeDefinition().Name;
				result = result.Remove(result.Length - 2, 2);
				result += "<" + type.GetGenericArguments()[0].Name;
				for (int i = 1; i < type.GetGenericArguments().Length; i++)
				{
					result += ", " + type.GetGenericArguments()[i].Name;
				}
				result += ">";
			}
			else
				result = type.Name;
			return result;
		}

		public static string GetAccessor(MemberInfo member)
		{
			if (member.MemberType == MemberTypes.Field && (member as FieldInfo).IsPublic ||
				member.MemberType == MemberTypes.Property && ((member as PropertyInfo).GetGetMethod() != null || (member as PropertyInfo).GetSetMethod() != null) ||
				member.MemberType == MemberTypes.Method && (member as MethodInfo).IsPublic)
				return "public";
			return "private";
		}
	}

	public class TypeData
	{
		public TypeData(string info)
        {
			View = info;
        }
		public string View { get; set; }
	}
}

