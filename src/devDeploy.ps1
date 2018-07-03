# devDeploy.ps1
#
# Deploys the extension into a VS experimental instance
#

param ($hive = $(throw "VS experimental hive must be specified"), $version = $(throw "Version must be specified"), $config = $(throw "Configuration must be specified"))

echo "Deploying AgentMulder $version ($config) to VS hive $hive";

# resetting working dir to the directory of the script
# this is necesary as we are using relative paths later on
$dir = Split-Path $MyInvocation.MyCommand.Path;
Push-Location $dir;
[Environment]::CurrentDirectory = $PWD;

#check if VS 2017 experimental hive exists
if (!(Test-Path "$env:LOCALAPPDATA\Microsoft\VisualStudio\15.0_*$hive"))
{
	echo "Visual Studio experimental hive $hive does not exist.";
	echo "EXTENSION NOT INSTALLED";
	return;
}

$binSourcePath = "..\output\$config\AgentMulder.*.dll";
$pdbSourcePath = "..\output\$config\AgentMulder.*.pdb";
$containerBinSourcePath = "..\output\$config\Containers\AgentMulder.Containers.*.dll";
$containerPdbSourcePath = "..\output\$config\Containers\AgentMulder.Containers.*.pdb";

# copy the main binaries and PDBs
$targetDir = "$env:LOCALAPPDATA\JetBrains\plugins\ERNI.AgentMulder.$version\dotFiles";
Copy-Item $binSourcePath $targetDir;
Copy-Item $pdbSourcePath $targetDir;

# copy container-specific binaries and PDBs
$targetDir = "$env:LOCALAPPDATA\JetBrains\plugins\ERNI.AgentMulder.$version\dotFiles\Containers";
New-Item $targetDir -type directory -force | Out-Null

Copy-Item $containerBinSourcePath $targetDir;
Copy-Item $containerPdbSourcePath $targetDir;

$installationsRoot = "$env:LOCALAPPDATA\JetBrains\Installations";

# target installation directories - we copy to all copies for he specified hive (there may be more than one)
$installations = Get-ChildItem -Directory -Filter ("ReSharperPlatformVs15_*$hive" + "_*") -Path $installationsRoot

foreach ($dir in $installations) {
	$targetDir = "$installationsRoot\$dir"

	echo "Copying files to $targetDir"

	# copy the main binaries and PDBs
	Copy-Item $binSourcePath $targetDir;
	Copy-Item $pdbSourcePath $targetDir;

	# copy container-specific binaries and PDBs
	$targetDir = "$targetDir\Containers";
	New-Item $targetDir -type directory -force | Out-Null

	Copy-Item $containerBinSourcePath $targetDir;
	Copy-Item $containerPdbSourcePath $targetDir;
}

# done
echo "AgentMulder $version ($config) copied to hive $hive";