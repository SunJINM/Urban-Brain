# 城市大脑（Urban Brain）一键构建
#
# 用法：
#   .\scripts\build.ps1          编译 C# + 构建界面（默认，推荐）
#   .\scripts\build.ps1 -NoUI    只编译 C#，跳过界面
#
# 注意：车道连接、信号相位这些核心功能都要靠界面里的工具操作，
# 设置页只有开关和默认值。所以正常情况下不要加 -NoUI。
# 只有在排查「问题出在 C# 还是前端」时，才需要单独编译一侧。

param([switch]$NoUI)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
$proj = Join-Path $root "src\UrbanBrain.csproj"

Write-Host "=== 城市大脑 构建 ===" -ForegroundColor Cyan

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

if (-not $NoUI) {
    $node = Get-Command node -ErrorAction SilentlyContinue
    if (-not $node) {
        Write-Host "[X] 找不到 Node.js，无法构建界面。" -ForegroundColor Red
        Write-Host "    Toolchain 本应装好 Node 18+。若确认要跳过界面，用 -NoUI 参数。" -ForegroundColor Yellow
        exit 1
    }
    Write-Host "[OK] Node.js $(node --version)"
}

$modsPath = Join-Path $env:USERPROFILE "AppData\LocalLow\Colossal Order\Cities Skylines II\Mods"
Write-Host "[i] 编译产物会自动进入：$modsPath"

# --- 构建 ---
Write-Host ""
Write-Host "开始编译..." -ForegroundColor Cyan

if ($NoUI) {
    dotnet build $proj -c Release
} else {
    dotnet build $proj -c Release -p:BuildUI=true
}

if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "[X] 编译失败。" -ForegroundColor Red
    Write-Host "    把上面所有红色的报错完整复制发回即可，不用自己判断哪条重要。" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "    如果报错是「找不到类型或命名空间」（CS0246），" -ForegroundColor Gray
    Write-Host "    多半是 csproj 少了一个游戏程序集引用，见 docs/04-待验证清单.md 第 1.1 条。" -ForegroundColor Gray
    if (-not $NoUI) {
        Write-Host "    如果报错来自 npm / webpack，可以先用 -NoUI 只编译 C#，缩小范围。" -ForegroundColor Gray
    }
    exit 1
}

Write-Host ""
Write-Host "[OK] 编译成功。" -ForegroundColor Green
Write-Host "     启动游戏后，在 Options 里应能看到两个设置页：" -ForegroundColor Green
Write-Host "       城市大脑 · 车道与优先级" -ForegroundColor Green
Write-Host "       城市大脑 · 信号配时" -ForegroundColor Green
if ($NoUI) {
    Write-Host ""
    Write-Host "[!] 本次跳过了界面构建，路口工具会用不了（只有设置页可用）。" -ForegroundColor Yellow
}
