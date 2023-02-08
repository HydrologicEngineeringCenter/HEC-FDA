# HEC-FDA

# Current Status of Main
![example workflow](https://github.com/HydrologicEngineeringCenter/workflows/IntegrationTesting.yml/badge.svg)
![example workflow](https://github.com/HydrologicEngineeringCenter/workflows/CI.yml/badge.svg?branch=main)

# Build Process with Visual Studio 2022
- Clone the HEC-FDA Repo to your PC
- Ensure you have the proper NuGet Source Connections. You'll need both a connection to the HEC Github and Nexus. Nexus is relatively simple to connect to, and will just need to be added to your NuGet Sources through visual studio. There are multiple ways of setting that source. a Microsoft resource is provided at the link below to help. There is also a Github Discussion on adding the Github source reference 
  - Github - https://github.com/HydrologicEngineeringCenter/HEC-FDA/discussions/170
  - Nexus - https://docs.microsoft.com/en-us/azure/devops/artifacts/nuget/consume?view=azure-devops&tabs=windows
     - Source = https://www.hec.usace.army.mil/nexus/repository/fda-nuget/
- Build your solution. Visual Studio should automatically do a package restore to restore your references. 
