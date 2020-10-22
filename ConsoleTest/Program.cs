using System;
using AssemblyBrowser;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            AsmBrowser asmBrowser = new AsmBrowser();
            AssemblyInfo browseResult = asmBrowser.CollectAssemblyInfo("C:\\Users\\home\\Desktop\\Labs\\Третий Сем\\СПП\\OldFaker\\Faker\\bin\\Debug\\netstandard2.0\\Faker.dll");

            Console.WriteLine("Dll Name: "+browseResult.Name);
            foreach (NamespaceInfo nsp in browseResult.Namespaces)
            {
                Console.WriteLine("   Namespace Name: " + nsp.Name);
                foreach (TypeInfo ti in nsp.TypesInfo)
                {
                    Console.WriteLine("      Type Name: " + ti.Name);
                    foreach (string mth in ti.TypeMembers)
                    {
                        Console.WriteLine("         "+mth);
                    }
                }
            }
        }
    }
}
