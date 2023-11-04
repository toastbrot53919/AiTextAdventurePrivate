using System.Threading.Tasks;

public static class SummaryGenerator
{
    
    public static async Task<string> GenerateChildSummary(TextGeneratorClient textGenerator, string childListText)
    {
        string prompt = $"<s>[INST] Provide a brief summary: '{childListText}'. One sentence per entry. [/INST]";
        return await textGenerator.SendPrompt(prompt, 50);
    }

    public static async Task<string> GenerateExtendedSummary(TextGeneratorClient textGenerator, string parentDescription, string childListText)
    {
        string prompt = $"<s>[INST] Summarize briefly: '{parentDescription}. {childListText}'. [/INST]";
        return await textGenerator.SendPrompt(prompt, 100);
    }
}