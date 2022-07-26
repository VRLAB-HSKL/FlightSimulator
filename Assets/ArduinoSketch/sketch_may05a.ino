//Active ports
int led = LED_BUILTIN;
int portMotor1_SET = 12;
int portMotor2_SET = 11;
int portMotor3_SET = 10;
int portMotor1_RESET = 9;
int portMotor2_RESET = 8;
int portMotor3_RESET = 7;
int ledState = LOW;
unsigned long previousMillis = 0;

const char separator = ':';
const char end = ';';
const char begin = '*';
const char setPortSymbol = '>';

bool stringComplete = false;
String inString = "";
int forces[3];


void setup() {
  Serial.begin(115200);
  initPorts();
  Serial.setTimeout(500);
  inString.reserve(200); // reserve 200 bytes for inputstring
}
/*
Initialise all digital pins to output and set the state to low
*/
void initPorts(){
  pinMode(LED_BUILTIN, OUTPUT);
  pinMode(portMotor1_SET, OUTPUT);
  pinMode(portMotor2_SET, OUTPUT);
  pinMode(portMotor3_SET, OUTPUT);
  pinMode(portMotor1_RESET, OUTPUT);
  pinMode(portMotor2_RESET, OUTPUT);
  pinMode(portMotor3_RESET, OUTPUT);
  pinMode(6, OUTPUT);
  pinMode(5, OUTPUT);
  pinMode(4, OUTPUT);
  pinMode(3, OUTPUT);
  pinMode(2, OUTPUT);
  

  digitalWrite(LED_BUILTIN, LOW);
  digitalWrite(portMotor1_SET,LOW);
  digitalWrite(portMotor2_SET,LOW);
  digitalWrite(portMotor3_SET,LOW);
  digitalWrite(portMotor1_RESET,LOW);
  digitalWrite(portMotor2_RESET,LOW);
  digitalWrite(portMotor3_RESET,LOW);
  digitalWrite(6, LOW);
  digitalWrite(5, LOW);
  digitalWrite(4, LOW);
  digitalWrite(3, LOW);
  digitalWrite(2, LOW);
}
void loop() {
  //every loop check if a end character has been passed through the serial port
  if(stringComplete){

    executeCommand(inString);
    
    //reset command buffer
    inString = "";
    stringComplete = false;
    
  }
}

//gets executed between loops if new data is available at the serialport
void serialEvent(){
  
  while(Serial.available()){
    char inChar = (char) Serial.read();
    inString += inChar;
    //line end = command complete => break out from loop
    if(inChar == '\n'){
      stringComplete = true;
      break;
    }
  }
}

//checks the first symbol of the command string to determine what command to execute
//set pin command = ">portnumber:value"
void executeCommand(String command){
  if(command.charAt(0) == 'r'){
      resetAllMotors();
      return;
  }

  //read the first character
  char operationSymbol = command.charAt(0);
  //cut the first char from command string
  command = command.substring(1, command.length()-1);

  if(operationSymbol == setPortSymbol){
    //extract portnumber and value from command
     int port = command.substring(0, command.indexOf(':')).toInt();
     int value = command.substring(command.indexOf(':') + 1, command.length() - 1).toInt();
     setValueOnPort(port, value);
  }else{
    logToSerialPort("Not a valid Operation");
  }
}

//
void logToSerialPort(String message) {
  Serial.flush();
  Serial.println(message);
}


void setValueOnPort(int port, int value){
  //VALUE EXCEPTION
  if(value != HIGH && value != LOW){
    String errorMessage = "Value '";
    errorMessage += value;
    errorMessage += "' ist kein gültiger Wert für Port '";
    errorMessage += port;
    errorMessage += "'";
    logToSerialPort(errorMessage);
  }
  String message = "Setting Port '"; 
  message += port;
  message += "' to '";
  message += value;
  message += "'";
  digitalWrite(port, value);
}


void resetAllMotors(){
  digitalWrite(portMotor1_SET,LOW);
  digitalWrite(portMotor2_SET,LOW);
  digitalWrite(portMotor3_SET,LOW);
  digitalWrite(portMotor1_RESET,LOW);
  digitalWrite(portMotor2_RESET,LOW);
  digitalWrite(portMotor3_RESET,LOW);
}


//=============================================================DEPRECATED===============================
void extractForcesFromSerialInput(String input) {
  int firstSeparatorIndex = input.indexOf(':');
  int lastSeparatorIndex = input.lastIndexOf(':');
  int x = input.substring(0, firstSeparatorIndex).toInt();
  int y = input.substring(firstSeparatorIndex + 1, lastSeparatorIndex).toInt();
  int z = input.substring(lastSeparatorIndex + 1, input.length() - 1).toInt();
  forces[0] = x;
  forces[1] = y;
  forces[2] = z;
}
String forcesToString() {
  String message = "X = ";
  message += forces[0];
  message += "/Y = ";
  message += forces[1];
  message += "/Z = ";
  message += forces[2];
  return message;
}
