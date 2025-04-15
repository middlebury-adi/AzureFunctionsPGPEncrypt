using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text;
using PgpCore;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Microsoft.Extensions.Configuration;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AzureFunctionsPGPEncryptRisepoint;

public class PGPEncryptRisepoint
{
    private readonly ILogger<PGPEncryptRisepoint> _logger;
    private readonly IConfiguration _configuration;

    public PGPEncryptRisepoint(ILogger<PGPEncryptRisepoint> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    [Function(nameof(PGPEncryptRisepoint))]
    public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
    {
        _logger.LogInformation($"C# HTTP trigger function {nameof(PGPEncryptRisepoint)} processed a request.");

        string publicKeyBase64 = _configuration["pgp-public-key-risepoint"];

        if (string.IsNullOrEmpty(publicKeyBase64))
        {
            return new BadRequestObjectResult($"Please add a base64 encoded public key to an environment variable called pgp-public-key-risepoint");
        }

        byte[] publicKeyBytes = Convert.FromBase64String(publicKeyBase64);
        string publicKey = Encoding.UTF8.GetString(publicKeyBytes);

        var inputStream = new MemoryStream();PGPEncryptRisepoint
        await req.Body.CopyToAsync(inputStream);
        inputStream.Seek(0, SeekOrigin.Begin);

        try
        {
            Stream encryptedData = await EncryptRisepointAsync(inputStream, publicKey);
            return new OkObjectResult(encryptedData);
        }
        catch (PgpException pgpException)
        {
            return new BadRequestObjectResult(pgpException.Message);
        }
    }

    private async Task<Stream> EncryptRisepointAsync(Stream inputStream, string publicKey)
    {
        using (PGP pgp = new PGP(new EncryptionKeys(publicKey))) 
        {
            var outputStream = new MemoryStream();

            using (inputStream)
            {
                await pgp.EncryptStreamAsync(inputStream, outputStream, true, true);
                outputStream.Seek(0, SeekOrigin.Begin);
                return outputStream;
            }
        }
    }
}
