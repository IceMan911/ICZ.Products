DB:
Mé řešení celkově obsahuje čtyři tabulky:
Category - číselník kategorií
Product - číselník produktů (je otázkou jestli sloupec Name měl mít unique)
Order - číselník objednávek
OrderItem - vazební tabulka, která vznikla mezi tabulkami Product a Order, protože je třeba si uvědomit, že objednávka může mít několik položek.
=> po návrhu DB jsem zjistil, že sloupec unit_price v OrderItem je zbytečný a mohl by se používat z tabulky Product.


Co se týče návrhu API:
Chybí mi v zadání API rozhraní pro tabulku Category (přidat, upravit, smazat)
Pro Objednávky by nebylo špatné vytvořit endpoint který dává uživateli lepší možnost filtrování podle sloupců nebo řazení dle jednotlivých sloupců.
Záleží jestli je požadavek aby BE řadil a filtroval záznamy nebo jestli to bude dělat FE v cachi.

Bohužel u navrženého řešení nevidím slabinu a moc by mě zajímalo jaké má?

