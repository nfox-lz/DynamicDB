using Newtonsoft.Json;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Windows;

namespace TestClient
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Compete.DynamicDB.NetDBInstance instance = new Compete.DynamicDB.NetDBInstance();
            instance.CreateDatabase("Database");

            Compete.DynamicDB.NetDatabase database = new Compete.DynamicDB.NetDatabase() { Name = "Database" };
            database.CreateCollection("Collection");

            var data = new { Id = Guid.NewGuid(), Code = "10101", Name = "abc123", No = 12345L, Tag = new { Id = Guid.NewGuid(), Code = "10101", Name = "abc123", No = 12345L }, Sa = new string[] { "111", "222" }, Oa = new object[] { "1111", 2222, new { Id = Guid.NewGuid(), Code = "111" } } };

            Guid id;
            var beginDateTime = DateTime.Now;
            for (int index = 0; index < 100; index++)
                id = Compete.DynamicDB.NetDatabase.Insert("Database", "Collection", data);
            var endDateTime = DateTime.Now;

            MessageBox.Show((endDateTime - beginDateTime).ToString());
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            const int count = 10000000;
            IList<string> list = new List<string>();
            IDictionary<int, string> dictionary = new Dictionary<int, string>();
            for(int index = 0; index < count; index++)
            {
                list.Add(index.ToString());
                dictionary.Add(index, index.ToString());
            }

            DateTime listBeginDateTime, listEndDateTime, dictionaryBeginDateTime, dictionaryEndDateTime;
            string val;

            listBeginDateTime = DateTime.Now;
            for (int index = 0; index < count; index++)
                val = list[index];
            listEndDateTime = DateTime.Now;
            var listTime = listEndDateTime - listBeginDateTime;

            dictionaryBeginDateTime = DateTime.Now;
            for (int index = 0; index < count; index++)
                val = dictionary[index];
            dictionaryEndDateTime = DateTime.Now;
            var dictionaryTime = dictionaryEndDateTime - dictionaryBeginDateTime;

            MessageBox.Show(listTime.ToString());
            MessageBox.Show(dictionaryTime.ToString());
            MessageBox.Show((dictionaryTime - listTime).ToString());
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            IList<string> list = new List<string>();
            for (int index = 0; index < int.MaxValue; index++)
                list.Add(index.ToString());
        }

        private CompilerErrorCollection Errors;

        public bool Compile(string script, out object result)
        {
            result = null;

            var codeCompileUnit = new CodeCompileUnit();
            var codeNamespace = new CodeNamespace("Compete.DynamicDB.Script.DynamicScript");
            codeNamespace.Imports.Add(new CodeNamespaceImport("System"));
            codeNamespace.Imports.Add(new CodeNamespaceImport("TestClient"));

            var codeTypeDeclaration = new CodeTypeDeclaration("DatabaseScript");
            codeTypeDeclaration.Attributes = MemberAttributes.Public;
            codeTypeDeclaration.BaseTypes.Add(typeof(TemplateBase));

            //var codeConstructor = new CodeConstructor();

            var executeMethod = new CodeMemberMethod();
            executeMethod.Attributes = MemberAttributes.Public | MemberAttributes.Override;
            executeMethod.Name = "Compete";
            executeMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(Compete.DynamicDB.NetDatabase), "database"));
            executeMethod.ReturnType = new CodeTypeReference(typeof(object));
            //executeMethod.Statements.Add(new CodeStatement());
            executeMethod.Statements.Add(new CodeMethodReturnStatement(new CodeArgumentReferenceExpression(script)));

            codeTypeDeclaration.Members.Add(executeMethod);

            codeNamespace.Types.Add(codeTypeDeclaration);
            codeCompileUnit.Namespaces.Add(codeNamespace);

            var provider = CodeDomProvider.CreateProvider("C#");

            var parameters = new CompilerParameters();
            parameters.ReferencedAssemblies.Add("System.dll");
            parameters.ReferencedAssemblies.Add("System.Core.dll");
            parameters.ReferencedAssemblies.Add("DynamicDB.Client.dll");
            parameters.ReferencedAssemblies.Add("TestClient.exe");
            parameters.GenerateInMemory = true;

            //provider.GenerateCodeFromExpression()
            //var results = provider.CompileAssemblyFromSource(parameters, script);
            //provider.GenerateCodeFromCompileUnit(codeCompileUnit, Console.Out, parameters);
            var results = provider.CompileAssemblyFromDom(parameters, codeCompileUnit);

            Errors = results.Errors;
            if (Errors.HasErrors)
                return false;

            var assembly = results.CompiledAssembly;
            var templateBase = results.CompiledAssembly.CreateInstance("Compete.DynamicDB.Script.DynamicScript.DatabaseScript") as TemplateBase;// as TemplateBase;
            //result = templateBase.GetType().GetMethod("Compete").Invoke(templateBase, new object[] { null }).ToString();
            result = templateBase.Compete(null);

            return true;
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            object result;
            if (Compile(textBox.Text, out result))
                MessageBox.Show(result.ToString());
            else
                foreach (CompilerError error in Errors)
                    MessageBox.Show(error.ErrorText);
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            Compete.DynamicDB.NetDBInstance instance = new Compete.DynamicDB.NetDBInstance();
            instance.CreateDatabase("Database");

            Compete.DynamicDB.NetDatabase database = new Compete.DynamicDB.NetDatabase() { Name = "Database" } ;
            database.CreateCollection("Collection");

            //var data = new { Id = Guid.NewGuid(), Code = "10101", Name = "abc123", No = 12345L, Tag = new { Id = Guid.NewGuid(), Code = "10101", Name = "abc123", No = 12345L }, Sa = new string[] { "111", "222" }, Oa = new object[] { "1111", 2222, new { Id = Guid.NewGuid(), Code = "111" } } };

            string[] a;
            var beginDateTime = DateTime.Now;
            for (int index = 0; index < 100; index++)
                a = database.Query<string[]>("from collection in Database[\"Collection\"] select collection[\"Code\"]");
            var endDateTime = DateTime.Now;

            MessageBox.Show((endDateTime - beginDateTime).ToString());
            //database.Open();
            //var a = database.Query<int>("Database[\"Collection\"].Count()");
            //database.Close();
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            const int count = 1000000;
            var data = new { Id = Guid.NewGuid(), Code = "10101", Name = "abc123", No = 12345L, Tag = new { Id = Guid.NewGuid(), Code = "10101", Name = "abc123", No = 12345L }, Sa = new string[] { "111", "222" }, Oa = new object[] { "1111", 2222, new { Id = Guid.NewGuid(), Code = "111" } } };

            var newtonsoftJson = JsonConvert.SerializeObject(data);

            var javaScriptSerializer = new JavaScriptSerializer();
            var systemJson = javaScriptSerializer.Serialize(data);

            DateTime newtonsoftBeginDateTime, newtonsoftEndDateTime, systemBeginDateTime, systemEndDateTime;

            newtonsoftBeginDateTime = DateTime.Now;
            for (int index = 0; index < count; index++)
                JsonConvert.DeserializeObject(newtonsoftJson);
            newtonsoftEndDateTime = DateTime.Now;
            var newtonsoftTime = newtonsoftEndDateTime - newtonsoftBeginDateTime;

            systemBeginDateTime = DateTime.Now;
            for (int index = 0; index < count; index++)
                javaScriptSerializer.Deserialize<dynamic>(systemJson);
            systemEndDateTime = DateTime.Now;
            var systemTime = systemEndDateTime - systemBeginDateTime;

            MessageBox.Show(newtonsoftTime.ToString());
            MessageBox.Show(systemTime.ToString());
            MessageBox.Show((systemTime - newtonsoftTime).ToString());
        }

        private void button6_Click(object sender, RoutedEventArgs e)
        {
            Compete.DynamicDB.NetDBInstance instance = new Compete.DynamicDB.NetDBInstance();
            instance.Save();
        }

        private void button7_Click(object sender, RoutedEventArgs e)
        {
            //Compete.DynamicDB.NetDBInstance instance = new Compete.DynamicDB.NetDBInstance();
            //instance.CreateDatabase("Database");

            Compete.DynamicDB.NetDatabase database = new Compete.DynamicDB.NetDatabase() { Name = "Database" };
            database.CreateProcedure("Procedure", string.Empty);
        }
    }
}
