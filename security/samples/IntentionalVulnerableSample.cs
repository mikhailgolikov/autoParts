// INTENTIONAL SECURITY SAMPLES — used only to verify SAST scanners work.
// This file is NOT part of the application build. Do not copy into src/.

namespace SecurityAuditSamples;

public static class IntentionalVulnerableSample
{
    // SAST trigger: hardcoded credential (CWE-798)
    private const string AdminPassword = "SuperSecretAdmin123!";

    // SAST trigger: SQL injection via string concatenation (CWE-89)
    public static string BuildUnsafeUserLookup(string email, Microsoft.EntityFrameworkCore.DbContext db)
    {
        var query = "SELECT * FROM Users WHERE Email = '" + email + "'";
        db.Database.ExecuteSqlRaw(query);
        return query;
    }

    public static string GetAdminPassword() => AdminPassword;
}
