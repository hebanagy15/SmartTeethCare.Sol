Add-Type -AssemblyName System.Drawing
$img = [System.Drawing.Image]::FromFile("SmartTeethCare.API\wwwroot\images\lunare-logo.png")
$newWidth = 300
$newHeight = [int]($img.Height * ($newWidth / $img.Width))
$bmp = New-Object System.Drawing.Bitmap $newWidth, $newHeight
$g = [System.Drawing.Graphics]::FromImage($bmp)
$g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
$g.DrawImage($img, 0, 0, $newWidth, $newHeight)
$ms = New-Object System.IO.MemoryStream
$bmp.Save($ms, [System.Drawing.Imaging.ImageFormat]::Png)
$bytes = $ms.ToArray()
$base64 = [Convert]::ToBase64String($bytes)
$base64 | Out-File "base64.txt" -Encoding ascii
Write-Host "Base64 Length: $($base64.Length)"
