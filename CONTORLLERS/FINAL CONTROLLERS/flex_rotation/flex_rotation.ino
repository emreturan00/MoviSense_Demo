#include "I2Cdev.h"
#include "MPU6050_6Axis_MotionApps20.h"

//Constants:
const int ledPin = 3;   //pin 3 has PWM function
const int flexPin[] = {A0, A1, A2}; //pins A0, A1, A2 to read analog input

//Variables:
int value[3]; //save analog values

#if I2CDEV_IMPLEMENTATION == I2CDEV_ARDUINO_WIRE
    #include "Wire.h"
#endif

MPU6050 mpu;
#define OUTPUT_READABLE_YAWPITCHROLL
#define INTERRUPT_PIN 2  // use pin 2 on Arduino Uno & most boards
bool blinkState = false;
bool dmpReady = false;
uint8_t mpuIntStatus, devStatus;
uint16_t packetSize, fifoCount;
uint8_t fifoBuffer[64];
Quaternion q;
VectorInt16 aa, aaReal, aaWorld;
VectorFloat gravity;
float euler[3], ypr[3];
uint8_t teapotPacket[14] = { '$', 0x02, 0,0, 0,0, 0,0, 0,0, 0x00, 0x00, '\r', '\n' };
volatile bool mpuInterrupt = false;

void dmpDataReady() {
    mpuInterrupt = true;
}

void setup() {
    pinMode(ledPin, OUTPUT);  // Set pin 3 as 'output'
    Serial.begin(38400);      // Begin serial communication
    
    #if I2CDEV_IMPLEMENTATION == I2CDEV_ARDUINO_WIRE
        Wire.begin();
        Wire.setClock(400000);
    #endif

    while (!Serial);  // Wait for serial port to connect
    
    mpu.initialize();
    pinMode(INTERRUPT_PIN, INPUT);
    
    // Set the low-pass filter configuration
    mpu.setDLPFMode(MPU6050_DLPF_BW_42);  // Low-Pass Filter at 42Hz, can be adjusted
    
    devStatus = mpu.dmpInitialize();
    mpu.setXGyroOffset(220);
    mpu.setYGyroOffset(76);
    mpu.setZGyroOffset(-85);
    mpu.setZAccelOffset(1788);
    
    if (devStatus == 0) {
        mpu.CalibrateAccel(6);
        mpu.CalibrateGyro(6);
        mpu.setDMPEnabled(true);
        attachInterrupt(digitalPinToInterrupt(INTERRUPT_PIN), dmpDataReady, RISING);
        mpuIntStatus = mpu.getIntStatus();
        dmpReady = true;
        packetSize = mpu.dmpGetFIFOPacketSize();
    } else {
        Serial.print(F("DMP Initialization failed (code "));
        Serial.print(devStatus);
        Serial.println(F(")"));
    }
}

void loop() {
  if (!dmpReady) return;
  if (mpu.dmpGetCurrentFIFOPacket(fifoBuffer)) {

    for(int i = 0; i < 3; i++){
    value[i] = analogRead(flexPin[i]);         //Read and save analog value from potentiometer
    analogWrite(ledPin, value[i]);             //Send PWM value to led
    }

    #ifdef OUTPUT_READABLE_YAWPITCHROLL
        mpu.dmpGetQuaternion(&q, fifoBuffer);
        Serial.print(q.w,2);
        Serial.print(",");
        Serial.print(q.x);
        Serial.print(",");
        Serial.print(q.y);
        Serial.print(",");
        Serial.print(q.z);
        Serial.print(",");
    #endif
    
    Serial.print(value[0]);
    Serial.print(",");
    Serial.print(value[1]);
    Serial.print(",");
    Serial.print(value[2]);
    Serial.println();

  }
}