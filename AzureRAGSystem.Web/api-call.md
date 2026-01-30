# API Call Analysis

Based on the project source code, this document outlines exactly **when** and **where** the API calls occur within the application flow.

## 1. Frontend UI Events (Blazor)
These calls are triggered by user interactions on the **Documents** page (`DocumentsComponent.razor`).

### **A. Page Load**
*   **Trigger:** When the component initializes (`OnInitializedAsync`).
*   **Action:** Calls `LoadDocuments()`, which fetches the list of files from the backend/search index.
*   **Code:**
    ```csharp
    indexedDocuments = await DocumentService.GetDocumentsAsync();
    ```

### **B. Uploading a File**
*   **Trigger:** Clicking the **"Upload File"** button (`<button ... @onclick="UploadFile">`).
*   **Action:** Calls `DocumentService.UploadDocumentAsync`. This triggers the `BlobService` to store the file and `SearchIndexerService` to index it.
*   **Code:**
    ```csharp
    var result = await DocumentService.UploadDocumentAsync(selectedFile, selectedCategory);
    ```

### **C. Deleting a Document**
*   **Trigger:** Clicking the **Trash/Delete** icon (`<button ... @onclick="() => DeleteDocument(doc.Id)">`).
*   **Action:** Calls `DocumentService.DeleteDocumentAsync`.
*   **Code:**
    ```csharp
    var success = await DocumentService.DeleteDocumentAsync(docId);
    ```

### **D. Previewing a Document**
*   **Trigger:** Clicking the **Eye/Preview** icon (`<button ... @onclick="() => OpenDocumentPreview(doc)">`).
*   **Action:** Calls `DocumentService.GetDocumentPreviewUrlAsync` (generates a SAS token via `BlobService`).
*   **Code:**
    ```csharp
    previewUrl = await DocumentService.GetDocumentPreviewUrlAsync(doc.FileName);
    ```

---

## 2. Chat & RAG Pipeline Events
These calls represent the "RAG" (Retrieval-Augmented Generation) pipeline, orchestrated between `ChatService.cs` (Client) and `RetrievalService.cs` (Server).

### **Step 1: User Sends a Message (Client-Side)**
*   **File:** `ChatService.cs`
*   **Method:** `AskQuestionAsync`
*   **Action:** Sends the message to the backend API.
*   **Code:**
    ```csharp
    _httpClient.PostAsJsonAsync("api/Chat/query", request)
    ```

### **Step 2: Backend Orchestration (Server-Side)**
`RetrievalService.cs` receives the request and runs the logic in `ProcessUserQueryAsync`. This triggers a chain of 3 distinct API calls:

#### **A. Retrieval (Search)**
*   **Goal:** Find relevant data in the knowledge base.
*   **File:** `SearchService.cs`
*   **Code:**
    ```csharp
    _searchClient.SearchAsync<SearchDocument>(...)
    ```

#### **B. Generation (AI)**
*   **Goal:** Send the user prompt + search results to Azure OpenAI (GPT-4o).
*   **File:** `AzureOpenAIService.cs`
*   **Code:**
    ```csharp
    chatClient.CompleteChatAsync(messages)
    ```

#### **C. Memory (Database)**
*   **Goal:** Save the conversation history to Cosmos DB.
*   **File:** `RetrievalService.cs`
*   **Code:**
    ```csharp
    _cosmosDbRepository.SaveSessionAsync(session)
    ```

---

## Summary

| Event Type | Trigger | Sequence |
| :--- | :--- | :--- |
| **UI Events** | User Interaction | Page Load, Upload, Delete, Preview |
| **Chat Events** | Message Submission | Search $\rightarrow$ OpenAI $\rightarrow$ Database |