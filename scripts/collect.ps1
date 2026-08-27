# 收集日志打包，方便回传
# 用法：.\scripts\collect.ps1

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
$userData = Join-Path $env:USERPROFILE "AppData\LocalLow\Colossal Order\Cities Skylines II"
$logsDir = Join-Path $userData "Logs"

$stamp = Get-Date -Format "yyyyMMdd-HHmmss"
$outDir = Join-Path $root "scratch\collect-$stamp"
New-Item -ItemType Directory -Force -Path $outDir | Out-Null

if (Test-Path $logsDir) {
    Copy-Item -Path (Join-Path $logsDir "*") -Destination $outDir -Recurse -Force -ErrorAction SilentlyContinue
    Write-Host "[OK] 已复制日志目录"
} else {
    Write-Host "[!] 找不到日志目录 $logsDir" -ForegroundColor Yellow
}

$playerLog = Join-Path $userData "Player.log"
if (Test-Path $playerLog) {
    Copy-Item $playerLog $outDir -Force
    Write-Host "[OK] 已复制 Player.log"
}

$zip = Join-Path $root "scratch\urbanbrain-logs-$stamp.zip"
Compress-Archive -Path (Join-Path $outDir "*") -DestinationPath $zip -Force
Remove-Item $outDir -Recurse -Force

Write-Host ""
Write-Host "[OK] 打包完成：$zip" -ForegroundColor Green
Write-Host "     把这个 zip 发回即可。"
