using AutoMapper;
using Delivery.Api.Endpoints.CourierTasks;
using Delivery.Api.Endpoints.Fares;
using Delivery.Api.Endpoints.Pricing;
using Delivery.Domain.Entities;

namespace Delivery.Api
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<CourierTask, ListCourierTasksResult>();
            CreateMap<CourierTask, CreateCourierTaskResponse>();
            CreateMap<Pricing, ListPricingResult>();
            CreateMap<Pricing, CreatePricingResponse>();
            CreateMap<Fare, CreateFareResponse>();
        }
    }
}