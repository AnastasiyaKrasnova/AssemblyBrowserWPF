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

		private readonly List<TypeMethod> typeMethods = new List<TypeMethod>();
		public IEnumerable<TypeMethod> TypeMethods { get { return typeMethods; } }

		private readonly List<TypeField> typeFields = new List<TypeField>();
		public IEnumerable<TypeField> TypeFields { get { return typeFields; } }

		private readonly List<TypeProperty> typeProperties = new List<TypeProperty>();
		public IEnumerable<TypeProperty> TypeProperties { get { return typeProperties; } }

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
			string acceptors = GetAccessor(info);
			string returnType = TypeNameFormat(methodInfo.ReturnType);
			string name=methodInfo.Name;

			ParameterInfo[] parameters = methodInfo.GetParameters();
			string[] param = new string[parameters.Length];
			for (int i = 0; i < parameters.Length; i++)
			{
				param[i] = TypeNameFormat(parameters[i].ParameterType);
			}
			typeMethods.Add(new TypeMethod(acceptors,returnType,name,param));
		}

		internal void AddField(MemberInfo info)
		{
			FieldInfo fieldInfo = (FieldInfo)info;
			string acceptors = GetAccessor(info);
			string returnType = TypeNameFormat(fieldInfo.FieldType);
			string name = info.Name;
			typeFields.Add(new TypeField(acceptors,returnType,name));
		}

		internal void AddProperty(MemberInfo info)
		{
			
			PropertyInfo propertyInfo = (PropertyInfo)info;
			string acceptors = GetAccessor(info);
			string returnType = TypeNameFormat(propertyInfo.PropertyType);
			string name = info.Name;
			typeProperties.Add(new TypeProperty(acceptors,returnType,name));
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


	public class TypeMethod
	{
		string Acceptors;
		string[] Params;
		string ReturnType;
		string Name;
		public TypeMethod(string acceptors, string returnType, string name, string[] param)
		{
			Acceptors = acceptors;
			ReturnType = returnType;
			Name = name;
			Params = param;
		}
		private string GetView()
        {
			string result =Acceptors+" " + ReturnType + " " + Name + "(";
			for (int i = 0; i < Params.Length; i++)
			{
				if (i != 0)
					result += ", ";
				result += Params[i];
			}
			result += ")";
			return result;
		}

		public string View { get { return GetView(); } }
	}

	public class TypeField
	{
		string Acceptors;
		string ReturnType;
		string Name;
		public TypeField(string acceptors, string returnType, string name)
		{
			Acceptors = acceptors;
			ReturnType = returnType;
			Name = name;
		}
		private string GetView()
		{
			string result;
			result = Acceptors + " " + ReturnType + " " + Name;
			return result;
		}

		public string View { get { return GetView(); } }
	}

	public class TypeProperty
	{
		string Acceptors;
		string ReturnType;
		string Name;
		public TypeProperty(string acceptors, string returnType, string name)
		{
			Acceptors = acceptors;
			ReturnType = returnType;
			Name = name;
		}
		private string GetView()
		{
			string result;
			result = Acceptors + " " + ReturnType + " " + Name;
			return result;
		}

		public string View { get { return GetView(); } }
	}
}

