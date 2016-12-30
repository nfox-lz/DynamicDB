using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;

namespace Compete.DynamicDB.Script
{
    internal sealed class ScriptCompiler
    {
        private static readonly string AssemblyName = string.Format("{0}.dll", typeof(ScriptCompiler).Assembly.GetName().Name);

        private static readonly string ProcedureTemplatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Script/Templates/Procedure.txt");

        public string Language { get; set; }

        public CompilerErrorCollection Errors { get; private set; }

        public Models.Instance CurrentInstance { get; private set; }

        public Models.Database CurrentDatabase { get; private set; }

        public ScriptCompiler(Models.Instance instance, Models.Database database)
        {
            Language = "C#";
            CurrentInstance = instance;
            CurrentDatabase = database;
        }

        private string lastScript = string.Empty;

        private ExpressionTemplateBase lastTemplateBase = null;

        private CompilerParameters CreateCompilerParameters(bool generateInMemory = true)
        {
            var parameters = new CompilerParameters();
            parameters.ReferencedAssemblies.Add("System.dll");
            parameters.ReferencedAssemblies.Add("System.Core.dll");
            parameters.ReferencedAssemblies.Add(AssemblyName);
            parameters.GenerateInMemory = generateInMemory;
            return parameters;
        }

        public bool CompileProcedure(string script, out string path)
        {
            path = null;
            if (!File.Exists(ProcedureTemplatePath))
                return true;

            var results = CodeDomProvider.CreateProvider(Language).CompileAssemblyFromSource(CreateCompilerParameters(false), string.Format(File.ReadAllText(ProcedureTemplatePath), script));

            Errors = results.Errors;
            if (Errors.HasErrors)
                return true;

            path = results.PathToAssembly;
            //binary = Commons.CompressionHelper.Compress(File.ReadAllBytes(results.PathToAssembly));
            //binary = File.ReadAllBytes(results.PathToAssembly);
            return false;
        }

        public int ExecuteProcedure(string script, object[] parameters, out object result)
        {
            result = null;

            var results = CodeDomProvider.CreateProvider(Language).CompileAssemblyFromSource(CreateCompilerParameters(), script);

            Errors = results.Errors;
            if (Errors.HasErrors)
                return -1;

            var templateBase = results.CompiledAssembly.CreateInstance("Compete.DynamicDB.Script.DynamicScript.ProcedureScript") as ProcedureTemplateBase;
            templateBase.Instance = CurrentInstance;
            templateBase.Database = CurrentDatabase;
            return templateBase.Execute(parameters, out result);
        }

        public bool ExecuteExpression(string script, out object result)
        {
            result = null;

            if (string.IsNullOrWhiteSpace(script))
                return false;

            if (script == lastScript)
                lock (lastScript)
                    if (script == lastScript)
                    {
                        result = lastTemplateBase.Execute();
                        return true;
                    }

            var codeCompileUnit = new CodeCompileUnit();
            var codeNamespace = new CodeNamespace("Compete.DynamicDB.Script.DynamicScript");
            codeNamespace.Imports.Add(new CodeNamespaceImport("System"));
            codeNamespace.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));
            codeNamespace.Imports.Add(new CodeNamespaceImport("System.Linq"));

            var codeTypeDeclaration = new CodeTypeDeclaration("ExpressionScript");
            codeTypeDeclaration.Attributes = MemberAttributes.Public;
            codeTypeDeclaration.BaseTypes.Add(typeof(ExpressionTemplateBase));

            //var codeConstructor = new CodeConstructor();

            var executeMethod = new CodeMemberMethod();
            executeMethod.Attributes = MemberAttributes.Public | MemberAttributes.Override;
            executeMethod.Name = "Execute";
            //executeMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(Models.Database), "database"));
            executeMethod.ReturnType = new CodeTypeReference(typeof(object));
            executeMethod.Statements.Add(new CodeMethodReturnStatement(new CodeArgumentReferenceExpression(script)));

            codeTypeDeclaration.Members.Add(executeMethod);

            codeNamespace.Types.Add(codeTypeDeclaration);
            codeCompileUnit.Namespaces.Add(codeNamespace);

            //provider.GenerateCodeFromExpression()
            //provider.GenerateCodeFromCompileUnit(codeCompileUnit, Console.Out, parameters);
            var results = CodeDomProvider.CreateProvider(Language).CompileAssemblyFromDom(CreateCompilerParameters(), codeCompileUnit);

            Errors = results.Errors;
            if (Errors.HasErrors)
                return false;

            //var assembly = results.CompiledAssembly;
            var templateBase = results.CompiledAssembly.CreateInstance("Compete.DynamicDB.Script.DynamicScript.ExpressionScript") as ExpressionTemplateBase;
            templateBase.Instance = CurrentInstance;
            templateBase.Database = CurrentDatabase;
            result = templateBase.Execute();

            lock(lastScript)
            {
                lastScript = script;
                lastTemplateBase = templateBase;
            }

            return true;
        }
    }
}
