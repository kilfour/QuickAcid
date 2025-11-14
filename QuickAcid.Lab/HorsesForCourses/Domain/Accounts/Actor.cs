using System.Security.Claims;

namespace QuickAcid.Lab.HorsesForCourses.Domain.Accounts;

public record Actor
{
    // ------------------------------------------------------------
    // -- BusinessRules to Stakes, .... not quite yet
    //        => see `ApplicationUser.EnterScene()`
    public Actor Declare(string type, string value)
    {
        stakes[type] = value;
        return this;
    }
    // ------------------------------------------------------------
    // -- Stakes To BusinesRules
    public void IsAuthenticated()
    {
        if (stakes.Count == 0) // no name, no nothing, empty claims => not authenticated
            throw new UnauthorizedAccessException(); // maybe better to check specific claim
    }

    public void CanCreateCoach()
    {
        if (HasStake(ClaimTypes.Role, ApplicationUser.AdminRole)) return;
        throw new UnauthorizedAccessException();
    }

    public void CanEditCoach(string coachEmail)
    {
        if (HasStake(ClaimTypes.Role, ApplicationUser.AdminRole)) return;
        if (HasStake(ClaimTypes.Role, ApplicationUser.CoachRole) && HasStake(ClaimTypes.Email, coachEmail)) return;
        throw new UnauthorizedAccessException();
    }

    public void CanEditCourses()
    {
        if (HasStake(ClaimTypes.Role, ApplicationUser.AdminRole)) return;
        throw new UnauthorizedAccessException();
    }
    // ------------------------------------------------------------
    // -- To Claims and Back
    public IEnumerable<Claim> ClaimIt()
        => stakes.Select(a => new Claim(a.Key, a.Value));
    public static Actor From(IEnumerable<Claim> claims)
    {
        var actor = new Actor();
        foreach (var claim in claims) actor.Declare(claim.Type, claim.Value);
        return actor;
    }
    // ------------------------------------------------------------
    // -- internals
    private readonly Dictionary<string, string> stakes = [];
    private bool HasStake(string type, string value)
    {
        if (!stakes.ContainsKey(type)) return false;
        if (stakes[type] != value) return false;
        return true;
    }

    public static Actor SystemActor()
        => new Actor().Declare(ClaimTypes.Role, ApplicationUser.SystemRole);

    // ------------------------------------------------------------
}