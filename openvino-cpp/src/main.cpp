#include <gflags/gflags.h>
#include <functional>
#include <iostream>
#include <chrono>
#include <vector>
#include <string>
#include <utility>
#include <map>

#include <inference_engine.hpp>

#include <opencv2/opencv.hpp>

#include <ie_iextension.h>

#include "FrameGrabber.hpp"
#include "OpenVinoEmotionsRecognizer.hpp"
#include "OpenVinoFaceDetector.hpp"
#include "OpenVinoObjectDetector.hpp"

using namespace std;
using namespace cv;
using namespace chrono;
using namespace InferenceEngine;

InferencePlugin plugin;

int main() {
	plugin = PluginDispatcher( { "" }).getPluginByDevice("MYRIAD");
	FrameGrabber grabber(1);
	OpenVinoFaceDetector fd(plugin);
	fd.ageGenderDetector = OpenVinoAgeGenderRecognizer(plugin);
	fd.emotionsDetector = OpenVinoEmotionsRecognizer(plugin);
	OpenVinoObjectDetector od(plugin);
	grabber.Start([&](FrameData data) {
		auto start = high_resolution_clock::now();

		fd.ProcessFrame(data);
		od.ProcessFrame(data);

		auto finish = high_resolution_clock::now();
		auto ms = duration_cast<milliseconds>(finish - start).count();

		data.Fps = 1000.0/ms;

		stringstream ss_fps;
		stringstream ss_dim;

		ss_fps << "FPS: " << setprecision(2) << fixed << data.Fps;
		ss_dim << data.width << "x" << data.height;

		putText(data.Image, ss_fps.str(), Point2f(0, 25), _fontFace, 0.6, _colorRed);
		putText(data.Image, ss_dim.str(), Point2f(0, 55), _fontFace, 0.6, _colorRed);

		imshow("processed", data.Image);
	});
	return 0;
}

