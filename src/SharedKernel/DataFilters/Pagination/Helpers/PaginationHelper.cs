//using SharedKernel.Contracts;
//using SharedKernel.DataFilters.Parameters;

//namespace SharedKernel.DataFilters.Pagination.Helpers
//{
//    public static class PaginationHelper
//    {
//        public static PagedResponse<List<dynamic>> CreatePagedReponse<T>(List<dynamic> pagedData, PaginationParameters validFilter, int totalRecords)
//        {
//            var response = new PagedResponse<List<dynamic>>(pagedData, validFilter.PageNumber, validFilter.PageSize);
//            var totalPages = (validFilter.PageSize != 0) ? (totalRecords / (double)validFilter.PageSize) : 0;
//            var roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));
//            response.TotalPages = roundedTotalPages;
//            response.TotalRecords = totalRecords;
//            response.Message = "Hay registros que coinciden con la búsqueda.";
//            if (response.TotalRecords <= 0)
//            {
//                response.Message = "No se encontraron registros.";
//            }

//            return response;
//        }
//    }
//}