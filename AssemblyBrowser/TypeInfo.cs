using System;
using System.Collections.Generic;
using System.Reflection;

namespace AssemblyBrowser
{
	public class TypeInfo
	{
		public string Name { get; }
		public string FullName { get; }

		private delegate void Method(MemberInfo member);

		private Dictionary<MemberTypes, Method> TypeMethods;

		private readonly List<string> typeMembers=new List<string>();
		public IEnumerable<string> TypeMembers { get { return typeMembers; } }

		internal TypeInfo(string _name, string _fullname)
		{
			Name = _name;
			FullName = _fullname;
			TypeMethods = new Dictionary<MemberTypes, Method>();
			TypeMethods.Add(MemberTypes.Field, AddField);
			TypeMethods.Add(MemberTypes.Property, AddProperty);
			TypeMethods.Add(MemberTypes.Method, AddMethod);
		}

		public void AddMember(Type type)
		{
			BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Static;
			foreach (MemberInfo member in type.GetMembers(flags))
			{
				if (TypeMethods.ContainsKey(member.MemberType))
					TypeMethods[member.MemberType](member);
			}
		}

		internal void AddMethod(MemberInfo info)
		{
			MethodInfo methodInfo=(MethodInfo)info;
			if (methodInfo.IsSpecialName == true)
				return;
			string result;
			result = GetAccessor(info)+" "+TypeNameFormat(methodInfo.ReturnType) + " " + methodInfo.Name + "(";
			ParameterInfo[] parameters = methodInfo.GetParameters();
			for (int i = 0; i < parameters.Length; i++)
			{
				if (i != 0)
					result += ", ";
				result += TypeNameFormat(parameters[i].ParameterType);
			}
			result += ")";
			typeMembers.Add(result);
		}

		internal void AddField(MemberInfo info)
		{
			string result;
			FieldInfo fieldInfo = (FieldInfo)info;
			result = GetAccessor(info) + " " + TypeNameFormat(fieldInfo.FieldType) + " " + info.Name;
			typeMembers.Add(result);
		}

		internal void AddProperty(MemberInfo info)
		{
			string result;
			PropertyInfo propertyInfo = (PropertyInfo)info;
			result = GetAccessor(info) + " " + TypeNameFormat(propertyInfo.PropertyType) + " " + info.Name;
			typeMembers.Add(result);
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
				member.MemberType == MemberTypes.Property && (member as PropertyInfo).GetSetMethod() != null ||
				member.MemberType == MemberTypes.Method && (member as MethodInfo).IsPublic)
				return "public";
			return "private";
		}
	}
}

