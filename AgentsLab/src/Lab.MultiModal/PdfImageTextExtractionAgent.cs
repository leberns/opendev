using System.Diagnostics;
using System.Runtime.InteropServices;
using AgentExtensions;
using Docnet.Core;
using Docnet.Core.Models;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using SkiaSharp;
using Contracts;
using Contracts.ChatClientBuilders;
using Contracts.LabTags;
using Docnet.Core.Readers;

namespace DataUnderstanding;

public class PdfImageTextExtractionAgent(
    IChatClientBuilder chatClientBuilder
    ) : IRunnableLab
{
    private const string BasePath = "Data/";
    private const string PdfPath = BasePath + "document.pdf"; // source pdf file

    public List<TagType> GetTags() => [TagType.MultiModal, TagType.Vision, TagType.Ocr];

    public List<string> GetLabDescriptions() => [
        "Demonstrate an agent extracting text from images embedded in a PDF file.",
        "The PDF file can contain several pages, each page is rasterized to PNG",
        "and sent to the vision model for OCR.",
        $"Each extracted page is saved in a separated file under the path {BasePath}.",
    ];

    public string GetUserInput() => "Read all the text from this document page exactly as it appears.";

    public async Task<string?> RunLabAsync(string userInput)
    {
        var chatClient = chatClientBuilder.BuildChatClient(ChatClientType.OllamaQwenVl4B);

        ChatClientAgentOptions agentOptions = new()
        {
            Name = "Agent",
            ChatOptions = new ChatOptions
            {
                Instructions = "You are a document OCR assistant. Output only the extracted text, preserving layout as much as possible."
            }
        };

        var agent = new ChatClientAgent(chatClient, agentOptions);

        var totalTime = TimeSpan.Zero;

        using var docReader = DocLib.Instance.GetDocReader(PdfPath, new PageDimensions(1500, 2000));
        var pageCount = docReader.GetPageCount();

        Console.WriteLine($"The PDF document has {pageCount} page(s)");

        for (var i = 0; i < pageCount; i++)
        {
            var pngPath = $"{BasePath}document-p{i + 1:D2}.png";
            var outputPath = $"{BasePath}document-p{i + 1:D2}.txt";

            var pngBytes = RenderPage(docReader, i);

            await File.WriteAllBytesAsync(pngPath, pngBytes);

            Console.WriteLine($"Page {i + 1}: PNG {pngBytes.Length} bytes saved to {Path.GetFullPath(pngPath)}");

            var rom = new ReadOnlyMemory<byte>(pngBytes);

            List<AIContent> contents = [
                new TextContent(userInput),
                new DataContent(rom, "image/png")
            ];

            List<ChatMessage> messages = [
                new(ChatRole.User, contents)
            ];

            var startTime = Stopwatch.GetTimestamp();

            var response = await agent.RunAsync(messages);

            var elapsedTime = Stopwatch.GetElapsedTime(startTime);

            totalTime += elapsedTime;

            response.LogResponseUsage();

            Console.WriteLine($"Page {i + 1} OCR took {elapsedTime.TotalSeconds} seconds");

            await File.WriteAllTextAsync(outputPath, response.Text);

            Console.WriteLine($"Extracted page saved to {outputPath}");
        }

        Console.WriteLine($"The OCR took in total {totalTime.TotalSeconds} s");

        return null;
    }

    private static byte[] RenderPage(IDocReader docReader, int pageIndex)
    {
        using var pageReader = docReader.GetPageReader(pageIndex);

        var rawBytes = pageReader.GetImage();
        var width = pageReader.GetPageWidth();
        var height = pageReader.GetPageHeight();

        using var bitmap = new SKBitmap(width, height, SKColorType.Bgra8888, SKAlphaType.Premul);
        Marshal.Copy(rawBytes, 0, bitmap.GetPixels(), rawBytes.Length);

        using var ms = new MemoryStream();
        bitmap.Encode(ms, SKEncodedImageFormat.Png, 100);

        return ms.ToArray();
    }
}
