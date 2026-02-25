@echo off
chcp 65001 >nul
echo ========================================
echo   تثبيت فتاوى العراق على التلفون عبر USB
echo ========================================
echo.

echo التحقق من الأجهزة المتصلة...
adb devices
echo.
echo إذا لم يظهر جهاز تحت "List of devices" فالتلفون غير معروف.
echo تأكد من: وضع المطور + تصحيح USB + قبول "السماح بتصحيح USB" على الشاشة.
echo.

dotnet build -f net9.0-android -c Debug -t:Install

echo.
if %ERRORLEVEL% EQU 0 (
    echo تم التثبيت بنجاح. افتح التطبيق من التلفون.
) else (
    echo فشل التثبيت. تحقق من ربط التلفون وتفعيل تصحيح USB.
)
echo.
pause
