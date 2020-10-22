using System;
using System.Reflection;

namespace AssemblyBrowser
{
	public class AsmBrowser
	{

		public AssemblyInfo CollectAssemblyInfo(string path)
		{
			Assembly assembly = Assembly.LoadFrom(path);
			AssemblyInfo result = new AssemblyInfo(assembly.GetName().Name);
			Type[] types = assembly.GetTypes();
			foreach (Type type in types)
			{
				if (!type.IsNested)
				{
					NamespaceInfo nsi=null;
					if (type.Namespace != null)
					{
						string[] namespaces = type.Namespace.Split('.');
						nsi= result.GetNamespace(namespaces[0]);
						for (int i = 1; i < namespaces.Length; i++)
						{
							result.GetNamespace(namespaces[i]);
						}
					}
					nsi.AddType(GenerateTypeInfo(type));
				}
			}
			return result;
		}

		private TypeInfo GenerateTypeInfo(Type type)
		{
			TypeInfo result = new TypeInfo(type.Name, type.FullName);
			result.AddMember(type);
			return result;
		}
	}
}
