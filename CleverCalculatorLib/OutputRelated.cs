using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CleverCalculatorLib;

public class OutputRelated
{
    #region OperationRelatedMainProcedure
    public void MakeNumericalOperationWithPriority(string currentOperationState, List<string> outputItems, ref string propableExceptionMessage)
    {
        int lastInputChr = currentOperationState[currentOperationState.Length - 1];
        if (lastInputChr == '/' || lastInputChr == 'x' || lastInputChr == '+' || lastInputChr == '-' || lastInputChr == ',') 
        {
            propableExceptionMessage = "The last character of the operation must be a digit not an operator";
            return;
        }

        outputItems.Add(currentOperationState);
        string leftPart = string.Empty;
        string rightPart = string.Empty;
        bool isStartingWithMinus = false;

        char operationSymbol = '/';
        if (currentOperationState.StartsWith('-')) 
        {
            isStartingWithMinus = true;
            currentOperationState = currentOperationState.Remove(0, 1);
        } 
        
        if (currentOperationState.Any(chr => chr == '/')) operationSymbol = '/';
        else if (currentOperationState.Any(chr => chr == 'x')) operationSymbol = 'x';
        else if (currentOperationState.Any(chr => chr == '+')) operationSymbol = '+';
        else operationSymbol = '-';
        
        Operation(leftPart, rightPart, currentOperationState, operationSymbol, outputItems, isStartingWithMinus, ref propableExceptionMessage);
    }
    #endregion
    
    #region RecursivelyCalledProcedure
    private void Operation(string leftPart, string rightPart, string currentOperationState, char operationSymbol, List<string> outputItems, bool isStartingWithMinus, ref string propableExceptionMessage)
    {
        int indexOfOperatorSymbol = PositionOfOperator(currentOperationState, operationSymbol);

        bool rightPartIsNegative = CheckIfRightPartIsNegative(ref currentOperationState, operationSymbol, indexOfOperatorSymbol);

        string firstCurrentOperationState = FirstCurrentOperationState(currentOperationState, indexOfOperatorSymbol);

        int leftLimit = FindTheLeftLimitOfCurrentOperationState(firstCurrentOperationState);

        leftPart = SpecifyTheLeftPartOfTheOperation(currentOperationState, indexOfOperatorSymbol, firstCurrentOperationState, ref leftLimit);

        MakeLeftPartNegative(ref leftPart, ref currentOperationState, isStartingWithMinus, ref indexOfOperatorSymbol);

        rightPart = SpecifyTheRightPartOfTheOperation(currentOperationState, indexOfOperatorSymbol);

        float result = ExecuteTheCurrentOperation(leftPart, rightPart, operationSymbol, rightPartIsNegative);

        bool isTryingToDivideByZero = CheckIfThereIsAdivisionWithZero(rightPart, operationSymbol);

        if (isTryingToDivideByZero)
        {
            propableExceptionMessage = TryingToDivideByZeroException();
            return;
        }

        currentOperationState = ReplaceThePreviousOperationWithTheResult(leftPart, rightPart, currentOperationState, operationSymbol, result);

        currentOperationState = ReplaceThdifferentCombinationsOfMinusAndPlus(currentOperationState);

        outputItems.Add(currentOperationState);

        isStartingWithMinus = IsTheNewOperationStartsWithAminus(currentOperationState, isStartingWithMinus);
        
        ConsequencesOfStartingWithMinus(ref currentOperationState, ref isStartingWithMinus);

        DoesTheOperationContinue(currentOperationState, leftPart, rightPart, outputItems, isStartingWithMinus, ref propableExceptionMessage);    
    }
    #endregion

    #region SubMethods
    private int PositionOfOperator(string currentOperationState, char operationSymbol)
    {
        return currentOperationState.IndexOf(operationSymbol);
    }
    private bool CheckIfRightPartIsNegative(ref string currentOperationState, char operationSymbol, int indexOfOperatorSymbol)
    {
        bool rightPartIsNegative = false;

        if ((operationSymbol == 'x' || operationSymbol == '/')
            && indexOfOperatorSymbol < currentOperationState.Length - 1
            && currentOperationState[indexOfOperatorSymbol + 1] == '-')
        {
            rightPartIsNegative = true;
            currentOperationState = currentOperationState.Remove(indexOfOperatorSymbol + 1, 1);
        }

        return rightPartIsNegative;
    }

    private string FirstCurrentOperationState(string currentOperationState, int indexOfOperatorSymbol)
    {
        return currentOperationState.Substring(0, indexOfOperatorSymbol);
    }

    private int FindTheLeftLimitOfCurrentOperationState(string firstCurrentOperationState)
    {
        return firstCurrentOperationState.LastIndexOf(firstCurrentOperationState.LastOrDefault(chr => chr == '/' || chr == 'x' || chr == '+' || chr == '-'));
    }

    private string SpecifyTheLeftPartOfTheOperation(string currentOperationState, int indexOfOperatorSymbol, string firstCurrentOperationState, ref int leftLimit)
    {
        string leftPart;
        if (leftLimit == 0)
        {
            leftPart = firstCurrentOperationState.Substring(leftLimit, firstCurrentOperationState.Length);
        }
        else if (currentOperationState.Count(chr => chr == '/' || chr == 'x' || chr == '+' || chr == '-') == 1)
        {
            leftLimit = 0;
            leftPart = firstCurrentOperationState.Substring(leftLimit, indexOfOperatorSymbol - leftLimit);
        }
        else
        {
            leftPart = firstCurrentOperationState.Substring(leftLimit + 1, indexOfOperatorSymbol - leftLimit - 1);
        }

        return leftPart;
    }

    private void MakeLeftPartNegative(ref string leftPart, ref string currentOperationState, bool isStartingWithMinus, ref int indexOfOperatorSymbol)
    {
        if (isStartingWithMinus && leftPart.Count(op => op == '/' || op == 'x' || op == '+' || op == '-') == 0)
        {
            leftPart = leftPart.Insert(0, "-");
            currentOperationState = currentOperationState.Insert(0, "-");
            indexOfOperatorSymbol += 1;
        }
    }

    private string SpecifyTheRightPartOfTheOperation(string currentOperationState, int indexOfOperatorSymbol)
    {
        string rightPart;
        List<char> rightCurrentOperation = currentOperationState.ToList<char>();

        int rightLimit = rightCurrentOperation.FindIndex(indexOfOperatorSymbol + 1, chr => chr == '/' || chr == 'x' || chr == '-' || chr == '+');
        if (rightLimit == -1) rightLimit = currentOperationState.Length;

        rightPart = currentOperationState.Substring(indexOfOperatorSymbol + 1, rightLimit - indexOfOperatorSymbol - 1);
        return rightPart;
    }

    private float ExecuteTheCurrentOperation(string leftPart, string rightPart, char operationSymbol, bool rightPartIsNegative)
    {
        float result = 0f;

        if (!string.IsNullOrEmpty(leftPart) && !string.IsNullOrEmpty(rightPart))
        {
            switch (operationSymbol)
            {
                case '/':
                    result = (float.Parse(leftPart) / float.Parse(rightPart));
                    if (rightPartIsNegative) result = -result;
                    break;
                case 'x':
                    result = (float.Parse(leftPart) * float.Parse(rightPart));
                    if (rightPartIsNegative) result = -result;
                    break;
                case '+': result = float.Parse(leftPart) + float.Parse(rightPart); break;
                case '-': result = float.Parse(leftPart) - float.Parse(rightPart); break;
                default: throw new Exception($"{operationSymbol} is not an operator");
            }
        }

        return result;
    }

    private bool CheckIfThereIsAdivisionWithZero(string rightPart, char operationSymbol)
    {
        return operationSymbol == '/' && rightPart == "0";
    }

    private string TryingToDivideByZeroException()
    {
        return "Cannot divide by zero";
    }

    private string ReplaceThePreviousOperationWithTheResult(string leftPart, string rightPart, string currentOperationState, char operationSymbol, float result)
    {
        string wholeOperation = leftPart + operationSymbol + rightPart;

        currentOperationState = currentOperationState.Replace(wholeOperation, result.ToString());
        return currentOperationState;
    }

    private string ReplaceThdifferentCombinationsOfMinusAndPlus(string currentOperationState)
    {
        if (currentOperationState.Contains("--")) currentOperationState = currentOperationState.Replace("--", "+");
        if (currentOperationState.Contains("+-")) currentOperationState = currentOperationState.Replace("+-", "-");
        if (currentOperationState.Contains("-+")) currentOperationState = currentOperationState.Replace("-+", "-");
        return currentOperationState;
    }

    private bool IsTheNewOperationStartsWithAminus(string currentOperationState, bool isStartingWithMinus)
    {
        if (isStartingWithMinus
                    &&
                    currentOperationState.Count(symbol =>
                       symbol == '/'
                    || symbol == 'x'
                    || symbol == '+'
                    || symbol == '-') == 1)
        {
            isStartingWithMinus = false;
        }

        return isStartingWithMinus;
    }

    private void ConsequencesOfStartingWithMinus(ref string currentOperationState, ref bool isStartingWithMinus)
    {
        if (currentOperationState.StartsWith('-'))
        {
            currentOperationState = currentOperationState.Remove(0, 1);
            isStartingWithMinus = true;
        }
    }

    private void DoesTheOperationContinue(string currentOperationState, string leftPart, string rightPart, List<string> outputItems, bool isStartingWithMinus, ref string propableExceptionMessage)
    {
        if (currentOperationState.Any(chr => chr == '/'))
            Operation(leftPart, rightPart, currentOperationState, '/', outputItems, isStartingWithMinus, ref propableExceptionMessage);
        else if (currentOperationState.Any(chr => chr == 'x'))
            Operation(leftPart, rightPart, currentOperationState, 'x', outputItems, isStartingWithMinus, ref propableExceptionMessage);
        else if (currentOperationState.Any(chr => chr == '+'))
            Operation(leftPart, rightPart, currentOperationState, '+', outputItems, isStartingWithMinus, ref propableExceptionMessage);
        else if (currentOperationState.Any(chr => chr == '-'))
            Operation(leftPart, rightPart, currentOperationState, '-', outputItems, isStartingWithMinus, ref propableExceptionMessage);
    }
    #endregion
}
