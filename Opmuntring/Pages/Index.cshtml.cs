using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Opmuntring.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IConfiguration _config;
        private static string[] grader = {"Let", "Udfordrende", "Meget svært" };
       

        public List<String> Errors { get; set; }

        public string Quote { get; set; }

        public IndexModel(ILogger<IndexModel> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public async Task OnGetAsync(String navn, String fag, String tidspunkt, String udfordring)
        {
            int skala = 0;

            Errors = new List<String>();
            if (String.IsNullOrEmpty(navn))
            {
                Errors.Add("Jeg havde regnet med at få indsendt et felt, der hedder navn, men det fik jeg ikke");
            }

            if (String.IsNullOrEmpty(fag))
            {
                Errors.Add("Jeg havde regnet med at få indsendt et felt, der hedder fag, men det fik jeg ikke");
            } else
            {
                if (fag != "Webudvikling" && fag != "Cloud" && fag != "Frontendprogrammering")
                {
                    Errors.Add($"Jeg kender kun fagene Webudvikling, Cloud, og Frontendprogrammering. Du indsendte {fag}");
                }
            }

            if (String.IsNullOrEmpty(tidspunkt))
            {
                Errors.Add("Jeg havde regnet med at få indsendt et felt, der hedder tidspunkt, men det fik jeg ikke");
            }
            else
            {
                if (tidspunkt != "morgen" && tidspunkt != "eftermiddag" && tidspunkt != "aften")
                {
                    Errors.Add($"Jeg kender kun tidspunkterne morgen, eftermiddag og aften. Du indsendte {tidspunkt}");
                }
            }

            if (String.IsNullOrEmpty(udfordring))
            {
                Errors.Add("Jeg havde regnet med at få indsendt et felt, der hedder udfordring, men det fik jeg ikke");
            } else
            {
                if (!int.TryParse(udfordring, out skala))
                {
                    Errors.Add($"Jeg havde regnet med, at udfordring var et heltal, men det er det ikke. Det er {udfordring}");
                } else if (skala < 1 || skala > 3)
                {
                    Errors.Add($"Jeg havde regnet med, at udfordring var et tal mellem 1 og 3 (inkl), men det er det ikke. Det er {udfordring}");
                } else
                {
                    skala = skala - 1; // Indsendes fra 1-3 men skal bruges i array
                }
            }

            if (Errors.Count > 0)
            {
                return;
            }

            string token = _config["Gpt3Token"];
            String prompt = $"Fortælhvorfor det er fantastisk for {navn} at lære om {fag} om {tidspunkt}en, selvom det er {grader[skala]}";
            Quote = await OpenAiService.promptGpt3Async(prompt, 200, token);
            
        }
    }
}