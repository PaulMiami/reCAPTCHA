$ErrorActionPreference = "Stop"

$outputLocation = 'testResults'

$outputPath = (Resolve-Path $outputLocation).Path
$outputFile = Join-Path $outputPath -childpath 'coverage.xml'

Write-Output $outputPath
Write-Output $outputFile

Function Verify-OnlyOnePackage
{
	param ($name)

	$location = $env:USERPROFILE + '\.nuget\packages\' + $name
	If ((Get-ChildItem $location).Count -ne 1)
	{
		throw 'Invalid number of packages installed at ' + $location
	}
}

Verify-OnlyOnePackage 'coveralls.io'
Verify-OnlyOnePackage 'ReportGenerator'

pushd
Try
{
	# Either display or publish the results
	If ($env:CI -eq 'True')
	{
		$command = (Get-ChildItem ($env:USERPROFILE + '\.nuget\packages\coveralls.io'))[0].FullName + '\tools\coveralls.net.exe' + ' --cobertura "' + $outputFile + '" --full-sources'
		Write-Output $command
		iex $command
	}
	Else
	{
		$command = (Get-ChildItem ($env:USERPROFILE + '\.nuget\packages\ReportGenerator'))[0].FullName + '\tools\net47\ReportGenerator.exe -reports:"' + $outputFile + '" -targetdir:"' + $outputPath + '"'
		Write-Output $command
		iex $command
		cd $outputPath
		./index.htm
	}
}
Finally
{
	popd
}
