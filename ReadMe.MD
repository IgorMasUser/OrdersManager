Název projektu: OrdersManager

Popis projektu
OrdersManager je REST API aplikace napsaná v C# a postavená na platformě .NET 8.0, která slouží k správě objednávek. Umožňuje vytváření, zobrazování a aktualizaci objednávek, včetně možnosti přijímat informace o zaplacení objednávky. Projekt využívá Entity Framework Core pro správu databáze, RabbitMQ jako message broker a MassTransit pro správu stavových strojů.

Funkcionalita

Aplikace nabízí následující funkce:
Vytvoření objednávky: API umožňuje vytvořit novou objednávku s unikátním číslem objednávky, jménem zákazníka nebo názvem firmy, datem vytvoření a stavem objednávky.
Výpis všech objednávek: API umožňuje získat seznam všech objednávek uložených v databázi.
Přijetí informace o zaplacení objednávky: API umožňuje změnit stav objednávky na "Zaplacena" nebo "Zrušena" na základě přijetí informace o zaplacení.
Smazání všech objednávek: Metoda DeleteAllOrders() byla přidána pouze pro účely testování.

Struktura objednávky

Objednávka obsahuje:
Číslo objednávky: Unikátní identifikátor objednávky.
Jméno zákazníka nebo název firmy: Identifikační informace o zákazníkovi.
Datum vytvoření objednávky: Datum a čas vytvoření objednávky.
Stav objednávky: Stav objednávky, který může být "Nová", "Zaplacena" nebo "Zrušena".
Položky objednávky: Seznam položek objednávky, kde každá položka obsahuje název zboží, počet kusů a cenu za jeden kus.

Technické požadavky
.NET 8.0
Entity Framework Core
RabbitMQ
MassTransit
SQL Server

Řešení stavů objednávek
Pro zpracování stavů objednávek je použit stavový stroj z knihovny MassTransit. Tento stroj umožňuje přechody mezi stavy objednávky, jako jsou "Nová", "Zaplacena" a "Zrušena", na základě událostí, které jsou publikovány do RabbitMQ.

Validace čísla zákazníka
Consumer, který zpracovává událost SubmitOrder, obsahuje kontrolu na platnost čísla zákazníka. Pokud číslo zákazníka obsahuje jiné znaky než číslice, systém vrátí chybu s vysvětlením, že číslo zákazníka musí obsahovat pouze číslice. Tento krok je nezbytný pro zajištění integrity dat.

Omezení
Z důvodu nedostatku času nebyl vyřešen problém s relacemi entit. I když byly explicitně nastaveny relace mezi entitami v metodě OnModelCreating pro Entity Framework Core, nebylo možné úspěšně zahrnout OrderState do Order. Tento problém zůstává k dalšímu řešení.
Pokud bych měl více času, zahrnul bych také následující vylepšení:

Budoucí vylepšení
Pokud bych měl více času, zahrnul bych také následující vylepšení:
Validace dat: Přidání validací na úrovni frontend, databáze a modelů entit.
Popis entit: Zahrnutí podrobného popisu entit.
Jednotkové testy: Přidání jednotkových a integračních testů.


Klonování repozitáře: https://github.com/IgorMasUser/OrdersManager