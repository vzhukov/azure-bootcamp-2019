import numpy as np
import cv2 as cv
import random
from FaceCounter import FaceCounter
from openvino.inference_engine import IENetwork, IEPlugin
import iothub_client
from iothub_client import IoTHubClient, IoTHubClientError, IoTHubTransportProvider, IoTHubClientResult
from iothub_client import IoTHubMessage, IoTHubMessageDispositionResult, IoTHubError, DeviceMethodReturnValue
from iothub_client import IoTHubClientRetryPolicy, GetRetryPolicyReturnValue

DeviceConnectionString = ""

class FaceCounter:

    def __init__(self, plugin:IEPlugin):
        self.net = IENetwork(model=self.modelXml, weights=self.modelBin)
        self.plugin = plugin
        self.input_blob = next(iter(self.net.inputs))
        self.out_blob = next(iter(self.net.outputs))
        self.net.batch_size = 1
        self.n, self.c, self.h, self.w = self.net.inputs[self.input_blob].shape
        _,_,self.maxProposalCount,self.objectSize = self.net.outputs[self.out_blob].shape
        self.exec_net = self.plugin.load(network=self.net)

    def CountFaces(self, image):

        count = 0

        images = np.ndarray(shape=(1, self.c, self.h, self.w))
        if image.shape[:-1] != (self.h, self.w):
            image = cv.resize(image, (self.w, self.h))
        image = image.transpose((2, 0, 1))
        images[0] = image
        res = self.exec_net.infer(inputs={self.input_blob: images})
        res = res[self.out_blob]
        _, _, out_h, out_w = res.shape

        out_items = res[0][0]

        for i in out_items:
            confidence = i[2]
            if(confidence > 0.5):
                count = count + 1
        
        return count

    modelBase = "./models/face-detection-adas-0001"
    modelXml = modelBase + ".xml"
    modelBin = modelBase + ".bin"

class FrameGrabber:
    def __init__(self, connectionString, camIndex = 0):
        """
        Parameters
        ----------
        connectionString : str
            Connection string for the device in Azure IoT Hub
        camIndex : int
            Device index
        """
        DeviceConnectionString = connectionString
        self.camIndex = camIndex
    
    def send_confirmation_callback(message, result, user_context, data):
        pass

    def Start(self):
        print ("Camera index: " + str(self.camIndex))
        cap = cv.VideoCapture(self.camIndex)

        # Flag to stop processing
        stop = 0

        # Init movidius device
        plugin = IEPlugin(device="MYRIAD")

        # Init face counter
        faceCounter = FaceCounter(plugin)

        if (cap.open(self.camIndex) == 0):
            print("Camera not found")
            return

        while(stop == 0):
            # Get image from camera
            res, frame = cap.read()
        
            if(res):
                # Detect faces                
                faces = faceCounter.CountFaces(frame)

                # Send data to IoT Hub
                client = IoTHubClient(DeviceConnectionString, IoTHubTransportProvider.MQTT)
                message = IoTHubMessage("{\"FaceCount\":\"" + str(faces) + "\"}")
                client.send_event_async(message, self.send_confirmation_callback, 0)

                print("Face count detected: " + str(faces))
            else:
                print("Could not get a frame")

            # Sleep for one second
            if(cv.waitKey(1000) != -1):
                stop = 1
        
        cap.release()