@echo off
chcp 65001 >nul
echo بناء تطبيق فتاوى العراق (APK)...
echo.

dotnet publish -f net9.0-android -c Debug -p:AndroidPackageFormat=apk

echo.
echo انتهى البناء.
echo ابحث عن الملف .apk في:
echo   bin\Debug\net9.0-android\publish\
echo.
pause
