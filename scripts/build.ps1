# Urban Brain 一键构建
# 用法：右键 -> 使用 PowerShell 运行，或在终端里执行  .\scripts\build.ps1

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
$proj = Join-Path $root "src\UrbanBrain.csproj"

Write-Host "=== Urban Brain 构建 ===" -ForegroundColor Cyan

# --- 环境检查 ---
$toolPath = [System.Environment]::GetEnvironmentVariable("CSII_TOOLPATH", "User")
if (-not $toolPath) {
    Write-Host "[X] 找不到环境变量 CSII_TOOLPATH" -ForegroundColor Red
    Write-Host "    说明游戏的 Modding Toolchain 没装好。" -ForegroundColor Yellow
    Write-Host "    请进游戏 -> Options -> Modding，把所有依赖项装到显示绿勾，然后重启电脑。" -ForegroundColor Yellow
    exit 1
}
Write-Host "[OK] CSII_TOOLPATH = $toolPath"

foreach ($f in @("Mod.props", "Mod.targets")) {
    $p = Join-Path $toolPath $f
    if (-not (Test-Path $p)) {
        Write-Host "[X] 缺少 $p" -ForegroundColor Red
        Write-Host "    Toolchain 装得不完整，请在游戏 Options -> Modding 里点修复。" -ForegroundColor Yellow
        exit 1
    }
}
Write-Host "[OK] Mod.props / Mod.targets 都在"

$modsPath = Join-Path $env:USERPROFILE "AppData\LocalLow\Colossal Order\Cities Skylines II\Mods"
Write-Host "[i] 编译产物会自动进入：$modsPath"

# --- 构建 ---
Write-Host ""
Write-Host "开始编译..." -ForegroundColor Cyan
dotnet build $proj -c Release

if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "[X] 编译失败。" -ForegroundColor Red
    Write-Host "    把上面所有红色的报错完整复制发回即可，不用自己判断哪条重要。" -ForegroundColor Yellow
    exit 1
}

Write-Host ""
Write-Host "[OK] 编译成功。启动游戏，在 Options 里找 Urban Brain。" -ForegroundColor Green
