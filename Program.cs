/*
 * CSC 4890 Midterm Project: Context-Free Grammar Sentence Generator
 *
 * This program generates random English-like sentences based on the provided CFL grammar.
 * It uses recursive functions to model non-terminals, with random selection for productions
 * having multiple right-hand sides (50/50 probability via Random.Next(2)).
 *
 * Grammar Recap:
 * <SENTENCE> -> <NOUN-PHRASE> <VERB-PHRASE>
 * <NOUN-PHRASE> -> <CMPLX-NOUN> | <CMPLX-NOUN> <PREP-PHRASE>
 * <VERB-PHRASE> -> <CMPLX-VERB> | <CMPLX-VERB> <PREP-PHRASE>
 * <PREP-PHRASE> -> <PREP> <CMPLX-NOUN>
 * <CMPLX-NOUN> -> <ARTICLE> <NOUN>
 * <CMPLX-VERB> -> <VERB> | <VERB> <NOUN-PHRASE>
 * <ARTICLE> -> a | the
 * <NOUN> -> boy | girl | flower | bird
 * <VERB> -> touches | likes | sees
 * <PREP> -> with
 *
 * Notes:
 * - Terminals are selected uniformly at random.
 * - No recursion depth issues expected (grammar is not left-recursive).
 * - Output: 5 sentences printed to console.
 */

namespace ParseTrees
{
    internal class Program
    {
        // Initialized without seed for true randomness;
        private static readonly Random random = new();

        // Terminal symbol collections: Arrays for uniform random selection via Choose().
        // These represent the leaf productions in the grammar.
        private static readonly string[] articles = ["a", "the"];  // <ARTICLE> -> a | the

        private static readonly string[] nouns = ["boy", "girl", "flower", "bird"];  // <NOUN> -> boy | girl | flower | bird
        private static readonly string[] verbs = ["touches", "likes", "sees"];  // <VERB> -> touches | likes | sees

        /// <summary>
        /// Entry point: Prints header, generates and displays 5 sentences, then footer.
        /// Each sentence is derived fully from the start symbol ;SENTENCE;.
        /// Uses a loop for batch generation; output formatted for readability.
        /// </summary>
        private static void Main(string[] args)
        {
            Console.WriteLine("=".PadRight(70, '='));
            Console.WriteLine("Context-Free Grammar Sentence Generator");
            Console.WriteLine("=".PadRight(70, '='));
            Console.WriteLine();

            // Generate exactly 5 sentences
            // Each call to GenerateSentence() triggers full recursive derivation.
            for (int i = 1; i <= 5; i++)
            {
                Console.WriteLine($"Sentence {i}:");
                string sentence = GenerateSentence();
                Console.WriteLine($" {sentence}");
                Console.WriteLine();
            }

            // Footer to signal completion.
            Console.WriteLine("=".PadRight(70, '='));
            Console.WriteLine("Program completed successfully.");
            Console.ReadKey();
        }

        /// <summary>
        /// Generates a complete sentence by applying the start production.
        /// SENTENCE; -> ;NOUN-PHRASE; ;VERB-PHRASE;
        /// Combines phrases with a space; no additional punctuation per grammar.
        /// </summary>
        /// <returns>A space-separated sentence string.</returns>
        private static string GenerateSentence()
        {
            string nounPhrase = GenerateNounPhrase();
            string verbPhrase = GenerateVerbPhrase();
            return $"{nounPhrase} {verbPhrase}";
        }

        /// <summary>
        /// Generates a noun phrase using one of two production rules.
        /// ;NOUN-PHRASE; -> ;CMPLX-NOUN; | ;CMPLX-NOUN; ;PREP-PHRASE;
        /// Delegates to AppendOptional for 50/50 random choice.
        /// </summary>
        /// <returns>The derived noun phrase string.</returns>
        private static string GenerateNounPhrase() => AppendOptional(GenerateComplexNoun, GeneratePrepPhrase);

        /// <summary>
        /// Generates a verb phrase using one of two production rules.
        /// ;VERB-PHRASE; -> ;CMPLX-VERB; | ;CMPLX-VERB; ;PREP-PHRASE;
        /// Delegates to AppendOptional for 50/50 random choice.
        /// </summary>
        /// <returns>The derived verb phrase string.</returns>
        private static string GenerateVerbPhrase() => AppendOptional(GenerateComplexVerb, GeneratePrepPhrase);

        /// <summary>
        /// Generates a prepositional phrase.
        /// ;PREP-PHRASE; -> ;PREP; ;CMPLX-NOUN;
        /// Always includes the fixed ;PREP; "with".
        /// </summary>
        /// <returns>The derived prep phrase string.</returns>
        private static string GeneratePrepPhrase() => $"{GeneratePreposition()} {GenerateComplexNoun()}";

        /// <summary>
        /// Generates a complex noun phrase.
        /// ;CMPLX-NOUN; -> ;ARTICLE; ;NOUN;
        /// Combines random article and noun with space.
        /// </summary>
        /// <returns>The derived complex noun string.</returns>
        private static string GenerateComplexNoun() => $"{GenerateArticle()} {GenerateNoun()}";

        /// <summary>
        /// Generates a complex verb using one of two production rules.
        /// ;CMPLX-VERB; -> ;VERB; | ;VERB; ;NOUN-PHRASE;
        /// Delegates to AppendOptional for 50/50 random choice (simple vs. transitive).
        /// </summary>
        /// <returns>The derived complex verb string.</returns>
        private static string GenerateComplexVerb() => AppendOptional(GenerateVerb, GenerateNounPhrase);

        /// <summary>
        /// Generates a random article from terminals.
        /// ;ARTICLE; -> a | the
        /// </summary>
        /// <returns>The selected article string.</returns>
        private static string GenerateArticle() => Choose(articles);

        /// <summary>
        /// Generates a random noun from terminals.
        /// ;NOUN; -> boy | girl | flower | bird
        /// </summary>
        /// <returns>The selected noun string.</returns>
        private static string GenerateNoun() => Choose(nouns);

        /// <summary>
        /// Generates a random verb from terminals.
        /// ;VERB; -> touches | likes | sees
        /// </summary>
        /// <returns>The selected verb string.</returns>
        private static string GenerateVerb() => Choose(verbs);

        /// <summary>
        /// Generates the fixed preposition.
        /// ;PREP; -> with
        /// No randomness needed (single terminal).
        /// </summary>
        /// <returns>The preposition string.</returns>
        private static string GeneratePreposition() => "with";

        /// <summary>
        /// Utility: Selects a random element from an array (uniform probability).
        /// Used for all terminal choices.
        /// </summary>
        /// <param name="options">Array of string options.</param>
        /// <returns>A randomly chosen string.</returns>
        private static string Choose(string[] options) => options[random.Next(options.Length)];

        /// <summary>
        /// Utility: Generates base text, then appends extra text with space ~50% of the time.
        /// Models binary productions in grammar (e.g., optional phrases).
        /// Probability: random.Next(2) == 0 (simple) vs. == 1 (extended); even split.
        /// </summary>
        /// <param name="baseGen">Func to generate base string.</param>
        /// <param name="extraGen">Func to generate optional extra string.</param>
        /// <returns>Concatenated string (base or base + " " + extra).</returns>
        private static string AppendOptional(Func<string> baseGen, Func<string> extraGen)
        {
            string baseText = baseGen();
            // 50% chance: ==0 keeps simple; ==1 appends. Matches grammar's non-determinism.
            return random.Next(2) == 0 ? baseText : $"{baseText} {extraGen()}";
        }
    }
}