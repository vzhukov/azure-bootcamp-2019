#pragma once

#include <gflags/gflags.h>
#include <iostream>
#include <vector>
#include <string>
#include <algorithm>
#include <map>
#include <inference_engine.hpp>
#include <ie_iextension.h>
#include <opencv2/opencv.hpp>

using namespace std;
using namespace cv;
using namespace InferenceEngine;

// Информация о кадре
class FrameData {
public:
	FrameData() {

	}

	FrameData(Mat image) {
		Image = image;
	}

	Mat Image;
	int width = 0;
	int height = 0;
	double Fps = 0;
};

// Класс для работы с камерой
class FrameGrabber {
public:
	FrameGrabber(int camIndex) {
		_camIndex = camIndex;
	}

	FrameGrabber() {
		_camIndex = 0;
	}

	void Start(const function<void(FrameData)>& frameProcessingFunc) {

		cap = VideoCapture(_camIndex);

		width = cap.get(cv::CAP_PROP_FRAME_WIDTH);
		height = cap.get(cv::CAP_PROP_FRAME_HEIGHT);

		if (cap.open(_camIndex) == false) {
			cout << "Camera not found" << endl;
			return;
		}

		while (!_stopping) {
			Mat frame;

			if (!cap.read(frame)) {
				cout << "Failed to get a frame from camera" << endl;
				continue;
			}

			FrameData data(frame);
			data.width = width;
			data.height = height;

			frameProcessingFunc(data);

			if (waitKey(10) != -1) {
				Stop();
			}
		}
	}

	void Stop() {
		if (!_stopping) {
			_stopping = true;
		}
	}

private:
	int _camIndex = 0;
	bool _stopping = false;
	bool _processing = false;
	int width = 0;
	int height = 0;

	VideoCapture cap;
};

