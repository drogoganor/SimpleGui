# https://github.com/NuGet/Home/issues/4126#issuecomment-533354997
# Hack to patch nuget.config with credentials

[CmdletBinding()]
param (
    [Parameter(Mandatory = $true)][string]$ConfigFile,
    [Parameter(Mandatory = $true)][string]$Source,
    [Parameter(Mandatory = $true)][string]$Username,
    [Parameter(Mandatory = $true)][string]$Password
)
$doc = New-Object System.Xml.XmlDocument
$filename = (Get-Item $ConfigFile).FullName
$doc.Load($filename)

$creds = $doc.DocumentElement.SelectSingleNode("packageSourceCredentials")
if ($creds -eq $null)
{
    $creds = $doc.CreateElement("packageSourceCredentials")
    $doc.DocumentElement.AppendChild($creds) | Out-Null
}

$sourceElement = $creds.SelectSingleNode($Source)
if ($sourceElement -eq $null)
{
    $sourceElement = $doc.CreateElement($Source)
    $creds.AppendChild($sourceElement) | Out-Null
}

$usernameElement = $sourceElement.SelectSingleNode("add[@key='Username']")
if ($usernameElement -eq $null)
{
    $usernameElement = $doc.CreateElement("add")
    $usernameElement.SetAttribute("key", "Username")
    $sourceElement.AppendChild($usernameElement) | Out-Null
}
$usernameElement.SetAttribute("value", $Username)

$passwordElement = $sourceElement.SelectSingleNode("add[@key='ClearTextPassword']")
if ($passwordElement -eq $null)
{
    $passwordElement = $doc.CreateElement("add")
    $passwordElement.SetAttribute("key", "ClearTextPassword")
    $sourceElement.AppendChild($passwordElement) | Out-Null
}
$passwordElement.SetAttribute("value", $Password)

$doc.Save($filename)