using ScratchSpace.EntryPoints;

const string BrennanWork = "EIWRW4EFAANB144";
const string BrennanVM = "DESKTOP-U5B7M40";

string pcName = GetPCName();

if (pcName.Equals(BrennanWork) || pcName.Equals(BrennanVM))
{
    Beam.EntryPoint();
}
static string GetPCName() => Environment.GetEnvironmentVariable("COMPUTERNAME");