# UnreflectedSerialize

## Zadání
Bylo zadáno přidádní jednoduchého serializeru pro základní data, ale bez použití reflection api

## Řešení
Bez reflection api udělat obecné řešení je velmi těžké, ale abych uživateli i tak poskytl co nejmenší starost o to, co se vypisuje a on aby se jen staral o to v jakém formátu jsou jeho data, tak jsem řešení upravil tak, že uživatel jen popíše strukturu dat a jaká tam jsou data... To co se vytiskne je defakto skryté čistě v rootDescriptoru. Takže se může jednoduše předělat na vytisknutí XML (jak je teď) nebo na vytisknutí do JSONu
