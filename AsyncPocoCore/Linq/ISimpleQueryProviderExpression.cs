using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncPoco.Linq
{
    public interface ISimpleQueryProviderExpression<TModel>
    {
        SqlExpression<TModel> AtlasSqlExpression { get; }
    }
}
