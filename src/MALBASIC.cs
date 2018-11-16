using System;
using System.IO;
using System.Collections.Generic;


// use "mcs MALBASIC.cs" to compile

namespace MALBASIC {
public class MALBASICInterpreter
{   

    static private string[] lines;

    static private string[] argsIn;

    //public static MALBASICInterpreter interpreter;
    static private Dictionary<int, string> variables = new Dictionary<int, string>();
    static private Dictionary<int, int> gotos = new Dictionary<int, int>();

    static private int currentLine = -1;

    static private string file = "print_to_ten.malbasic";
    static public void Main (string[] args)
    {

        argsIn = args;
        string fileToRead = args[0];
        //string fileToRead = file;

        string fileText = System.IO.File.ReadAllText(fileToRead);
        

        lines = fileText.Split(
            new[] {Environment.NewLine},
            StringSplitOptions.None
        );


        for (int i = 0; i < lines.Length - 1; i++) {
            if (lines[i].Split(null)[0] == "MRK") {
                FuncMRK(lines[i].Split(null), i);
            }
        }

        //lines = new string[] { "Hello", "World"};

        //string[] lines = fileText.Split(new string[] {"\\n"}, StringSplitOptions.None);

        //System.Console.WriteLine(lines);

        EvaluateCode();

    }

    static public void EvaluateCode() {
        for (currentLine = 0; currentLine < lines.Length; currentLine++) {
            EvaluateLine(lines[currentLine]);
        }
    }

    static public void EvaluateLine(string line) {
        string[] partsOfLine = line.Split(null);
        string instructionName = partsOfLine[0];


        //System.Console.WriteLine("about to check the instruction on this line: " + line);

        switch (instructionName) {
            case "SET":
                FuncSET(partsOfLine);
                break;
            case "ADD":
                FuncADD(partsOfLine);
                break;
            case "SUB":
                FuncSUB(partsOfLine);
                break;
            // case "MRK":
            //     FuncMRK(partsOfLine);
            //     break;
            case "MOV":
                FuncMOV(partsOfLine);
                break;
            case "PRT":
                FuncPRT(partsOfLine);
                break;
            case "PRS":
                FuncPRS(partsOfLine);
                break;
            case "EQU":
                FuncEQU(partsOfLine);
                break;
            case "NEQ":
                FuncNEQ(partsOfLine);
                break;
            case "MOR":
                FuncMOR(partsOfLine);
                break;
            case "LES":
                //System.Console.WriteLine("blarb");
                FuncLES(partsOfLine);
                break;
            case "///":
                break;
        }
        return;
    }

    // static void NextLine() {
    //     currentLine++;

    //     // System.Console.WriteLine(variables);
    //     // System.Console.WriteLine(gotos);
    //     //System.Console.WriteLine("Current line is " + lines[currentLine].ToString());

    //     //System.Console.WriteLine("Current line is " + currentLine.ToString() + ", there are " + (lines.Length - 1).ToString() + " total");
    //     if (currentLine > lines.Length - 1) {
    //         //System.Console.WriteLine("All done!");
    //         return;
    //     }

    //     //System.Console.WriteLine("Is this where the fuckening is happening?");
    //     //EvaluateLine(lines[currentLine]);
    // }

    static void FuncSET(string[] parts) {
        string destination = parts[1];
        string value = "";
        for (int g = 1; g < parts.Length; g++) {
            value += ParseNumber(parts[g]);
        }

        int destID = ParseVariableName(destination);

        string realValue = ParseNumber(value);

        if (variables.ContainsKey(destID)) {
            variables[destID] = realValue;
        } else {
            variables.TryAdd(destID, realValue);
        }
        

        //System.Console.WriteLine("SET " + destination + " to " + value);
        return;
    }

    static void FuncADD(string[] parts) {
        string destination = parts[1];
        string value = parts[2];

        int destID = ParseVariableName(destination);

        int realValue = Convert.ToInt32(ParseNumber(value), 16);

        if (variables.ContainsKey(destID)) {
            variables[destID] = (Convert.ToInt32(variables[destID], 16) + realValue).ToString("X");
            return;
        } else {
            System.Console.WriteLine("ID " + destID.ToString() + " does not exist!");
            Environment.Exit(0);
        }
        

        //System.Console.WriteLine("ADD " + value + " to " + destination);
        
    }

    static void FuncSUB(string[] parts) {
        string destination = parts[1];
        string value = parts[2];

        int destID = ParseVariableName(destination);

        int realValue = Convert.ToInt32(ParseNumber(value), 16);

        if (variables.ContainsKey(destID)) {
            variables[destID] = (Convert.ToInt32(variables[destID], 16) - realValue).ToString("X");
        }
        
        return;
    }

    static void FuncMRK(string[] parts, int i) {
        string markerID = parts[1];
        if (markerID[0] != 'g') {
            System.Console.WriteLine("YOU MESSED UP ON LINE " + i.ToString() + " - YOU DIDNT PUT A G");
        }

        int markerNum = int.Parse(markerID.Substring(1,markerID.Length - 1));

        //System.Console.WriteLine("MARKER ON LINE " + i.ToString());

        if (gotos.ContainsKey(markerNum)) {
            gotos[markerNum] = i;
        } else {
            gotos.Add(markerNum, i);
        }
        return;
    }

    static void FuncMOV(string[] parts) {
        string markerID = parts[1];
        if (markerID[0] != 'g') {
            System.Console.WriteLine("YOU MESSED UP ON LINE " + currentLine.ToString() + " - YOU DIDNT PUT A G");
        }

        int markerNum = int.Parse(markerID.Substring(1,markerID.Length - 1));
        currentLine = gotos[markerNum-1];
        return;
    }

    static void FuncPRT(string[] parts) {
        string destination = parts[1];

        int newline = 0;
        if (parts.Length-1 >= 2) {
            newline = Convert.ToInt32(ParseNumber(parts[2]), 16);
        }
        

        if (newline == 0) {
            System.Console.WriteLine(ParseNumber(destination));
        } else {
            System.Console.Write(ParseNumber(destination));
        }
        return;
    }

    static void FuncPRS(string[] parts) {
        string destination = "";
        for (int g = 1; g < parts.Length; g++) {
            destination += ParseNumber(parts[g]);
        }

        System.Console.WriteLine("VALUE: " + destination);
        destination = ParseNumber(destination);

        // construct array of values
        string currentHexBit = "";
        List<string> hexBits = new List<string>();
        for (int g = 0; g < destination.Length; g++) {
            currentHexBit += destination[g];
            if (currentHexBit.Length == 2) {
                hexBits.Add(currentHexBit);
                currentHexBit = "";
            }
        }

        foreach (string hexBit in hexBits) {
            System.Console.Write((char)Convert.ToInt32(hexBit, 16));
        }
        return;
    }

    static void FuncLES(string[] parts) {
        
        string dest1 = parts[1];
        string dest2 = parts[2];
        string gotoStr = parts[3];

        //System.Console.WriteLine("LES Func - compare " + dest1 + " to " + dest2 + " and goto " + gotoStr);

        int dest1_Val = Convert.ToInt32(ParseNumber(dest1), 16);

        int dest2_Val = Convert.ToInt32(ParseNumber(dest2), 16);

        int gotoID = int.Parse(gotoStr.Substring(1,gotoStr.Length-1));

        //System.Console.WriteLine("Comparing if " + dest1_Val.ToString() + " is less than " + dest2_Val.ToString());
        if (dest1_Val < dest2_Val) {
            currentLine = gotos[gotoID];
            //System.Console.WriteLine("GOTO Line " + (gotoID + 1).ToString());
            return;
        } else {
            //System.Console.WriteLine("nope");
            return;
        }

    }

    static void FuncEQU(string[] parts) {
        string dest1 = parts[1];
        string dest2 = parts[2];
        string gotoStr = parts[3];

        int dest1_Val = Convert.ToInt32(ParseNumber(dest1), 16);
        int dest2_Val = Convert.ToInt32(ParseNumber(dest2), 16);

        int gotoID = int.Parse(gotoStr.Substring(1,gotoStr.Length-1));

        //System.Console.WriteLine("Comparing if " + dest1_Val.ToString() + " is equal to " + dest2_Val.ToString());
        if (dest1_Val == dest2_Val) {
            currentLine = gotos[gotoID];
            //System.Console.WriteLine("GOTO Line " + (gotoID + 1).ToString());
            return;
        } else {
            //System.Console.WriteLine("nope");
            return;
        }
    }

    static void FuncNEQ(string[] parts) {
        string dest1 = parts[1];
        string dest2 = parts[2];
        string gotoStr = parts[3];

        int dest1_Val = Convert.ToInt32(ParseNumber(dest1), 16);
        int dest2_Val = Convert.ToInt32(ParseNumber(dest2), 16);

        int gotoID = int.Parse(gotoStr.Substring(1,gotoStr.Length-1));

        //System.Console.WriteLine("Comparing if " + dest1_Val.ToString() + " is more than " + dest2_Val.ToString());
        if (dest1_Val != dest2_Val) {
            currentLine = gotos[gotoID];
            //System.Console.WriteLine("GOTO Line " + (gotoID + 1).ToString());
            return;
        } else {
            //System.Console.WriteLine("nope");
            return;
        }
    }

    static void FuncMOR(string[] parts) {
        string dest1 = parts[1];
        string dest2 = parts[2];
        string gotoStr = parts[3];

        int dest1_Val = Convert.ToInt32(ParseNumber(dest1), 16);
        int dest2_Val = Convert.ToInt32(ParseNumber(dest2), 16);

        int gotoID = int.Parse(gotoStr.Substring(1,gotoStr.Length-1));

        System.Console.WriteLine("Comparing if " + dest1_Val.ToString() + " is more than " + dest2_Val.ToString());
        if (dest1_Val > dest2_Val) {
            currentLine = gotos[gotoID];
            System.Console.WriteLine("GOTO Line " + (gotoID + 1).ToString());
            return;
        } else {
            System.Console.WriteLine("nope");
            //Environment.Exit(345);
            return;
        }
    }

    static private bool IsValidHex(string val) {
        // from https://stackoverflow.com/a/223857
        return System.Text.RegularExpressions.Regex.IsMatch(val, @"\A\b[0-9a-fA-F]+\b\Z");
    }



    static private string ParseNumber(string val) {
        string outVal = "";
        if (val[0] == 'v') {
            // this is good, we have a destination 1
            string possibleHex = val.Substring(1,val.Length-1);
            if (IsValidHex(possibleHex)) {
                int hexVal = Convert.ToInt32(possibleHex, 16);
                if (variables.TryGetValue(hexVal, out string z)) {
                    outVal = z;
                }
            }
            // if (variables.TryGetValue(int.Parse(), out int z)) {
            //     outVal = z;
            // }
        } else if (val[0] == 'i') {
            string possibleHex = val.Substring(1,val.Length-1);
            if (IsValidHex(possibleHex)) {
                int argVal = Convert.ToInt32(possibleHex, 16);
                if (argsIn.Length - 1 >= argVal) {
                    return int.Parse(argsIn[argVal]).ToString("X");
                }
            }

        } else if (val[0] == '&') {
            // it's a string

        }
        
        else if (IsValidHex(val)) { // its just a number
            outVal = val;
        }

        return outVal;
    }

    static private int ParseVariableName(string val) {
        int outVal = 0;
        if (val[0] == 'v') {
            // this is good, we have a destination 1
            string possibleHex = val.Substring(1,val.Length -1);
            if (IsValidHex(possibleHex)) {
                outVal = Convert.ToInt32(possibleHex, 16);
            }
            
        } else if (int.TryParse(val, out int v)) { // its just a number
            string possibleHex = val;
            if (IsValidHex(possibleHex)) {
                outVal = Convert.ToInt32(possibleHex, 16);
            }
        }

        return outVal;
    }

    static private void RaiseErrorAndQuit(ErrorType errorType) {
        System.Console.WriteLine("Error on line" + currentLine.ToString() + ": " + errorType.ToString());
        // switch (errorType) {
        //     case ErrorType.VariableDoesntExist:
        //         break;
        //     case ErrorType.InputDoesntExist:
        //         break;
        //     case ErrorType.NotEnoughArguments:
        //         break;
        // }
        Environment.Exit(2);
    }

}


enum ErrorType {
    VariableDoesntExist,
    InputDoesntExist,
    NotEnoughArguments
}
}