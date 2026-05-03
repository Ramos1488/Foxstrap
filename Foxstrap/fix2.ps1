$file = "Windows\SettingsWindow.xaml.cs"
$content = Get-Content $file -Raw

$content = $content -replace 'MakeLinkButton\("●", "\?  Репозиторий GitHub"', 'MakeLinkButton("⬡", "Репозиторий GitHub"'
$content = $content -replace 'MakeLinkButton\("●", "Помощь и справка"', 'MakeLinkButton("?", "Помощь и справка"'
$content = $content -replace 'private global::System\.Windows\.Controls\.Button MakeLinkButton\(string text, Action onClick\)', 'private global::System.Windows.Controls.Button MakeLinkButton(string icon, string text, Action onClick)'
$content = $content -replace 'Content = text,', 'Content = icon + "  " + text,'

Set-Content $file -Value $content -Encoding UTF8
Write-Host "Done!" -ForegroundColor Green
