using System;
using System.Reflection;

namespace AssemblyBrowser
{
	public class AsmBrowser
	{

		public AssemblyInfo CollectAssemblyInfo(string path)
		{
			Assembly assembly = Assembly.LoadFrom(path);
			AssemblyInfo result = new AssemblyInfo(assembly.GetName().FullName);
			Type[] types = assembly.GetTypes();
			foreach (Type type in types)
			{
				if (!type.IsNested)
				{
					NamespaceInfo nsi = null;
					if (type.Namespace != null)
					{
						string[] namespaces = type.Namespace.Split('.');
						nsi= result.GetNamespace(type.Namespace);
					}
                    else
                    {
						nsi = new NamespaceInfo("No namespace");
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
