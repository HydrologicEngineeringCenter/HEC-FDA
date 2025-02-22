using ScratchSpace.EntryPoints;

// get your computer name quickly using powershell. enter the following:  '$env:COMPUTERNAME'
const string BrennanWork = "EIWRW4EFAANB455";
const string BrennanVM = "BEAM-VM00";
const string BrennanHome = "BRENNANDESKTOP";
const string RichardWork = "";
const string WillWork = "";

string pcName = GetPCName();

if (pcName.Equals(BrennanWork) || pcName.Equals(BrennanVM) || pcName.Equals(BrennanHome))
{
    Beam.EntryPoint();
}
if (pcName.Equals(RichardWork))
{
    Nugent.EntryPoint();
}
if (pcName.Equals(WillWork))
{
    Lehman.EntryPoint();
}
static string GetPCName() => Environment.GetEnvironmentVariable("COMPUTERNAME");