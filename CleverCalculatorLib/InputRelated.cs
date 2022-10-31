using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace CleverCalculatorLib;

public class InputRelated
{
    #region MainInputMethod
    public string InputUpdated(char inputSymbol, string inputText)
    {
        if (IsTryingToAddAcommaAtThebeginningOfANumber(inputSymbol, inputText))
        {
            return inputText += "0,";
        }
        if ((IsInputEmpty(inputText) 
            && IsOperatorMinus(inputSymbol, inputText)) 
            || AreTheTwoLastCharactersAnOperator(inputSymbol, inputText))
        {
            return inputText;
        }
        if (inputText.Length == 0 && inputSymbol == '-') return "-";
        if (inputText.StartsWith("-") && inputSymbol == '-') return "";
        if (inputText.Length == 0 && inputSymbol == '+') return "";
        if (inputText.EndsWith("-") && inputSymbol == '+')
        {
            return inputText;
        }
        if (inputText.EndsWith('+') && inputSymbol == '-')
        {
            inputText = inputText.Remove(inputText.Length - 1, 1);
            return inputText + '-';
        }
        if (inputText.EndsWith('-') && inputSymbol == '-')
        {
            inputText = inputText.Remove(inputText.Length - 1, 1);
            return inputText + '+';
        }
        if (inputText.EndsWith('+') && inputSymbol == '+')
        {
            return inputText;
        }
        if (IsCharAnOperator(inputSymbol) || inputSymbol == '-')
        {
            return LastNumberIsZero(inputSymbol, inputText);
        }
        if (TryingToAddTwoMinusAtTheBeggining(inputSymbol, inputText))
        {
            return "";
        }
        if (TryingToAddAsecondCommaToTheSameNumber(inputSymbol, inputText))
        {
            return inputText;
        }
        if (!TheNumberOfDecimalPlacesAreBelowNine(inputSymbol, inputText)) 
        {
            return inputText;
        }
        return inputText + inputSymbol;
    }
    #endregion
    
    #region PrivateHelpingMethods
    private bool IsInputEmpty(string currentInputText) => currentInputText.Length == 0;
    private bool IsOperatorMinus(char input, string currentInputText) => currentInputText.Length > 0 && input == '-';
    private bool AreTheTwoLastCharactersAnOperator(char lastChar, string currentInputText) 
    {
        if(currentInputText.Length == 0) return false;
        
        char lastCharOfInputText = currentInputText[currentInputText.Length - 1];

        return (IsCharAnOperator(lastChar))
                &&
                (IsCharAnOperator(lastCharOfInputText)
                || IsCharAnOperator(lastCharOfInputText, '-')
                || IsCharAnOperator(lastCharOfInputText,'+'));
    }
    private bool TryingToAddAsecondCommaToTheSameNumber(char lastChar, string currentInputText) 
    {
        if (lastChar == ',')
        {
            int indexOfLastOperator = currentInputText.LastIndexOfAny(new char[] { '/', 'x', '+', '-' });
            if (indexOfLastOperator == -1) indexOfLastOperator = 0;
            string currentText = currentInputText.Substring(indexOfLastOperator);
            int numberOfCommas = currentInputText.Count(chr => chr == ',');
            return numberOfCommas > 0;
        }
        return false;
    }
    private string LastNumberIsZero(char lastChar, string currentInputText) 
    {
        int indexOfLastOperator = currentInputText.LastIndexOfAny(new char[] { '/', 'x', '+', '-' });

        string currentText = currentInputText.Substring(indexOfLastOperator + 1);
        if (currentText.All(chr => chr == ',' || chr == '0') && currentText != "")
            return currentInputText.Replace(currentText, "0" + lastChar);
        else
        {
            return currentInputText + lastChar;
        }
    }
    private bool IsTryingToAddAcommaAtThebeginningOfANumber(char lastChar, string currentInputText) 
    {
        if (lastChar == ',')
        {
            int indexOfLastOperator = currentInputText.LastIndexOfAny(new char[] { '/', 'x', '+', '-' });
            if (indexOfLastOperator == -1) indexOfLastOperator = 0;
            string currentText = currentInputText.Substring(indexOfLastOperator);
            if (currentText.Length <= 1) return true;
        }
        return false;
    }
    private bool IsCharAnOperator(char character) => character == '.' || character == '/' || character == 'x' || character == '+';
    private bool IsCharAnOperator(char character, char minus = '-') => character == minus;
    private bool TryingToAddTwoMinusAtTheBeggining(char inputSymbol, string currentInputText) => currentInputText.Length == 1 && currentInputText[0] == '-' && inputSymbol == '-';
    private bool TheNumberOfDecimalPlacesAreBelowNine(char inputSymbol, string currentInputText) 
    {
        return currentInputText.Substring(currentInputText.LastIndexOf(',')).Length < 10;
    }
    #endregion
}
