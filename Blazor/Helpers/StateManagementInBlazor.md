# State Management i Blazor WebAssembly (WASM)

## Introduktion
Blazor WebAssembly (WASM) kører direkte i browseren og giver mulighed for at bygge rige, interaktive webapplikationer med C#. En vigtig del af enhver webapp er at holde styr på brugerens state – f.eks. om brugeren er logget ind, hvilke roller de har, og andre session-relaterede data.

## Måder at håndtere state på i Blazor WASM
Der findes flere måder at håndtere state på i Blazor WASM:

1. **In-memory state**
   - Data gemmes i services (f.eks. singleton services via Dependency Injection).
   - Fordel: Hurtigt og nemt.
   - Ulempe: State forsvinder, hvis brugeren opdaterer siden eller lukker browseren.

2. **localStorage/sessionStorage**
   - Data gemmes i browserens storage, så det overlever sideopdateringer og genstart af browseren (localStorage).
   - sessionStorage varer kun så længe fanen er åben.
   - Fordel: Simpelt, og data bevares mellem sessioner (localStorage).
   - Ulempe: Data er let tilgængeligt for brugeren og kan manipuleres.

3. **Server-side storage**
   - Data gemmes på serveren, og klienten får kun et token eller en session-id.
   - Fordel: Mere sikkert, da følsomme data ikke ligger i browseren.
   - Ulempe: Kræver server-side logik og ofte mere kompleksitet.

## Gemme state i localStorage
I Blazor WASM kan man bruge JavaScript interop til at gemme og hente data fra localStorage:

```csharp
// Gem data
await JS.InvokeVoidAsync("localStorage.setItem", "key", "value");
// Hent data
var value = await JS.InvokeAsync<string>("localStorage.getItem", "key");
```

Dette er hurtigt og nemt, men data ligger i klartekst i browseren.

## Valgfri AES-kryptering
For at beskytte følsomme data kan man vælge at kryptere det, før det gemmes i localStorage. Et eksempel er at bruge AES-kryptering via et JavaScript-bibliotek som CryptoJS:

```js
const encrypted = CryptoJS.AES.encrypt(value, key).toString();
localStorage.setItem(storageKey, encrypted);
// ...
const bytes = CryptoJS.AES.decrypt(encrypted, key);
const decrypted = bytes.toString(CryptoJS.enc.Utf8);
```

Dette gør det sværere for en angriber at læse data direkte, men **nøglen til krypteringen skal også ligge i browseren**, hvilket begrænser sikkerheden.

## Hvorfor er det svært at gøre ting sikre i en browser?
- **Alt kode og data på klientsiden kan læses og manipuleres**: Brugeren har fuld adgang til browserens hukommelse, localStorage, JavaScript-kode osv.
- **Krypteringsnøgler skal være tilgængelige for at dekryptere data**: Hvis du kan dekryptere i browseren, kan en angriber også.
- **Man-in-the-browser-angreb**: Hvis en bruger har malware eller ondsindede browser-udvidelser, kan de læse og ændre alt.

Derfor bør man aldrig stole på data, der kun ligger i browseren, og aldrig gemme hemmeligheder eller følsomme oplysninger på klientsiden.

## Hvorfor bruger vi JWT (JSON Web Token)?
JWT er en standard til at overføre information sikkert mellem klient og server:
- JWT er signeret (ofte med HMAC eller RSA), så serveren kan verificere, at data ikke er ændret.
- JWT kan indeholde claims (f.eks. bruger-id, roller), som serveren kan stole på, hvis signaturen er gyldig.
- JWT gemmes typisk i localStorage eller sessionStorage på klienten, men **følsomme data bør aldrig ligge i selve tokenet** – kun oplysninger, som ikke er hemmelige.
- JWT gør det muligt at have stateless authentication, hvor serveren ikke behøver at gemme sessioner.

**Bemærk:** JWT beskytter ikke mod tyveri af tokenet! Hvis en angriber får fat i JWT fra localStorage, kan de bruge det. Derfor er det vigtigt at beskytte mod XSS-angreb og overveje at bruge HttpOnly cookies, hvis muligt.

## Konklusion
- State i Blazor WASM kan gemmes i memory, localStorage eller på serveren.
- localStorage er praktisk, men ikke sikkert – kryptering kan bruges, men giver kun begrænset ekstra sikkerhed.
- Alt på klientsiden kan manipuleres, så følsomme data bør aldrig ligge i browseren.
- JWT bruges til at sikre, at data ikke kan manipuleres, men beskytter ikke mod tyveri af tokenet.

**Best practice:** Brug localStorage til ikke-følsomme data, og brug JWT til authentication/authorization, men vær opmærksom på sikkerhedsrisici i browseren. 