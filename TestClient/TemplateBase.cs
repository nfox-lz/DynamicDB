using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestClient
{
    public abstract class TemplateBase
    {
        public abstract object Compete(Compete.DynamicDB.NetDatabase database);
    }
}
