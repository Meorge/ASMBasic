using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;


// use "mcs MALBASIC.cs" to compile

namespace ASMBASIC
{
public class ASMBASICInterpreter
{   

    private string[] lines;
    public string[] argsIn;

    //public  MALBASICInterpreter interpreter;
    public Dictionary<int, string> variables = new Dictionary<int, string>();
    public Dictionary<int, int> gotos = new Dictionary<int, int>();

    public int currentLine = -1;

    private string file = "print_to_ten.malbasic";

    public bool useGFX = false;

    public MainWindow GFXWindow;

    public ASMBASICInterpreter(string codePath, MainWindow wind) {
            GFXWindow = wind;
            Start(new string[] { codePath });
    }

     public void Start (string[] args)
    {

        System.Console.WriteLine("Executing code at " + args[0]);

            //argsIn = args;
        argsIn = new string[4];
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

        EvaluateCode(0);

    }

    public void EvaluateCode(int lineToStart = 0, bool resetTimerAfterDone = false) {
        System.Console.WriteLine("Evaluating code line");
            for (currentLine = lineToStart; currentLine < lines.Length; currentLine++) {
            EvaluateLine(lines[currentLine]);
        }

        if (resetTimerAfterDone) { GFXWindow.DoneInterpreting(); }
    }



     public void EvaluateLine(string line) {
        string[] partsOfLine = line.Split(null);
        string instructionName = partsOfLine[0];


        System.Console.WriteLine("about to check the instruction on this line: " + line);

        System.Console.WriteLine(instructionName);

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
            case "GFX":
                System.Console.WriteLine("GFX");
                FuncGFX(partsOfLine);
                break;
            case "RND":
                FuncRND(partsOfLine);
                break;
            case "END":
                currentLine = lines.Length;
                break;
        }
        return;
    }

    void FuncRND(string[] parts) {
        int min = Convert.ToInt32(ParseNumber(parts[1]), 16);
        int max = Convert.ToInt32(ParseNumber(parts[2]), 16);
        int toVar = ParseVariableName(parts[3]);

        Random randomGen = new Random();

        int randVal = randomGen.Next(min, max);
        variables[toVar] = randVal.ToString("X2");
        return;
    }

    void FuncSET(string[] parts) {
        string destination = parts[1];
        string value = "";
        for (int g = 2; g < parts.Length; g++) {
            System.Console.WriteLine(parts[g]);
            value += ParseNumber(parts[g]);
        }

        System.Console.WriteLine("Setting to the value " + value);

        int destID = ParseVariableName(destination);

        string realValue = ParseNumber(value);

        if (variables.ContainsKey(destID)) {
            variables[destID] = realValue;
        } else {
            variables.Add(destID, realValue);
        }
        

        //System.Console.WriteLine("SET " + destination + " to " + value);
        return;
    }

     void FuncADD(string[] parts) {
        string destination = parts[1];
        string value = parts[2];

        int destID = ParseVariableName(destination);

        int realValue = Convert.ToInt32(ParseNumber(value), 16);

        System.Console.WriteLine("ADD " + value + " to " + ParseNumber(destination));
        if (variables.ContainsKey(destID)) {
            variables[destID] = (Convert.ToInt32(variables[destID], 16) + realValue).ToString("X");
            return;
        } else {
            System.Console.WriteLine("ID " + destID.ToString() + " does not exist!");
            Environment.Exit(0);
        }
        
        
    }

     void FuncSUB(string[] parts) {
        string destination = parts[1];
        string value = parts[2];

        int destID = ParseVariableName(destination);

        int realValue = Convert.ToInt32(ParseNumber(value), 16);

        if (variables.ContainsKey(destID)) {
            variables[destID] = (Convert.ToInt32(variables[destID], 16) - realValue).ToString("X");
        }
        
        return;
    }

     void FuncMRK(string[] parts, int i) {
        string markerID = parts[1];
        if (markerID[0] != 'g') {
            System.Console.WriteLine("YOU MESSED UP ON LINE " + i.ToString() + " - YOU DIDNT PUT A G");
        }

        int markerNum = Convert.ToInt32(markerID.Substring(1,markerID.Length - 1), 16);

        //System.Console.WriteLine("MARKER ON LINE " + i.ToString());

        if (gotos.ContainsKey(markerNum)) {
            gotos[markerNum] = i;
        } else {
            gotos.Add(markerNum, i);
        }
        return;
    }

     void FuncMOV(string[] parts) {
        string markerID = parts[1];
        if (markerID[0] != 'g') {
            System.Console.WriteLine("YOU MESSED UP ON LINE " + currentLine.ToString() + " - YOU DIDNT PUT A G");
        }

        int markerNum = Convert.ToInt32(markerID.Substring(1,markerID.Length - 1), 16);
        currentLine = gotos[markerNum];
        return;
    }

     void FuncPRT(string[] parts) {
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

     void FuncPRS(string[] parts) {
        string destination = "";
        for (int g = 1; g < parts.Length; g++) {
            destination += ParseNumber(parts[g]);
        }

        //System.Console.WriteLine("VALUE: " + destination);
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

     void FuncLES(string[] parts) {
        
        string dest1 = parts[1];
        string dest2 = parts[2];
        string gotoStr = parts[3];

        //System.Console.WriteLine("LES Func - compare " + dest1 + " to " + dest2 + " and goto " + gotoStr);

        int dest1_Val = Convert.ToInt32(ParseNumber(dest1), 16);

        int dest2_Val = Convert.ToInt32(ParseNumber(dest2), 16);

        int markerNum = Convert.ToInt32(gotoStr.Substring(1, gotoStr.Length - 1), 16);

        //System.Console.WriteLine("Comparing if " + dest1_Val.ToString() + " is less than " + dest2_Val.ToString());
        if (dest1_Val < dest2_Val) {
            currentLine = gotos[markerNum];
            //System.Console.WriteLine("GOTO Line " + (gotoID + 1).ToString());
            return;
        } else {
            //System.Console.WriteLine("nope");
            return;
        }

    }

     void FuncEQU(string[] parts) {
        string dest1 = parts[1];
        string dest2 = parts[2];
        string gotoStr = parts[3];

            System.Console.WriteLine("aint done the first one yet");
        int dest1_Val = Convert.ToInt32(ParseNumber(dest1), 16);
            System.Console.WriteLine(ParseNumber(dest2));
            // for some reason does not make it past this line
        int dest2_Val = Convert.ToInt32(ParseNumber(dest2), 16);

        int gotoID = Convert.ToInt32(gotoStr.Substring(1, gotoStr.Length - 1), 16);

        System.Console.WriteLine("Comparing if " + dest1_Val.ToString() + " is equal to " + dest2_Val.ToString());
        if (dest1_Val == dest2_Val) {
            currentLine = gotos[gotoID];
            System.Console.WriteLine("GOTO Line " + (gotoID + 1).ToString());
                //System.Console.WriteLine("GOTO Line " + (gotoID + 1).ToString());
            return;
        } else {
            //System.Console.WriteLine("nope");
            return;
        }
    }

     void FuncNEQ(string[] parts) {
        string dest1 = parts[1];
        string dest2 = parts[2];
        string gotoStr = parts[3];

        Console.WriteLine(ParseNumber(dest1));
        int dest1_Val = Convert.ToInt32(ParseNumber(dest1), 16);
        int dest2_Val = Convert.ToInt32(ParseNumber(dest2), 16);

        int gotoID = Convert.ToInt32(gotoStr.Substring(1, gotoStr.Length - 1), 16);

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

     void FuncMOR(string[] parts) {
        string dest1 = parts[1];
        string dest2 = parts[2];
        string gotoStr = parts[3];

        int dest1_Val = Convert.ToInt32(ParseNumber(dest1), 16);
        int dest2_Val = Convert.ToInt32(ParseNumber(dest2), 16);

        int gotoID = Convert.ToInt32(gotoStr.Substring(1, gotoStr.Length - 1), 16);

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

    void FuncGFX(string[] parts) {
            System.Console.WriteLine(parts[1]);
            string xPos = parts[1];
            string yPos = parts[2];
            string val = parts[3];
            System.Console.WriteLine("yoho");

            if (xPos == "CLE")
            {
                GFXWindow.ClearScreen();
                System.Console.WriteLine("clear screen");
            }
            else
            {

                int xPos_int = Convert.ToInt32(ParseNumber(xPos), 16);
                int yPos_int = Convert.ToInt32(ParseNumber(yPos), 16);
                int colorVal = Convert.ToInt32(ParseNumber(val), 16);

                System.Console.WriteLine("Putting pixel with color ID " + colorVal.ToString() + " at (" + xPos_int.ToString() + ", " + yPos_int.ToString() + ")");
                //System.Console.WriteLine(GFXWindow);

                if (xPos_int < GFXWindow.pixelValues.GetLength(0) && yPos_int < GFXWindow.pixelValues.GetLength(1))
                {
                    GFXWindow.pixelValues[xPos_int, yPos_int] = colorVal;
                }
            }

        }

     private bool IsValidHex(string val) {
        // from https://stackoverflow.com/a/223857
        return System.Text.RegularExpressions.Regex.IsMatch(val, @"\A\b[0-9a-fA-F]+\b\Z");
    }



     private string ParseNumber(string val) {
        string outVal = "";
        if (val[0] == 'v') {
            // this is good, we have a destination 1
            string possibleHex = ParseNumber(val.Substring(1,val.Length-1));
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
            Console.WriteLine("Possible hex for input: " + possibleHex);
            if (IsValidHex(possibleHex)) {
                int argVal = Convert.ToInt32(possibleHex, 16);
                if (argsIn.Length - 1 >= argVal) {
                    Console.WriteLine("hahaa lol " + argsIn[argVal]);
                    return argsIn[argVal];
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

     private int ParseVariableName(string val) {
        int outVal = 0;
        if (val[0] == 'v') {
            // this is good, we have a destination 1
            string possibleHex = val.Substring(1,val.Length -1);
            if (IsValidHex(possibleHex)) {
                outVal = Convert.ToInt32(possibleHex, 16);
            }
            
            }

            else if (int.TryParse(val, out int v)) { // its just a number
            string possibleHex = val;
            if (IsValidHex(possibleHex)) {
                outVal = Convert.ToInt32(possibleHex, 16);
            }
        }

        return outVal;
    }

     private void RaiseErrorAndQuit(ErrorType errorType) {
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