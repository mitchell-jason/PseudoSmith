using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;

namespace CCValidator;

public class ValidatorWindow : Window
{
  private readonly TextBox _cardInput;
  private bool _reformatting;

  public ValidatorWindow()
  {
    Title = "Credit Card Validator";
    Width = 400;
    Height = 180;
    Background = SolidColorBrush.Parse("#F0F4F8");

    var instruction = new TextBlock
    {
      Text = "Enter the credit card number:",
      FontFamily = new FontFamily("Arial"),
      FontSize = 13,
      FontWeight = FontWeight.Bold,
      Foreground = SolidColorBrush.Parse("#2C3E50"),
    };

    _cardInput = new TextBox { Watermark = "e.g., 4532 1234 5678 9012" };
    _cardInput.TextChanged += OnCardInputChanged;

    var validateBtn = new Button { Content = "Validate", Width = 100 };
    validateBtn.Click += OnValidateClicked;

    var exitBtn = new Button { Content = "Exit", Width = 100 };
    exitBtn.Click += (_, _) => Close();

    var buttonPanel = new StackPanel
    {
      Orientation = Orientation.Horizontal,
      HorizontalAlignment = HorizontalAlignment.Center,
      Spacing = 20,
      Children = { validateBtn, exitBtn },
    };

    Content = new StackPanel
    {
      Orientation = Orientation.Vertical,
      Margin = new Thickness(20),
      Spacing = 12,
      Children = { instruction, _cardInput, buttonPanel },
    };
  }

  // Numbers only + live CC formatting. Caret anchored to digit-count and re-applied
  // asynchronously (Background priority) so it runs AFTER Avalonia's own caret handling.
  private void OnCardInputChanged(object? sender, TextChangedEventArgs e)
  {
    if (_reformatting) return;
    _reformatting = true;

    var current = _cardInput.Text ?? "";
    var caret = _cardInput.CaretIndex;
    var digitsLeft = current[..System.Math.Min(caret, current.Length)].Count(char.IsDigit);

    var raw = CardValidator.DigitsOnly(current);
    if (raw.Length > CardValidator.MaxCardLen) raw = raw[..CardValidator.MaxCardLen];
    var formatted = CardValidator.FormatCardDigits(raw);

    if (formatted != current)
      _cardInput.Text = formatted;

    Dispatcher.UIThread.Post(() =>
    {
      var pos = 0;
      var seen = 0;
      while (pos < formatted.Length && seen < digitsLeft)
      {
        if (char.IsDigit(formatted[pos])) seen++;
        pos++;
      }
      _cardInput.CaretIndex = pos;
      _reformatting = false;
    }, DispatcherPriority.Background);
  }

  private async void OnValidateClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
  {
    var raw = CardValidator.DigitsOnly((_cardInput.Text ?? "").Trim());
    if (raw.Length == 0)
    {
      await ShowMessage("Please enter a credit card number.", "Input Error");
      return;
    }
    if (raw.Length < CardValidator.MinCardLen || raw.Length > CardValidator.MaxCardLen)
    {
      await ShowMessage("Invalid credit card number", "Input Error");
      return;
    }
    await ShowMessage(
      CardValidator.ValidateCardNumber(raw)
        ? "The credit card number is VALID."
        : "The credit card number is INVALID.",
      CardValidator.ValidateCardNumber(raw) ? "Success" : "Failure");
  }

  // DISPLAY dialog box -> custom modal Window (Avalonia ships no MessageBox).
  private System.Threading.Tasks.Task ShowMessage(string messageText, string titleText)
  {
    var ok = new Button { Content = "OK", Width = 80, HorizontalAlignment = HorizontalAlignment.Center };
    var dialog = new Window
    {
      Title = titleText,
      Width = 300,
      Height = 130,
      WindowStartupLocation = WindowStartupLocation.CenterOwner,
      Content = new StackPanel
      {
        Margin = new Thickness(20),
        Spacing = 16,
        Children =
        {
          new TextBlock { Text = messageText, TextWrapping = TextWrapping.Wrap },
          ok,
        },
      },
    };
    ok.Click += (_, _) => dialog.Close();
    return dialog.ShowDialog(this);
  }
}
