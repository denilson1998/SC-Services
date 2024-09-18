using SharedKernel.Contracts;
using SharedKernel.DataFilters.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.DataFilters
{
    public interface IOperations
    {
        public Task<PagedResponse<List<dynamic>>> ApplyDataOperations<T>(IQueryable<T> query, GeneralParameters parameters, Func<T, dynamic> applyMapper, string groupedBy = null);
    }
}
