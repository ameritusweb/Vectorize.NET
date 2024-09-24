using System;
using System.Collections.Generic;

namespace Vectorize.NET
{
    public class VectorizationFeedback
    {
        public List<string> FeedbackMessages { get; } = new List<string>();

        public void AddMessage(string message)
        {
            FeedbackMessages.Add(message);
        }

        public void PrintFeedback()
        {
            Console.WriteLine("Vectorization Feedback:");
            foreach (var message in FeedbackMessages)
            {
                Console.WriteLine($"- {message}");
            }
        }
    }
}