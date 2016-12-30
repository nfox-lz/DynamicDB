using Compete.DynamicDB.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Compete.DynamicDB.Models
{
    public sealed class Database : ItemDictionary<Collection>, IChlid
    {
        private class DatabaseData
        {
            public IDictionary<string, Collection> Collections { get; set; }

            public IDictionary<string, Procedure> Procedures { get; set; }
        }

        public IDictionary<string, Procedure> Procedures { get; private set; }

        public object Parent { get; set; }

        public Database()
        {
            Procedures = new Dictionary<string, Procedure>();
        }

        private static void LoadProcedure(Procedure procedure)
        {
            procedure.Binary = Commons.CompressionHelper.Compress(File.ReadAllBytes(procedure.Path));
        }

        private static void SaveProcedure(Procedure procedure)
        {
            File.WriteAllBytes(procedure.Path, Commons.CompressionHelper.Decompress(procedure.Binary));
            procedure.Binary = null;
        }

        public bool Save(string path)
        {
            foreach (var procedure in Procedures)
                LoadProcedure(procedure.Value);

            var data = new DatabaseData()
            {
                Collections = Items,
                Procedures = Procedures
            };

            try
            {
                data.SaveAs(path);

                return false;
            }
            catch
            {
                return true;
            }
        }

        public bool Load(string path)
        {
            if (!File.Exists(path))
                return true;

            var data = Commons.JsonHelper.Deserialize<DatabaseData>(path);

            Items = data.Collections;
            Procedures = data.Procedures;

            foreach (var procedure in Procedures)
                SaveProcedure(procedure.Value);

            return false;
        }

        public bool CreateProcedure(string name, string script, string language = "C#")
        {
            if (Procedures.ContainsKey(name))
                return true;

            string path;
            if ((new Script.ScriptCompiler(Parent as Instance, this) { Language = language }).CompileProcedure(script, out path))
                return true;

            var procedure = new Procedure() { Text = script, Path = path };
            lock (Procedures)
                if (!Procedures.ContainsKey(name))
                {
                    Procedures.Add(name, procedure);
                    return false;
                }

            return true;
        }

        public bool DropProcedure(string name)
        {
            if (Procedures.ContainsKey(name))
                lock (Procedures)
                    if (Procedures.ContainsKey(name))
                    {
                        Procedures.Remove(name);
                        return false;
                    }

            return true;
        }

        public int ExecuteProcedure(string name, out object result, params object[] parameters)
        {
            result = null;
            if (!Procedures.ContainsKey(name))
                return -1;

            var procedureScript = Assembly.LoadFile(Procedures[name].Path).CreateInstance("Compete.DynamicDB.Script.DynamicScript.ProcedureScript") as Script.ProcedureTemplateBase;
            return procedureScript.Execute(parameters, out result);
        }
    }
}
