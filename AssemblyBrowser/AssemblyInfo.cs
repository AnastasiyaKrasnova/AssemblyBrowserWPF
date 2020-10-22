using System.Collections.Generic;

namespace AssemblyBrowser
{
	public class AssemblyInfo
	{
		public  string AssemblyName { get; }

		private readonly List<NamespaceInfo> namespaces= new List<NamespaceInfo>();
		public IEnumerable<NamespaceInfo> Namespaces { get { return namespaces; } }

		internal AssemblyInfo(string _name)
		{
			AssemblyName = _name;
		}

		internal NamespaceInfo GetNamespace(string name)
		{
			NamespaceInfo result;

			if ((result = namespaces.Find(x => x.NamespaceName == name)) != null)
				return result;
			result = new NamespaceInfo(name);
			namespaces.Add(result);
			return result;
		}
	}
}