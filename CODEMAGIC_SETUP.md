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

## 4) توقيع iOS — استخدام App Store Connect API Key (تلقائي)

بعد إضافة المفتاح في Codemagic (Team Keys / Individual Keys) باسم **Codemagic_Key**، تحتاج مجموعة متغيرات حتى يتم جلب الشهادات وملف التوزيع تلقائياً.

### مجموعة المتغيرات apple_auth (مطلوبة للـ fetch-signing-files)

1. في Codemagic: **Team settings** (أو من صفحة التطبيق **Environment variables**) → **Groups** → **Add group**.
2. اسم المجموعة: **apple_auth** (بالضبط).
3. أضف المتغيرات التالية (اختر **Secure** للـ Private key):

| الاسم | القيمة |
|--------|--------|
| `APP_STORE_CONNECT_ISSUER_ID` | `4fcac339-4ae4-4845-bd14-aec6f83163b5` |
| `APP_STORE_CONNECT_KEY_IDENTIFIER` | `4MP9RRU8DQ` |
| `APP_STORE_CONNECT_PRIVATE_KEY` | **محتوى ملف الـ .p8 بالكامل** (الملف اللي نزّلته من App Store Connect عند إنشاء المفتاح — افتحه بمحرر نصوص وانسخ كل المحتوى بما فيه بداية `-----BEGIN PRIVATE KEY-----` ونهاية `-----END PRIVATE KEY-----`) |

4. احفظ المجموعة وربطها بالتطبيق إذا كانت على مستوى الفريق.

**ملاحظة:** إذا لم يعد عندك ملف الـ .p8 (يُحمّل مرة واحدة من App Store Connect)، أنشئ مفتاح API جديد من App Store Connect → Users and Access → Integrations → App Store Connect API، ثم حمّل الـ .p8 واحفظ محتواه في المتغير `APP_STORE_CONNECT_PRIVATE_KEY`.

### ربط المفتاح في الـ workflow

في `codemagic.yaml` تم ضبط الآتي:
- **integrations: app_store_connect: Codemagic_Key** — اسم المفتاح اللي أضفته في Team Keys.
- **groups: apple_auth** — مجموعة المتغيرات أعلاه.
- سكربت **Fetch signing files** يجلب الشهادات وملف التوزيع لـ `com.iraqifatawa.app` تلقائياً (ويُنشئها إن لم تكن موجودة مع `--create`).
- **publishing.app_store_connect.auth: integration** — لاستخدام نفس المفتاح عند رفع الـ IPA لاحقاً.

بعد إنشاء مجموعة **apple_auth** وتشغيل البناء، يفترض أن يختفي خطأ "No matching profiles found".

---

## 4ب) توقيع iOS — الطريقة اليدوية (بدون API Key)

تحتاج حساب **Apple Developer** (مدفوع) ثم تنفيذ التالي.

### أ) إنشاء App ID في Apple Developer

1. ادخل [developer.apple.com](https://developer.apple.com) → **Account** → **Certificates, Identifiers & Profiles**.
2. من القائمة اليسرى: **Identifiers** → **+** (إضافة).
3. اختر **App IDs** → **Continue** → **App** → **Continue**.
4. **Description:** مثلاً `فتاوى العراق`.
5. **Bundle ID:** اختر **Explicit** واكتب بالضبط: `com.iraqifatawa.app`.
6. فعّل أي **Capabilities** تحتاجها (مثلاً إن احتجت Push لاحقاً)، ثم **Continue** → **Register**.

### ب) إنشاء شهادة توزيع (Distribution Certificate)

1. **Certificates** → **+** → اختر **Apple Distribution** → **Continue**.
2. اختر **Create a new certificate** واتبع الخطوات (يُطلب منك إنشاء طلب توقيع CSR من جهازك أو من Keychain على ماك).
3. حمّل الشهادة (.cer)، ثم على ماك افتحها وادخلها في **Keychain Access**.
4. من Keychain: انقر يمين على الشهادة → **Export** → احفظ كملف **.p12** مع كلمة مرور. هذا الملف تحتاجه في Codemagic.

(إذا عندك ماك وربطت **App Store Connect API key** في Codemagic، تقدر من Codemagic تضغط **Create certificate** وتنشئ الشهادة دون تصدير يدوي.)

### ج) إنشاء Provisioning Profile لـ App Store

1. **Profiles** → **+** → اختر **App Store Connect** (أو **App Store**) → **Continue**.
2. اختر **App ID:** `com.iraqifatawa.app` → **Continue**.
3. اختر **Apple Distribution** certificate اللي أنشأته → **Continue**.
4. **Profile name:** مثلاً `Fatawa Iraq App Store` → **Generate**.
5. **Download** الملف (امتداده `.mobileprovision`).

### د) رفع الشهادة والـ Profile في Codemagic

1. Codemagic → **Team settings** (أيقونة الترس) → **Code signing identities**.
2. **iOS certificates:**
   - **Add certificate** → ارفع ملف الـ **.p12** اللي صدّرته.
   - أدخل **كلمة مرور الـ .p12** و**Reference name** (مثلاً `fatwa_distribution`) → احفظ.
3. **iOS provisioning profiles:**
   - **Add profile** → ارفع ملف الـ **.mobileprovision** اللي نزّلته.
   - **Reference name:** أي اسم (مثلاً `fatwa_appstore`). المهم أن الـ **Bundle ID** داخل الملف يكون `com.iraqifatawa.app` ونوعه **App Store**.

بعد الرفع، Codemagic يطابق تلقائياً بين `bundle_identifier: com.iraqifatawa.app` و`distribution_type: app_store` في `codemagic.yaml` والملفات المرفوعة، ويختار الـ profile المناسب. عندها خطأ **No matching profiles found** يختفي.

### بديل: جلب الملفات تلقائياً من Apple

- في **Team integrations** أضف **App Store Connect API key** (من [App Store Connect → Users and Access → Integrations](https://appstoreconnect.apple.com/access/integrations)).
- من **Code signing identities** استخدم **Fetch** للشهادات و**Download selected** للـ provisioning profiles، واختر الـ profile اللي bundle ID فيه `com.iraqifatawa.app` وادفع له **Reference name**.

## 5) تشغيل البناء

- من صفحة التطبيق في Codemagic اختر الـ workflow (**Android Release** أو **iOS Release**) واضغط **Start new build**.
- بعد النجاح حمّل الـ **AAB** (Android) أو **IPA** (iOS) من الـ Artifacts أو من الرابط اللي يوصلك بالإيميل.

## ملاحظات

- **Android:** الخرج AAB جاهز لرفعه على Google Play.
- **iOS:** الخرج IPA — ترفعه يدوياً عبر **Transporter** (من ويندوز أو ماك) إلى App Store Connect، أو تضيف في `codemagic.yaml` قسم `publishing: app_store_connect` لرفع تلقائي.
