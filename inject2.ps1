$b64 = (Get-Content base64.txt -Raw).Trim()
$htmlPath = 'SmartTeethCare.Service\EmailTemplates\AppointmentBooked.html'
$html = Get-Content $htmlPath -Raw
$pattern = '<img src="[^"]+" alt="Lunare Dental"[^>]*>'
$replacement = '<img src="data:image/png;base64,' + $b64 + '" alt="Lunare Dental" style="max-width: 280px; height: auto; margin-bottom: 15px;" />'
$html = [regex]::Replace($html, $pattern, $replacement, [System.Text.RegularExpressions.RegexOptions]::Singleline)
Set-Content $htmlPath -Value $html -Encoding utf8
Write-Host "Replaced successfully in AppointmentBooked"
