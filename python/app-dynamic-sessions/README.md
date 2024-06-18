# Azure Container Dynamic Sessions

## Running Locally

To run the application locally, follow these steps:

1. **Create a virtual environment:**

    ```bash
    python -m venv .venv
    ```

2. **Activate the virtual environment:**

    ```bash
    source .venv/bin/activate
    ```

3. **Install the dependencies management tool:**

    ```bash
    pip install pip-tools
    ```

4. **Compile the requirements:**

    ```bash
    pip-compile requirements.in
    ```

5. **Install the dependencies:**

    ```bash
    pip install -r requirements.txt
    ```

6. **Login with Azure CLI:**
    ```bash
        az login
    ```

7. **Authetnication for Azure Container Dynamic sessions:**
    ```bash
        az role assignment create \
        --role "Azure ContainerApps Session Executor" \
        --assignee <PRINCIPAL_ID> \
        --scope <SESSION_POOL_RESOURCE_ID>
    
    az role assignment create \
        --role "Contributor" \
        --assignee <PRINCIPAL_ID> \
        --scope <SESSION_POOL_RESOURCE_ID>
    ```
    
## Resources

- [Azure Container Apps dynamic sessions overview](https://learn.microsoft.com/en-us/azure/container-apps/sessions)
- [anthonychu/20240528-sessions-csv](https://github.com/anthonychu/20240528-sessions-csv)
- [Role-based access control for Azure OpenAI Service](https://learn.microsoft.com/en-us/azure/ai-services/openai/how-to/role-based-access-control)
- [container-apps-dynamic-sessions-samples](https://github.com/Azure-Samples/container-apps-dynamic-sessions-samples)
- [Azure Container Apps Newsletter – June 2024](https://techcommunity.microsoft.com/t5/apps-on-azure-blog/azure-container-apps-newsletter-june-2024/ba-p/4165737)
