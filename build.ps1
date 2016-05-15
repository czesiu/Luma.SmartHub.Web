$Location = Get-Location
$Configuration = "Release"

# log the runtimes
dnvm list
dnvm install 1.0.0-rc1-update2 -r coreclr -a x86
dnvm install 1.0.0-rc1-update2 -r clr -a x86
dnvm install 1.0.0-rc1-update2 -r coreclr -a x64
dnvm install 1.0.0-rc1-update2 -r clr -a x64
dnvm list
dnvm use 1.0.0-rc1-update2 -a x64 -r clr
dnu feeds list
dnu restore

# run the build
MSBuild src\Luma.SmartHub.sln /property:Configuration=$Configuration

// Search for all nuspec files
Get-ChildItem -r *.nuspec | ForEach-Object {

    // Finding csproj files in directory with nuspec file
    Get-ChildItem $_.Directory -r *.csproj| ForEach-Object {

        // Packaging project to nupkg
        Write-Host Packaging $_.FullName
        NuGet Pack $_.FullName -Prop Configuration=$Configuration
    }
}

Set-Location $Location

# tests
# dnvm use 1.0.0-rc1-update2 -a x64 -r clr
# dnx -p tests\Luma.SmartHub.Tests test -xml xunit-results.xml

# upload results to AppVeyor
# $wc = New-Object 'System.Net.WebClient'
# $wc.UploadFile("https://ci.appveyor.com/api/testresults/xunit/$($env:APPVEYOR_JOB_ID)", (Resolve-Path .\xunit-results.xml))