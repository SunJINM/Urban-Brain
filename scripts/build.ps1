# Urban Brain 一键构建
#
# 用法：
#   .\scripts\build.ps1           只编译 C#（推荐先用这个）
#   .\scripts\build.ps1 -WithUI   同时构建 React 前端
#
# 建议先只编译 C#。C# 侧还没在真实游戏里验证过，
# 加上 UI 构建会多一层可能失败的地方，出错时不好判断问题出在哪一侧。

param([switch]$WithUI)

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

if ($WithUI) {
    Write-Host "（同时构建 UI 前端，需要 Node.js 18+）" -ForegroundColor Cyan
    dotnet build $proj -c Release -p:BuildUI=true
} else {
    dotnet build $proj -c Release
}

if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "[X] 编译失败。" -ForegroundColor Red
    Write-Host "    把上面所有红色的报错完整复制发回即可，不用自己判断哪条重要。" -ForegroundColor Yellow
    if ($WithUI) {
        Write-Host "    如果报错来自 npm / webpack，可以先去掉 -WithUI 只编译 C#。" -ForegroundColor Yellow
    }
    exit 1
}

Write-Host ""
Write-Host "[OK] 编译成功。启动游戏，在 Options 里找 Urban Brain。" -ForegroundColor Green
if (-not $WithUI) {
    Write-Host "[i] 本次没有构建 UI 面板，所有功能都可以在设置页操作。" -ForegroundColor Gray
}
