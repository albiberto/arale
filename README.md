#AlertOwnerService

###Info
Il servizio presenta due job che:
1. notifica sul canale Slack i turni di oggi e domani, taggando il corrispettivo teammate di turno. Esso viene chiamato dal lunedi' al venerdi'.
2. si preoccupa di scrivere il calendario sullo sheet presente su drive tramite la SheetsAPI, il calendario viene pulito da turni in giorni feriali e nei weekend. Esso viene chiamato ogni primo del mese.

Il servizio interagisce con la SheetsApi di Google per:
1. Recuperare i compagni di team
2. Recuperare i patronati
3. Recuperare il calendario
4. Recuperare i turni di oggi e domani
5. Pulire il calendario
6. Scrivere il calendario

Documentazione, come creare un client per slack https://api.slack.com/messaging/webhooks.
Documentazione, come creare e condividere un foglio di stile trami google sheet api https://github.com/juampynr/google-spreadsheet-reader.
