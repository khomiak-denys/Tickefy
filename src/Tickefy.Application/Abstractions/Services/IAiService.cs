using Tickefy.Application.AI.Dtos;

namespace Tickefy.Application.Abstractions.Services
{
    /// <summary>
    /// Defines contract requirements for interacting with external artificial intelligence language models to automate ticket triage and metadata suggestion.
    /// </summary>
    public interface IAiService
    {
        /// <summary>
        /// Asynchronously submits ticket textual context and scheduling constraints to an LLM provider to infer appropriate categorization and priority classification.
        /// </summary>
        /// <param name="title">
        /// The headline summary of the reported issue or work item. Supplying concise, highly descriptive titles improves classification accuracy; empty or non-informative titles may degrade model confidence.
        /// </param>
        /// <param name="description">
        /// The comprehensive text body explaining the bug, feature request, or task. Must be sanitized of sensitive personal identifiable information (PII) before transmission to external AI endpoints if required by policy.
        /// </param>
        /// <param name="deadline">
        /// The target completion timestamp or SLA deadline. Used by the model to evaluate urgency and recommend an appropriate priority level (e.g., Critical vs. Low).
        /// </param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// A task representing the asynchronous remote inference call. The task result contains an <see cref="AiResponse"/> data transfer object with suggested metadata classifications.
        /// </returns>
        /// <remarks>
        /// Callers must implement appropriate resilience and fallback handling (such as retry policies or default category assignment), as network latency or third-party rate limits may cause AI service timeouts.
        /// </remarks>
        Task<AiResponse> AnalyzeTicketAsync(string title, string description, DateTime deadline, CancellationToken cancellationToken = default);

    }
}
