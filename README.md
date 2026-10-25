# Autonomiczny pojazd wyścigowy - Uczenie ze Wzmocnieniem w Unity ML-Agents

## Opis Projektu

Projekt dotyczy stworzenia i wytrenowania autonomicznego agenta kierującego pojazdem wyścigowym w trójwymiarowym środowisku symulacyjnym Unity. Trening odbywa się przy użyciu metod uczenia ze wzmocnieniem z wykorzystaniem pakietu Unity ML-Agents.

## 1. Cel główny
Stworzenie, wytrenowanie i weryfikacja autonomicznego agenta sterującego pojazdem w środowisku symulacyjnym Unity 3D z wykorzystaniem metod uczenia ze wzmocnieniem

### Cele szczegółowe
* **Budowa środowiska:** Przygotowanie modelu toru wyścigowego oraz fizyki pojazdu w Unity.
* **Sformułowanie problemu RL:** Określenie sposobu postrzegania otoczenia przez agenta, możliwych do podjęcia akcji oraz opracowanie funkcji nagród i kar sterującej procesem uczenia.
* **Trening i analiza:** Przeprowadzenie sesji uczenia agenta, obserwacja i analiza metryk postępu.
* **Ewaluacja i prezentacja:** Weryfikacja działania optymalnego modelu w czasie rzeczywistym oraz opracowanie raportu naukowego w LaTeX.

---

## 2. Zakres Projektu

### Wchodzi w zakres
* **Środowisko symulacyjne:**
  * Stworzenie trójwymiarowego toru wyścigowego wyposażonego w bandy oraz punkty kontrolne.
  * Przygotowanie podstawowego modelu pojazdu reagującego na polecenia sterujące.
* **Integracja z uczeniem ze wzmocnieniem:**
  * Dobór czujników otoczenia (np. czujniki odległości).
  * Zaprojektowanie funkcji nagrody wspierającej płynną i bezkolizyjną jazdę.
  * Konfiguracja i uruchomienie procesu uczenia w oparciu o bibliotekę Unity ML-Agents.
* **Eksperymenty i ocena:**
  * Przeprowadzenie próbnych sesji treningowych i monitorowanie wskaźników uczenia.
  * Dostrajanie parametrów procesu uczenia i funkcji nagrody w celu poprawy wyników.
  * Zapisanie i prezentacja zachowania wyuczonego modelu.
* **Dokumentacja i raportowanie:**
  * Cotygodniowe prezentacje stanu prac podczas zajęć.
  * Opracowanie pełnej dokumentacji projektowej w systemie LaTeX.

### Wyłączenia z zakresu
* Zaawansowany wygląd graficzny otoczenia i pojazdu.
* Skomplikowana symulacja fizyki pojazdu oraz otoczenia.

## 3. Harmonogram projektu
| Lp. | Zadanie | Planowany termin |
| :---: | :--- | :---: |
| **1.** | Opracowanie szczegółowego harmonogramu, celu i zakresu projektu. | 12.10 |
| **2.** | Przegląd materiałów źródłowych i tutoriali: rozeznanie w oficjalnej dokumentacji Unity ML-Agents, analiza wybranych poradników wideo/artykułów dotyczących implementacji autonomicznych pojazdów za pomocą RL. | 19.10 |
| **3.** | Instalacja i konfiguracja środowiska. | 26.10 |
| **4.** | Stworzenie modelu toru wyścigowego w Unity (geometria, zakręty, bandy, punkty kontrolne) oraz przygotowanie prefabrykatu samochodu (napęd, skręcanie, hamowanie). | 09.11 |
| **5.** | Implementacja skryptu agenta: definicja obserwacji, definicja akcji, definicja funkcji nagrody. | 16.11 |
| **6.** | Konfiguracja parametrów i środowiska treningowego. | 23.11 |
| **7.** | Przeprowadzenie pierwszych krótkich treningów i obserwacja zachowania agenta w TensorBoard. | 30.11 |
| **8.** | Diagnoza problemów i optymalizacja zachowania agenta. | 07.12 |
| **9.** | Testowanie modeli i porównywanie różnych wariantów uczenia. | 14.12 |
| **10.** | Przygotowanie ostatecznej wersji aplikacji z możliwością wizualizacji najlepszego modelu. | 21.12 |
| **11.** | Zebranie metryk i wykresów do raportu. | 04.01 |
| **12.** | Napisanie dokumentacji projektowej. | 11.01 |
| **13.** | Przedstawienie i obrona projektu. | 11.01 |
