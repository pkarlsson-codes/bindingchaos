How to add a slice of functionality
Go through this list and add all your components in this order.
All entries are optional, they only help with the order to implement things in.

## Add Domain
1. Add Events
2. Add AggregateRoots, Entities, ValueObjects and Enumerations
3. Add ReadModels and Projections
4. Add Commands, Queries and DTOs

## Add Application
1. Add ServiceMethods
2. Add CommandHandlers
3. Add IntegrationEventMappers and EventHandlers

## Add API
1. Add Contracts
2. Add Endpoints
3. Add ApiClients

## Add Gateway
1. Add/Update ViewModels
2. Add Endpoint