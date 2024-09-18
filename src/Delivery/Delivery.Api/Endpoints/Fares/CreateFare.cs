using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using AutoMapper;
using Delivery.Domain.Dtos;
using Delivery.Domain.Entities;
using Delivery.Domain.Interfaces.Repositories;
using Delivery.Infrastructure.Services.GoogleMaps;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Delivery.Api.Endpoints.Fares
{
    public class CreateFare : EndpointBaseAsync
        .WithRequest<CreateFareCommand>
        .WithActionResult<CreateFareResponse>
    {
        private readonly IMapper _mapper;
        private readonly IGoogleMapsService _googleMapsService;
        private readonly IFareRepository _fareRepository;
        private readonly IPricingRepository _pricingRepository;

        public CreateFare(IMapper mapper,
            IGoogleMapsService googleMapsService,
            IFareRepository fareRepository,
            IPricingRepository pricingRepository
        )
        {
            _mapper = mapper;
            _googleMapsService = googleMapsService;
            _fareRepository = fareRepository;
            _pricingRepository = pricingRepository;
        }

        [HttpPost("organizations/{OrganizationId}/fares")]
        [Authorize(Policy = "UserM2MPolicy")]
        [SwaggerOperation(
            Summary = "Create a fare",
            Description = "Create a fare",
            OperationId = "Fares.CreateFare",
            Tags = new[] { "FaresEndpoint" })
        ]
        public override async Task<ActionResult<CreateFareResponse>> HandleAsync([FromRoute] CreateFareCommand command,
            CancellationToken cancellationToken = default)
        {
            var originLatLong = new LatLong(command.CreateFareCommandBody.OriginLatitude,
                command.CreateFareCommandBody.OriginLongitude);
            var destinationLatLong = new LatLong(command.CreateFareCommandBody.DestinationLatitude,
                command.CreateFareCommandBody.DestinationLongitude);
            // TODO delimitar zonas
            var distanceAndDuration =
                await _googleMapsService.GetDistanceAndDurationAsync(originLatLong, destinationLatLong);

            if (distanceAndDuration.Distance is null || distanceAndDuration.Distance is null)
            {
                return BadRequest("Distance and duration are invalid");
            }

            const int maxDurationInSeconds = 3600;
               if (distanceAndDuration.Duration > maxDurationInSeconds)
            {
                return BadRequest("The distance can't be more than a 60 minutes drive");
            }

            var pricingFound = await _pricingRepository.GetForStateIsActive(cancellationToken);

            if (pricingFound is null)
            {
                return BadRequest("There is no pricing active");
            }

            var price = pricingFound.CalculatePrice((int)distanceAndDuration.Distance, (int)distanceAndDuration.Duration);

            var fare = new Fare(
                command.OrganizationId,
                (int)distanceAndDuration.Distance,
                (int)distanceAndDuration.Duration,
                price,
                command.CreateFareCommandBody.OriginLatitude,
                command.CreateFareCommandBody.OriginLongitude,
                command.CreateFareCommandBody.DestinationLatitude,
                command.CreateFareCommandBody.DestinationLongitude
            );

            await _fareRepository.CreateAsync(fare, cancellationToken);
            var result = _mapper.Map<CreateFareResponse>(fare);

            return Ok(result);
        }
    }
}