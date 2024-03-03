using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Author: Joshua Silvagnoli
///
/// This script serves as a trial to investigate the feasibility of managing infinitely large numbers without succumbing to rounding errors attributable to floating-point precision limitations.
/// Upon reaching the threshold of 1 million, the number is reset to 0, and a counter is incremented. The counter determines the numerical suffix, with 'Million' assigned for a counter of 1, 'Billion' for a counter of 2, and so forth.
/// The operational principle lies in the systematic adjustment of the counter and suffix once the number surpasses 1 million. Consequently, the numerical value should never exceed 1 million, as the counter and suffix manage all manipulations.
/// In scientific notation, a million is represented as 1e6. In this system, the number is effectively raised to the power of the counter. Thus, any number associated with a counter of 1 represents millions, while a counter of 2 signifies billions, and so forth.
/// </summary>

public class LargeNumbers : MonoBehaviour
{
    public SerializableBigInteger num1 = new SerializableBigInteger(BigInteger.Parse("0"));
    public SerializableBigInteger num2 = new SerializableBigInteger(BigInteger.Parse("0"));

    private bool calculationCompleted = false;

    void Update()
    {
        if (!calculationCompleted)
        {
            Add();
            Subtract();
            Multiply();
            Divide();

            calculationCompleted = true;
        }
    }

    private void Add()
    {
        // Example of adding two large numbers
        BigInteger sum = num1.Value + num2.Value;
        Debug.Log("Sum: " + FormatLargeNumber(sum));
    }

    private void Subtract()
    {
        // Example of subtracting two large numbers
        BigInteger difference = num1.Value - num2.Value;
        Debug.Log("Difference: " + FormatLargeNumber(difference));
    }

    private void Multiply()
    {
        // Example of multiplying two large numbers
        BigInteger product = num1.Value * num2.Value;
        Debug.Log("Product: " + FormatLargeNumber(product));
    }

    private void Divide()
    {
        // Example of dividing two large numbers
        // Ensure the divisor is not zero
        if (num2.Value != BigInteger.Zero)
        {
            BigInteger quotient = num1.Value / num2.Value;
            Debug.Log("Quotient: " + FormatLargeNumber(quotient));
        }
        else
        {
            Debug.Log("Cannot divide by zero.");
        }
    }

    // Dictionary to map the exponent to the suffix
    private static readonly Dictionary<int, string> suffixes = new Dictionary<int, string>
    {
        {3, " Thousand"},
        {6, " Million"},
        {9, " Billion"},
        {12, " Trillion"},
        {15, " Quadrillion"},
        {18, " Quintillion"},
        {21, " Sextillion"},
        {24, " Septillion"},
        {27, " Octillion"},
        {30, " Nonillion"},
        {33, " Decillion"},
        {36, " Undecillion"},
        {39, " Duodecillion"},
        {42, " Tredecillion"},
        {45, " Quattuordecillion"},
        {48, " Quindecillion"},
        {51, " Sexdecillion"},
        {54, " Septendecillion"},
        {57, " Octodecillion"},
        {60, " Novemdecillion"},
        {63, " Vigintillion"},
        {66, " Unvigintillion"},
        {69, " Duovigintillion"},
        {72, " Trevigintillion"},
        {75, " Quattuorvigintillion"},
        {78, " Quinvigintillion"},
        {81, " Sexvigintillion"},
        {84, " Septenvigintillion"},
        {87, " Octovigintillion"},
        {90, " Novemvigintillion"},
        {93, " Trigintillion"},
        {96, " Untrigintillion"},
        {99, " Duotrigintillion"},
        {100, " Googol"},

    };

    public static string FormatLargeNumber(BigInteger number)
    {
        if (number == 0) return "0";

        // Determine the exponent based on the number of digits
        int numberOfDigits = number.ToString().Length;
        int exponent = (numberOfDigits - 1) / 3; // Calculate the exponent based on teh number of digits

        // Determine the appropriate suffix
        string suffix = "";
        if (suffixes.TryGetValue(exponent * 3, out string foundSuffix))
        {
            suffix = foundSuffix;
        }

        // Convert the BigInteger to a double to perform division and rounding
        double scaledNumber = (double)number / Math.Pow(1000, exponent);

        // Round the number to three decimal places
        scaledNumber = Math.Round(scaledNumber, 2);

        // Format the number with the suffix
        return $"{scaledNumber}{suffix}";
    }
}

[Serializable]
public class SerializableBigInteger
{
    [SerializeField]
    private string bigIntegerValue;

    public BigInteger Value
    {
        get { return BigInteger.Parse(bigIntegerValue); }
        set { bigIntegerValue = value.ToString(); }
    }

    public SerializableBigInteger(BigInteger value)
    {
        Value = value;
    }
}

[CustomPropertyDrawer(typeof(SerializableBigInteger))]
public class BigIntegerDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty bigIntegerValue = property.FindPropertyRelative("bigIntegerValue");
        string value = EditorGUI.TextField(position, label, bigIntegerValue.stringValue);

        try
        {
            BigInteger.Parse(value); // Check if the value is a valid BigInteger
            bigIntegerValue.stringValue = value;
        }
        catch (FormatException)
        {
            // Handle the case where the value is not a valid BigInteger
            EditorGUI.HelpBox(position, "Invalid BigInteger value.", MessageType.Error);
        }
    }
}