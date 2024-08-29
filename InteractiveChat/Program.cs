using Azure.AI.OpenAI;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Spectre.Console;
using System.Diagnostics;

namespace InteractiveChat
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Task.Run(async () => await StartInteractiveChat()).Wait();
            Console.ReadLine();
        }
    
        static async Task StartInteractiveChat()
        {
            //LocalModelHandler handler = new LocalModelHandler(@"http://localhost:1234");
            //HttpClient client = new HttpClient(handler);
            //client.Timeout = TimeSpan.FromMinutes(10);

            //var BuilderLocal = Kernel.CreateBuilder();
            //BuilderLocal.AddOpenAIChatCompletion("phi3", "api-key", httpClient: client);
            //var LocalKernel = BuilderLocal.Build();

            var GetKey = () => Environment.GetEnvironmentVariable("OpenAiTestKey", EnvironmentVariableTarget.Machine);
            var Builder = Kernel.CreateBuilder();
            Builder.AddOpenAIChatCompletion("gpt-4o", GetKey.Invoke());
            var LocalKernel = Builder.Build();

            IChatCompletionService ChatService = LocalKernel.GetRequiredService<IChatCompletionService>();

            ChatHistory history = new ChatHistory();

            history.Add(new ChatMessageContent(AuthorRole.System,"You are an assistant that will give me a list of 5 facts about a country"));

            var settings = new OpenAIPromptExecutionSettings { MaxTokens = 3000, Temperature = 0.7, TopP = 0.5 };


            AnsiConsole.Background = Spectre.Console.Color.DeepSkyBlue4_1;
            AnsiConsole.Markup("[underline red]Welcome to our interactive chat[/] " + System.Environment.NewLine);
            AnsiConsole.Markup("[underline red]write a question for the Historian to start the chat[/] " + System.Environment.NewLine);

            //Chat loop
            while (true)
            {

                //Read user input
                var readUserInput = Console.ReadLine();

                //Add user input to the history
                history.AddUserMessage(readUserInput);

                //Get the response from the AI
                IReadOnlyList<ChatMessageContent> result = await ChatService.GetChatMessageContentsAsync(
                history,
                    executionSettings: settings,
                    kernel: LocalKernel
                );

                //Save the response in the history for the context
                history.AddRange(result);

                //output the answer to the console
                foreach (ChatMessageContent chatMessageContent in result)
                {
                    Console.WriteLine($"Historian:{chatMessageContent.ToString()}");
                }
               

            }
        }
    }
}
