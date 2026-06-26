```text
/*
 ============================================================
 TARGET_LANGUAGE    : Python
 TARGET_PLATFORM    : linux
 TARGET_UI_FRAMEWORK: Tkinter
 NAMING_CONVENTIONS : PascalCase
 INDENTATION        : tabs
 DELIVERY_MODE      : inline
 ============================================================
 */

MODULE "CCValidator"
  // ------------------------------------------
  // 1. THE CREDIT CARD VALIDATION LOGIC (Luhn)
  // ------------------------------------------
  PROCEDURE validateCardNumber(cardNumberString: STRING) -> BOOLEAN
    LISTOFVARIABLES { 
        cleanString: STRING, 
        reversedDigits: STRING,
        totalSum: INT,
        currentDigit: INT,
        doubledDigit: INT,
        i: INT
    }
    START
        // Remove spaces and hyphens, then reverse the string
        cleanString = remove spaces and hyphens from cardNumberString
        reversedDigits = reverse the string cleanString
        totalSum = 0

        // Loop through each character by position
        FOR i FROM 0 TO length of reversedDigits minus 1
            // Get the numeric value of this character (e.g., '5' becomes 5)
            currentDigit = INT(reversedDigits[i])

            // Double every second digit (odd positions)
            IF i is odd THEN
                doubledDigit = currentDigit * 2

                // If doubled is more than 9, add its digits (same as subtract 9)
                IF doubledDigit > 9 THEN
                    doubledDigit = doubledDigit - 9
                END IF

                totalSum = totalSum + doubledDigit
            ELSE
                totalSum = totalSum + currentDigit
            END IF
        END FOR

        // Valid if the sum is divisible by 10
        IF totalSum can be divided evenly by 10 THEN
            RETURN TRUE
        ELSE
            RETURN FALSE
        END IF
    END

  // ------------------------------------------
  // 2. MAIN PROCEDURE TO SHOW THE WINDOW
  // ------------------------------------------
  PROCEDURE ShowValidatorWindow()
    LISTOFVARIABLES { 
        isValid: BOOLEAN,
        cardNumber: STRING,
        resultMessage: STRING
    }
    START
        // Build the main window
        CONTROL WINDOW "ccWindow"
            TITLE = "Credit Card Validator"
            WIDTH = 400
            HEIGHT = 180
            BACKCOLOUR = "#F0F4F8"

            CONTROL PANEL "MainPanel"
                LAYOUT = VERTICAL
                PADDING = 20
                SPACING = 12

                // Instruction Label
                CONTROL LABEL "InstructionLabel"
                    TEXT = "Enter the credit card number:"
                    FONT = "Arial, 10pt, Bold"
                    FORECOLOUR = "#2C3E50"
                CONTROLEND

                // Textbox for card number input
                CONTROL TEXTBOX "CardInput"
                    WIDTH = "fill"
                    HEIGHT = 30
                    PLACEHOLDER = "e.g., 4532 1234 5678 9012"
                    Textbox should only allow numbers to be entered and be formatted in a CC format as it is entered.
                CONTROLEND

                // Button Panel (Horizontal layout for buttons)
                CONTROL PANEL "ButtonPanel"
                    LAYOUT = HORIZONTAL
                    ALIGN = CENTER
                    SPACING = 20

                    // Validate Button
                    CONTROL BUTTON "ValidateBtn"
                        TEXT = " Validate "
                        WIDTH = 100
                        HEIGHT = 30
                    CONTROLEND

                    // Exit Button
                    CONTROL BUTTON "ExitBtn"
                        TEXT = " Exit "
                        WIDTH = 100
                        HEIGHT = 30
                    CONTROLEND

                CONTROLEND
            CONTROLEND
        CONTROLEND

        // ----- EVENT HANDLERS -----
        // Validate Button Click
        WHEN_CLICKED(ValidateBtn)
        START
            // Retrieve the text from the input box
            cardNumber = GET_TEXT(CardInput)

            // Trim spaces just in case
            cardNumber = trim leading and trailing spaces from cardNumber
            Check the length the credit card number and ensure it is the correct length otherwise show error "Invalid credit card number"
            // Ensure input is not empty
            IF length of cardNumber == 0 THEN
                CALL ShowMessage("Please enter a credit card number.", "Input Error")
                RETURN
            END IF

            // Perform validation
            isValid = CALL validateCardNumber(cardNumber)

            // Prepare and display the result
            IF isValid == TRUE THEN
                resultMessage = "The credit card number is VALID."
                CALL ShowMessage(resultMessage, "Success")
            ELSE
                resultMessage = "The credit card number is INVALID."
                CALL ShowMessage(resultMessage, "Failure")
            END IF
        END

        // Exit Button Click
        WHEN_CLICKED(ExitBtn)
        START
            CLOSE_WINDOW(ccWindow)
            EXIT_APPLICATION()
        END

        // Show the window (blocks until closed)
        SHOW(ccWindow)
    END

  // ------------------------------------------
  // 3. HELPER PROCEDURE FOR MESSAGE BOXES
  // ------------------------------------------
  PROCEDURE ShowMessage(messageText: STRING, titleText: STRING)
    START
        // AI maps to MsgBox in VBA
        DISPLAY dialog box with messageText and titleText
    END

END MODULE

// ----- ENTRY POINT -----
PROCEDURE Main
START
    CALL ShowValidatorWindow()
END
```
