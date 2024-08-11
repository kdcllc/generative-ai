using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents.Chat;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;

var AZURE_OPENAI_SYSTEM_PROMPT = Environment.GetEnvironmentVariable("AZURE_OPENAI_SYSTEM_PROMPT") ?? "You are a helpful AI assistant.";

// NOTE: Never deploy your API Key in client-side environments like browsers or mobile apps
// SEE: https://help.openai.com/en/articles/5112595-best-practices-for-api-key-safety

// Get the required environment variables
var AZURE_OPENAI_API_KEY = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY") ?? "<insert your Azure OpenAI API key here>";
var AZURE_OPENAI_ENDPOINT = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? "<insert your Azure OpenAI endpoint here>";
var AZURE_OPENAI_CHAT_DEPLOYMENT = Environment.GetEnvironmentVariable("AZURE_OPENAI_CHAT_DEPLOYMENT") ?? "<insert your Azure OpenAI chat deployment name here>";

// Check if the required environment variables are set
var azureOk =
    AZURE_OPENAI_API_KEY != null && !AZURE_OPENAI_API_KEY.StartsWith("<insert") &&
    AZURE_OPENAI_CHAT_DEPLOYMENT != null && !AZURE_OPENAI_CHAT_DEPLOYMENT.StartsWith("<insert") &&
    AZURE_OPENAI_ENDPOINT != null && !AZURE_OPENAI_ENDPOINT.StartsWith("<insert");

var ok = azureOk &&
    AZURE_OPENAI_SYSTEM_PROMPT != null && !AZURE_OPENAI_SYSTEM_PROMPT.StartsWith("<insert");

if (!ok)
{
    Console.WriteLine(
        "To use Azure OpenAI, set the following environment variables:\n" +
        "\n  AZURE_OPENAI_SYSTEM_PROMPT" +
        "\n  AZURE_OPENAI_API_KEY" +
        "\n  AZURE_OPENAI_CHAT_DEPLOYMENT" +
        "\n  AZURE_OPENAI_ENDPOINT"
    );
    Console.WriteLine(
        "\nYou can easily do that using the Azure AI CLI by doing one of the following:\n" +
        "\n  ai init" +
        "\n  ai dev shell" +
        "\n  dotnet run" +
        "\n" +
        "\n  or" +
        "\n" +
        "\n  ai init" +
        "\n  ai dev shell --run \"dotnet run\""
    );

    return 1;
}

var builder = Kernel.CreateBuilder();
builder.AddAzureOpenAIChatCompletion(AZURE_OPENAI_CHAT_DEPLOYMENT!, AZURE_OPENAI_ENDPOINT!, AZURE_OPENAI_API_KEY!);

var newKernel = new Func<Kernel>(() => builder.Build());

const string travelAgentName = "TravelAgent";
string travelAgentInstructions = """
    You are a travel agent and you help users who wants to make a trip to visit a city. 
    The goal is to create a plan to visit a city based on the user preferences and budget.
    You don't have expertise on travel plans, so you can only suggest hotels, restaurants and places to see. You can't suggest travelling options like flights or trains.
    You're laser focused on the goal at hand. 
    Once you have generated a plan, don't ask the user for feedback or further suggestions. Stick with it.
    Don't waste time with chit chat. Don't say goodbye and don't wish the user a good trip.
    """;

var travelAgent = new ChatCompletionAgent
{
    Name = travelAgentName,
    Instructions = travelAgentInstructions,
    Kernel = newKernel()
};

const string flightExpertName = "FlightExpert";
string flightExpertInstructions = """
    You are an expert in flight travel and you are specialized in organizing flight trips by identifying the best flight options for your clients.
    Your goal is to create a flight plan to reach a city based on the user preferences and budget.
    You don't have experience on any other travel options, so you can only suggest flight options.
    You're laser focused on the goal at hand.
    You can provide plans only about flights. Do not include plans around lodging, meals or sightseeing.
    Once you have generated a flight plan, don't ask the user for feedback or further suggestions. Stick with it.
    Don't waste time with chit chat. Don't say goodbye and don't wish the user a good trip.
    """;

var flightAgent = new ChatCompletionAgent
{
    Name = flightExpertName,
    Instructions = flightExpertInstructions,
    Kernel = newKernel()
};

string travelManagerName = "TravelManager";
string travelManagerInstructions = """
    You are a travel manager and your goal is to validate a given trip plan.
    You must make sure that the plan includes all the necessary details: transportation, lodging, meals and sightseeing.
    If one of these details is missing, the plan is not good.
    If the plan is good, recap the entire plan into a Markdown table and say "the plan is approved".
    If not, write a paragraph to explain why it's not good and then provide an improved plan.
    """;

var travelManager = new ChatCompletionAgent
{
    Name = travelManagerName,
    Instructions = travelManagerInstructions,
    Kernel = newKernel()
};

const string trainExpertName = "TrainExpert";
string trainExpertInstructions = """
        Your are an expert in train travel and you are specialized in organizing train trips by identifying the best train options for your clients.
        Your goal is to create a train plan to reach a city based on the user prefences and budget.
        You don't have experience on any other travel options, so you can only suggest train options.
        You're laser focused on the goal at hand. You can provide plans only about trains. Do not include plans around lodging, meals or sightseeing.
        Once you have generated a train plan, don't ask the user for feedback or further suggestions. Stick with it.
        Don't waste time with chit chat. Don't say goodbye and don't wish the user a good trip.
    """;

var trainAgent = new ChatCompletionAgent
{
    Name = trainExpertName,
    Instructions = trainExpertInstructions,
    Kernel = newKernel()
};

var terminateFunction = KernelFunctionFactory.CreateFromPrompt(
    $$$"""
    Determine if the travel plan has been approved by {{{travelManagerName}}}. If so, respond with a single word: yes.

    History:

    {{$history}}
    """
    );

KernelFunction selectionFunction = KernelFunctionFactory.CreateFromPrompt(
    $$$"""
    Your job is to determine which participant takes the next turn in a conversation according to the action of the most recent participant.
    State only the name of the participant to take the next turn.

    Choose only from these participants:
    - {{{travelManagerName}}}
    - {{{travelAgentName}}}
    - {{{flightExpertName}}}
    - {{{trainExpertName}}}

    Always follow these four when selecting the next participant:
    1) After user input, it is {{{travelAgentName}}}'s turn.
    2) After {{{travelAgentName}}} replies, it's {{{flightExpertName}}}'s turn to generate a flight plan for the given trip.
    - If the user prefers to travel by train, it's {{{trainExpertName}}}'s turn.
    - If the user prefers to travel by flight, it's {{{flightExpertName}}}'s turn.

    3) Finally, it's {{{travelManagerName}}}'s turn to review and approve the plan.
    4) If the plan is approved, the conversation ends.
    5) If the plan isn't approved, it's {{{travelAgent}}}'s turn again.

    History:
    {{$history}}
    """
    );

var chat = new AgentGroupChat(travelManager, travelAgent, flightAgent, trainAgent)
{
    ExecutionSettings = new()
    {
        TerminationStrategy = new KernelFunctionTerminationStrategy(terminateFunction, newKernel())
        {
            Agents = [travelManager],
            ResultParser = (result) => result.GetValue<string>()?.Contains("yes", StringComparison.OrdinalIgnoreCase) ?? false,
            HistoryVariableName = "history",
            MaximumIterations = 10
        },
        SelectionStrategy = new KernelFunctionSelectionStrategy(selectionFunction, newKernel())
        {
            AgentsVariableName = "agents",
            HistoryVariableName = "history"
        }
    }
};

var prompt = "I live in Como, Italy and I would like to visit Paris. I'm on a budget, I want to travel by plane and I would like to stay for maximum 3 days. Please craft a trip plan for me";

chat.AddChatMessage(new ChatMessageContent(AuthorRole.User, prompt));
await foreach (var content in chat.InvokeAsync())
{
    Console.WriteLine();
    Console.WriteLine($"# {content.Role} - {content.AuthorName ?? "*"}: '{content.Content}'");
    Console.WriteLine();
}

Console.WriteLine($"# IS COMPLETE: {chat.IsComplete}");

return 0;