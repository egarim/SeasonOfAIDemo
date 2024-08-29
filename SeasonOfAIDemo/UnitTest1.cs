using Microsoft.SemanticKernel;
using System.Diagnostics;

namespace SeasonOfAIDemo
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task SimpleChatCompletions()
        {

            LocalModelHandler handler = new LocalModelHandler(@"http://localhost:1234");
            HttpClient client = new HttpClient(handler);
            client.Timeout = TimeSpan.FromMinutes(10);


            //var Builder = Kernel.CreateBuilder();
            //Builder.AddAzureOpenAIChatCompletion("Model", "Endpoint", "Key");
            //AzureKernel = Builder.Build();


            var BuilderLocal = Kernel.CreateBuilder();
            var GetKey = () => Environment.GetEnvironmentVariable("OpenAiTestKey", EnvironmentVariableTarget.Machine);
            BuilderLocal.AddOpenAIChatCompletion("gpt-4o", GetKey.Invoke());
            var LocalKernel = BuilderLocal.Build();


            string Prompt = "Tell me about the country El Salvador, in 100 words the output format should be markdown";
            var Response = await LocalKernel.InvokePromptAsync(Prompt);
            Debug.WriteLine(Response);

            Assert.IsTrue(Response?.ToString().Length > 0);

            Assert.Pass();
        }
    }
}