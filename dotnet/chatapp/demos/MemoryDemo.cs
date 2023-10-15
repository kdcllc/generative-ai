using System;
using System.Text;
using ChatApp.Services;
using Microsoft.ApplicationInsights.WindowsServer;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.ChatCompletion;

namespace ChatApp.Demos
{
    public class MemoryDemo : IDemo
    {
        private readonly IKernel? _kernel;
        private readonly InMemoryService _memory;

        public MemoryDemo(
            KernelService kernel,
            InMemoryService memory)
        {
            _kernel = kernel.GetKernel();
            _memory = memory;
        }

        public string Name => nameof(MemoryDemo);

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            // Create a new chat
            IChatCompletion ai = _kernel.GetService<IChatCompletion>();
            ChatHistory chat = ai.CreateNewChat("You are an AI assistant that helps people find information.");
            StringBuilder builder = new();

            // Q&A loop
            while (true)
            {
                Console.Write("Question: ");
                var question = Console.ReadLine()!;

                var urls = UrlExtractor.ExtractUrls(question);
                if (urls.Count() > 0)
                {

                    foreach (var url in urls)
                    {
                        await _memory.AddContextFromUrlAsync(url, url, cancellationToken);
                        var found = await _memory.SearchContextAsync(url, question, cancellationToken);
                        builder.AppendLine(found);
                    }
                }

                builder.Clear();

                int contextToRemove = -1;
                if (builder.Length != 0)
                {
                    builder.Insert(0, "Here's some additional information: ");
                    contextToRemove = chat.Count;
                    chat.AddUserMessage(builder.ToString());
                }

                chat.AddUserMessage(question);

                builder.Clear();
                await foreach (string message in ai.GenerateMessageStreamAsync(chat))
                {
                    Console.Write(message);
                    builder.Append(message);
                }

                Console.WriteLine();
                chat.AddAssistantMessage(builder.ToString());

                if (contextToRemove >= 0) chat.RemoveAt(contextToRemove);
                Console.WriteLine();
            }
        }
    }
}
