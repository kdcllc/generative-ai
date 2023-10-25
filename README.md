# Generative AI

Generative AI

## Usage

This sample requires either OpenAI endpoint or Azure OpenAI endpoint to run.


```bash
    # to run from the root of the solution
    dotnet run --project  dotnet/chatapp/ChatApp.csproj --name BasicDemo
     dotnet run --project  dotnet/chatapp/ChatApp.csproj --name DateTimeDemo
```

```bash
    export DOTNET_ROLL_FORWARD=LatestMajor
    export DOTNET_HOST_PATH=/home/kdcll/.dotnet/dotnet
```

Each request/ response between in the chat consumes tokens that (Azure) OpenAI charges and in addition models have context token limitations.

Context of the chat must contain only relevant information in form of Embeddings -> to specific storage of the text -> Embeddings


## References

- [semantic kernel](https://github.com/microsoft/semantic-kernel)
- [semantic memory](https://github.com/microsoft/semantic-memory)
- [semantic kernel recipes](https://github.com/johnmaeda/SK-Recipes)
- [Semantic Kernel Starters](https://github.com/microsoft/semantic-kernel-starters)
- [Demystifying Retrieval Augmented Generation with .NET](https://devblogs.microsoft.com/dotnet/demystifying-retrieval-augmented-generation-with-dotnet/)
- [chat copilot app](https://github.com/microsoft/chat-copilot)
- [Qdrant Vector Database on Azure Cloud](https://github.com/Azure-Samples/qdrant-azure)
- [AKS Store (Vue) Demo with OpenAi (Sematic Kernel/Python)](https://github.com/Azure-Samples/aks-store-demo)
- [Sample Chat App with AOAI (Python/React)](https://github.com/microsoft/sample-app-aoai-chatGPT)
- 
- [Prompt flow is a suite of development tools](https://github.com/microsoft/promptflow)
- [huggingface transformers](https://github.com/huggingface/transformers)