using System.Linq;

namespace CCValidator;

public static class CardValidator
{
  // Resolved (Step 3.3): ISO/IEC 7812 valid length range = 13..19 digits.
  public const int MinCardLen = 13;
  public const int MaxCardLen = 19;

  public static bool ValidateCardNumber(string cardNumberString)
  {
    var cleanString = cardNumberString.Replace(" ", "").Replace("-", "");
    // Defensive guard: digits only.
    if (cleanString.Length == 0 || !cleanString.All(char.IsDigit))
      return false;

    var reversedDigits = new string(cleanString.Reverse().ToArray());
    var totalSum = 0;
    for (var i = 0; i < reversedDigits.Length; i++)
    {
      var currentDigit = reversedDigits[i] - '0';
      if (i % 2 == 1)
      {
        var doubledDigit = currentDigit * 2;
        if (doubledDigit > 9) doubledDigit -= 9;
        totalSum += doubledDigit;
      }
      else
      {
        totalSum += currentDigit;
      }
    }
    return totalSum % 10 == 0;
  }

  public static string DigitsOnly(string source)
    => new string(source.Where(char.IsDigit).ToArray());

  public static string FormatCardDigits(string rawDigits)
    => string.Join(" ", Enumerable
        .Range(0, (rawDigits.Length + 3) / 4)
        .Select(g => rawDigits.Substring(g * 4, System.Math.Min(4, rawDigits.Length - g * 4))));
}
