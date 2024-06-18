import datetime
import os
from dotenv import load_dotenv
from azure.identity import DefaultAzureCredential
from langchain_openai import AzureChatOpenAI


# Load the environment variables from the .env file
load_dotenv()


# Set up the Azure Chat OpenAI model using managed identity for authentication
azure_token = None
def token_factory():
    global azure_token
    if azure_token is None or datetime.datetime.fromtimestamp(azure_token.expires_on, datetime.timezone.utc) < datetime.datetime.now(datetime.timezone.utc) + datetime.timedelta(minutes=5):
        print("Refreshing Azure token...")
        azure_token = DefaultAzureCredential().get_token("https://cognitiveservices.azure.com/.default")
    return azure_token.token

llm = AzureChatOpenAI(
    azure_deployment="gpt-35-turbo-16k",
    api_version="2024-02-01",
    streaming=True,
    temperature=0,
    azure_ad_token_provider=token_factory,
    azure_endpoint=os.environ["AZURE_OPENAI_ENDPOINT"],
)

from langchain_azure_dynamic_sessions import SessionsPythonREPLTool
from langchain import hub, agents
from langchain.agents import create_tool_calling_agent, AgentExecutor
from langchain_core.messages import SystemMessage


repl = SessionsPythonREPLTool(
    pool_management_endpoint=os.environ["POOL_MANAGEMENT_ENDPOINT"],
)

tools = [repl]
repl.invoke("6 * 7")

prompt = hub.pull("hwchase17/openai-functions-agent")

agent = create_tool_calling_agent(llm, [repl], prompt)

agent_executor = agents.AgentExecutor(
    agent=agent, tools=tools, verbose=True, handle_parsing_errors=True
)

# Initialize chat history with instructions on how to query data
chat_history = []
chat_history.append(SystemMessage(content=(
    "To analyze data, execute Python code to read the file and extract the data you need. "
    "Pandas is available. "
    "You must start by using `head()` checking the first few rows to see what the data looks like, "
    "and then write additional queries to complete the analysis."
)))

# Upload the data file
# remote_file = repl.upload_file(local_file_path="customers.csv")
# chat_history.append(SystemMessage(content=f"The data file `{remote_file.full_path}` has been uploaded."))

response = agent_executor.invoke(
    {
        "input": "What is the average amount purchased by all customers in Arizona?",
        "chat_history": chat_history,
    }
)

# response = agent_executor.invoke(
#     {
#         "input": "What is the state who has purchased the most stuff?",
#         "chat_history": chat_history,
#     }
# )

# response = agent_executor.invoke(
#     {
#         "input": "Who are the top 5 happiest customers?",
#         "chat_history": chat_history,
#     }
# )