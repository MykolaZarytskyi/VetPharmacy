namespace VetPharmacy.Api.Common.Authorization;

public static class Policies
{
    public const string UserAccess = nameof(UserAccess);
    public const string AdminAccess = nameof(AdminAccess);
    public const string OwnerOrAdminBasketAccess = nameof(OwnerOrAdminBasketAccess);
}
