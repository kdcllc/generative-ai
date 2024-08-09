using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents.Chat;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;

var configuration = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

var apiKey = configuration["AzureOpenAI:ApiKey"];
var deploymentName = configuration["AzureOpenAI:DeploymentName"];
var endpoint = configuration["AzureOpenAI:Endpoint"];

var kernel = Kernel.CreateBuilder().AddAzureOpenAIChatCompletion(deploymentName, endpoint, apiKey).Build();

const string travelAgentName = "TravelAgent";
string travelAgentInstructions = """
    You are a travel agent and you help users who want to make a trip to visit a city.
    The goal is to create a plan to visit a city based on the user preferences and budget.
    You don't have expertise on travel plans, so you can only suggest hotels, restaurants and places to see. 
    You can't suggest traveling options like flights or trains.
    You're laser focused on the goal at hand.
    Once you have generated a plan, don't ask the user for feedback or further suggestions. Stick with it.
    Don't waste time with chit chat. Don't say goodbye and don't wish the user a good trip.
    """;

var travelAgent = new ChatCompletionAgent
{
    Name = travelAgentName,
    Instructions = travelAgentInstructions,
    Kernel = kernel
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
    Kernel = kernel
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
    Kernel = kernel
};

var terminateFunction = KernelFunctionFactory.CreateFromPrompt(
    """
    Determine if the travel plan has been approved. If so, respond with a single word: yes.
    History:
    {{$history}}
    """
);

var selectionFunction = KernelFunctionFactory.CreateFromPrompt(
    """
    Your job is to determine which participant takes the next turn in a conversation according to the action of the most recent participant.
    State only the name of the participant to take the next turn.
    Choose only from these participants:
    - {{{travelManagerName}}}
    - {{{travelAgentName}}}
    - {{{flightExpertName}}}
    Always follow these steps when selecting the next participant:
    1) After user input, it is {{{travelAgentName}}}'s turn.
    2) After {{{travelAgentName}}} replies, it's {{{flightExpertName}}}'s turn.
    3) After {{{flightExpertName}}} replies, it's {{{travelManagerName}}}'s turn to review and approve the plan.
    4) If the plan is approved, the conversation ends.
    5) If the plan isn't approved, it's {{{travelAgent}}}'s turn again.
    History:
    {{$history}}
    """
);

var chat = new AgentGroupChat(travelManager, travelAgent, flightAgent)
{
    ExecutionSettings = new()
    {
        TerminationStrategy = new KernelFunctionTerminationStrategy(terminateFunction, kernel)
        {
            Agents = [travelManager],
            ResultParser = (result) => result.GetValue<string>()?.Contains("yes", StringComparison.OrdinalIgnoreCase) ?? false,
            HistoryVariableName = "history",
            MaximumIterations = 10
        },
        SelectionStrategy = new KernelFunctionSelectionStrategy(selectionFunction, kernel)
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
