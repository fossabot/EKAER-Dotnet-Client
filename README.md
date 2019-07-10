[![GitHub](https://img.shields.io/github/license/Pethical/EKAER-Dotnet-Client.svg?style=popout)](LICENSE)
[![Build Status](https://travis-ci.org/Pethical/EKAER-Dotnet-Client.svg?branch=master)](https://travis-ci.org/Pethical/EKAER-Dotnet-Client)
[![CodeFactor](https://www.codefactor.io/repository/github/pethical/ekaer-dotnet-client/badge)](https://www.codefactor.io/repository/github/pethical/ekaer-dotnet-client)
![netstandard 2.0](https://img.shields.io/badge/netstandard-2.0-blue.svg)
[![GitHub release](https://img.shields.io/github/release/Pethical/EKAER-Dotnet-Client.svg?style=popout)](releases)
[![Nuget](https://img.shields.io/nuget/vpre/EKAER.Client.svg?style=popout)](https://www.nuget.org/packages/EKAER.Client)

[<img src="https://raw.githubusercontent.com/hjnilsson/country-flags/master/png250px/gb.png" width="32" />](#preamble-and-warnings)
[<img src="https://raw.githubusercontent.com/hjnilsson/country-flags/master/png250px/hu.png" width="32" />](#fontos-figyelmezetések)

## Fontos figyelmezetés

Ez nem egy hivatalos programkönyvtár. Sem az EKAER support, sem a Nemzeti Adó és Vámhivatal nem tud segítséget nyújtani a használatában, mert a fejlesztése tőlük függetlenül történt. Amennyiben bármilyen hibát tapasztalsz, vagy kérdés merül fel ezzel a kliens könyvtárral kapcsolatban, kérlek jelezd itt GitHub-on!

**Figyelmesen olvasd el és értsd meg a [felhasználási feltételeket](LICENSE) mielőtt elkezded a használatot.**

A MIT licensz egy megengedő licensz, azonban **mindennemű felelősséget kizár a szoftver miatt keletkező károk tekintetében**.

**THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.**

## Mi ez és mire való?
Ez egy nem hivatalos .NET standard kliens könyvtár az EKAER rendszer bejelentés kezelő XML interfészéhez. A használata megkönnyítheti számunkra az adatok továbbítását és megóvhat minket pár alapvető hibától. Netstandard 2.0 kompatibilis, platform független megoldás amely windowsra és linuxra is fordítható.

**A segítségével:**

* Létrehozhatsz bejelentéseket
* Lekérdezheted a bejelentéseidet
* Módosíthatod a bejelentéseket
* Törölheted a bejelentéseket
* Lezárhatod a bejelentéseket

Több információért látogasd meg a hivatalos weboldalt a https://ekaer.nav.gov.hu/ és nézd meg a hivatalos dokumentációt a https://ekaer.nav.gov.hu/faq/?page_id=9 címen.

## Hogyan használd?
A használatához először mindenképp át kell tanulmányozni az EKAER hivatalos dokumentációit és regisztrálnunk kell a rendszerben. Ezután adjuk hozzá referenciaként a projektet a saját megoldásunkhoz és az ApiClient osztályon keresztül hívhatjuk az interfészt.

### Bejelentés lekérdezése TCN alapján
```csharp
var client = new ApiClient(apiUser, apiPassword, VATNumber, apiSecret);
var tradeCard = client.QueryTradeCard("TCN NUMBER");
```
### Bejelentések lekérdezése
```csharp
// Az elmúlt 7 nap bejelentéseinek lekérdezése
var tradeCards = client.QueryTradeCard(new QueryParams() { InsertFromDate = DateTime.Now.Subtract(TimeSpan.FromDays(7)), InsertToDate = DateTime.Now });
```

### Bejelentés módosítása
```csharp
tradeCard = client.ModifyTradeCard(tradeCard);
```

### Bejelentés lezárása
```csharp
client.FinalizeTradeCard(tradeCard.Tcn);
```

### Bejelentés létrehozása 
```csharp
client.CreateTradeCard(tradeCard);
```
### Bejelentés törlése
```csharp
client.DeleteTradeCard(tradeCard.Tcn, "A törlés indoka");
```
### Hibakezelés
Hiba esetén `EKAERException` kivétél keletkezik amelyből ki tudjuk nyerni a hiba részleteit.
```csharp
try
{
    var finalized = client.FinalizeTradeCard(tradeCard.Tcn);
}
catch (EKAERException e)
{
    Console.WriteLine(e.Result.ReasonCode);
    Console.WriteLine(e.Result.FuncCode);
    Console.WriteLine(e.Message);
}
```
### Csoportos műveletek

A legtöbb metódus két verzióban is létezik és képes egy, vagy több bejelentésen is műveletet végezni, pl. egyszerre le tudunk zárni, vagy tudunk törölni több bejelentést is.

## Hogyan tudsz segíteni?
Minden segítségnek örülök, legyen az kód, dokumentáció, teszt, vagy bármi ami megkönnyíti a használatot. Ha szeretnél segíteni a fejlesztésben ne habozz és küldj pull requestet!

---

## Preamble and warnings

**PLEASE NOTE**: this is not an official client, and not supported by the EKAER or by the National Tax and Customs Administration of Hungary. 

**Please read and understand the [license](LICENSE) before you start to use it!**

**THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.**

## What is it?
This is an unofficial .NET standard client library for the hungarian ekaer API interface. With this library you can:
* Create trade cards
* List your trade cards
* Modify your trade cards
* Delete your trade cards
* Finalize your trade cards

For more information see the official website at https://ekaer.nav.gov.hu/ and the official API documentation at: https://ekaer.nav.gov.hu/faq/?page_id=9

## How to use?
Basically you must understand the official documentation, then add this project as reference.

### Query tradecard 
```csharp
var client = new ApiClient(apiUser, apiPassword, VATNumber, apiSecret);
var tradeCard = client.QueryTradeCard("TCN NUMBER");
```
### Modify tradecard
```csharp
tradeCard = client.ModifyTradeCard(tradeCard);
```

### Finalize tradecard
```csharp
client.FinalizeTradeCard(tradeCard.Tcn);
```

### Create tradecard 
```csharp
client.CreateTradeCard(tradeCard);
```
### Delete tradecard
```csharp
client.DeleteTradeCard(tradeCard.Tcn, "Reason text");
```
### Error handling
When something goes wrong, you can catch the EKAERException exception for error details.

```csharp
try
{
    var finalized = client.FinalizeTradeCard(tradeCard.Tcn);
}
catch (EKAERException e)
{
    Console.WriteLine(e.Result.ReasonCode);
    Console.WriteLine(e.Result.FuncCode);
    Console.WriteLine(e.Message);
}
```
## Contributing
Contributors are welcome! Don't hesitate if you have any idea!
