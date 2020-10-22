using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AssemblyBrowser;

namespace UnitTests
{
    [TestClass]
    public class AsmBrowserTest
    {
        public AsmBrowser asmBrowser;
        public AssemblyInfo result;

        [TestInitialize]
        public void Setup()
        {
            asmBrowser = new AsmBrowser();
        }

        [TestMethod]
        public void TestNamespaces()
        {
            result = asmBrowser.CollectAssemblyInfo("C:\\Users\\home\\Desktop\\Labs\\Третий Сем\\СПП\\OldFaker\\Faker\\bin\\Debug\\netstandard2.0\\Faker.dll");
            Assert.AreEqual(result.Namespaces.Count(), 1);
            Assert.AreEqual(result.Namespaces.First().NamespaceName, "Faker");
            Assert.AreEqual(result.Namespaces.First().TypesInfo.Count(), 12);
        }

        [TestMethod]
        public void TestTypes()
        {
            result = asmBrowser.CollectAssemblyInfo("C:\\Users\\home\\Desktop\\Labs\\Третий Сем\\СПП\\OldFaker\\Faker\\bin\\Debug\\netstandard2.0\\Faker.dll");
            Assert.IsTrue(result.Namespaces.First().TypesInfo.Any(obj => obj.TypeName == "CollectionGen"));
            Assert.IsTrue(result.Namespaces.First().TypesInfo.Any(obj => obj.TypeName == "Plugin"));
            Assert.IsTrue(result.Namespaces.First().TypesInfo.Any(obj => obj.TypeName == "ObjectCreator"));
        }

        [TestMethod]
        public void TestMembers()
        {
            result = asmBrowser.CollectAssemblyInfo("C:\\Users\\home\\Desktop\\Labs\\Третий Сем\\СПП\\OldFaker\\Faker\\bin\\Debug\\netstandard2.0\\Faker.dll");
            IEnumerable<TypeData> methods = result.Namespaces.First().TypesInfo.First(obj => obj.TypeName == "ArrayGen").TypeMethods;
            Assert.IsTrue(methods.Any(obj => obj.View == "private Void FillArray(Array, Int32, Int32[])"));
            methods = result.Namespaces.First().TypesInfo.First(obj => obj.TypeName == "ArrayGen").TypeFields;
            Assert.IsTrue(methods.Any(obj => obj.View == "private Random _numGen"));
            methods = result.Namespaces.First().TypesInfo.First(obj => obj.TypeName == "ArrayGen").TypeProperties;
            Assert.IsTrue(methods.Any(obj => obj.View == "public Type[] PossibleTypes"));

        }

        [TestMethod]
        public void TestInvalidAssembly()
        {
            result = asmBrowser.CollectAssemblyInfo("C:\\Users\\home\\Desktop\\Третий Сем\\СПП\\OldFaker\\Faker\\bin\\Debug\\netstandard2.0\\Faker.dll");
            Assert.AreEqual(result, null);
            result = asmBrowser.CollectAssemblyInfo("C:\\Users\\home\\Desktop\\Labs\\dll\\dsofile.dll");
            Assert.AreEqual(result, null);
        }

        [TestMethod]
        public void TestSeveralNamespacesAssembly()
        {
            result = asmBrowser.CollectAssemblyInfo("C:\\Users\\home\\Desktop\\Labs\\dll\\Json120r3\\Bin\\net45\\Newtonsoft.Json.dll");
            Assert.AreEqual(result.Namespaces.Count(), 11);
            Assert.IsTrue(result.Namespaces.All(obj => obj.TypesInfo.Count() > 0));
            Assert.IsTrue(result.Namespaces.ElementAt(5).TypesInfo.ElementAt(5).TypeMethods.Count() > 0);
        }
    }

}


