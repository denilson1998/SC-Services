
namespace Delivery.Infrastructure.Services.Tookan;
public static class TookanCourierConstants
{
    public const string AutoAssignTask = "1";
    public const string HasPickup = "1";
    public const string HasDelivery = "1";
    public const int HasTrackingLink = 1;
    public const string Timezone = "240";
    public const int NonPoolTask = 0;
    public const int PoolTask = 1;
    public const string PickupAndDeliveryLayoutType = "0";
    public const int PricingFormulaType = 2;
    public const string PricingFormulaFGield = "2";
}

public enum TookanJobStatus
{
    Unassigned = 6,
    Assigned = 0,
    AcceptedOrAcknowledged = 7,
    Started = 1,
    Arrived = 4,
    Successful = 2,
    Deleted = 10,
    Failed = 3,
    Decline = 8,
    Cancel = 9,
}