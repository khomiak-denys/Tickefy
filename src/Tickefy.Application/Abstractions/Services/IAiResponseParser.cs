using Tickefy.Application.AI.Dtos;
using Tickefy.Domain.Common.Category;
using Tickefy.Domain.Common.Priority;

namespace Tickefy.Application.Abstractions.Services
{
    /// <summary>
    /// Defines contract requirements for translating unstructured or semi-structured artificial intelligence inference outputs into strongly-typed domain classification enumerations.
    /// </summary>
    public interface IAiResponseParser
    {
        /// <summary>
        /// Analyzes the raw categorization string emitted by an LLM and maps it to a valid domain <see cref="Category"/> enumeration value.
        /// </summary>
        /// <param name="response">
        /// The data transfer object encapsulating raw AI text predictions. Callers must verify that the response object is not null prior to invocation.
        /// </param>
        /// <returns>
        /// The corresponding <see cref="Category"/> enum value matching the AI prediction, or a safe default (e.g., <see cref="Category.Other"/>) if the model output is unrecognized or ambiguous.
        /// </returns>
        /// <remarks>
        /// Parsing should be resilient to common LLM formatting hallucinations such as markdown formatting, extra whitespace, or case mismatches.
        /// </remarks>
        public Category ParseCategory(AiResponse response);

        /// <summary>
        /// Evaluates the priority suggestion emitted by an AI model and converts it into a standardized domain <see cref="Priority"/> enumeration value.
        /// </summary>
        /// <param name="response">
        /// The AI inference response payload containing the suggested priority string. Must not be null.
        /// </param>
        /// <returns>
        /// The resolved <see cref="Priority"/> enum value representing urgency, defaulting to a moderate or standard priority level if the model output cannot be deterministically mapped.
        /// </returns>
        /// <remarks>
        /// Callers should ensure that critical SLAs are not solely reliant on unvalidated AI priority parsers without human review or business rule validation.
        /// </remarks>
        public Priority ParsePriority(AiResponse response);
    }
}
