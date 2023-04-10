# csutil

> Some more or less useful C# utilities i created for/at work

### [Backup](csutil/Backup.cs)

A backup utility that can be used to backup files.
Configurable with a path and a time to delete older backups.

### [Encryption](csutil/Encryption.cs)

An encryption utility that can be used to encrypt/decrypt files.
Using AES encryption with a 256 bit key and a 128 bit IV or something similar.

### [HttpServer](csutil/HttpServer.cs)

A dependencyless HTTP server that can be used to handle different HTTP request.
Basically a wrapper around the `System.Net.HttpListener` namespace with some useful methods/extensions.

### [Logger](csutil/Logger.cs)

A basic and configurable logging utility that can be used to log messages to files or directly to the console.

### [Mailer](csutil/Mailer.cs)

A wrapper around the `System.Net.Mail` namespace that can be used to send emails.
Probably not that useful in it's current state due to the passwordless usage.

### [Settings](csutil/Settings.cs)

A simple settings class that can be used to store/load settings in/from a file.
Can be used in combination with the `Encryption` utility to encrypt the settings file.

### [RowComp](csutil/RowComp/RowComparer.cs)

A utility used to compare two different CSV (or similar) files and output the differences (additions/deletions/changes).
Can probably be adapted to compare other file types as well by changing the parsing (aka `GetSingleData`/`FromString`).