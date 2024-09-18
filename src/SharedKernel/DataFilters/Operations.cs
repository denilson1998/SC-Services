//using Microsoft.EntityFrameworkCore;
//using SharedKernel.Contracts;
//using SharedKernel.DataFilters.Filtering;
//using SharedKernel.DataFilters.Pagination.Helpers;
//using SharedKernel.DataFilters.Parameters;
//using SharedKernel.DataFilters.Sorting;

//namespace SharedKernel.DataFilters
//{
//    public class Operations : IOperations
//    {
//        public async Task<PagedResponse<List<dynamic>>> ApplyDataOperations<T>(IQueryable<T> query,
//            GeneralParameters parameters, Func<T, dynamic> applyMapper, string groupedBy = null)
//        {
//            var paginationOptions = new PaginationParameters();
//            var type = typeof(T);
//            IEnumerable<dynamic> result;
//            var totalRecords = 0;
//            try
//            {
//                totalRecords = await query.CountAsync();
//            }
//            catch (Exception error)
//            {
//                Console.Write(error.ToString());
//                totalRecords = query.ToList().Count;
//            }

//            if (parameters is not null)
//            {
//                paginationOptions = parameters.PaginationOptions;
//                query = FilteringHelper.ApplyFilterOptions(parameters.FilterOptions, query);
//                query = SortingHelper.ApplySortOptions(parameters.SortOptions, query);
//                if (groupedBy != null)
//                {
//                    var temp = query.AsEnumerable().GroupBy(p => p.GetType().GetProperty(groupedBy).GetValue(p))
//                        .Select(obj => obj.First());
//                    query = temp.AsQueryable();
//                }

//                query = query
//                    .Skip((parameters.PaginationOptions.PageNumber - 1) * parameters.PaginationOptions.PageSize)
//                    .Take(parameters.PaginationOptions.PageSize);
//                IEnumerable<T> pagedData = null;
//                if (groupedBy != null)
//                {
//                    pagedData = query.ToList();
//                }
//                else
//                {
//                    pagedData = await query.ToListAsync();
//                }

//                result = pagedData.Select(i => applyMapper(i));
//            }
//            else
//            {
//                IEnumerable<T> rawProducts = null;
//                if (groupedBy != null)
//                {
//                    var temp = query.AsEnumerable().GroupBy(p => p.GetType().GetProperty(groupedBy).GetValue(p))
//                        .Select(obj => obj.First());
//                    query = temp.AsQueryable();
//                    rawProducts = query.ToList();
//                }
//                else
//                {
//                    rawProducts = await query.ToListAsync();
//                }

//                result = rawProducts.Select(i => applyMapper(i));
//            }

//            var pagedReponse = PaginationHelper.CreatePagedReponse<T>(result.ToList(), paginationOptions, totalRecords);
//            return pagedReponse;
//        }
//    }
//}