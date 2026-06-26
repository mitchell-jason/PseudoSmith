#!/usr/bin/env python3
# ============================================================
# TARGET_LANGUAGE    : Python
# TARGET_PLATFORM    : linux
# TARGET_UI_FRAMEWORK: Tkinter
# NAMING_CONVENTIONS : PascalCase
# INDENTATION        : tabs
# DELIVERY_MODE      : inline
# ============================================================
"""Module: CCValidator"""

import tkinter as tk
from tkinter import messagebox

# Resolved (Step 3.3): ISO/IEC 7812 valid length range = 13..19 digits.
MIN_CARD_LEN = 13
MAX_CARD_LEN = 19


def ValidateCardNumber(CardNumberString):
	CleanString = CardNumberString.replace(" ", "").replace("-", "")
	# Defensive guard: INT(reversedDigits[i]) is non-total over arbitrary input.
	if not CleanString.isdigit():
		return False
	ReversedDigits = CleanString[::-1]
	TotalSum = 0
	for i in range(len(ReversedDigits)):
		CurrentDigit = int(ReversedDigits[i])
		if i % 2 == 1:
			DoubledDigit = CurrentDigit * 2
			if DoubledDigit > 9:
				DoubledDigit = DoubledDigit - 9
			TotalSum = TotalSum + DoubledDigit
		else:
			TotalSum = TotalSum + CurrentDigit
	return TotalSum % 10 == 0


def DigitsOnly(Source):
	return "".join(Ch for Ch in Source if Ch.isdigit())


def FormatCardDigits(RawDigits):
	return " ".join(RawDigits[i:i + 4] for i in range(0, len(RawDigits), 4))


def ShowMessage(MessageText, TitleText):
	messagebox.showinfo(TitleText, MessageText)


def ShowValidatorWindow():
	CcWindow = tk.Tk()
	CcWindow.title("Credit Card Validator")
	CcWindow.geometry("400x180")
	CcWindow.configure(bg="#F0F4F8")

	MainPanel = tk.Frame(CcWindow, bg="#F0F4F8", padx=20, pady=20)
	MainPanel.pack(fill="both", expand=True)

	InstructionLabel = tk.Label(
		MainPanel,
		text="Enter the credit card number:",
		font=("Arial", 10, "bold"),
		fg="#2C3E50",
		bg="#F0F4F8",
	)
	InstructionLabel.pack(anchor="w", pady=(0, 12))

	# Numbers only + live CC formatting. Caret anchored to digit-count (a stable token),
	# not a raw char index, and reset via after_idle so it runs AFTER Tk's own key handling.
	CardInput = tk.Entry(MainPanel)
	CardInput.pack(fill="x", pady=(0, 12), ipady=4)

	ReentryGuard = {"busy": False}

	def ReformatCardField(_event=None):
		if ReentryGuard["busy"]:
			return
		ReentryGuard["busy"] = True
		Current = CardInput.get()
		Caret = CardInput.index(tk.INSERT)
		DigitsLeft = sum(1 for Ch in Current[:Caret] if Ch.isdigit())
		Raw = DigitsOnly(Current)[:MAX_CARD_LEN]
		Formatted = FormatCardDigits(Raw)
		if Formatted != Current:
			CardInput.delete(0, tk.END)
			CardInput.insert(0, Formatted)

		def RestoreCaret():
			NewPos = 0
			Seen = 0
			while NewPos < len(Formatted) and Seen < DigitsLeft:
				if Formatted[NewPos].isdigit():
					Seen += 1
				NewPos += 1
			CardInput.icursor(NewPos)
			ReentryGuard["busy"] = False

		CardInput.after_idle(RestoreCaret)

	CardInput.bind("<KeyRelease>", ReformatCardField)

	ButtonPanel = tk.Frame(MainPanel, bg="#F0F4F8")
	ButtonPanel.pack()

	def OnValidateClicked():
		RawDigits = DigitsOnly(CardInput.get().strip())
		if len(RawDigits) == 0:
			ShowMessage("Please enter a credit card number.", "Input Error")
			return
		if len(RawDigits) < MIN_CARD_LEN or len(RawDigits) > MAX_CARD_LEN:
			ShowMessage("Invalid credit card number", "Input Error")
			return
		if ValidateCardNumber(RawDigits):
			ShowMessage("The credit card number is VALID.", "Success")
		else:
			ShowMessage("The credit card number is INVALID.", "Failure")

	def OnExitClicked():
		CcWindow.destroy()

	ValidateBtn = tk.Button(ButtonPanel, text="Validate", width=12, command=OnValidateClicked)
	ValidateBtn.pack(side="left", padx=10)

	ExitBtn = tk.Button(ButtonPanel, text="Exit", width=12, command=OnExitClicked)
	ExitBtn.pack(side="left", padx=10)

	CcWindow.mainloop()


def Main():
	ShowValidatorWindow()


if __name__ == "__main__":
	Main()
