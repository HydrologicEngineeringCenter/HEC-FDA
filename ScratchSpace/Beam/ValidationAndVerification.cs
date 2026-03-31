using Amazon.Auth.AccessControlPolicy;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ScratchSpace.Beam;

public static class ValidationAndVerification
{
    private static readonly string ConfigPath = Path.GetFullPath(
        Path.Combine(@"C:\Programs\Source\HEC-FDA2\HEC.FDA.TestingUtility\all_case_studies.json"));

    private static readonly string OutputPath = Path.GetFullPath(
        Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Beam", "output"));

    public static async Task<int> RunValidationAndVerificationReport()
    {
        string[] args = ["compute", "-c", ConfigPath];
        return await HEC.FDA.TestingUtility.Program.Main(args);
    }
}
