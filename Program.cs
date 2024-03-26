using Gotenberg.Sharp.API.Client.Domain.Builders;
using Gotenberg.Sharp.API.Client;

class Program
{
    static string ResourcePath = @"html-files-source-directory";
    static Random Rand = new Random(Math.Abs((int)DateTime.Now.Ticks));

    public static async Task Main()
    {
        await ConvertHtmlToPdfAsync(@"destination-directory");
    }

    public static async Task ConvertHtmlToPdfAsync(string destinationDirectory)
    {
        var sharpClient = new GotenbergSharpClient("http://localhost:3000");

        var builder = new HtmlRequestBuilder()
               .AddAsyncDocument(async doc =>
                    doc.SetBody(await GetHtmlFile("*.html"))
                       ).WithDimensions(dims => dims.UseChromeDefaults());

        var request = await builder.BuildAsync();

        var resultPath = @$"{destinationDirectory}\ResultPDF-{Rand.Next()}.pdf";
        var response = await sharpClient.HtmlToPdfAsync(request);

        using (var destinationStream = File.Create(resultPath))
        {
            await response.CopyToAsync(destinationStream);
        }

        Console.WriteLine("Conversion completed. PDF saved at: " + resultPath);
    }
    static async Task<byte[]> GetHtmlFile(string fileName)
    {
        return await File.ReadAllBytesAsync($@"{ResourcePath}\{fileName}");
    }
}