namespace Kestridge.Api.Data;

// The three lockout columns, which admin_accounts and admin_enrollments both
// carry, so one AdminAccess.ChargeAsync serves a sign-in, a step-up code and a
// guess at a new user's initial password.
public interface IFailureCounted
{
    ushort FailedAttempts { get; set; }

    DateTime? FirstFailedAt { get; set; }

    DateTime? LockedUntil { get; set; }
}
