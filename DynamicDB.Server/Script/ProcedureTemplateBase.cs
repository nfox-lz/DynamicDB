namespace Compete.DynamicDB.Script
{
    public abstract class ProcedureTemplateBase : TemplateBase
    {
        public abstract int Execute(object[] parameters, out object result);
    }
}
