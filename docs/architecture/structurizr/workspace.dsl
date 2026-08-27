workspace "Blast Planning Event Sourcing Reference" "Reference implementation demonstrating CQRS, event sourcing and asynchronous projections on Azure." {

    model {

        user = person "User" "Uses the Blast Planning application to create, approve and review blast plans."

        entra = softwareSystem "Microsoft Entra ID" "Provides authentication and issues access tokens."

        blastPlanning = softwareSystem "Blast Planning System" "Event-sourced blast planning reference application." {

            web = container "Blast Planning Web UI" "Provides the browser-based user interface for managing blast plans." "Angular"

            api = container "Blast Planning API" "Handles commands and queries for blast plans. Commands are persisted as domain events and queries are served from the read model." ".NET 10 / ASP.NET Core"

            projectionFunction = container "Projection Function" "Consumes committed domain events asynchronously and maintains the SQL read model." ".NET 10 / Azure Functions"

            eventStore = container "Event Store" "Stores the immutable event stream for blast plan aggregates." "Azure Cosmos DB"

            messageBus = container "Domain Event Bus" "Distributes committed domain events to asynchronous consumers using the domain-events topic and blast-plan-projections subscription." "Azure Service Bus"

            readModel = container "Blast Plan Read Model" "Stores query-optimised blast plan projections." "Azure SQL Database"
        }

        user -> web "Uses" "HTTPS"

        web -> entra "Authenticates with" "OAuth 2.0 / OpenID Connect"

        web -> api "Sends commands and queries to" "HTTPS / REST / JSON"

        api -> entra "Validates access tokens issued by"

        api -> eventStore "Appends and reads domain events" "Cosmos DB SDK"

        api -> messageBus "Publishes committed domain events" "Azure Service Bus"

        projectionFunction -> messageBus "Consumes domain events from blast-plan-projections subscription" "Azure Service Bus"

        projectionFunction -> readModel "Creates and updates projections" "SQL"

        api -> readModel "Queries blast plan projections" "SQL"
    }


    views {

        systemContext blastPlanning "SystemContext" {
            include *
            autoLayout lr
        }


        container blastPlanning "Containers" {
            include *
            autoLayout lr
        }


        dynamic blastPlanning "CreateBlastPlan" "Create a blast plan and asynchronously project it into the read model." {

            user -> web "1. Enter blast plan details"

            web -> api "2. POST blast plan"

            api -> eventStore "3. Append BlastPlanCreated event"

            api -> messageBus "4. Publish BlastPlanCreated event"

            projectionFunction -> messageBus "5. Receive BlastPlanCreated event"

            projectionFunction -> readModel "6. Create blast plan projection"

            autoLayout lr
        }


        dynamic blastPlanning "ApproveBlastPlan" "Approve a blast plan and asynchronously update its read model." {

            user -> web "1. Approve blast plan"

            web -> api "2. Send approve command"

            api -> eventStore "3. Load blast plan event stream"

            api -> eventStore "4. Append BlastPlanApproved event"

            api -> messageBus "5. Publish BlastPlanApproved event"

            projectionFunction -> messageBus "6. Receive BlastPlanApproved event"

            projectionFunction -> readModel "7. Update blast plan projection"

            autoLayout lr
        }


        dynamic blastPlanning "QueryBlastPlans" "Query blast plans from the SQL read model." {

            user -> web "1. Request blast plans"

            web -> api "2. GET blast plans"

            api -> readModel "3. Query blast plan projections"

            autoLayout lr
        }


        styles {

          element "Person" {
            shape person
          }

          element "External" {
            border dashed
          }

          element "System" {
            shape roundedBox
          }

          element "Web" {
            shape roundedBox
          }

          element "Gateway" {
            shape roundedBox
          }

          element "Service" {
            shape roundedBox
          }

          element "Database" {
            shape cylinder
          }

          element "Messaging" {
            shape pipe
          }

          element "Infra" {
            shape roundedBox
          }
        }

    }

    configuration {
        scope softwaresystem
    }
}