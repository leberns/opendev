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

namespace DataUnderstanding;

public class PdfImageTextExtractionAgent(
    IChatClientBuilder chatClientBuilder
    ) : IRunnableLab
{
    private const string BasePath = "Data/";
    private const string PdfPath = BasePath + "document.pdf"; // source pdf file
    private const string PngPath = BasePath + "document.png"; // rendered pdf page as png
    private const string OutputPath = BasePath + "document.txt"; // extracted text output

    public List<TagType> GetTags() => [TagType.MultiModal, TagType.Vision, TagType.DataExtraction];

    public List<string> GetLabDescriptions() => [
        "Demonstrate an agent extracting text from images embedded in a PDF file.",
        "The PDF page is rasterized to PNG and sent to a vision model for OCR.",
        "The extracted text is saved to data/document.txt.",
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

        var pngBytes = RenderPdfPageToPng(PdfPath, pageIndex: 0);

        await File.WriteAllBytesAsync(PngPath, pngBytes);

        Console.WriteLine($"Rendered PNG size: {pngBytes.Length} bytes, saved to {Path.GetFullPath(PngPath)}");

        var rom = new ReadOnlyMemory<byte>(pngBytes);

        List<AIContent> contents = [
            new TextContent(userInput),
            new DataContent(rom, "image/png")
        ];

        List<ChatMessage> messages = [
            new(ChatRole.User, contents)
        ];

        long startTime = Stopwatch.GetTimestamp();

        var response = await agent.RunAsync(messages);

        var elapsedTime = Stopwatch.GetElapsedTime(startTime);

        response.LogResponseAndUsage();

        Console.WriteLine($"The image processing took {elapsedTime.TotalSeconds} s");

        var extractedText = response.Text;

        await File.WriteAllTextAsync(OutputPath, extractedText);

        Console.WriteLine($"Extracted text written to {OutputPath}");

        return extractedText;
    }

    private static byte[] RenderPdfPageToPng(string pdfPath, int pageIndex)
    {
        using var docReader = DocLib.Instance.GetDocReader(pdfPath, new PageDimensions(1500, 2000));
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
