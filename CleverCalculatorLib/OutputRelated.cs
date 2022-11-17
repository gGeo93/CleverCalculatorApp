namespace CleverCalculatorLib;

public class OutputRelated
{

    #region MainOperation
    public void MakeNumericalOperationWithPriority(string currentOperationState, List<string> outputItems)
    {
        if (currentOperationState.Length <= 2 || (currentOperationState.StartsWith("-") && currentOperationState.Length <= 3)) return;

        outputItems.Add(currentOperationState);
        
        string leftPart = string.Empty;
        string rightPart = string.Empty;

        char operationSymbol = 'f';
        if (currentOperationState.Substring(1).FirstOrDefault(chr => chr == '/' || chr == 'x') != '\0') operationSymbol = currentOperationState.Substring(1).FirstOrDefault(chr => chr == '/' || chr == 'x');
        else if (currentOperationState.Substring(1).FirstOrDefault(chr => chr == '-' || chr == '+') != '\0') operationSymbol = currentOperationState.Substring(1).FirstOrDefault(chr => chr == '+' || chr == '-');
        else if (operationSymbol == 'f')
        {
            outputItems.Add(currentOperationState);
            return;
        }
        float possibleInfinity = 0;
        Operation(leftPart, rightPart,ref currentOperationState, operationSymbol, outputItems, ref possibleInfinity);
    }
    #endregion

    #region RecursivelyCalledProcedure
    private void Operation(string leftPart, string rightPart,ref string currentOperationState, char operationSymbol, List<string> outputItems, ref float possibleInfinity)
    {
        int indexOfOperatorSymbol = PositionOfOperator(currentOperationState, operationSymbol);

        bool rightPartIsNegative = CheckIfRightPartIsNegative(ref currentOperationState, operationSymbol, indexOfOperatorSymbol);

        string firstCurrentOperationState = FirstCurrentOperationState(currentOperationState, indexOfOperatorSymbol);

        int leftLimit = FindTheLeftLimitOfCurrentOperationState(firstCurrentOperationState);

        leftPart = SpecifyTheLeftPartOfTheOperation(currentOperationState, indexOfOperatorSymbol, firstCurrentOperationState, ref leftLimit);

        rightPart = SpecifyTheRightPartOfTheOperation(currentOperationState, indexOfOperatorSymbol);

        float result = ExecuteTheCurrentOperation(leftPart, rightPart, operationSymbol, rightPartIsNegative);

        bool isTryingToDivideByZero = CheckIfThereIsAdivisionWithZero(rightPart, operationSymbol);

        if (isTryingToDivideByZero)
        {
            TryingToDivideByZeroException(outputItems);
            possibleInfinity = float.Parse(outputItems[outputItems.Count - 1]);
            return;
        }
        currentOperationState = ReplaceThePreviousOperationWithTheResult(leftPart, rightPart, currentOperationState, operationSymbol, result);

        DoesTheOperationContinue(currentOperationState, leftPart, rightPart, outputItems, ref possibleInfinity);
    }
    #endregion

    #region SubMethods
    private int PositionOfOperator(string currentOperationState, char operationSymbol)
    {
        return currentOperationState.Substring(1).IndexOf(operationSymbol) + 1;
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
                    result = float.Parse(leftPart) / float.Parse(rightPart);
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

    private void TryingToDivideByZeroException(List<string> outputItems)
    {
        float zero = 0;
        var infinity = (1 / zero).ToString();
        outputItems.Add(infinity);
    }

    private string ReplaceThePreviousOperationWithTheResult(string leftPart, string rightPart, string currentOperationState, char operationSymbol, float result)
    {
        string wholeOperation = leftPart + operationSymbol + rightPart;

        currentOperationState = currentOperationState.Replace(wholeOperation, result.ToString());
        return currentOperationState;
    }

    private void DoesTheOperationContinue(string currentOperationState, string leftPart, string rightPart, List<string> outputItems, ref float possibleInfinity)
    {
        char operationSymbol = 'f';
        if (currentOperationState.Substring(1).FirstOrDefault(chr => chr == '/' || chr == 'x') != '\0')
        {
            operationSymbol = currentOperationState.Substring(1).FirstOrDefault(chr => chr == '/' || chr == 'x');
        }

        else if (currentOperationState.Substring(1).FirstOrDefault(chr => chr == '-' || chr == '+') != '\0')
        {
            operationSymbol = currentOperationState.Substring(1).FirstOrDefault(chr => chr == '+' || chr == '-');
        }
        else if (operationSymbol == 'f')
        {
            outputItems.Add(currentOperationState);
            return;
        }

        outputItems.Add(currentOperationState);

        Operation(leftPart, rightPart,ref currentOperationState, operationSymbol, outputItems, ref possibleInfinity);
    }
    #endregion
}
