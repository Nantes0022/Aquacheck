  //TempSensor
  #include <OneWire.h>
  #include <DallasTemperature.h>
  #include <SPI.h>
  #include <SD.h>
  #include <ArduinoJson.h>
  #include <List.hpp>
  #include <Wire.h>
  #include <RTClib.h>
  #include <Servo.h>
  #include <SD.h>

  File myFile;
  String fileContent;


  Servo myServo; 

// Create an RTC object
  RTC_DS3231 rtc;

  int timezone_offset = 12;  // UTC+8 for the Philippines

  const int oneWireBus = 7; 
  OneWire oneWire(oneWireBus);
  DallasTemperature sensors(&oneWire);
  const int turbidityPin = A1;
  const int chipSelect = 53;  // SS pin for Mega 2560


String Response="";
int loading = 0;
String data;
bool isOffline;
bool isFinish;

const int TempChangeRelay = 10;
  const int WaterInRelay = 11;
  const int WaterOutRelay = 12;
  List<String> ControlType;
  List<String> ControlStatus;

void setup() {
  // put your setup code here, to run once:
  Serial.begin(115200);
  Serial1.begin(115200);
  rtcSetup();
  myServo.attach(9);
  myServo.write(0);
  pinMode(34,OUTPUT);
  pinMode(36,OUTPUT);
  Serial.println();
  digitalWrite(34,LOW);
  digitalWrite(36,HIGH);
  pinMode(TempChangeRelay, OUTPUT);
  pinMode(WaterInRelay, OUTPUT);
  pinMode(WaterOutRelay, OUTPUT);
  if (!SD.begin(53)) {
    Serial.println("Initialization failed!");
    return;
  }
  
    digitalWrite(TempChangeRelay, HIGH); // Assuming HIGH turns off the relay
    digitalWrite(WaterInRelay, HIGH);
    digitalWrite(WaterOutRelay, HIGH);

    
  rtc.adjust(DateTime(F(__DATE__), F(__TIME__)));

    
}

void loop() {
    if(isConnected()){
      
      digitalWrite(36,LOW);
      digitalWrite(34,HIGH);
      isOffline=false;
      Serial1.flush();
      feedingSystem();
      Serial1.flush();
      updateSensors();
      Serial1.flush();
      checkControls();
      Serial.println();
      Serial.print("Time: ");
      Serial.println(time());
      Serial.println(datenow());
    }else{
      digitalWrite(36,HIGH);
      digitalWrite(34,LOW);
      isOffline=true;
      offlinemode();
      
      Serial.println();
      Serial.print("Time: ");
      Serial.println(time());
      Serial.println(datenow());
      delay(3000);
    }
}
//Hardware

double tempSensor(){
  sensors.requestTemperatures();
  return sensors.getTempCByIndex(0);
}

double tdsSensor(){
  int tdsValue = analogRead(A0);
  float voltage = tdsValue * (5.0 / 1023.0);
  return voltage * 1000;
}


double turbiditySensor(){  // Pin connected to the turbidity sensor
  int sensorValue = 0;          // Variable to store the sensor value
  float voltage = 0.0;          // Voltage from the sensor
  float NTU = 0.0;     

  
  sensorValue = analogRead(turbidityPin);  // Read the sensor value
  voltage = sensorValue * (5.0 / 1023.0); // Convert to voltage (Assuming 5V supply)

  // Map the sensorValue to NTU using linear interpolation
  if (sensorValue >= 486 && sensorValue <= 745) { 
    NTU = mapFloat(sensorValue, 745, 486, 0, 100);
  } else if (sensorValue >= 413 && sensorValue < 486) {
    NTU = mapFloat(sensorValue, 486, 413, 100, 200);
  } else if (sensorValue >= 196 && sensorValue < 413) {
    NTU = mapFloat(sensorValue, 413, 196, 200, 500);
  } else if (sensorValue >= 97 && sensorValue < 196) {
    NTU = mapFloat(sensorValue, 196, 97, 500, 1000);
  } else if (sensorValue < 97) {
    NTU = 1000;  // Beyond the range, max turbidity
  } else {
    NTU = 0;     // Beyond the range, clear water
  }
 return NTU;
}

float mapFloat(float x, float in_min, float in_max, float out_min, float out_max) {
  return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
}

double pHSensor(){
  float calibration_value = 21.34;
  int phval = 0; 
  unsigned long int avgval; 
  int buffer_arr[10],temp;

  float ph_act;

  for(int i=0;i<10;i++) 
  { 
    buffer_arr[i]=analogRead(A2);
    delay(30);
  }
  for(int i=0;i<9;i++)
  {
    for(int j=i+1;j<10;j++)
    {
      if(buffer_arr[i]>buffer_arr[j])
      {
        temp=buffer_arr[i];
        buffer_arr[i]=buffer_arr[j];
        buffer_arr[j]=temp;
      }
    }
  }
  avgval=0;
  for(int i=2;i<8;i++)
  avgval+=buffer_arr[i];
  float volt=(float)avgval*5.0/1024/6; 
  ph_act = -5.70 * volt + calibration_value;
  
  return ph_act;
}

void rtcSetup(){
  Wire.begin();

  if (!rtc.begin()) {
    Serial.println("Couldn't find RTC");
    while (1);
  }
}


//HardCode


bool isConnected(){
    Serial1.println("AT+PING=\"8.8.8.8\"\r\n");
    String res = Serial1.readString();
    if(res.indexOf("OK") >= 0){
      return true;
    }else{
      return false;
    }

}

void updateSensors(){
  
// Adjust the delay times to ensure proper communication
Serial1.print("AT+CIPSTART=\"TCP\",\"aquacheck.site\",80\r\n");
delay(500);  // Adjust to 300-500ms for connection establishment

String httpRequest = String("PATCH /putSensor/updateSensors.php?api_key=iu311DXlOK42kGrBNHzPhQ8X8ZrmXXvbrRR8sbUz") +
                 "&tds=" + urlEncode(String(tdsSensor())) +
                 "&temp=" + urlEncode(String(tempSensor())) +
                 "&turbid=" + urlEncode(String(turbiditySensor())) +
                 "&pH=" + urlEncode(String(pHSensor())) +
                 " HTTP/1.1\r\n" +
                 "Host: aquacheck.site\r\n" +
                 "Connection: close\r\n\r\n";

// Send data size
Serial1.println("AT+CIPSEND=" + String(httpRequest.length()) +"\r\n");
delay(500);  // Adjust to 300ms


// Send HTTP request
Serial1.println(httpRequest);
Response = Serial1.readString();
delay(1500);  // Adjust to 500ms - 1 second depending on server response time
// Read and print response from server
Serial.println("Full Response: " + Response);

}


void feedingSystem(){
  
// Adjust the delay times to ensure proper communication
Serial1.println("AT+CIPSTART=\"TCP\",\"aquacheck.site\",80\r\n");
delay(500);  // Adjust to 300-500ms for connection establishment

String apiKey = urlEncode("iu311DXlOK42kGrBNHzPhQ8X8ZrmXXvbrRR8sbUz");
String httpRequest = "GET /feeding/feedAndroidAPI.php?api_key=" + apiKey + 
" HTTP/1.1\r\nHost: aquacheck.site\r\nConnection: close\r\n\r\n";

// Send data size
Serial1.println("AT+CIPSEND=" + String(httpRequest.length()) +"\r\n");
delay(700);  // Adjust to 300ms

// Send HTTP request
Serial1.println(httpRequest);
Response = Serial1.readString(); // Adjust to 500ms - 1 second depending on server response time

Serial.println(Response);
delay(3000); 
// Read and print response from server
int ResponseIndex = Response.indexOf("{");
int ResponseEndIndex = Response.indexOf("CLOSED");
String Responses = Response.substring(ResponseIndex, ResponseEndIndex);
updateData(Responses);
ODeserializeJSON(Responses);
}

void ODeserializeJSON(String responses){
  DynamicJsonDocument FDoc(2048);
  List<String> fishArray;
  List<String> fishDateArray;
  List<String> fishTimeArray;
  DeserializationError error = deserializeJson(FDoc, responses);
  const char* date;
  if (error) {
    Serial.print("Deserialization failed: ");
    Serial.println(error.f_str());
    return;
  }

  for (JsonPair kv : FDoc.as<JsonObject>()) {
    if(kv.value()["date"].as<String>() == datenow()){
      fishArray.add((kv.value()["fish"].as<String>()));
      fishDateArray.add((kv.value()["date"].as<String>()));
      fishTimeArray.add((kv.value()["times"].as<String>()));
    }
  }
  int i=0;
  
  Serial.println();
  Serial.println();
  for(int i = 0; i < fishArray.getSize(); i++) {
    Serial.print(fishArray.get(i)+": ");
    Serial.print(fishDateArray.get(i));
    Serial.println(fishTimeArray.get(i));
  }
  
  for(int i = 0; i < fishArray.getSize(); i++) {

    if(fishTimeArray.get(i).indexOf(time()) >= 0 && fishArray.get(i).indexOf("Goldfish") >= 0){
      if(!checkReports("Goldfish",datenow(),"AutomaticFeeding",time())){    
        Serial.println("Automatic Feeding- Feeding Now");
        servoFeed();
        updateReports(datenow(), time(), "AutomaticFeeding", "Goldfish");
      }
    }

    if(fishTimeArray.get(i).indexOf(time()) >= 0 && fishArray.get(i).indexOf("Manual") >= 0){
      if(!checkReports("ManualFeeding",datenow(),"ManualFeeding",time())){
          Serial.println("Manual Feeding- Feeding Now");
          servoFeed();
          updateReports(datenow(), time(), "ManualFeeding", "ManualFeeding");
      }
    } 
  }

}

String time(){
  DateTime now = rtc.now();

  

  return (now.hour() < 10 ? "0" + String(now.hour()) : String(now.hour())) + ":" + (now.minute() < 10 ? "0" + String(now.minute()) : String(now.minute()));
}


String datenow(){
  DateTime now = rtc.now();
  

  return (now.day() < 10 ? "0" : "") + String(now.day()) + "-" +
         (now.month() < 10 ? "0" : "") + String(now.month()) + "-" +
         String(now.year());
}

void servoFeed(){
  myServo.write(26);
  delay(1000);
  myServo.write(0);
}


String urlEncode(String input) {
  String encoded = "";
  char c;
  for (int i = 0; i < input.length(); i++) {
    c = input.charAt(i);
    if (isalnum(c) || c == '-' || c == '_' || c == '.' || c == '~') {
      encoded += c;
    } else {
      encoded += "%";
      encoded += String(c, HEX);
    }
  }
  return encoded;
}

bool checkReports(String fish, String date, String type, String time){
  
  // Adjust the delay times to ensure proper communication
  Serial1.println("AT+CIPSTART=\"TCP\",\"aquacheck.site\",80\r\n");
  delay(500);  // Adjust to 300-500ms for connection establishment

  String httpRequest = "GET /feeding/feedReport.php?api_key=iu311DXlOK42kGrBNHzPhQ8X8ZrmXXvbrRR8sbUz"
                      "&SchedKey=" + urlEncode(fish) +
                      "&date=" + urlEncode(date) +
                      "&type=" + urlEncode(type) + " HTTP/1.1\r\nHost: aquacheck.site\r\nConnection: close\r\n\r\n";


  // Send data size
  Serial1.println("AT+CIPSEND=" + String(httpRequest.length()) +"\r\n");
  delay(1000);  // Adjust to 300ms

  // Send HTTP request
  Serial1.println(httpRequest);
  Response = Serial1.readString(); // Adjust to 500ms - 1 second depending on server response time

  delay(1000); 
  
  // Read and print response from server
  int ResponseIndex = Response.indexOf("{");
  int ResponseEndIndex = Response.indexOf("CLOSED");
  String Responses = Response.substring(ResponseIndex, ResponseEndIndex);
  if(Responses.indexOf(time) >=0){
    return true;
  }else{
    return false;
  }
}

String updateReports(String date, String time, String type, String fish){
  
// Adjust the delay times to ensure proper communication
Serial1.print("AT+CIPSTART=\"TCP\",\"aquacheck.site\",80\r\n");
delay(500);  // Adjust to 300-500ms for connection establishment

String httpRequest = String("PUT /feeding/feedArdUpdate.php?api_key=iu311DXlOK42kGrBNHzPhQ8X8ZrmXXvbrRR8sbUz") +
                     "&date=" + urlEncode(date) +
                     "&times[]=" + urlEncode(time) +
                     "&type=" + urlEncode(type) +
                     "&SchedKey=" + urlEncode(fish) +
                     "&fish=" + urlEncode(fish) +
                     " HTTP/1.1\r\n" +
                     "Host: aquacheck.site\r\n" +
                     "Connection: close\r\n\r\n";



// Send data size
Serial1.println("AT+CIPSEND=" + String(httpRequest.length()) +"\r\n");
delay(500);  // Adjust to 300ms


// Send HTTP request
Serial1.println(httpRequest);
Response = Serial1.readString();
delay(1500);  // Adjust to 500ms - 1 second depending on server response time
// Read and print response from server
return "Full Response: " + Response;

}

void updateData(String Response){
  // Open the file for writing (this will overwrite it)
  myFile = SD.open("data.txt", O_CREAT | O_RDWR | O_TRUNC);

  if (myFile) {
    Serial.println("File opened for writing.");

    // Write some data to the file (overwrite if it exists)
    myFile.println(Response);

    Serial.println("File content written.");
    
    // Close the file after writing
    myFile.close();

    // Reopen the file in read mode to read back the content
  } else {
    Serial.println("Error opening file for writing.");
  } 
}

  void offlinemode(){
    myFile = SD.open("data.txt", O_RDONLY);
    
    if (myFile) {
      fileContent = myFile.readString();
      // Print the file content as a string
      myFile.close();
    } else {
      Serial.println("Error opening file for reading.");
    }
     DeserializeOffline(fileContent);
  }

void DeserializeOffline(String responses){
  DynamicJsonDocument FDoc(2048);
  List<String> fishArray;
  List<String> fishDateArray;
  List<String> fishTimeArray;
  DeserializationError error = deserializeJson(FDoc, responses);
  const char* date;
  if (error) {
    Serial.print("Deserialization failed: ");
    Serial.println(error.f_str());
    return;
  }

  for (JsonPair kv : FDoc.as<JsonObject>()) {
    if(kv.value()["date"].as<String>() == datenow()){
      fishArray.add((kv.value()["fish"].as<String>()));
      fishDateArray.add((kv.value()["date"].as<String>()));
      fishTimeArray.add((kv.value()["times"].as<String>()));
    }
  }
  int i=0;
  
  Serial.println();
  Serial.println();
  for(int i = 0; i < fishArray.getSize(); i++) {
    Serial.print(fishArray.get(i)+": ");
    Serial.print(fishDateArray.get(i));
    Serial.println(fishTimeArray.get(i));
  }
  
  for(int i = 0; i < fishArray.getSize(); i++) {

    if (fishTimeArray.get(i).indexOf(time()) >= 0 && fishArray.get(i).indexOf("Goldfish") >= 0) {
      if (!isFinish) { // Check if feeding is not already done
          Serial.println("Automatic Feeding - Feeding Now");
          servoFeed();
          isFinish = true; // Mark as finished for this feeding time
      } else {
          Serial.println("Feeding already completed for this time slot.");
      }
    } else {
        // Reset the flag if the current time is outside the scheduled feeding time
        isFinish = false;
    }

    if (fishTimeArray.get(i).indexOf(time()) >= 0 && fishArray.get(i).indexOf("Manual") >= 0) {
      if (!isFinish) { // Check if feeding is not already done
          Serial.println("Manual Feeding - Feeding Now");
          servoFeed();
          isFinish = true; // Mark as finished for this feeding time
      } else {
          Serial.println("Feeding already completed for this time slot.");
      }
    } else {
        // Reset the flag if the current time is outside the scheduled feeding time
        isFinish = false;
    }
  }    
}


void checkControls(){
  
  // Adjust the delay times to ensure proper communication
Serial1.println("AT+CIPSTART=\"TCP\",\"aquacheck.site\",80\r\n");
delay(500);  // Adjust to 300-500ms for connection establishment

String httpRequest = "GET /Controls/Control.php?api_key=iu311DXlOK42kGrBNHzPhQ8X8ZrmXXvbrRR8sbUz HTTP/1.1\r\nHost: aquacheck.site\r\nConnection: close\r\n\r\n";

// Send data size
Serial1.println("AT+CIPSEND=" + String(httpRequest.length()) +"\r\n");
delay(700);  // Adjust to 300ms

// Send HTTP request
Serial1.println(httpRequest);
Response = Serial1.readString(); // Adjust to 500ms - 1 second depending on server response time

delay(3000); 

Serial.println(Response);
// Read and print response from server
  int ResponseIndex = Response.indexOf("{");
  int ResponseEndIndex = Response.indexOf("CLOSED");
  String Responses = Response.substring(ResponseIndex, ResponseEndIndex);

  DeserializeControls(Responses);
}


void DeserializeControls(String responses){
  ControlType.clear();            // Add key to ControlType
    ControlStatus.clear();
  DynamicJsonDocument FDoc(2048);
  DeserializationError error = deserializeJson(FDoc, responses);
  const char* date;
  if (error) {
    Serial.print("Deserialization failed: ");
    Serial.println(error.f_str());
    return;
  }

  for (JsonPair kv : FDoc.as<JsonObject>()) {
    ControlType.add(kv.key().c_str());            // Add key to ControlType
    ControlStatus.add(kv.value().as<String>());  // Add value to ControlStatus
  }
  ControlProcess();
}

void ControlProcess() {
  Serial.println();
  Serial.println();
  bool anyTrue = false; // Initialize the flag to track if any condition is true

  for (int i = 0; i < ControlType.getSize(); i++) {
    if (ControlStatus.get(i).indexOf("true") >= 0) {
      anyTrue = true; // Set the flag to true if a condition is met
      switch (i) {
        case 0:
          Serial.println(0);
          digitalWrite(TempChangeRelay, LOW);
          digitalWrite(WaterInRelay, HIGH);
          digitalWrite(WaterOutRelay, HIGH);
          break;
        case 1:
          Serial.println(1);
          digitalWrite(TempChangeRelay, HIGH);
          digitalWrite(WaterInRelay, LOW);
          digitalWrite(WaterOutRelay, HIGH);
          break;
        case 2:
          Serial.println(2);
          digitalWrite(TempChangeRelay, HIGH);
          digitalWrite(WaterInRelay, HIGH);
          digitalWrite(WaterOutRelay, LOW);
          break;
      }
    }
  }

  // If no condition was true, turn off all relays
  if (!anyTrue) {
    Serial.println("All choices are false. Turning off all relays.");
    digitalWrite(TempChangeRelay, HIGH); // Assuming HIGH turns off the relay
    digitalWrite(WaterInRelay, HIGH);
    digitalWrite(WaterOutRelay, HIGH);
  }
}


