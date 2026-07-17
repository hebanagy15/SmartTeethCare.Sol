$b64 = (Get-Content base64.txt -Raw).Trim()
$htmlPath = 'SmartTeethCare.Service\EmailTemplates\ReminderEmail.html'
$html = Get-Content $htmlPath -Raw
$pattern = '<!-- Logo Simulation -->.*?</div>'
$replacement = '<!-- Logo Image --><div style="margin-bottom:10px;"><img src="data:image/png;base64,' + $b64 + '" alt="Lunare Dental" style="max-height: 60px; max-width: 100%; display: inline-block;"></div>'
$html = [regex]::Replace($html, $pattern, $replacement, [System.Text.RegularExpressions.RegexOptions]::Singleline)
Set-Content $htmlPath -Value $html -Encoding utf8
Write-Host "Replaced successfully"
