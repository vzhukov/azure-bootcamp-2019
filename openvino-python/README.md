# OpenVINO based Computer Vision (Python) and Azure IoT Hub

Face detection with OpenCV and sending the result to Azure IoT Hub

## Prerequisites

### Azure Subscription

First of all you need active Azure Subscription. In case you don't have one yet just follow this instruction:
[Create your Azure free account today](https://azure.microsoft.com/en-us/free/)

### Azure IoT Hub

https://azure.microsoft.com/en-us/services/iot-hub/

### Intel OpenVINO toolkit

https://software.intel.com/en-us/articles/OpenVINO-Install-Linux

### Movidius

Intel movidius chip on the board is required. You can buy USB stick:
https://www.amazon.com/dp/B07KT6361R/ref=cm_sw_em_r_mt_dp_U_zYRXCbKPAZPY7 
or other movidius based hardware in a form-factor you need:

![Intel NCS2](../assets/movidius.jpg "Intel NCS2")
![AI CORE ](../assets/movidius-2.jpg "AI CORE ")

## Run the sample

Clone the repository:

```
git clone https://github.com/vzhukov/azure-bootcamp-2019.git
```

Open file ```openvino-python\main.py```.

Update ```deviceConnectionString``` variable with your device connection string.

Start the application to detect faces and send the result to IoT Hub.

## How it works

Pseudo-code to explain how the application works.

Prepare network and weights:

```py
modelXml = "./models/face-detection-adas-0001.xml"
modelBin = "./models/face-detection-adas-0001.bin"
```

Init movidius device:
```py
plugin = IEPlugin(device="MYRIAD")
```

Init network and weights:
```py
net = IENetwork(model=modelXml, weights=modelBin)
netInput = next(iter(self.net.inputs))
netOut = next(iter(self.net.outputs))
net.batch_size = 1
```

Load network to the device:
```py
execNetwork = plugin.load(network=net)
```

Init video capture:
```py
cap = cv2.VideoCapture(0)
```

Get frame from camera:
```py
res, frame = cap.read()
```

Send image to the device to recognize faces:
```py
res = execNetwork.infer(inputs={netInput: images})
```

Send the result to IoT Hub:
```py
client = IoTHubClient(DeviceConnectionString, IoTHubTransportProvider.MQTT)
message = IoTHubMessage("{\"FaceCount\":\"" + str(faces) + "\"}")
client.send_event_async(message, self.send_confirmation_callback, 0)
```
