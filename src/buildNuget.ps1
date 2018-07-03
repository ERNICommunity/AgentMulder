# BuildNuget.ps1
#
# Builds the AgentMulder nuget package
#

param ($version = $(throw "Version must be specified"), $waveVersionLow = $(throw "Minimum Wave version must be specified"), $waveVersionHigh = $(throw "Maximum Wave version must be specified"), $config = "Release")

if ($waveVersionLow -notmatch "\d+\.\d+")
{
    Write-Output "The WaveVersionLow parameter has incorrect format. Must match '\d+\.\d+' (e.g. 8.0)";
    return;
}

if ($waveVersionHigh -notmatch "\d+\.\d+")
{
    Write-Output "The WaveVersionHigh parameter has incorrect format. Must match '\d+\.\d+' (e.g. 8.0)";
    return;
}

Write-Output "Building AgentMulder version $version for Wave [$waveVersionLow, $waveVersionHigh), Configuration: $config";

# resetting working dir to the directory of the script
# this is necesary as we are using relative paths later on
$dir = Split-Path $MyInvocation.MyCommand.Path;
Push-Location $dir;
[Environment]::CurrentDirectory = $PWD

$nugetOutputDir = "nuget"

md -Force $nugetOutputDir | Out-Null

$nugetPath = ".nuget\NuGet.exe";
$nugetParams = "pack AgentMulder.nuspec -nopackageanalysis -outputdirectory $nugetOutputDir -properties 'version=$version;waveVersionLow=$waveVersionLow;waveVersionHigh=$waveVersionHigh;config=$config'";

Write-Output "Running NuGet: ", "$nugetPath $nugetParams"
Invoke-Expression "$nugetPath $nugetParams";

Write-Output "AgentMulder $version built successfully";
