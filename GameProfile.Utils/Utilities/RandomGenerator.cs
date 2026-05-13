namespace GameProfile.Utils.Utilities;

public static class RandomGenerator
{
    private static readonly string[] FirstNames =
    {
        "Alex", "Jordan", "Casey", "Morgan", "Riley", "Avery", "Quinn", "Reese",
        "Skylar", "Cameron", "Drew", "Emerson", "Finley", "Hayden", "Kendall", "Logan"
    };

    private static readonly string[] LastNames =
    {
        "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis",
        "Rodriguez", "Martinez", "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson", "Thomas"
    };

    private static readonly string[] Countries =
    {
        "USA", "Canada", "UK", "Germany", "France", "Spain", "Italy", "Brazil",
        "Japan", "Australia", "Mexico", "Sweden", "Poland", "Netherlands"
    };

    public static int GetInt(int minValue, int maxValue) =>
        Random.Shared.Next(minValue, maxValue + 1);

    public static string GetFullName() =>
        $"{Pick(FirstNames)} {Pick(LastNames)}";

    public static string GetCountry() => Pick(Countries);

    public static string GetGender() => Random.Shared.Next(2) == 0 ? "male" : "female";

    private static string Pick(string[] options) => options[Random.Shared.Next(options.Length)];
}
