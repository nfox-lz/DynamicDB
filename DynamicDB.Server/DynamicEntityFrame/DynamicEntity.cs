namespace Compete.DynamicDB.DynamicEntityFrame
{
    public abstract class DynamicEntity
    {
        public object this[string name]
        {
            //实现索引器的get方法
            get
            {
                return GetType().GetProperty(name).GetValue(this);
            }

            //实现索引器的set方法
            set
            {
                GetType().GetProperty(name).SetValue(this, value);
            }
        }
    }
}
