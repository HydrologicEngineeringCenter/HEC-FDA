using System;
using System.IO;
using System.Threading.Tasks;

namespace ScratchSpace.Beam;

public static class ValidationAndVerification
{
    private static readonly string ConfigPath = Path.GetFullPath(
        Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Beam", "west-sac-config.json"));

    private static readonly string OutputPath = Path.GetFullPath(
        Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Beam", "output"));

    public static async Task<int> RunValidationAndVerificationReport()
    {
        string[] args = ["compute", "-c", ConfigPath, "-o", OutputPath];
        return await HEC.FDA.TestingUtility.Program.Main(args);
    }
}
