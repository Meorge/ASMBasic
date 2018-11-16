//Sample using LiquidCrystal library
#include <LiquidCrystal.h>

#include <SPI.h>
#include <SD.h>


// FOR ASMBASIC
//int currentLine = 0;
//int variables[100];
//int gotos[100]; 

char rawText[] = "";

//char codeLines[20][50];
 
// select the pins used on the LCD panel
LiquidCrystal lcd(8, 9, 4, 5, 6, 7);
 
//// define some values used by the panel and buttons
int lcd_key     = 0;
int adc_key_in  = 0;
#define btnRIGHT  0
#define btnUP     1
#define btnDOWN   2
#define btnLEFT   3
#define btnSELECT 4
#define btnNONE   5


const int chipSelect = 4;
// read the buttons
int read_LCD_buttons()
{
 adc_key_in = analogRead(0);      // read the value from the sensor
 // my buttons when read are centered at these valies: 0, 144, 329, 504, 741
 // we add approx 50 to those values and check to see if we are close
 if (adc_key_in > 1000) return btnNONE; // We make this the 1st option for speed reasons since it will be the most likely result
 if (adc_key_in < 50)   return btnRIGHT; 
 if (adc_key_in < 195)  return btnUP;
 if (adc_key_in < 380)  return btnDOWN;
 if (adc_key_in < 555)  return btnLEFT;
 if (adc_key_in < 790)  return btnSELECT;  
 return btnNONE;  // when all others fail, return this...
}
 
void setup()
{
 Serial.begin(19200);
 lcd.begin(16, 2);              // start the library
 lcd.setCursor(0,0);

 lcd.print("BasicASM");
 lcd.setCursor(0,1);
 lcd.print("Loading...");
 delay(500);
 lcd.clear();
 if (!SD.begin(chipSelect)) {
   lcd.print("Can't read card");
   // don't do anything more:
   while (1);
 }
  
 File dataFile = SD.open("file.txt");

 // if the file is available, write to it:
 if (dataFile) {
  int blarf = 0;
  while (dataFile.available()) {
    rawText[blarf] = (char)dataFile.read();
    //lcd.clear();
    
    lcd.print((char)dataFile.read());
    Serial.print((char)dataFile.read());
    blarf++;
    delay(300);
  }
  
  dataFile.close();
  lcd.clear();
  lcd.print(rawText);
  Serial.print(rawText);

   
 }
 // if the file isn't open, pop up an error:
 else {
   lcd.print("Error opening");
   while(1);
 }

// int fileLength = sizeof(rawText);
//
// char lineBuffer[] = "";
// int linesSoFar = 0;
//
// delay(5000);
 
// for (int i = 0; i < fileLength; i++) {
//  delay(2000);
//  //lcd.clear();
//  char charInFile = rawText[i];
//  //Serial.println(charInFile);
//  //lcd.print(charInFile);
//  delay(2000);
//  if (charInFile != "\n" && charInFile != "\r") {
//    strcat(lineBuffer, charInFile);
//  } else {
//    //strcpy(codeLines[linesSoFar], lineBuffer);
//    //codeLines[linesSoFar] = lineBuffer;
//    //lcd.print(lineBuffer);
//    delay(500);
//    
//    lineBuffer[0] = (char)0;
//    linesSoFar++;
//    
//  }
// }
 
}

void ReadNextLine() {

}

void loop()
{
 //lcd_key = read_LCD_buttons();  // read the buttons
 
}
