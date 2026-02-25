# نشر تطبيق فتاوى العراق كـ APK
# يشغّل: dotnet publish -f net9.0-android -c Release
# الملف الناتج: bin\Release\net9.0-android\*-Signed.apk

$ErrorActionPreference = "Stop"
$projectDir = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $projectDir

# التأكد من وجود debug.keystore (يُنشأ عادة مع Android SDK)
$keystore = Join-Path $env:USERPROFILE ".android\debug.keystore"
if (-not (Test-Path $keystore)) {
    Write-Host "تحذير: لم يُعثر على debug.keystore في $keystore"
    Write-Host "شغّل مرة واحدة بناء Android من Visual Studio أو: dotnet build -f net9.0-android"
    Write-Host "أو أنشئ keystore يدوياً ثم أعد تشغيل هذا السكربت."
    Write-Host ""
}

Write-Host "جاري نشر APK (Release)..." -ForegroundColor Cyan
dotnet publish -f net9.0-android -c Release

if ($LASTEXITCODE -eq 0) {
    $apkDir = Join-Path $projectDir "bin\Release\net9.0-android"
    $apk = Get-ChildItem -Path $apkDir -Filter "*-Signed.apk" -Recurse -ErrorAction SilentlyContinue | Select-Object -First 1
    if ($apk) {
        Write-Host ""
        Write-Host "تم النشر بنجاح." -ForegroundColor Green
        Write-Host "ملف الـ APK: $($apk.FullName)" -ForegroundColor Green
    } else {
        $apk = Get-ChildItem -Path $apkDir -Filter "*.apk" -Recurse -ErrorAction SilentlyContinue | Select-Object -First 1
        if ($apk) { Write-Host "ملف الـ APK: $($apk.FullName)" -ForegroundColor Green }
    }
} else {
    Write-Host "فشل النشر." -ForegroundColor Red
    exit 1
}
