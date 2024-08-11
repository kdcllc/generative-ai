# Generative AI

This project demonstrates how to build a multi-agent framework using the Azure OpenAI Assistant API, including the use of various Azure AI services like GPT-4 Turbo with Vision and DALL-E 3.

## Setup

1. **Clone the repository:**
    ```bash
    git clone <repository_url>
    cd <repository_directory>
    ```

2. **Create a virtual environment:**
    ```bash
    python -m venv .venv
    source .venv/bin/activate  # On Windows use `venv\Scripts\activate`
    ```

3. **Install dependencies:**
    ```bash
    pip install -r requirements.txt
    ```

3. **Set up environment variables:**
    Create a `.env` file in the root directory and add the following variables:
    ```env
    GPT4_AZURE_OPENAI_KEY=<your_gpt4_azure_openai_key>
    GPT4_AZURE_OPENAI_API_VERSION=<your_gpt4_azure_openai_api_version>
    GPT4_AZURE_OPENAI_ENDPOINT=<your_gpt4_azure_openai_endpoint>
    GPT4_DEPLOYMENT_NAME=<your_gpt4_deployment_name>
    DALLE3_AZURE_OPENAI_KEY=<your_dalle3_azure_openai_key>
    DALLE3_AZURE_OPENAI_API_VERSION=<your_dalle3_azure_openai_api_version>
    DALLE3_AZURE_OPENAI_ENDPOINT=<your_dalle3_azure_openai_endpoint>
    DALLE3_DEPLOYMENT_NAME=<your_dalle3_deployment_name>
    GPT4VISION_AZURE_OPENAI_KEY=<your_gpt4vision_azure_openai_key>
    GPT4VISION_AZURE_OPENAI_API_VERSION=<your_gpt4vision_azure_openai_api_version>
    GPT4VISION_AZURE_OPENAI_ENDPOINT=<your_gpt4vision_azure_openai_endpoint>
    GPT4VISION_DEPLOYMENT_NAME=<your_gpt4vision_deployment_name>
    ```

## Usage

Run the main script to start the multi-agent framework:
```bash
python python/agents-with-assistants/main.py
```

## Example Queries

1. Generate an image of a boat drifting in the water and analyze it and enhance the image.
2. Following your plan strictly and step by step. Generate an image of a space civilization, analyze it and enhance it. Analyze and enhance it several times until the image satisfies the request.

## References

- [Azure OpenAI Service](https://azure.microsoft.com/en-us/services/cognitive-services/openai-service/)
- [OpenAI API](https://beta.openai.com/docs/)
