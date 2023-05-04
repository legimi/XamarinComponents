$dir = "/Users/pawel.kanarek/Git/XamarinComponents/iOS/MaterialComponents/source/MaterialComponents/bin/Release/net6.0-ios/MaterialComponents.resources/"
$files = Get-ChildItem $dir -Recurse

foreach ($file in $files) {
  $relativePath = $file.FullName.Replace($dir, "")
  $targetPath = ""
  if ($relativePath.EndsWith("/Info.plist")) {
    $path = $relativePath.Replace("/Info.plist", "")
    $targetPath = "lib/net6.0-ios16.1/MaterialComponents.resources/$path"
  }
  else {
    $pathWithoutFileName = $relativePath.Replace($file.Name, "")
    $path = $pathWithoutFileName.Substring(0, $path.Length - 1)
    $targetPath = "lib/net6.0-ios16.1/MaterialComponents.resources/$path"
  }
  $output = "<file src=`"../source/MaterialComponents/bin/Release/net6.0-ios/MaterialComponents.resources/$relativePath`" target=`"$targetPath`" />"
  Write-Output $output
}