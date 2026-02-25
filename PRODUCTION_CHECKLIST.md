# قائمة جاهزية الإنتاج — فتاوى العراق

## نتيجة الفحص التقني

من ناحية **الكود والإعدادات الحالية** التطبيق قريب من الجاهزية، لكن **لا يصلح للنشر كما هو** بدون تنفيذ الخطوات التالية.

---

## ما هو جاهز تقنياً

| البند | الحالة |
|-------|--------|
| معرف التطبيق (ApplicationId) | ✅ `com.iraqifatawa.app` |
| رقم الإصدار (1.0 / versionCode 1) | ✅ |
| الاسم المعروض (فتاوى العراق) | ✅ |
| صلاحيات Android (إنترنت + حالة الشبكة) | ✅ |
| هدف Android (targetSdk 35، minSdk 21) | ✅ |
| API (https://fatwairaq.com) | ✅ جاهز للإنتاج |
| تسجيل Debug فقط في Release | ✅ (MauiProgram) |
| أيقونة وشاشة بداية (splash) | ✅ |
| دعم RTL والعربية | ✅ |

---

## ما يجب فعله قبل النشر

### 1. Android — توقيع الإنتاج (إلزامي)

**المشكلة:** المشروع يستخدم حالياً **مفتاح التطوير (debug.keystore)**. Google Play **لا يقبل** تطبيقات موقعّة بمفتاح debug للنشر العام.

**ما تفعله:**

1. **إنشاء مفتاح إصدار (keystore) مرة واحدة فقط:**
   ```bash
   keytool -genkey -v -keystore fatwa-release.keystore -alias fatwa-key -keyalg RSA -keysize 2048 -validity 10000
   ```
   احفظ **كلمة مرور المستودع** و**كلمة مرور المفتاح** و**الـ alias** في مكان آمن. من يفقد المفتاح لا يستطيع تحديث التطبيق على المتجر.

2. **تعيين متغيرات البيئة قبل بناء Release (مثال في PowerShell):**
   ```powershell
   $env:ANDROID_KEYSTORE_PATH = "C:\path\to\fatwa-release.keystore"
   $env:ANDROID_KEYSTORE_PASSWORD = "كلمة_مرور_المستودع"
   $env:ANDROID_KEY_ALIAS = "fatwa-key"
   $env:ANDROID_KEY_PASSWORD = "كلمة_مرور_المفتاح"
   ```

3. **بناء حزمة الإنتاج (AAB) لـ Google Play:**
   ```bash
   dotnet build -f net9.0-android -c Release
   ```
   الملف الجاهز للرفع يكون في مسار مثل:
   `bin\Release\net9.0-android\com.iraqifatawa.app-Signed.aab`

---

### 2. سياسة الخصوصية (إلزامي للمتجرين)

- **Google Play و App Store** يطلبون رابط **سياسة خصوصية** عامة.
- يُفضّل أن تكون الصفحة على نطاقك (مثلاً: `https://fatwairaq.com/privacy`).
- أضف الرابط في:
  - إعدادات التطبيق (صفحة الإعدادات) حتى يراه المستخدم من داخل التطبيق.
  - في لوحة المطور في Google Play (سياسة الخصوصية).
  - في App Store Connect (سياسة الخصوصية).

إذا رغبت، يمكن إضافة زر "سياسة الخصوصية" في `SettingsPage` يفتح هذا الرابط.

---

### 3. Google Play — إعدادات الحساب والقائمة

- إنشاء تطبيق جديد في [Google Play Console](https://play.google.com/console).
- اختيار **تسعير وتوزيع** (مجاني/مدفوع والبلدان).
- إكمال **لوحة المحتوى** (التصنيف العمري، استبيان الأخبار، إن وجد).
- رفع **AAB** من الخطوة 1 في "الإصدارات" (Production أو Internal testing أولاً).
- إضافة **نص الوصف القصير والطويل** و**لقطات شاشة** (حسب المتطلبات).
- **أيقونة 512×512** للمتجر إن طُلبت.

---

### 4. App Store (iOS) — ملاحظة مهمة

- المشروع يدعم **iOS** عند البناء من **macOS** فقط (الـ TargetFrameworks تشمل `net9.0-ios` عند عدم Windows).
- على **Windows** لا يمكن بناء IPA لتسليمه لـ App Store من نفس الجهاز.
- للنشر على App Store تحتاج أحد الخيارات:
  - بناء المشروع على **Mac** (Visual Studio for Mac أو `dotnet build -f net9.0-ios -c Release`).
  - أو استخدام **CI/CD** على جهاز Mac (مثل GitHub Actions مع runner بنظام macOS، أو Azure DevOps مع وكيل Mac).

بعد الحصول على IPA:
- إنشاء التطبيق في **App Store Connect**.
- رفع IPA عبر **Transporter** أو Xcode.
- إكمال الميتادات (وصف، لقطات شاشة، سياسة خصوصية، التصنيف العمري).

---

### 5. تحديث الإصدار لاحقاً

عند كل إصدار جديد:

- في `MyMauiApp.csproj`:
  - `ApplicationDisplayVersion` (مثلاً 1.1 للمستخدم).
  - `ApplicationVersion` (رقم صحيح، مثلاً 2 لـ versionCode على Android و CFBundleVersion على iOS).
- إعادة بناء AAB (Android) و/أو IPA (iOS) ورفعها للمتجر.

---

## ملخص سريع — ماذا تفعل الآن

1. **إنشاء keystore** للإنتاج وحفظ كلمات المرور والـ alias.
2. **تعيين متغيرات البيئة** للتوقيع ثم **بناء AAB** بـ `-c Release`.
3. **نشر صفحة سياسة خصوصية** وإضافتها في التطبيق وفي قوائم المتجر.
4. **إكمال قائمة التطبيق في Google Play** (الوصف، اللقطات، الأيقونة 512 إن لزم).
5. إذا أردت **App Store**: البناء من Mac أو عبر CI على Mac، ثم إكمال App Store Connect ورفع IPA.

بعد تنفيذ هذه النقاط يكون التطبيق من ناحية التقنية والمتطلبات جاهزاً للنشر على Google Play، وعلى App Store بعد إعداد البناء من Mac وإكمال البيانات في App Store Connect.
