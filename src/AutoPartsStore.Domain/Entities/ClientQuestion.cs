using AutoPartsStore.Domain.Enums;

namespace AutoPartsStore.Domain.Entities;

public class ClientQuestion : Appeal
{
    public AppealCategory Category { get; set; }
}
