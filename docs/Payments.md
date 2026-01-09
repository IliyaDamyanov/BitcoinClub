# Payments

## Purpose
This project will use the Breez SDK (package `Breez.Sdk.Spark`) to support Lightning-based subscription payments.

The current integration only provides configuration and service registration. Payment flows will be implemented in later tasks.

## Configuration
Configuration is read from the `Breez` section.

Keys:
- `Breez:ApiKey` (required)
- `Breez:Environment` (optional, default `test`)
- `Breez:DefaultNodeUrl` (optional)

## Security considerations
- Do not commit real API keys to source control.
- Prefer using environment variables, user secrets, or a deployment secret store.
- Treat any Breez credentials and node configuration as sensitive.
- Avoid logging configuration values.
