# Documentație Proiect - Gestiune Buget Personal (PursePalette)

## Descriere temă
Aplicație de gestiune a bugetului personal și a cheltuielilor zilnice.
Opțiuni disponibile pentru utilizator:
- Vizualizarea soldului curent și a tranzacțiilor recente.
- Adăugare tranzacție (Venit sau Cheltuială) cu opțiuni avansate (urgent, personal, recurent, esențial).
- Editare / ștergere tranzacție.
- Căutare tranzacții după descriere sau tip.
- Vizualizare grafic de activitate (venituri vs. cheltuieli) pe ultimele 7 zile.
- Efectuarea unui transfer rapid către o altă persoană.

## De ce ai ales tema?
Gestiunea finanțelor personale este o necesitate de bază în viața de zi cu zi. Am ales această temă deoarece o astfel de aplicație rezolvă o problemă reală: urmărirea banilor care intră și ies, ajutând utilizatorul să-și optimizeze cheltuielile. Din punct de vedere tehnic, tema este ideală pentru a exersa concepte esențiale în programare, precum manipularea fișierelor, filtrarea datelor, Data Binding-ul și crearea unei interfețe grafice intuitive (GUI).

---

# CUPRINS
**CAPITOLUL I**
Descriere proiect din punct de vedere al utilizatorului
**CAPITOLUL II**
Descriere proiect din punct de vedere al programatorului
**CAPITOLUL III**
Dezvoltări ulterioare

---

# CAPITOLUL I
## Descriere proiect din punct de vedere al utilizatorului

Interfața cu utilizatorul este împărțită în mai multe module (ecrane) principale, pentru a asigura o experiență de utilizare simplă și modernă.
Aplicația se deschide pe **Ecranul Principal (Acasă)**, care afișează:
- Soldul curent al contului, actualizat în timp real.
- O bară de căutare rapidă pentru a găsi tranzacții specifice.
- Un buton pentru a adăuga o tranzacție nouă.
- O zonă de acces rapid către "Transfer Rapid".
- Lista tranzacțiilor recente, fiecare având un buton de Editare și unul de Ștergere.

Dacă utilizatorul alege să **Adauge** sau să **Editeze** o tranzacție, aplicația afișează **Formularul de Tranzacție**. Aici se pot specifica:
- Tipul (Venit / Cheltuială).
- Suma și o scurtă descriere.
- Categoria (Mâncare, Transport, Salariu etc.) și Metoda de plată (Cash, Card, Transfer).
- Data tranzacției și opțiuni suplimentare (Urgent, Personal, Recurent, Esențial).

Dacă utilizatorul accesează meniul **Activitate**, i se prezintă un grafic vizual sub formă de coloane, care îi arată comparația dintre Venituri (culoare roz) și Cheltuieli (culoare maro închis) din ultimele 7 zile.

Dacă utilizatorul apasă pe **Trimite bani (transfer rapid)**, i se deschide un formular securizat unde trebuie să introducă datele destinatarului (telefon, nume, descriere și suma de transferat).

---

# CAPITOLUL II
## Descriere proiect din punct de vedere al programatorului

**● Diagrama de clase (Structură arhitecturală)**
Aplicația a fost gândită folosind separarea responsabilităților, având trei proiecte principale în soluție:
1. `BugetApp.Models` - Modelele de date.
2. `BugetApp.Persistence` - Logica de salvare (Nivelul de Stocare).
3. `BugetApp.WPF` - Interfața grafică și logica de business (Code-Behind).

**● Descrierea claselor**
1. **Tranzactie (Model):** Reprezintă structura de bază a unei tranzacții. Implementează `INotifyPropertyChanged` pentru a actualiza automat interfața grafică atunci când o proprietate (precum Suma sau Descrierea) este modificată.
2. **FileService (Persistență):** Externează logica de citire și scriere în fișiere, înlăturând necesitatea duplicării codului. Formatează datele tip CSV folosind separatorul `;`.
3. **MainWindow (View & Code-Behind):** Fereastra principală care gestionează logica. Funcționează ca propriul ei `DataContext` pentru a face legătura (Binding) între proprietățile definite în ea (ex: `SumaText`, `ChartData`) și XAML.
4. **DailyChartData:** O clasă de suport utilizată pentru a calcula și a desena corect dimensiunile coloanelor din graficul de activitate săptămânală.
5. Enum-urile **TipTranzactie** și **OptiuniTranzactie**: Simplifică operarea logică prin utilizarea flag-urilor (pe biți pentru opțiuni multiple).

**● Secțiuni de cod deosebite**
O funcționalitate deosebită o reprezintă mecanismul de filtrare și afișare folosind `ObservableCollection` și Data Binding. În loc ca programatorul să reîncarce manual lista de pe ecran la fiecare modificare, `MainWindow.xaml.cs` actualizează o colecție numită `FilteredTranzactii`. Componenta `ListBox` din XAML este "legată" (Bound) la această colecție, iar prin implementarea `INotifyPropertyChanged`, interfața vizuală se actualizează instantaneu atunci când o nouă tranzacție este adăugată sau când se aplică o căutare (prin funcția `ApplySearchFilter`).

O altă zonă notabilă este funcția `UpdateChart()`, care calculează proporțional înălțimea vizuală a barelor graficului în funcție de cheltuielile zilnice maxime, pentru a afișa un Chart dinamic din elemente vizuale de bază (Border).

---

# CAPITOLUL III
## Dezvoltări ulterioare

**● Funcționalități noi care ar fi necesare**
- **Autentificare utilizatori:** Crearea unui sistem de logare (Username/Parolă) cu posibilitatea de a gestiona conturi multiple, astfel încât o întreagă familie să-și poată gestiona separat finanțele în aceeași aplicație.
- **Export de Date:** Opțiunea de a exporta rapoartele lunare sub formă de fișiere PDF sau documente Excel pentru a ajuta la contabilitate.
- **Sistem de categorii personalizabil:** Posibilitatea ca utilizatorul să adauge sau să șteargă propriile sale categorii de cheltuieli direct din interfața aplicației, nu doar din cod.

**● Modificări de cod sau de tehnologii de implementare**
- **Migrarea la o Bază de Date Reală:** Înlocuirea clasei curente `FileService` (care folosește un fișier `.txt`) cu un pachet ORM precum Entity Framework Core. Datele ar urma să fie salvate într-o bază de date relațională (ex: SQL Server sau SQLite).
- **Arhitectură MVVM Strictă:** Acum, `MainWindow` conține atât UI-ul, cât și logica de prezentare (Code-Behind). Pentru o scalare mai mare, ar fi necesară decuplarea totală a view-urilor de logică folosind o ierarhie strictă cu clase de tip `ViewModel` (ex: `MainViewModel`, `AddTransactionViewModel`) și injectarea dependențelor (Dependency Injection).
