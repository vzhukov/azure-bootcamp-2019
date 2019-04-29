import numpy as np
import cv2
from FrameGrabber import FrameGrabber

# Connection string for the device
deviceConnectionString = "HostName=azurebootcamp.azure-devices.net;DeviceId=dc401_e418ad37-e7ff-4372-8593-7417bc550a8b;SharedAccessKey=mtexqbZu1+4o3eZb3WaBNuAy3xtIvquAfQzjc49O2tU="

# Initialize grabber
grabber = FrameGrabber(deviceConnectionString, 0)

grabber.Start()

