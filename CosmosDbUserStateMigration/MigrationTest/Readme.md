# Bot Framework User State Storage Migration from V3 to V4


## Overview

This is an example of User State migration from **Bot Framework V3** CosmosDb storage to  **Microsoft Bot Framework V4** CosmosDb storage.

## Goals of this work

It is a non-trivial effort to retain User State across V3V4 migration. However, if you cannot afford to lose User State, here is one method you can use to migrate it.

## Projects in Sample

### Bot.Builder.Storage.Migration

This project is the code that connects to the v3 cosmos/documentdb, calls CosmosDbDocumentConverter.GetProperties for every record (which is expected to pull properties from the v3 BotData object), then converts the properties from v3 UserData serialization format to v4 format and saves those into v4 cosmosdb store.

### V4CosmosDbStateBot

This is an example project used to convert some v3 cosmosdb data to v4.  This is the code you will need to write.  This project will require knowledge of what was stored in v3, including the names of the properties used in the v3 bot codebase.  Also required to be present are the classes, within their proper namespaces, of the V4 bot.  The bulk of the work required for consuming this library is within the CosmosDbDocumentConverter.GetProperties delegate function.