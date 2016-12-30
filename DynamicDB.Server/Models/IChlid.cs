using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compete.DynamicDB.Models
{
    internal interface IChlid
    {
        object Parent { get; set; }
    }
}
