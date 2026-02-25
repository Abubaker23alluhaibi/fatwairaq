# إعداد Codemagic لبناء فتاوى العراق

## 1) رفع المشروع على GitHub

- أنشئ ريبو جديد (مثلاً `fatawa-iraq-app`).
- **لا ترفع ملف الـ keystore** (تم إضافته في `.gitignore`).
- ارفع بقية الملفات وادفع `codemagic.yaml` معهم.

## 2) ربط Codemagic

1. ادخل [codemagic.io](https://codemagic.io) وسجّل دخول.
2. **Add application** → اختر GitHub وربط الريبو.
3. اختر الفرع (مثلاً `main`) واضغط **Check for configuration file** ليتعرّف على `codemagic.yaml`.

## 3) توقيع Android (مهم)

1. من Codemagic: **Team settings** → **Code signing identities** → **Android keystores**.
2. **Add keystore**:
   - **Reference name:** اكتب بالضبط: `fatwa_keystore`
   - **Keystore password:** Baker@123 (أو كلمة المرور اللي عندك)
   - **Key alias:** fatwa-key
   - **Key password:** Baker@123
   - ارفع ملف **fatwa-release.keystore** (من جهازك، من مجلد المشروع).
3. احفظ. هذا الاسم `fatwa_keystore` مستخدم في `codemagic.yaml`.

## 4) توقيع iOS (للبناء فقط أو للنشر)

- تحتاج حساب **Apple Developer** (مدفوع).
- من Codemagic: **Team settings** → **Code signing identities**:
  - **iOS certificates:** أضف **Apple Distribution** (رفع .p12 أو إنشاؤه عبر Codemagic).
  - **iOS provisioning profiles:** أضف **App Store** profile لـ `com.iraqifatawa.app`.
- أو اربط **App Store Connect API key** (من Apple) في **Team integrations** ثم استخدم "Fetch" للشهادات والملفات.

بعد ذلك workflow الـ **iOS Release** يبني الـ IPA ويُرسله لك بريد (artifacts).

## 5) تشغيل البناء

- من صفحة التطبيق في Codemagic اختر الـ workflow (**Android Release** أو **iOS Release**) واضغط **Start new build**.
- بعد النجاح حمّل الـ **AAB** (Android) أو **IPA** (iOS) من الـ Artifacts أو من الرابط اللي يوصلك بالإيميل.

## ملاحظات

- **Android:** الخرج AAB جاهز لرفعه على Google Play.
- **iOS:** الخرج IPA — ترفعه يدوياً عبر **Transporter** (من ويندوز أو ماك) إلى App Store Connect، أو تضيف في `codemagic.yaml` قسم `publishing: app_store_connect` لرفع تلقائي.
