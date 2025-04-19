using System;
using System.Collections.Generic;
using System.Text;

namespace DeliveryAssignmentEngine.Domain.Enums;

public enum DeliveryStatus
{
    Created,
    PendingAssignment,
    Assigned,
    InTransit,
    Completed,
    Failed,
    Canceled
}

public enum DeliveryAgentStatus
{
    Available,
    Busy,
    OnBreak,
    Offline
}

public enum VehicleType
{
    Bicycle,
    Motorcycle,
    Car,
    Van,
    Truck
}