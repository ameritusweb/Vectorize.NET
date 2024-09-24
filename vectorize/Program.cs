using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.IO;

namespace Vectorize.NET
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Please provide a C# file to vectorize.");
                return;
            }

            string sourceCode = File.ReadAllText(args[0]);
            SyntaxTree tree = CSharpSyntaxTree.ParseText(sourceCode);
            var root = tree.GetRoot();

            var compilation = CSharpCompilation.Create("Vectorization")
                .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
                .AddSyntaxTrees(tree);

            var model = compilation.GetSemanticModel(tree);

            var hardwareInfo = new HardwareInfo { SimdWidth = 4, CacheSize = 1024 * 1024 }; // Example values
            var vectorizer = new Vectorizer(hardwareInfo);

            foreach (var method in root.DescendantNodes().OfType<MethodDeclarationSyntax>())
            {
                Console.WriteLine($"Vectorizing method: {method.Identifier}");
                string vectorizedCode = vectorizer.VectorizeFunction(method, model);
                Console.WriteLine("Vectorized code:");
                Console.WriteLine(vectorizedCode);
            }
        }
    }
}