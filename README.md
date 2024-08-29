# SeasonOfAIDemo

Add the nuget references

```<language>
 <ItemGroup>
    <PackageReference Include="Microsoft.KernelMemory.Core" Version="0.62.240605.1" />
    <PackageReference Include="Microsoft.SemanticKernel" Version="1.14.1" />
    <PackageReference Include="Microsoft.SemanticKernel.Connectors.OpenAI" Version="1.14.1" />
    <PackageReference Include="SharpToken" Version="2.0.3" />
  </ItemGroup>
```


## Use cases

### Simple chat completions

1) Create an http handler
We can use this handler to redirect server request to any other OpenAI compatible API

```<language>

   public class LocalModelHandler : HttpClientHandler
    {
        public LocalModelHandler(string customUrl)
        {
            CustomUrl = customUrl;
        }
        string CustomUrl { get; set; }
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
           
            request.RequestUri=new Uri($"{CustomUrl}{request.RequestUri.PathAndQuery}");
            Task<HttpResponseMessage> task = base.SendAsync(request, cancellationToken);
            return task;
        }
    }


```

2.1) Create an instance of the http client

```<language>
     
             LocalModelHandler handler = new LocalModelHandler(@"http://localhost:1234");
             HttpClient client = new HttpClient(handler);
             client.Timeout = TimeSpan.FromMinutes(10);
	 
```

3) Instantiate a kernel

```<language>

            //var Builder = Kernel.CreateBuilder();
            //Builder.AddAzureOpenAIChatCompletion("Model", "Endpoint", "Key");
            //AzureKernel = Builder.Build();

            var BuilderLocal = Kernel.CreateBuilder();
            BuilderLocal.AddOpenAIChatCompletion("phi3", "api-key", httpClient: client);
            var LocalKernel = BuilderLocal.Build();
```

4) Create a prompt and invoke it with the kernel


```<language>
            string Prompt = "Tell me about the country El Salvador, in 100 words the output format should be markdown";
            var Response = await LocalKernel.InvokePromptAsync(Prompt);
            Debug.WriteLine(Response);
```


### Interactive chats

1) Create an http handler
We can use this handler to redirect server request to any other OpenAI compatible API

```<language>

   public class LocalModelHandler : HttpClientHandler
    {
        public LocalModelHandler(string customUrl)
        {
            CustomUrl = customUrl;
        }
        string CustomUrl { get; set; }
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
           
            request.RequestUri=new Uri($"{CustomUrl}{request.RequestUri.PathAndQuery}");
            Task<HttpResponseMessage> task = base.SendAsync(request, cancellationToken);
            return task;
        }
    }


```

2.1) Create an instance of the http client

```<language>
     
             LocalModelHandler handler = new LocalModelHandler(@"http://localhost:1234");
             HttpClient client = new HttpClient(handler);
             client.Timeout = TimeSpan.FromMinutes(10);
	 
```

3) Instantiate a kernel

```<language>

            //var Builder = Kernel.CreateBuilder();
            //Builder.AddAzureOpenAIChatCompletion("Model", "Endpoint", "Key");
            //AzureKernel = Builder.Build();

            var BuilderLocal = Kernel.CreateBuilder();
            BuilderLocal.AddOpenAIChatCompletion("phi3", "api-key", httpClient: client);
            var LocalKernel = BuilderLocal.Build();
```

4) Get chat completions service
```<language>


            IChatCompletionService ChatService = LocalKernel.GetRequiredService<IChatCompletionService>();


```

5) Create the chat history object and system prompt

```<language>

            ChatHistory history = new ChatHistory();

            history.Add(new ChatMessageContent(AuthorRole.System,"You are an assistant that will give me a list of 5 facts about a country"));



```


6) Create the chat loop

```<language>
 
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


```

