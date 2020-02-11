// Sharer-Arduino
//
// shows the use of the Sharer library.
// Sharer - https://github.com/devdotnetorg/Sharer
// Sharer-NETStandard - https://github.com/devdotnetorg/Sharer-NETStandard
//
// Copyright DevDotNet.ORG (c) 2020
//
// Arduino - LED, DS18S20
// LED built-in
// DS18S20 Signal pin on digital 8
//
// other pins can be left unconnected.

#include <Sharer.h>
#include <Arduino.h>
#include <Wire.h>
#include <OneWire.h> // https://www.pjrc.com/teensy/td_libs_OneWire.html   https://github.com/PaulStoffregen/OneWire

int DS18S20_Pin = 8; //DS18S20 Signal pin on digital 8

//Temperature chip i/o
OneWire ds(DS18S20_Pin);  // on digital pin 8

//Sharer Func

// A simple function that sums an integer and a byte and return an integer
int Sum(int a, byte b) {
  return a + b;
}

//LED control
int setLed(bool state){
 if(state)
  {
     digitalWrite(LED_BUILTIN, HIGH);   // turn the LED on (HIGH is the voltage level)
  }else
  {
    digitalWrite(LED_BUILTIN, LOW);    // turn the LED off by making the voltage LOW
  }
  return 0;
}

float getTemperature()
{
  return getTemp();
}

float getTemp(){
  //returns the temperature from one DS18S20 in DEG Celsius
  byte data[12];
  byte addr[8];
  if ( !ds.search(addr)) {
      //no more sensors on chain, reset search
      ds.reset_search();
      return -1000;
  }
  if ( OneWire::crc8( addr, 7) != addr[7]) {
      Serial.println("CRC is not valid!");
      return -1000;
  }
  if ( addr[0] != 0x10 && addr[0] != 0x28) {
      Serial.print("Device is not recognized");
      return -1000;
  }
  ds.reset();
  ds.select(addr);
  ds.write(0x44,1); // start conversion, with parasite power on at the end
  byte present = ds.reset();
  ds.select(addr);
  ds.write(0xBE); // Read Scratchpad
  for (int i = 0; i < 9;  i++  ) { // we need 9 bytes
    data[i] = ds.read();
  }
  ds.reset_search();
  byte MSB = data[1];
  byte LSB = data[0];
  float tempRead = ((MSB << 8) | LSB); //using two's compliment
  float TemperatureSum = tempRead / 16;
  return TemperatureSum;
}

void setup() {
  
  //debug
  // put your setup code here, to run once:
  //Serial.begin(9600);

  //LED
  pinMode(LED_BUILTIN, OUTPUT);
  // Init Sharer and declare your function to share
  Sharer.init(115200); // Init Serial communication with 115200 bauds
  // Expose this function to Sharer
  Sharer_ShareFunction(int, Sum, int, a, byte, b);
  Sharer_ShareFunction(int, setLed, bool, state);
  Sharer_ShareFunction(float, getTemperature);
  
}

// Run Sharer engine in the main Loop
void loop() {
  Sharer.run();
  
  //For local Test
  /*
  float temperature = getTemp();
  Serial.println(temperature);
  delay(100); //just here to slow down the output so it is easier to read
  setLed(true);
  delay(1000);                       // wait for a second
  setLed(false);
  delay(1000);  
  */ 
}
