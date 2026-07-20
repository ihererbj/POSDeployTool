# Sprint 6 - Enterprise Foundation

## Included

- Centralized `AppServices` service container. UI no longer creates service instances directly.
- Persistent production settings at `Data/appsettings.json`.
- Daily application logs in `Logs/POS_Deploy_Tool_yyyyMMdd.log`.
- Immutable JSON Lines audit trail in `Logs/Audit/audit_yyyyMMdd.jsonl`.
- Global UI, AppDomain and unobserved Task exception handling.
- Configurable log retention and automatic cleanup.
- Centralized application paths through `AppPaths`.
- Atomic settings save with `.bak` backup.

## First run

The program automatically creates:

- `Data/appsettings.json`
- `Logs/`
- `Logs/Audit/`

Review `Data/appsettings.json` before Production deployment, especially:

- `EnvironmentName`
- `MaxParallelism`
- timeouts
- retry count
- retention days

## Production recommendation

Place the application in a folder where the operator has Modify permission, or move Data/Logs to a controlled writable path in the next sprint. Do not store plaintext production passwords in `stores.json`; credential protection is scheduled for the next security sprint.
