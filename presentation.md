<h1><p align=center> CERTIFICATI </p></h1>

----

## Cos'è un certificato

Un certificato è un file crittografato, generalmente binario, utilizzato per garantire l'autenticità di un soggetto

I formati più comuni sono ```  .der, .cer, .pem, .crt ```  i quali derivano dall' ASN.1

È formato da:
- Versione
- Numero di Serie
- Algoritmo di hashing
- Issuer
- Limiti di validità
- Distinguished Name (soggetto)
- Chiave pubblica
- Informazioni aggiuntive: Issuer OID, DN OID, Key Usage..
- Algoritmo utilizzato per la firma
- Firma
---
## Issuer

I certificati vengono validati dall' emanatore (issuer), solitamente una CA, ma è possibile siano autocertificati

Le CA sono gli enti che firmano i certificati. 

Sono un Trusted Third Party, ovvero i loro certificati sono considerati affidabili sia dal soggetto che li espone che da colui che li utilizza.

All'interno del protocollo HTTPS sono utilizzati per garantire una connessione sicura.

---
## Crittografia

Per lo hashing gli algoritmi più usati sono AES, Triple-DES e SHA-2

Per la firma ECDSA e RSA

---
## Distinguished Name

Il soggetto che utilizza il certificato è contrassegnato del distinguished name.
Esso è composoto da :
- Common Name (cn:  )
  E opzionali: 
- Organization (o:  )
- Organizational Unit (ou:  )
- Locality (l: )
- State (s: )
- Country (c: )
---
## Firma

Una digital signature è composta da:
- Algoritmo di generazione delle chiavi
- Algoritmo di firma
- Algoritmo di verifica delle chiavi
---
## Dove sono i certificati

In Windows si trovano unicamente in store dipendenti dal gestore dei certificati

I certificati non in questo percorso non vengono trovati automaticamente.

OPC UA inserisce i propri certificati in cartelle di default, per ricavare i certificati bisogna installarli e ridirigere i percorsi

---
## Public Key Infrastructure

Le CA sono una parte fondamentale della cosiddetta PKI.
All'interno della quale svolgono tre ruoli:
- Registrazione dei certificati
- Distribuzione di certificati e chiavi pubbliche
- Firma dei certificati
- Validazione
---
## Self-Signed Certificates

È possibile anche creare una struttura di autenticazione basata su certificati autofirmati
Per realizzarla ci sono fondamentalmente due vie:
- Installazione manuale di tutti i certificati ove necessario
- Contrattazione tra le parti
---
## Contrattazione tra le parti

Per assicurare l'autenticità dei soggetti si può ricorrera ad una contrattazione simile alla tipico comunicazione TCP. 

Si stabiliscono in concordato tutti termini necessari, come algoritmi da usare, dimensione della chiave, formati di codifica.

In alcune circostanze uno dei due può firmare il certificato dell'altro.  

---
## Struttura classica OPC

La classica applicazione OPC parte da un file .xml di configurazione in cui sono esposte le variabili globali che vengono caricate dalla libreria ogni qual volta necessario.

Anche per questo file vanno tenuti in considerazione i problemi visti in precedenza sulla locazione dei certificati.

Inoltre un certificato che si volesse usare per autenticazione potrebbe non essere comunque letto correttamente se caricato da percorso.

Generalmente un'applicazione possiede varie cartelle dove deposita i certificati di cui ha bisogno.

Le più importanti sono TrustedIssuers e AppicationCertificate, locati in UA Applications e MachineDefault. 

Tuttavia essendo directory da Windows non è possibile estrarvi i certificati.

È importante notare lo scollamento tra le strutture native .NET e le strutture costruite dalla libreria nell'ambito della gestione degli stessi

---
## Autenticazione tramite certificato in OPC

L'infrastruttura OPC UA permette l'utilizzo dei certificati per l'autenticazione client.

Essa permette sia il percorso PKI che il percorso self-signed

Per quest'ultimo è previsto che il server ottenga la chiave privata per verificare il certificato e il client possieda il certificato del server

Proprio qui possono giungere i problemi di convalida, in quanto venendo caricati da path essi non contengono la chiave privata, come giusto che sia

È possibile anche usare il sistema delle firme anche se difficile da attuare senza strumenti esterni

---
## Contestualizzazione

Questo tipo di autenticazione non si applica a livello di trasporto, ma a livello di applicazione.

Non è permesso l'accesso modale ai servizi OPC, bisogna accedervi tramite applicazione in una procedura codificata e automatizzata.

Questo talvolta genera problemi nella comprensione degli errori

Da lato codice questo utilizzo dei certificati è un UserIdentityToken, se vogliamo autenticarci, una UserTokenPolicy dall'altro lato.

---
## Configurazione server

È necessario il server supporti l'identità cui sopra e riceva la chiave privata

È possibile farlo direttamente dal PLC


---
## Security mode

All'interno di OPC è molto importante la funzione svolta dalla security mode. 

Essa descrive il livello di sicurezza delle comunicazioni tra server e client, è stabilito direttamente da OPC all'interno dei profiles. 

Impostarlo a #None prima dell'attivazione della sessione e lasciare che venga aggiornato dal server a connessione avvenuta evita problemi di fiducia solo se attivo l'AutoAcceptUntrusted  

Coerentenente con la struttura self-signed.

---













































