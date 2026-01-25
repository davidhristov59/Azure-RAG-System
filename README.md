# AzureRAGSystem

## Overview
Azure-RAG-System is a professional .NET 8 web application that implements a Retrieval-Augmented Generation (RAG) architecture. It leverages Azure's robust AI ecosystem to provide intelligent, context-aware responses to user queries. By combining Azure AI Search for precise document retrieval and Azure OpenAI for natural language generation, the system delivers accurate answers grounded in your proprietary data.

## Key Features
- **Retrieval-Augmented Generation (RAG)**: Orchestrates the flow between user queries, vector search, and LLM generation.
- **Azure AI Search**: Implements vector search and semantic ranking to retrieve relevant context.
- **Azure OpenAI Integration**: Generates human-like responses using advanced models (e.g., GPT-4o).
- **Blazor Interactive Server UI**: A modern, responsive interface built with Blazor for real-time user interaction.
- **Document Processing**: Ingests and vectorizes documents stored in **Azure Blob Storage**.
- **Session Management**: Persists chat history and user sessions using **Azure Cosmos DB**.

## Data Ingestion & Indexing
The system uses **Azure AI Search Indexers** to automate data ingestion from Azure Blob Storage.
- **Indexer**: An indexer (`is-docs-indexer`) connects to the Blob Storage data source and periodically (or on-demand) crawls for new or updated documents.
- **Vectorization**: During indexing, documents are cracked, and content is vectorized to enable semantic search.
- **Management**: The `SearchIndexerService` allows for manual triggering of the indexer, checking its status, and removing documents from the index when they are deleted from storage.

## Architecture
The solution follows a Clean Architecture pattern to ensure scalability and maintainability:
- **Web App**: The entry point hosting the Blazor UI and API endpoints.
- **Service Layer**: Encapsulates business logic, including the RAG orchestration and AI interactions.
- **Infrastructure Layer**: Manages communication with external Azure services (OpenAI, Search, Cosmos DB, Blob Storage).
- **Domain Layer**: Defines core entities and interfaces used across the system.
- **Repository Layer**: Handles data access and persistence patterns.

## Project Structure
The solution is organized into the following projects:
- `AzureRAGSystem.Web`: The main executable and host for the application.
- `AzureRAGSystem.UI`: Contains Blazor components, pages, and UI logic.
- `AzureRAGSystem.Infrastructure`: Implements interfaces for Azure services.
- `AzureRAGSystem.Service`: Contains the core business logic and service implementations.
- `AzureRAGSystem.Repository`: Manages database operations.
- `AzureRAGSystem.Domain`: Holds domain models and shared abstractions.

## Prerequisites
To run this application locally, ensure you have the following:
- **.NET SDK 8.0**
- **Azure Subscription** with these resources deployed:
    - Azure OpenAI Service
    - Azure AI Search
    - Azure Cosmos DB (NoSQL)
    - Azure Blob Storage

## Environment Variables
Configure the following settings in `appsettings.json` or via environment variables.

| Section | Key | Description |
| :--- | :--- | :--- |
| **AzureOpenAI** | `Endpoint` | The endpoint URL for your Azure OpenAI resource. |
| | `ApiKey` | The API key for authentication. |
| | `DeploymentName` | The model deployment name (e.g., gpt-4o). |
| **AzureSearch** | `Endpoint` | The URL of your Azure AI Search service. |
| | `ApiKey` | The admin key for the search service. |
| | `IndexName` | The name of the search index (e.g., azure-rag-index). |
| **CosmosDb** | `Endpoint` | The URI for your Cosmos DB account. |
| | `Key` | The primary key for Cosmos DB. |
| | `DatabaseName` | The name of the database. |
| | `ContainerName` | The name of the container for chat sessions. |
| **ConnectionStrings** | `AzureBlobStorage` | Connection string for Azure Blob Storage. |

## Installation & Running
1.  **Clone the repository**
    ```bash
    git clone https://github.com/your-username/AzureRAGSystem.git
    cd AzureRAGSystem
    ```

2.  **Restore dependencies**
    ```bash
    dotnet restore
    ```

3.  **Configure settings**
    Update `AzureRAGSystem.Web/appsettings.json` with your Azure credentials.

4.  **Run the application**
    ```bash
    cd AzureRAGSystem.Web
    dotnet watch run 
    
    cd ..
    cd AzureRAGSystem.UI
    dotnet watch run 
    ```

## Deployment
This project utilizes **GitHub Actions** for Continuous Integration and Continuous Deployment (CI/CD). The workflow is configured to:
1.  Build and test the .NET application.
2.  Publish the artifact.
3.  Deploy the application to an **Azure App Service**.

Ensure that your GitHub repository secrets are configured with the necessary Azure publish profile or service principal credentials.
