using System.Collections.Generic;

namespace AssemblyBrowser
{
	public class NamespaceInfo
	{
		public string Name { get; }
		private readonly List<TypeInfo> typesInfo = new List<TypeInfo>();
		public  IEnumerable<TypeInfo> TypesInfo { get { return typesInfo; } }

		internal NamespaceInfo(string _name)
		{
			Name = _name;
		}

		internal void AddType(TypeInfo typeInfo)
		{
			typesInfo.Add(typeInfo);
		}
	}
}
