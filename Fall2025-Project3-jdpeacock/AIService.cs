using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;
using System;
using System.ClientModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VaderSharp2;

namespace Fall2025_Project3_jdpeacock
{
    public class AIService
    {
        private readonly ChatClient _client;

        public AIService(IConfiguration configuration)
        {
            var endpoint = configuration["AIF_ENDPOINT"] ?? throw new ArgumentException("AIF Endpoint is missing");
            var apiKey = configuration["AIF_API_KEY"] ?? throw new ArgumentException("AIF ApiKey is missing");
            var deployment = configuration["AIF_DEPLOYMENT_NAME"] ?? throw new ArgumentException("AIF Deployment Name is missing");

            Console.WriteLine(endpoint);
            Console.WriteLine("API KEY", apiKey);
            Console.WriteLine(deployment);

            _client = new AzureOpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey)).GetChatClient(deployment);
        }

        public async Task<string> GetChatResponseAsync(string systemPrompt, string userMessage)
        {
            var options = new ChatCompletionOptions
            {
                MaxOutputTokenCount = 100,
                Temperature = 1
            };

            var message = new ChatMessage[] {
                new SystemChatMessage(systemPrompt),
                new UserChatMessage(userMessage)
            };

            ClientResult<ChatCompletion> result = await _client.CompleteChatAsync(message);
            var response = result.Value.Content[0].Text;

            return response;
        }
    }
}