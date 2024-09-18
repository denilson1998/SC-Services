using System;
using System.Collections.Generic;
using System.Linq;

namespace Delivery.Api.OperationDefaults
{
    public class Operations
    {
        public List<dynamic> ApplyDataOperations<dynamic>(List<dynamic> response, Func<dynamic, dynamic> applyMapper)
        {
            var result = response.Select(applyMapper).ToList();
            return result;
        }
    }
}